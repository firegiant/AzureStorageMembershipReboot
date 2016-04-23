// Copyright (c) FireGiant.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using BrockAllen.MembershipReboot;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FireGiant.MembershipReboot.AzureStorage
{
    public class AtsUserAccountRepository : IUserAccountRepository<AtsUserAccount>
    {
        private readonly string _defaultTenant;
        private readonly CloudTable _table;

        public AtsUserAccountRepository(AtsUserAccountConfig config)
            : this(CloudStorageAccount.Parse(config.TableStorageConnectionString), config)
        {
        }

        public AtsUserAccountRepository(CloudStorageAccount storage, AtsUserAccountConfig config)
        {
            _defaultTenant = config.DefaultTenant;

            var client = storage.CreateCloudTableClient();
            _table = client.GetTableReference(config.TableName);
            _table.CreateIfNotExists();
        }

        public AtsUserAccount Create()
        {
            return new AtsUserAccount();
        }

        public void Add(AtsUserAccount user)
        {
            user.SetEntityKeys();

            var usernameref = this.CreateUsernameReference(user);
            var emailref = this.CreateEmailReference(user);
            var phoneref = this.CreatePhoneReference(user);
            var verificationRef = this.CreateVerificationKeyReference(user);

            var operations = new[]
            {
                TableOperation.Insert(user),
                (usernameref == null) ? null : TableOperation.Insert(usernameref),
                (emailref == null) ? null : TableOperation.Insert(emailref),
                (phoneref == null) ? null : TableOperation.Insert(phoneref),
                (verificationRef == null) ? null : TableOperation.Insert(verificationRef),
            };

            this.ExecuteOperations(operations);
        }

        public void Remove(AtsUserAccount user)
        {
            user.SetEntityKeys();

            var usernameref = this.CreateUsernameReference(user);
            var emailref = this.CreateEmailReference(user);
            var phoneref = this.CreatePhoneReference(user);
            var verificationRef = this.CreateVerificationKeyReference(user);

            var operations = new[]
            {
                TableOperation.Delete(user),
                (usernameref == null) ? null : TableOperation.Delete(usernameref),
                (emailref == null) ? null : TableOperation.Delete(emailref),
                (phoneref == null) ? null : TableOperation.Delete(phoneref),
                (verificationRef == null) ? null : TableOperation.Delete(verificationRef),
            };

            this.ExecuteOperations(operations);
        }

        public void Update(AtsUserAccount user)
        {
            user.SetEntityKeys();
            user.ETag = "*";

            var operations = new List<TableOperation>()
            {
                TableOperation.Replace(user)
            };

            if (user.OriginalUserName != user.Username)
            {
                var oldRef = this.CreateUsernameReference(user, true);
                var newRef = this.CreateUsernameReference(user);

                if (newRef != null) { operations.Add(TableOperation.Insert(newRef)); }
                if (oldRef != null) { operations.Add(TableOperation.Delete(oldRef)); }
            }

            if (user.OriginalEmail != user.Email)
            {
                var oldRef = this.CreateEmailReference(user, true);
                var newRef = this.CreateEmailReference(user);

                if (newRef != null) { operations.Add(TableOperation.Insert(newRef)); }
                if (oldRef != null) { operations.Add(TableOperation.Delete(oldRef)); }
            }

            if (user.OriginalPhoneNumber != user.MobilePhoneNumber)
            {
                var oldRef = this.CreatePhoneReference(user, true);
                var newRef = this.CreatePhoneReference(user);

                if (newRef != null) { operations.Add(TableOperation.Insert(newRef)); }
                if (oldRef != null) { operations.Add(TableOperation.Delete(oldRef)); }
            }

            if (user.OriginalVerificationKey != user.VerificationKey)
            {
                var oldRef = this.CreateVerificationKeyReference(user, true);
                var newRef = this.CreateVerificationKeyReference(user);

                if (newRef != null) { operations.Add(TableOperation.Insert(newRef)); }
                if (oldRef != null) { operations.Add(TableOperation.Delete(oldRef)); }
            }

            this.ExecuteOperations(operations);
        }

        public AtsUserAccount GetByID(Guid id)
        {
            var key = AtsUserAccountKey.ForUserId(id);

            var op = TableOperation.Retrieve<AtsUserAccount>(key.Partition, key.Row);

            var result = _table.Execute(op);

            return (AtsUserAccount)result.Result;
        }

        public AtsUserAccount GetByUsername(string username)
        {
            return this.GetByUsername(_defaultTenant, username);
        }

        public AtsUserAccount GetByUsername(string tenant, string username)
        {
            var referenceKey = AtsUserReferenceKey.ForUsername(tenant, username);
            return this.GetUserAccountByReference(referenceKey);
        }

        public AtsUserAccount GetByEmail(string tenant, string email)
        {
            var referenceKey = AtsUserReferenceKey.ForEmail(tenant, email);
            return this.GetUserAccountByReference(referenceKey);
        }

        public AtsUserAccount GetByMobilePhone(string tenant, string phone)
        {
            var referenceKey = AtsUserReferenceKey.ForPhoneNumber(tenant, phone);
            return this.GetUserAccountByReference(referenceKey);
        }

        public AtsUserAccount GetByVerificationKey(string key)
        {
            var referenceKey = AtsUserReferenceKey.ForVerificationKey(key);
            return this.GetUserAccountByReference(referenceKey);
        }

        public AtsUserAccount GetByLinkedAccount(string tenant, string provider, string id)
        {
            var referenceKey = AtsUserReferenceKey.ForLinkedAccount(tenant, provider, id);
            return this.GetUserAccountByReference(referenceKey);
        }

        public AtsUserAccount GetByCertificate(string tenant, string thumbprint)
        {
            var referenceKey = AtsUserReferenceKey.ForCertificate(tenant, thumbprint);
            return this.GetUserAccountByReference(referenceKey);
        }

        private AtsUserAccount GetUserAccountByReference(AtsUserReferenceKey referenceKey)
        {
            var op = TableOperation.Retrieve<AtsUserReference>(referenceKey.Partition, referenceKey.Row);

            var result = _table.Execute(op);

            if (result.HttpStatusCode != 200)
            {
                return null;
            }

            var reference = (AtsUserReference)result.Result;

            var userKey = AtsUserAccountKey.ForUserId(reference.UserId);

            op = TableOperation.Retrieve<AtsUserAccount>(userKey.Partition, userKey.Row);

            result = _table.Execute(op);

            return (AtsUserAccount)result.Result;
        }

        private AtsUserReference CreateUsernameReference(AtsUserAccount user, bool original = false)
        {
            var value = original ? user.OriginalUserName : user.Username;
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            var referenceKey = AtsUserReferenceKey.ForUsername(user.Tenant, value);
            return new AtsUserReference(referenceKey, user.ID) { ETag = "*" };
        }

        private AtsUserReference CreateEmailReference(AtsUserAccount user, bool original = false)
        {
            var value = original ? user.OriginalEmail : user.Email;
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            var referenceKey = AtsUserReferenceKey.ForEmail(user.Tenant, value);
            return new AtsUserReference(referenceKey, user.ID) { ETag = "*" };
        }

        private AtsUserReference CreatePhoneReference(AtsUserAccount user, bool original = false)
        {
            var value = original ? user.OriginalPhoneNumber : user.MobilePhoneNumber;
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            var referenceKey = AtsUserReferenceKey.ForPhoneNumber(user.Tenant, value);
            return new AtsUserReference(referenceKey, user.ID) { ETag = "*" };
        }

        private AtsUserReference CreateVerificationKeyReference(AtsUserAccount user, bool original = false)
        {
            var value = original ? user.OriginalVerificationKey : user.VerificationKey;
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            var referenceKey = AtsUserReferenceKey.ForVerificationKey(value);
            return new AtsUserReference(referenceKey, user.ID) { ETag = "*" };
        }

        private void ExecuteOperations(IEnumerable<TableOperation> operations)
        {
            foreach (var op in operations.Where(op => op != null))
            {
                var result = _table.Execute(op);

                // TODO: error if result.Result is less than desirable.
            }
        }
    }
}
