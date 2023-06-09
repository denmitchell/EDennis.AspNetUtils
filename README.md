# EDennis.AspNetUtils
*A library that facilitates development and testing of ASP.NET applications*

## Features
The most notable features of this library are:
- **Generic ICrudService Repository Interface** with implementations for Entity Framework and API Clients.  The service exposes CRUD operations and includes support for flexible querying/paging via Dynamic Linq Expressions.  The Entity Framework implementation automatically updates a username (SysUser) property of all tracked and modified entities.  The ICrudService interface makes it much easier to implement Blazor UI components that can work with Blazor Server or Web Assembly.

<img
  src="https://github.com/denmitchell/EDennis.AspNetUtils/blob/7ac546cf454c1deeb1a79328ce1383ab1a6ff182/ICrudService.png"
  alt="ICrudService"
  title="ICrudService"
  style="display: inline-block; margin: 0 auto; max-width: 300px">

- **Simple Authorization Classes and UI** that provide the ability to manage users and roles -- with two simple tables -- independent of user authentication.  In a world where authentication is becoming increasingly easy to outsource (e.g., with the Microsoft Authentication Library), it only makes sense to rework custom application security to focus on authorization.  This library demonstrates how to do that.  The library also caches user roles (for performance, but with configurable expiration) and allows the developer to configure support for single or multiple roles per user without a change to the database schema. 

<img
  src="https://github.com/denmitchell/EDennis.AspNetUtils/blob/197f768143705ec43eee92a244123bd814a2aed0/SimpleAuthorizationMvc.PNG"
  alt="SimpleAuthorization for MVC"
  title="SimpleAuthorization for MVC"
  style="display: inline-block; margin: 0 auto; max-width: 300px">
  
<img
  src="https://github.com/denmitchell/EDennis.AspNetUtils/blob/948d417ead5b82281ea84c6514b099b496fb1650/SimpleAuthorizationBlazorServer.PNG"
  alt="SimpleAuthorization for Blazor Server"
  title="SimpleAuthorization for Blazor Server"
  style="display: inline-block; margin: 0 auto; max-width: 300px">

<img
  src="https://github.com/denmitchell/EDennis.AspNetUtils/blob/197f768143705ec43eee92a244123bd814a2aed0/SimpleAuthorizationWasm.PNG"
  alt="SimpleAuthorization for Blazor Web Assembly"
  title="SimpleAuthorization for Blazor Web Assembly"
  style="display: inline-block; margin: 0 auto; max-width: 300px">
  
- **Fake Authentication Classes and Launch Settings** that make it easy to test different roles.
- **DbContextService Class** that facilitates swapping out a normal DbContext instance for a DbContext instance that is more suitable for testing (e.g., one with an open transaction that gets rolled back automatically when the instance is disposed)

## Sample Projects and Tests
In addition to the library (AspNetUtils and AspNetUtils.Core -- with the latter supporting WASM clients), there are serveral supporting sample/test projects:
- **...BlazorSample.Shared** -- A Razor component library that has sample pages for managing artists, songs, and application users using Radzen components.  The pages utilize ICrudService so that they can be used interchangeably with Blazor Server and Blazor Web Assembly projects.
- **...BlazorSample.BS** -- A Blazor Server project that uses the Shared project's pages
- **...BlazorSample.WA.Server/Client** -- A Blazor Web Assembly project pair that uses the Shared project's pages 
- **...MvcSample** -- An MVC application that has sample pages for managing application users
- **...Tests** -- An Xunit test project that demonstrates how to test CRUD operations with automatic rollback of transactions

## Setup
- Clone the repository
- Register your applications in Azure AD.  See the [Microsoft Documentation](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
  - You can get a developer instance of Azure AD through Microsoft: [Microsoft 365 Develop Program](https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/how-to-create-a-free-developer-account)
  - When creating a new Visual Studio Project with Microsoft Authentication Library security, Visual Studio will retrieve the tenant id, client id (etc.) and put this information in appsettings.json.  
  - Especially for testing purposes, you can point multiple projects to the same registered application in Azure AD.
- To run the sample projects, you will need to create an appsettings.json in the Blazor Client project and uncomment and update Azure AD sections in other appsettings files.
