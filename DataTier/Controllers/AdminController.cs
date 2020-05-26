using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataTier.Models;

namespace DataTier.Controllers
{
    public class AdminController : ApiController
    {
        private static BankDB.BankDB db = BankDBProvider.getInstance();
        
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