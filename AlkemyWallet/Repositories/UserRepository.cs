﻿using AlkemyWallet.Core.Models;
using AlkemyWallet.Repositories.Interfaces;

namespace AlkemyWallet.Repositories
{   
        public class UserRepository : RepositoryBase<User>, IUserRepository
        {
            public UserRepository(WalletDbContext context)
                : base(context)
            {

            }


        }
}

