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
    public class UserController : ApiController
    {
        private static BankDB.BankDB db = new BankDB.BankDB();
        private static UserAccessInterface access = db.GetUserAccess();

        [Route("api/user/{userID}")]
        [HttpGet]
        public UserDetailStruct GetUserDetail(uint userID)
        {
            UserDetailStruct result = new UserDetailStruct();

            access.SelectUser(userID);
            result.userID = userID;
            access.GetUserName(out result.firstName, out result.lastName);
            return result;
        }

        [Route("api/User/create")]
        [HttpPost]
        public uint CreateUser([FromBody]UserDetailStruct newUser)
        {
            UserDetailStruct result = new UserDetailStruct();

            result.userID = access.CreateUser();
            access.SelectUser(result.userID);
            access.SetUserName(newUser.firstName, newUser.lastName);

            return result.userID;
        }

        [Route("api/User/all")]
        [HttpGet]
        public List<UserDetailStruct> GetAllUsers()
        {
            List<uint> userIDs = access.GetUsers();
            List<UserDetailStruct> result = new List<UserDetailStruct>();

            foreach(uint ID in userIDs)
            {
                UserDetailStruct entry = new UserDetailStruct();

                access.SelectUser(ID);
                entry.userID = ID;
                access.GetUserName(out entry.firstName, out entry.lastName);
                result.Add(entry);
            }
            return result;
        }

        [Route("api/User/{userID}/setDetails")]
        [HttpPost]
        public void SetUserDetails(uint userID, [FromBody]UserDetailStruct user)
        {
            access.SelectUser(userID);
            access.SetUserName(user.firstName, user.lastName);
        }
    }
}