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

        public AtsUserServiceConfig(string connectionString, string tableName = null, string tenant = null)
        {
            this.TableStorageConnectionString = connectionString;

            this.TableName = String.IsNullOrEmpty(tableName) ? "user" : tableName;

            this.DefaultTenant = String.IsNullOrEmpty(tenant) ? "default" : tenant;
        }

        public string TableStorageConnectionString { get; set; }

        public string TableName { get; set; }
    }
}
