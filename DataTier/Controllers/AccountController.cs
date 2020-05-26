using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIClasses;
using BankDB;
using DataTier.Models;

namespace DataTier.Controllers
{
    public class AccountController : ApiController
    {
        private static BankDB.BankDB db = BankDBProvider.getInstance();
        private static AccountAccessInterface access = db.GetAccountInterface();

        [Route("api/Account/create/{userID}")]
        [HttpGet]
        public AccountDetailStruct CreateAccount(uint userID)
        {
            AccountDetailStruct result = new AccountDetailStruct();

            try
            {
                result.accountID = access.CreateAccount(userID);
            }
            catch (Exception e)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Account could not be created"),
                    ReasonPhrase = "User could not be found"
                };
                throw new HttpResponseException(response);
            }

            return GetAccountDetail(result.accountID);
        }

        [Route("api/Account/{accountID}")]
        [HttpGet]
        public AccountDetailStruct GetAccountDetail(uint accountID)
        {
            AccountDetailStruct result = new AccountDetailStruct();

            try
            {
                access.SelectAccount(accountID);
                result.accountID = accountID;
                result.userID = access.GetOwner();
                result.balance = access.GetBalance();
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Account details could not be retreived"),
                    ReasonPhrase = "Account could not be found"
                };
                throw new HttpResponseException(response);
            }
            return result;
        }

        [Route("api/Account/all/{userID}")]
        [HttpGet]
        public List<AccountDetailStruct> GetUserAccounts(uint userID)
        {
            List<AccountDetailStruct> result = new List<AccountDetailStruct>();
            try
            {
                List<uint> accountIDs = access.GetAccountIDsByUser(userID);
                foreach(uint ID in accountIDs)
                {
                    AccountDetailStruct entry = new AccountDetailStruct();

                    access.SelectAccount(ID);
                    entry.accountID = ID;
                    entry.userID = access.GetOwner();
                    entry.balance = access.GetBalance();
                    result.Add(entry);
                }
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("User's accounts could not be retreived"),
                    ReasonPhrase = "User could not be found"
                };
                throw new HttpResponseException(response);
            }
            return result;
        }

        [Route("api/Account/{accountID}/deposit")]
        [HttpPost]
        public void DoAccountDeposit(uint accountID, [FromBody]uint amount)
        {
            try
            {
                access.SelectAccount(accountID);
                access.Deposit(amount);
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Could not perform deposit in account"),
                    ReasonPhrase = "Account could not be found"
                };
                throw new HttpResponseException(response);
            }
        }

        [Route("api/Account/{accountID}/withdraw")]
        [HttpPost]
        public void DoAccountWithdraw(uint accountID, [FromBody]uint amount)
        {
            try
            {
                access.SelectAccount(accountID);
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Could not perform withdrawal from account"),
                    ReasonPhrase = "Account could not be found"
                };
                throw new HttpResponseException(response);
            }

            try
            {
                access.Withdraw(amount);
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Could not perform withdrawal from account"),
                    ReasonPhrase = "Account has insufficient funds"
                };
                throw new HttpResponseException(response);
            }
        }
    }
}