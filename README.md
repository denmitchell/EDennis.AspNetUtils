# EDennis.AspNetUtils
*A library that facilitates development and testing of ASP.NET applications*

## Features
The most notable features of this library are:
- **Simple Authorization Classes and UI** that provide the ability to manage users and roles -- with two simple tables -- independent of user authentication.  In a world where authentication is becoming increasingly easy to outsource (e.g., with the Microsoft Authentication Library), it only makes sense to rework custom application security to focus on authorization.  This library demonstrates how to do that.  The library also caches user roles (for performance, but with configurable expiration) and allows the developer to configure support for single or multiple roles per user without a change to the database schema. 
- **Fake Authentication Classes and Launch Settings** that make it easy to test different roles.
- **Generic CrudService Repository Class** that wraps basic Entity Framework operations but also includes support for Dynamic Linq Expressions and automatically updating the user property of all tracked and modified entities.
- **DbContextService Class** that facilitates swapping out a normal DbContext instance for a DbContext instance that is more suitable for testing (e.g., one with an open transaction that gets rolled back automatically when the instance is disposed)

## Sample Projects and Tests
In addition to the library project, there are three supporting sample/test projects:
- **Blazor Sample** that has sample pages for managing artists, songs, and application users using Radzen components
- **MVC Sample** that has sample pages for managing application users
- **Test Project** that demonstrates how to test CRUD operations with automatic rollback of transactions

## Setup
- Clone the repository
- Register your applications in Azure AD.  See the [Microsoft Documentation](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
  -- You can get a developer instance of Azure AD through Microsoft: [Microsoft 365 Develop Program](https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/how-to-create-a-free-developer-account)
  -- When creating a new Visual Studio Project with Microsoft Authentication Library security, Visual Studio will retrieve the tenant id, client id (etc.) and put this information in appsettings.json.  
  -- Especially for testing purposes, you can point multiple projects to the same registered application in Azure AD.
- Add the AzureAd section to configuration in one of two ways:
  -- Uncomment the existing AzureAd section in appsettings.json and update the values with your registered application credentials.  
  -- Alternatively, copy the AzureAd configuration JSON, wrap that JSON in curly brackets, minify the JSON so that there are no line breaks or tabs, and place the minified JSON in the following two environment variables.  (Note: I did this so that I wouldn't commit my application credentials to git.)
     --- EDennis.AspNetUtils.Tests.BlazorSample.Configuration
     --- EDennis.AspNetUtils.Tests.MvcSample.Configuration
