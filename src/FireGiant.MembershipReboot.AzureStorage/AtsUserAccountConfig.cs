// Copyright (c) FireGiant.  All Rights Reserved.

using System;
using BrockAllen.MembershipReboot;

namespace FireGiant.MembershipReboot.AzureStorage
{
    public class AtsUserAccountConfig : MembershipRebootConfiguration<AtsUserAccount>
    {
        public AtsUserAccountConfig(string connectionString, string tableName = null)
        {
            this.TableStorageConnectionString = connectionString;

            this.TableName = String.IsNullOrEmpty(tableName) ? "user" : tableName;
        }

        public string TableStorageConnectionString { get; }

        public string TableName { get; }
    }
}
