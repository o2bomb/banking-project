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
    public class UserController : ApiController
    {
        private static BankDB.BankDB db = BankDBProvider.getInstance();
        private static UserAccessInterface access = db.GetUserAccess();

        [Route("api/user/{userID}")]
        [HttpGet]
        public UserDetailStruct GetUserDetail(uint userID)
        {
            UserDetailStruct result = new UserDetailStruct();

            access.SelectUser(userID);
            result.userID = userID;
            try
            {
                access.GetUserName(out result.firstName, out result.lastName);
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("User details could not be retreived"),
                    ReasonPhrase = "User could not be found"
                };
                throw new HttpResponseException(response);
            }
            return result;
        }

        [Route("api/User/create")]
        [HttpPost]
        public uint CreateUser([FromBody]UserDetailStruct newUser)
        {
            if (newUser.firstName.Length < 1 || newUser.lastName.Length < 1)
            {
                // User's name cannot be empty
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("User could not be created"),
                    ReasonPhrase = "User cannot have an empty first or last name"
                };
                throw new HttpResponseException(response);
            }
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

            foreach (uint ID in userIDs)
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
            if (user.firstName.Length < 1 || user.lastName.Length < 1)
            {
                // User's updated name cannot be empty
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("User details could not be updated"),
                    ReasonPhrase = "User cannot have an empty first or last name"
                };
                throw new HttpResponseException(response);
            }
            access.SelectUser(userID);
            try
            {
                access.SetUserName(user.firstName, user.lastName);
            }
            catch (Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("User details could not be updated"),
                    ReasonPhrase = "User could not be found"
                };
                throw new HttpResponseException(response);
            }
        }
    }
}