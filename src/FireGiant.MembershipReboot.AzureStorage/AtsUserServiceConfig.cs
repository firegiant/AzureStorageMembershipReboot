// Copyright (c) FireGiant.  All Rights Reserved.

using System;
using BrockAllen.MembershipReboot;

namespace FireGiant.MembershipReboot.AzureStorage
{
    public class AtsUserServiceConfig : MembershipRebootConfiguration<AtsUser>
    {
        public AtsUserServiceConfig()
            : this(null)
        {
        }

        public AtsUserServiceConfig(string connectionString, string tenant = null, string tableName = null)
        {
            this.TableStorageConnectionString = connectionString;

            this.DefaultTenant = String.IsNullOrEmpty(tenant) ? "default" : tenant;

            this.TableName = String.IsNullOrEmpty(tableName) ? "user" : tableName;
        }

        public string TableStorageConnectionString { get; set; }

        public string TableName { get; set; }
    }
}
