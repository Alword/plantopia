﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plantopia.Service.Auth.Models;
using Plantopia.WebApi2.Data.Enums;
using Plantopia.WebApi2.Data.Interfaces;
using Plantopia.WebApi2.Data.Model;
using Plantopia.WebApi2.Data.Model.Person;
using Plantopia.WebApi2.Data.Repositories;

namespace Plantopia.WebApi2.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountIdentityProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly AccountRepository AccountRepository;

        private readonly ILogger logger;
        private readonly IPasswordSalter passwordSalter;

        /// <summary>
        ///     Create provider via repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="logger"></param>
        public AccountIdentityProvider(AccountRepository repository, ILogger logger)
        {
            this.logger = logger;
            AccountRepository = repository;
            passwordSalter = new SaltManager();
        }

        /// <summary>
        ///     This is to check user identity [FromBody]
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public TokenResponse GetIdentity(BasicAuthClaims user)
        {
            return GetIdentity(user.Email, user.Password);
        }

        /// <summary>
        ///     This is to check user identity
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TokenResponse GetIdentity(string email, string password)
        {
            Account account = AccountRepository.Get(x => x.Email == email).SingleOrDefault();
            if (account == null)
                throw new ArgumentNullException($"Email not found {email}");

            bool isValidPassword = passwordSalter.EqualsSequence(password, account.Hash);

            if (!isValidPassword)
                throw new ArgumentException("Invalid password");

            string nameId = account.AccountId.ToString();
            email = account.Email;
            string defaultRole = account.Role.ToString();

            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nameId),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, defaultRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims);

            var response = new TokenResponse(claimsIdentity);

            return response;
        }

        public async Task<Account> CreateUserAccount(string email, string password)
        {
            byte[] saltedKey = passwordSalter.SaltPassword(password);
            var newAccount = new Account
            {
                Email = email,
                Hash = saltedKey,
                Role = RoleType.User,
                RegistrationDate = DateTime.Now,
                User = null
            };

            newAccount.User = new User
            {
                NickName = "YourNick",
                Experience = 0
            };

            await AccountRepository.Create(newAccount);
            //await Update(newAccount);

            return newAccount;
        }
    }
}
