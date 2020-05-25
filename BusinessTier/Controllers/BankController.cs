using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using RestSharp;
using APIClasses;

namespace BusinessTier.Controllers
{
    public class BankController : ApiController
    {
        private static string endpoint = "https://localhost:44328/";
        private static RestClient client = new RestClient(endpoint);
        private static RestRequest saveToDiskRequest = new RestRequest("api/admin/save");
        private static RestRequest processTransactionsRequest = new RestRequest("api/admin/processtransactions");

        [Route("api/Bank/user/{userID}")]
        [HttpGet]
        public UserDetailStruct GetUserDetails(uint userID)
        {
            RestRequest req = new RestRequest(String.Format("api/user/{0}", userID));
            IRestResponse res = client.Execute(req);
            UserDetailStruct result = JsonConvert.DeserializeObject<UserDetailStruct>(res.Content);

            return result;
        }

        [Route("api/Bank/user/{userID}/transactions")]
        [HttpGet]
        public List<TransactionDetailStruct> GetUserTransactions(uint userID)
        {
            RestRequest req1 = new RestRequest("api/transaction/all");
            IRestResponse res1 = client.Execute(req1);
            List<TransactionDetailStruct> temp = JsonConvert.DeserializeObject<List<TransactionDetailStruct>>(res1.Content);
            // Filter out transactions that don't belong to the user
            List<TransactionDetailStruct> result = temp.Where(value => value.senderID == userID || value.receiverID == userID).ToList();

            return result;
        }

        [Route("api/Bank/user/{userID}/accounts")]
        [HttpGet]
        public List<AccountDetailStruct> GetUserAccounts(uint userID)
        {
            RestRequest req = new RestRequest(String.Format("api/account/all/{0}", userID));
            IRestResponse res = client.Execute(req);
            List<AccountDetailStruct> result = JsonConvert.DeserializeObject<List<AccountDetailStruct>>(res.Content);

            return result;
        }

        [Route("api/Bank/account/{accountID}")]
        [HttpGet]
        public AccountDetailStruct GetAccountDetail(uint accountID)
        {
            // Get the account
            RestRequest req1 = new RestRequest(String.Format("api/account/{0}", accountID));
            IRestResponse res1 = client.Execute(req1);
            AccountDetailStruct result = JsonConvert.DeserializeObject<AccountDetailStruct>(res1.Content);

            return result;
        }

        [Route("api/Bank/account/{accountID}/deposit")]
        [HttpPost]
        public AccountDetailStruct DoAccountDeposit(uint accountID, [FromBody]uint amount)
        {
            // Perform the deposit
            RestRequest req1 = new RestRequest(String.Format("api/account/{0}/deposit", accountID), Method.POST);
            uint body = amount;
            req1.AddJsonBody(body);
            IRestResponse res1 = client.Execute(req1);

            // Process pending transactions
            IRestResponse processRes = client.Execute(processTransactionsRequest);
            // Save to disk
            IRestResponse saveRes = client.Execute(saveToDiskRequest);

            // Get updated account details
            RestRequest req2 = new RestRequest(String.Format("api/account/{0}", accountID));
            IRestResponse res2 = client.Execute(req2);
            AccountDetailStruct result = JsonConvert.DeserializeObject<AccountDetailStruct>(res2.Content);

            return result;
        }

        [Route("api/Bank/account/{accountID}/withdraw")]
        [HttpPost]
        public AccountDetailStruct DoAccountWithdraw(uint accountID, [FromBody]uint amount)
        {
            // Perform the withdrawal
            RestRequest req = new RestRequest(String.Format("api/account/{0}/withdraw", accountID), Method.POST);
            uint body = amount;
            req.AddJsonBody(body);
            IRestResponse res = client.Execute(req);

            // Process pending transactions
            IRestResponse processRes = client.Execute(processTransactionsRequest);
            // Save to disk
            IRestResponse saveRes = client.Execute(saveToDiskRequest);

            // Get updated account details
            RestRequest req2 = new RestRequest(String.Format("api/account/{0}", accountID));
            IRestResponse res2 = client.Execute(req2);
            AccountDetailStruct result = JsonConvert.DeserializeObject<AccountDetailStruct>(res2.Content);

            return result;
        }

        [Route("api/Bank/account/create/{userID}")]
        [HttpGet]
        public void CreateAccount(uint userID)
        {
            // Create the account
            RestRequest req = new RestRequest(String.Format("api/account/create/{0}", userID));
            IRestResponse res = client.Execute(req);

            // Process pending transactions
            IRestResponse processRes = client.Execute(processTransactionsRequest);
            // Save to disk
            IRestResponse saveRes = client.Execute(saveToDiskRequest);
        }

        [Route("api/Bank/user/create")]
        [HttpPost]
        public UserDetailStruct CreateUser([FromBody]UserDetailStruct newUser)
        {
            // Create the user
            RestRequest req = new RestRequest("api/user/create", Method.POST);
            req.AddJsonBody(newUser);
            IRestResponse res = client.Execute(req);
            uint userID = JsonConvert.DeserializeObject<uint>(res.Content);

            // Process pending transactions
            IRestResponse processRes = client.Execute(processTransactionsRequest);
            // Save to disk
            IRestResponse saveRes = client.Execute(saveToDiskRequest);

            // Get created user details
            RestRequest req2 = new RestRequest(String.Format("api/user/{0}", userID));
            IRestResponse res2 = client.Execute(req2);
            UserDetailStruct result = JsonConvert.DeserializeObject<UserDetailStruct>(res2.Content);

            return result;
        }

        [Route("api/Bank/user/{userID}/update")]
        [HttpPost]
        public UserDetailStruct UpdateUser(uint userID, [FromBody]UserDetailStruct user)
        {
            // Update the user
            RestRequest req = new RestRequest(String.Format("api/user/{0}/setDetails", userID), Method.POST);
            req.AddJsonBody(user);
            IRestResponse res = client.Execute(req);

            // Process pending transactions
            IRestResponse processRes = client.Execute(processTransactionsRequest);
            // Save to disk
            IRestResponse saveRes = client.Execute(saveToDiskRequest);

            // Get updated user details
            RestRequest req2 = new RestRequest(String.Format("api/user/{0}", userID));
            IRestResponse res2 = client.Execute(req2);
            UserDetailStruct result = JsonConvert.DeserializeObject<UserDetailStruct>(res2.Content);

            return result;
        }

        [Route("api/Bank/transaction/create")]
        [HttpPost]
        public TransactionDetailStruct CreateTransaction([FromBody]TransactionDetailStruct newTransaction)
        {
            // Create the transaction
            RestRequest req = new RestRequest("api/transaction/create", Method.POST);
            req.AddJsonBody(newTransaction);
            IRestResponse res = client.Execute(req);
            TransactionDetailStruct result = JsonConvert.DeserializeObject<TransactionDetailStruct>(res.Content);

            // Process the transaction
            IRestResponse processRes = client.Execute(processTransactionsRequest);

            return result;
        }
    }
}