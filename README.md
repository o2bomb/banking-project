# Three-tier Web Service Application
A .NET web application that consists of two RESTful web interfaces and a MVC web app

## Project structure
- **/BusinessTier** contains the web app client and a RESTful api (business tier) for the client to access
- **/DataTier** contains a RESTful api that exposes various database operations
- **/APIClasses** contains class definitions that are utilized by both the /BusinessTier and /Datatier
- **BankDB.dll** the dll that provides the core database functionality
