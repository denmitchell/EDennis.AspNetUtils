# EDennis.AspNetUtils
*A library that facilitates development and testing of ASP.NET applications*

## Features
The most notable features of this library are:
- **Generic ICrudService Repository Interface** with implementations for Entity Framework and API Clients.  The service exposes CRUD operations and includes support for flexible querying/paging via Dynamic Linq Expressions.  The Entity Framework implementation automatically updates a username (SysUser) property of all tracked and modified entities.  The ICrudService interface makes it much easier to implement Blazor UI components that can work with Blazor Server or Web Assembly.
- **Simple Authorization Classes and UI** that provide the ability to manage users and roles -- with two simple tables -- independent of user authentication.  In a world where authentication is becoming increasingly easy to outsource (e.g., with the Microsoft Authentication Library), it only makes sense to rework custom application security to focus on authorization.  This library demonstrates how to do that.  The library also caches user roles (for performance, but with configurable expiration) and allows the developer to configure support for single or multiple roles per user without a change to the database schema. 
- **Fake Authentication Classes and Launch Settings** that make it easy to test different roles.
- **DbContextService Class** that facilitates swapping out a normal DbContext instance for a DbContext instance that is more suitable for testing (e.g., one with an open transaction that gets rolled back automatically when the instance is disposed)

## Sample Projects and Tests
In addition to the library project, there are three supporting sample/test projects:
- **Blazor Sample** that has sample pages for managing artists, songs, and application users using Radzen components
- **MVC Sample** that has sample pages for managing application users
- **Test Project** that demonstrates how to test CRUD operations with automatic rollback of transactions

## Setup
- Clone the repository
- Register your applications in Azure AD.  See the [Microsoft Documentation](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
  - You can get a developer instance of Azure AD through Microsoft: [Microsoft 365 Develop Program](https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/how-to-create-a-free-developer-account)
  - When creating a new Visual Studio Project with Microsoft Authentication Library security, Visual Studio will retrieve the tenant id, client id (etc.) and put this information in appsettings.json.  
  - Especially for testing purposes, you can point multiple projects to the same registered application in Azure AD.
- To run the sample projects, you will need to create an appsettings.json in the Blazor Client project and uncomment and update Azure AD sections in other appsettings files.
