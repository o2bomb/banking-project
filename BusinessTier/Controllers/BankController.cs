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
            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                // If the server returns a NOT FOUND status code, throw a HttpResponseException
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(res.Content + ", " + res.StatusDescription),
                    ReasonPhrase = res.StatusDescription
                };
                throw new HttpResponseException(response);
            }
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
            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(res.Content + ", " + res.StatusDescription),
                    ReasonPhrase = res.StatusDescription
                };
                throw new HttpResponseException(response);
            }
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
            if (res1.StatusCode == HttpStatusCode.NotFound)
            {
                // If the server returns a NOT FOUND status code, throw a HttpResponseException
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(res1.Content + ", " + res1.StatusDescription),
                    ReasonPhrase = res1.StatusDescription
                };
                throw new HttpResponseException(response);
            }
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
            if (res1.StatusCode == HttpStatusCode.NotFound || res1.StatusCode == HttpStatusCode.BadRequest)
            {
                // If the server returns a NOT FOUND or BAD REQUEST status code, throw a HttpResponseException
                var response = new HttpResponseMessage(res1.StatusCode)
                {
                    Content = new StringContent(res1.Content + ", " + res1.StatusDescription),
                    ReasonPhrase = res1.StatusDescription
                };
                throw new HttpResponseException(response);
            }
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
            if (res.StatusCode == HttpStatusCode.NotFound || res.StatusCode == HttpStatusCode.BadRequest || res.StatusCode == HttpStatusCode.Forbidden)
            {
                // If the server returns a NOT FOUND or BAD REQUEST or FORBIDDEN status code, throw a HttpResponseException
                var response = new HttpResponseMessage(res.StatusCode)
                {
                    Content = new StringContent(res.Content + ", " + res.StatusDescription),
                    ReasonPhrase = res.StatusDescription
                };
                throw new HttpResponseException(response);
            }

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
        public AccountDetailStruct CreateAccount(uint userID)
        {
            // Create the account
            RestRequest req = new RestRequest(String.Format("api/account/create/{0}", userID));
            IRestResponse res = client.Execute(req);
            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                // If the server returns a NOT FOUND status code, throw a HttpResponseException
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(res.Content + ", " + res.StatusDescription),
                    ReasonPhrase = res.StatusDescription
                };
                throw new HttpResponseException(response);
            }
            AccountDetailStruct result = JsonConvert.DeserializeObject<AccountDetailStruct>(res.Content);

            // Process pending transactions
            IRestResponse processRes = client.Execute(processTransactionsRequest);
            // Save to disk
            IRestResponse saveRes = client.Execute(saveToDiskRequest);

            return result;
        }

        [Route("api/Bank/user/create")]
        [HttpPost]
        public UserDetailStruct CreateUser([FromBody]UserDetailStruct newUser)
        {
            // Create the user
            RestRequest req = new RestRequest("api/user/create", Method.POST);
            req.AddJsonBody(newUser);
            IRestResponse res = client.Execute(req);
            if (res.StatusCode == HttpStatusCode.BadRequest)
            {
                // If the server returns a BAD REQUEST status code, throw a HttpResponseException
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(res.Content + ", " + res.StatusDescription),
                    ReasonPhrase = res.StatusDescription
                };
                throw new HttpResponseException(response);
            }
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
            if (res.StatusCode == HttpStatusCode.BadRequest || res.StatusCode == HttpStatusCode.NotFound)
            {
                // If the server returns a BAD REQUEST status code, throw a HttpResponseException
                var response = new HttpResponseMessage(res.StatusCode)
                {
                    Content = new StringContent(res.Content + ", " + res.StatusDescription),
                    ReasonPhrase = res.StatusDescription
                };
                throw new HttpResponseException(response);
            }

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
            if (res.StatusCode == HttpStatusCode.BadRequest)
            {
                // If the server returns a BAD REQUEST status code, throw a HttpResponseException
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(res.Content + ", " + res.StatusDescription),
                    ReasonPhrase = res.StatusDescription
                };
                throw new HttpResponseException(response);
            }
            TransactionDetailStruct result = JsonConvert.DeserializeObject<TransactionDetailStruct>(res.Content);

            // Process the transaction
            IRestResponse processRes = client.Execute(processTransactionsRequest);

            return result;
        }
    }
}