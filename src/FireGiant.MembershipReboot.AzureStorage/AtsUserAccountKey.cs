// Copyright (c) FireGiant.  All Rights Reserved.

using System;

namespace FireGiant.MembershipReboot.AzureStorage
{
    internal class AtsUserAccountKey
    {
        private AtsUserAccountKey(Guid userId)
        {
            this.Partition = CalculatePartitionKeyForUserId(userId);
            this.Row = String.Empty;
        }

        public string Partition { get; }

        public string Row { get; }

        public static AtsUserAccountKey ForUserId(Guid userId)
        {
            return new AtsUserAccountKey(userId);
        }

        private static string CalculatePartitionKeyForUserId(Guid userId)
        {
            return userId.ToString("N").ToLowerInvariant();
        }
    }
}
