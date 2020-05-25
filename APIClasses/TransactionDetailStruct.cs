using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClasses
{
    public class TransactionDetailStruct
    {
        public uint transactionID;
        public uint senderID; // SENDER
        public uint receiverID; // RECEIVER
        public uint amount;

        public TransactionDetailStruct()
        {
            transactionID = 0;
            senderID = 0;
            receiverID = 0;
            amount = 0;
        }
    }
}
