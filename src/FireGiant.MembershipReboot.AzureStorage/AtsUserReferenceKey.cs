// Copyright (c) FireGiant.  All Rights Reserved.

using System;

namespace FireGiant.MembershipReboot.AzureStorage
{
    internal class AtsUserReferenceKey
    {
        private AtsUserReferenceKey(string referenceKey)
        {
            this.Partition = referenceKey;
            this.Row = String.Empty;
        }

        private AtsUserReferenceKey(string tenant, string referenceKey)
        {
            this.Partition = "tenant|" + tenant + "|" + referenceKey;
            this.Row = String.Empty;
        }

        public string Partition { get; }

        public string Row { get; }

        public static AtsUserReferenceKey ForUsername(string tenant, string username)
        {
            return new AtsUserReferenceKey(tenant, "user|" + username.ToBase64IfUnsafe());
        }

        public static AtsUserReferenceKey ForEmail(string tenant, string email)
        {
            return new AtsUserReferenceKey(tenant, "email|" + email.ToBase64IfUnsafe());
        }

        public static AtsUserReferenceKey ForPhoneNumber(string tenant, string phone)
        {
            return new AtsUserReferenceKey(tenant, "phone|" + phone);
        }

        public static AtsUserReferenceKey ForVerificationKey(string key)
        {
            return new AtsUserReferenceKey("key|" + key);
        }

        public static AtsUserReferenceKey ForLinkedAccount(string tenant, string provider, string id)
        {
            return new AtsUserReferenceKey(tenant, "link|" + provider + "|" + id.ToBase64IfUnsafe());
        }

        public static AtsUserReferenceKey ForCertificate(string tenant, string thumbprint)
        {
            return new AtsUserReferenceKey(tenant, "cert|" + thumbprint);
        }
    }
}
