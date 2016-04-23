// Copyright (c) FireGiant.  All Rights Reserved.

using System;

namespace FireGiant.MembershipReboot.AzureStorage
{
    internal class AtsUserKey
    {
        private AtsUserKey(Guid userId)
        {
            this.Partition = CalculatePartitionKeyForUserId(userId);
            this.Row = String.Empty;
        }

        public string Partition { get; }

        public string Row { get; }

        public static AtsUserKey ForUserId(Guid userId)
        {
            return new AtsUserKey(userId);
        }

        private static string CalculatePartitionKeyForUserId(Guid userId)
        {
            return userId.ToString("N").ToLowerInvariant();
        }
    }
}
