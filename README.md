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
