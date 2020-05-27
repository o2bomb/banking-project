# Three-tier Web Service Application
A .NET web application that consists of two RESTful web interfaces and a MVC web app

## Features
- Create new users, update, and query for existing users
- Create multiple accounts for each user, deposit and withdraw from accounts, and list all of the accounts for a given user
- Create new transactions, and list all of the transactions for a given account
- Complete exception handling between the presentation, business, and data tier

## Project structure
- **/BusinessTier** contains the web app client and a RESTful api (business tier) for the client to access
- **/DataTier** contains a RESTful api that exposes various database operations
- **/APIClasses** contains class definitions that are utilized by both the /BusinessTier and /Datatier
- **BankDB.dll** the dll that provides the core database functionality
