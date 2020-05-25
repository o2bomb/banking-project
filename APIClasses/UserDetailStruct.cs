using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClasses
{
    public class UserDetailStruct
    {
        public uint userID;
        public string firstName;
        public string lastName;

        public UserDetailStruct()
        {
            userID = 0;
            firstName = "";
            lastName = "";
        }
    }
}
