// Copyright (c) FireGiant.  All Rights Reserved.

using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace FireGiant.MembershipReboot.AzureStorage
{
    internal class AtsUserReference : TableEntity
    {
        public AtsUserReference()
        {
        }

        public AtsUserReference(AtsUserReferenceKey keys, Guid id)
        {
            this.PartitionKey = keys.Partition;
            this.RowKey = keys.Row;
            this.UserId = id;
        }

        public Guid UserId { get; set; }
    }
}
