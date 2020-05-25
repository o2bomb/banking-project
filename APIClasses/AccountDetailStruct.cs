using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClasses
{
    public class AccountDetailStruct
    {
        public uint accountID;
        public uint userID;
        public uint balance;

        public AccountDetailStruct()
        {
            accountID = 0;
            userID = 0;
            balance = 0;
        }
    }
}
