using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BankDB;

namespace DataTier.Controllers
{
    public class AdminController : ApiController
    {
        private static BankDB.BankDB db = new BankDB.BankDB();
        
        [Route("api/Admin/processTransactions")]
        [HttpGet]
        public void DoTransactionProcess()
        {
            db.ProcessAllTransactions();
        }

        [Route("api/Admin/save")]
        [HttpGet]
        public void DoSaveToDisk()
        {
            db.SaveToDisk();
        }
    }
}