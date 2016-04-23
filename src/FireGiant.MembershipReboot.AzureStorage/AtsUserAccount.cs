// Copyright (c) FireGiant.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Hierarchical;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FireGiant.MembershipReboot.AzureStorage
{
    public class AtsUserAccount : HierarchicalUserAccount, ITableEntity
    {
        private const string ClaimValueEncodedPrefix = "base64:";
        private const string ClaimNameValueSeparator = "=";
        private const string ClaimSeparator = "\n";

        public AtsUserAccount()
        {
        }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string ETag { get; set; }

        internal string OriginalUserName { get; private set; }

        internal string OriginalEmail { get; private set; }

        internal string OriginalPhoneNumber { get; private set; }

        internal string OriginalVerificationKey { get; private set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            foreach (var property in properties)
            {
                var value = property.Value;

                switch (property.Key)
                {
                    case "Id":
                        this.ID = value.GuidValue ?? Guid.Empty;
                        break;

                    case nameof(this.Tenant):
                        this.Tenant = value.StringValue;
                        break;

                    case nameof(this.Username):
                        this.Username = value.StringValue;
                        this.OriginalUserName = this.Username;
                        break;

                    case nameof(this.Email):
                        this.Email = value.StringValue;
                        this.OriginalEmail = this.Email;
                        break;

                    case nameof(this.Created):
                        this.Created = value.DateTimeOffsetValue?.DateTime ?? DateTime.MinValue;
                        break;

                    case nameof(this.LastUpdated):
                        this.LastUpdated = value.DateTimeOffsetValue?.DateTime ?? DateTime.MinValue;
                        break;

                    case nameof(this.IsAccountClosed):
                        this.IsAccountClosed = value.BooleanValue ?? false;
                        break;

                    case nameof(this.AccountClosed):
                        this.AccountClosed = value.DateTimeOffsetValue?.DateTime;
                        break;

                    case nameof(this.IsLoginAllowed):
                        this.IsLoginAllowed = value.BooleanValue ?? false;
                        break;

                    case nameof(this.LastLogin):
                        this.LastLogin = value.DateTimeOffsetValue?.DateTime;
                        break;

                    case nameof(this.LastFailedLogin):
                        this.LastFailedLogin = value.DateTimeOffsetValue?.DateTime;
                        break;

                    case nameof(this.FailedLoginCount):
                        this.FailedLoginCount = value.Int32Value ?? 0;
                        break;

                    case nameof(this.PasswordChanged):
                        this.PasswordChanged = value.DateTimeOffsetValue?.DateTime;
                        break;

                    case nameof(this.RequiresPasswordReset):
                        this.RequiresPasswordReset = value.BooleanValue ?? false;
                        break;

                    case nameof(this.IsAccountVerified):
                        this.IsAccountVerified = value.BooleanValue ?? false;
                        break;

                    case nameof(this.LastFailedPasswordReset):
                        this.LastFailedPasswordReset = value.DateTimeOffsetValue?.DateTime;
                        break;

                    case nameof(this.FailedPasswordResetCount):
                        this.FailedPasswordResetCount = value.Int32Value ?? 0;
                        break;

                    case nameof(this.MobileCode):
                        this.MobileCode = value.StringValue;
                        break;

                    case nameof(this.MobileCodeSent):
                        this.MobileCodeSent = value.DateTimeOffsetValue?.DateTime;
                        break;

                    case nameof(this.MobilePhoneNumber):
                        this.MobilePhoneNumber = value.StringValue;
                        this.OriginalPhoneNumber = this.MobilePhoneNumber;
                        break;

                    case nameof(this.AccountTwoFactorAuthMode):
                        this.AccountTwoFactorAuthMode = (TwoFactorAuthMode)Enum.Parse(typeof(TwoFactorAuthMode), value.StringValue);
                        break;

                    case nameof(this.CurrentTwoFactorAuthStatus):
                        this.CurrentTwoFactorAuthStatus = (TwoFactorAuthMode)Enum.Parse(typeof(TwoFactorAuthMode), value.StringValue);
                        break;

                    case nameof(this.VerificationKey):
                        this.VerificationKey = value.StringValue;
                        this.OriginalVerificationKey = this.VerificationKey;
                        break;

                    case nameof(this.VerificationPurpose):
                        if (!String.IsNullOrEmpty(value.StringValue))
                        {
                            this.VerificationPurpose = (VerificationKeyPurpose)Enum.Parse(typeof(VerificationKeyPurpose), value.StringValue);
                        }
                        break;

                    case nameof(this.VerificationKeySent):
                        this.VerificationKeySent = value.DateTimeOffsetValue?.DateTime;
                        break;

                    case nameof(this.VerificationStorage):
                        this.VerificationStorage = value.StringValue;
                        break;

                    case nameof(this.HashedPassword):
                        this.HashedPassword = value.StringValue;
                        break;

                    case nameof(this.Claims):
                        this.ParseClaims(value.StringValue);
                        break;

                    default:
                        this.ReadProperty(property.Key, value);
                        break;
                }
            }
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var result = new Dictionary<string, EntityProperty>
            {
                { "Id", EntityProperty.GeneratePropertyForGuid(this.ID) },
                { nameof(this.Tenant), EntityProperty.GeneratePropertyForString(this.Tenant) },
                { nameof(this.Username), EntityProperty.GeneratePropertyForString(this.Username) },
                { nameof(this.Email), EntityProperty.GeneratePropertyForString(this.Email) },
                { nameof(this.Created), EntityProperty.GeneratePropertyForDateTimeOffset(this.Created) },
                { nameof(this.LastUpdated), EntityProperty.GeneratePropertyForDateTimeOffset(this.LastUpdated) },
                { nameof(this.IsAccountClosed), EntityProperty.GeneratePropertyForBool(this.IsAccountClosed) },
                { nameof(this.AccountClosed), EntityProperty.GeneratePropertyForDateTimeOffset(this.AccountClosed) },
                { nameof(this.IsLoginAllowed), EntityProperty.GeneratePropertyForBool(this.IsLoginAllowed) },
                { nameof(this.LastLogin), EntityProperty.GeneratePropertyForDateTimeOffset(this.LastLogin) },
                { nameof(this.LastFailedLogin), EntityProperty.GeneratePropertyForDateTimeOffset(this.LastFailedLogin) },
                { nameof(this.FailedLoginCount), EntityProperty.GeneratePropertyForInt(this.FailedLoginCount) },
                { nameof(this.PasswordChanged), EntityProperty.GeneratePropertyForDateTimeOffset(this.PasswordChanged) },
                { nameof(this.RequiresPasswordReset), EntityProperty.GeneratePropertyForBool(this.RequiresPasswordReset) },
                { nameof(this.IsAccountVerified), EntityProperty.GeneratePropertyForBool(this.IsAccountVerified) },
                { nameof(this.LastFailedPasswordReset), EntityProperty.GeneratePropertyForDateTimeOffset(this.LastFailedPasswordReset) },
                { nameof(this.FailedPasswordResetCount), EntityProperty.GeneratePropertyForInt(this.FailedPasswordResetCount) },

                { nameof(this.MobileCode), EntityProperty.GeneratePropertyForString(this.MobileCode) },
                { nameof(this.MobileCodeSent), EntityProperty.GeneratePropertyForDateTimeOffset(this.MobileCodeSent) },
                { nameof(this.MobilePhoneNumber), EntityProperty.GeneratePropertyForString(this.MobilePhoneNumber) },
                { nameof(this.MobilePhoneNumberChanged), EntityProperty.GeneratePropertyForDateTimeOffset(this.MobilePhoneNumberChanged) },

                { nameof(this.AccountTwoFactorAuthMode), EntityProperty.GeneratePropertyForString(this.AccountTwoFactorAuthMode.ToString()) },
                { nameof(this.CurrentTwoFactorAuthStatus), EntityProperty.GeneratePropertyForString(this.CurrentTwoFactorAuthStatus.ToString()) },
                { nameof(this.VerificationKey), EntityProperty.GeneratePropertyForString(this.VerificationKey) },
                { nameof(this.VerificationPurpose), EntityProperty.GeneratePropertyForString(this.VerificationPurpose?.ToString()) },
                { nameof(this.VerificationKeySent), EntityProperty.GeneratePropertyForDateTimeOffset(this.VerificationKeySent) },
                { nameof(this.VerificationStorage), EntityProperty.GeneratePropertyForString(this.VerificationStorage) },
                { nameof(this.HashedPassword), EntityProperty.GeneratePropertyForString(this.HashedPassword) },
                { nameof(this.Claims), GeneratePropertyForClaims(this.Claims) },
                { nameof(this.LinkedAccounts), GeneratePropertyForLinkedAccounts(this.LinkedAccounts) },
                { nameof(this.LinkedAccountClaims), GeneratePropertyForLinkedAccountClaims(this.LinkedAccountClaims) },
                //{ nameof(this.Certificates), GeneratePropertyForCertificates(this.Certificates) },
                //{ nameof(this.TwoFactorAuthTokens), GeneratePropertyForTwoFactorAuthTokens(this.TwoFactorAuthTokens) },
                { nameof(this.PasswordResetSecrets), GeneratePropertyForPasswordResetSecrets(this.PasswordResetSecrets) },
            };

            this.WriteProperties(result);

            // Ideally, we'd only update these values when we were certain the entity was
            // persisted to storage.
            this.OriginalUserName = this.Username;
            this.OriginalEmail = this.Email;
            this.OriginalPhoneNumber = this.MobilePhoneNumber;
            this.OriginalVerificationKey = this.VerificationKey;

            return result;
        }

        protected virtual void ReadProperty(string name, EntityProperty property)
        {
        }

        protected virtual void WriteProperties(IDictionary<string, EntityProperty> properties)
        {
        }

        internal AtsUserAccount SetEntityKeys()
        {
            var key = AtsUserAccountKey.ForUserId(this.ID);

            this.PartitionKey = key.Partition;
            this.RowKey = key.Row;

            return this;
        }

        private void ParseClaims(string claims)
        {
            var start = 0;
            var end = -1;

            while ((end = claims.IndexOf(ClaimNameValueSeparator, start, StringComparison.Ordinal)) > 0)
            {
                var type = claims.Substring(start, end - start);

                start = end + 1;
                end = claims.IndexOf(ClaimSeparator, start, StringComparison.Ordinal);

                var value = claims.Substring(start, end - start);

                if (value.StartsWith(ClaimValueEncodedPrefix, StringComparison.Ordinal))
                {
                    value = value.Substring(ClaimValueEncodedPrefix.Length).FromBase64();
                }

                this.AddClaim(new UserClaim(type, value));

                start = end + 1;
            }
        }

        private static EntityProperty GeneratePropertyForClaims(IEnumerable<UserClaim> claims)
        {
            if (!claims.Any())
            {
                return EntityProperty.GeneratePropertyForString(null);
            }

            var result = new StringBuilder();

            foreach (var claim in claims)
            {
                var value = claim.Value ?? String.Empty;

                if (value.StartsWith(ClaimValueEncodedPrefix, StringComparison.Ordinal) || value.Contains(ClaimSeparator))
                {
                    value = ClaimValueEncodedPrefix + value.ToBase64();
                }

                result.Append(claim.Type);
                result.Append(ClaimNameValueSeparator);
                result.Append(value);
                result.Append(ClaimSeparator);
            }

            return EntityProperty.GeneratePropertyForString(result.ToString());
        }

        private static EntityProperty GeneratePropertyForLinkedAccounts(IEnumerable<LinkedAccount> linkedAccounts)
        {
            if (!linkedAccounts.Any())
            {
                return EntityProperty.GeneratePropertyForString(null);
            }

            var result = new StringBuilder();

            foreach (var linkedAccount in linkedAccounts)
            {
                result.AppendFormat("{0}\t{1}\t{2}\n", linkedAccount.ProviderName, linkedAccount.ProviderAccountID.ToBase64(), linkedAccount.LastLogin.ToString("O"));
            }

            return EntityProperty.GeneratePropertyForString(result.ToString());
        }

        private static EntityProperty GeneratePropertyForLinkedAccountClaims(IEnumerable<LinkedAccountClaim> linkedAccountClaims)
        {
            if (!linkedAccountClaims.Any())
            {
                return EntityProperty.GeneratePropertyForString(null);
            }

            var result = new StringBuilder();

            foreach (var claim in linkedAccountClaims)
            {
                result.AppendFormat("{0}\t{1}\t{2}\t{3}\n", claim.ProviderName, claim.ProviderAccountID.ToBase64(), claim.Type, claim.Value.ToBase64());
            }

            return EntityProperty.GeneratePropertyForString(result.ToString());
        }

        private static EntityProperty GeneratePropertyForPasswordResetSecrets(IEnumerable<PasswordResetSecret> resets)
        {
            if (!resets.Any())
            {
                return EntityProperty.GeneratePropertyForString(null);
            }

            var result = new StringBuilder();

            foreach (var reset in resets)
            {
                result.AppendFormat("{0}\t{1}\t{2}\n", reset.PasswordResetSecretID.ToString("N"), reset.Question.ToBase64(), reset.Answer.ToBase64());
            }

            return EntityProperty.GeneratePropertyForString(result.ToString());
        }
    }
}
