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
    public class TransactionController : ApiController
    {
        private static BankDB.BankDB db = new BankDB.BankDB();
        private static TransactionAccessInterface access = db.GetTransactionInterface();

        [Route("api/Transaction/{transactionID}")]
        [HttpGet]
        public TransactionDetailStruct GetTransaction(uint transactionID)
        {
            TransactionDetailStruct result = new TransactionDetailStruct();

            access.SelectTransaction(transactionID);
            result.transactionID = transactionID;
            result.senderID = access.GetSendrAcct();
            result.receiverID = access.GetRecvrAcct();
            result.amount = access.GetAmount();
            return result;
        }

        [Route("api/Transaction/all")]
        [HttpGet]
        public List<TransactionDetailStruct> GetAllTransactions()
        {
            List<uint> transactionIDs = access.GetTransactions();
            List<TransactionDetailStruct> result = new List<TransactionDetailStruct>();

            foreach(uint ID in transactionIDs)
            {
                TransactionDetailStruct entry = new TransactionDetailStruct();

                access.SelectTransaction(ID);
                entry.transactionID = ID;
                entry.senderID = access.GetSendrAcct();
                entry.receiverID = access.GetRecvrAcct();
                entry.amount = access.GetAmount();
                result.Add(entry);
            }
            return result;
        }

        [Route("api/Transaction/create")]
        [HttpPost]
        public TransactionDetailStruct CreateTransaction([FromBody]TransactionDetailStruct newTransaction)
        {
            TransactionDetailStruct result = new TransactionDetailStruct();

            result.transactionID = access.CreateTransaction();
            access.SelectTransaction(result.transactionID);
            access.SetSendr(newTransaction.senderID);
            access.SetRecvr(newTransaction.receiverID);
            access.SetAmount(newTransaction.amount);

            result.senderID = access.GetSendrAcct();
            result.receiverID = access.GetRecvrAcct();
            result.amount = access.GetAmount();
            return result;
        }
    }
}