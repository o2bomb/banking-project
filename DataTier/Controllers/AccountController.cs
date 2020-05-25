using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIClasses;
using BankDB;

namespace DataTier.Controllers
{
    public class AccountController : ApiController
    {
        private static BankDB.BankDB db = new BankDB.BankDB();
        private static AccountAccessInterface access = db.GetAccountInterface();

        [Route("api/Account/create/{userID}")]
        [HttpGet]
        public void CreateAccount(uint userID)
        {
            AccountDetailStruct result = new AccountDetailStruct();

            result.accountID = access.CreateAccount(userID);
        }

        [Route("api/Account/{accountID}")]
        [HttpGet]
        public AccountDetailStruct GetAccountDetail(uint accountID)
        {
            AccountDetailStruct result = new AccountDetailStruct();

            access.SelectAccount(accountID);
            result.accountID = accountID;
            result.userID = access.GetOwner();
            result.balance = access.GetBalance();
            return result;
        }

        [Route("api/Account/all/{userID}")]
        [HttpGet]
        public List<AccountDetailStruct> GetUserAccounts(uint userID)
        {
            List<uint> accountIDs = access.GetAccountIDsByUser(userID);
            List<AccountDetailStruct> result = new List<AccountDetailStruct>();

            foreach(uint ID in accountIDs)
            {
                AccountDetailStruct entry = new AccountDetailStruct();

                access.SelectAccount(ID);
                entry.accountID = ID;
                entry.userID = access.GetOwner();
                entry.balance = access.GetBalance();
                result.Add(entry);
            }
            return result;
        }

        [Route("api/Account/{accountID}/deposit")]
        [HttpPost]
        public void DoAccountDeposit(uint accountID, [FromBody]uint amount)
        {
            AccountDetailStruct result = new AccountDetailStruct();

            access.SelectAccount(accountID);
            access.Deposit(amount);
        }

        [Route("api/Account/{accountID}/withdraw")]
        [HttpPost]
        public void DoAccountWithdraw(uint accountID, [FromBody]uint amount)
        {
            AccountDetailStruct result = new AccountDetailStruct();

            access.SelectAccount(accountID);
            access.Withdraw(amount);
        }
    }
}