// Copyright (c) FireGiant.  All Rights Reserved.

using BrockAllen.MembershipReboot;

namespace FireGiant.MembershipReboot.AzureStorage
{
    public class AtsUserService : UserAccountService<AtsUser>
    {
        public AtsUserService(AtsUserServiceConfig config, AtsUserRepository repo)
            : base(config, repo)
        {
        }
    }
}
