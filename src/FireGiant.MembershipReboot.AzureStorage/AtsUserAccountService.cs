// Copyright (c) FireGiant.  All Rights Reserved.

using BrockAllen.MembershipReboot;

namespace FireGiant.MembershipReboot.AzureStorage
{
    public class AtsUserAccountService : UserAccountService<AtsUserAccount>
    {
        public AtsUserAccountService(AtsUserAccountConfig config, AtsUserAccountRepository repo)
            : base(config, repo)
        {
        }
    }
}
