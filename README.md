## 📌 Overview  
**GameZone** is a modern **E-Commerce Web Application** specializing in video games. The project follows the **Onion Architecture** and adheres to **Clean Code** principles.  

## 🏗️ Architecture  
The application is structured following **Onion Architecture**, ensuring high maintainability and separation of concerns:  
- **Presentation Layer**: Handles the UI and user interactions.  
- **Application Layer**: Business logic, services, and use cases.  
- **Infrastructure Layer**: Database access, external APIs, authentication, and repositories.  
- **Domain Layer**: Core domain models and business rules.  

## 🔑 Features  
✅ **Admin & User Authorization**  
✅ **Authentication & Authorization (Identity & Roles, Google Authentication)**  
✅ **User Management (Manage User Accounts, Delete/Lock Users, Role Assignment)**  
✅ **Product & Category Management (CRUD Operations for Games, Categories, and Devices)**  
✅ **User Service Management (Delete, Lock, or Modify User Accounts)**  
✅ **AutoMapper for Object Mapping**  
✅ **Clean & Maintainable Code (SOLID Principles, Repository Pattern, Dependency Injection)**  

## 🔐 Authentication & Authorization  
- Uses **ASP.NET Identity** for user authentication.  
- **Google Authentication** integration for seamless login.  
- User roles:  
  - **Admin**: Can manage games, orders, categories, and users.  
  - **User**: Can browse games, place orders, and manage their cart.  

## 📦 Technologies Used  
- **ASP.NET Core MVC**  
- **Entity Framework Core**  
- **SQL Server**  
- **Bootstrap & jQuery**  
- **Google Authentication**  
- **Session Management**  
- **Repository Design Pattern**  
- **SOLID Principles**  
- **Dependency Injection**  
- **Onion Architecture**  

## 🛠️ Models  
- **Game**  
- **Category**  
- **GameDevice**  
- **ApplicationUser** (Extends IdentityUser for User Management)  

## 🚀 Getting Started  
### ✅ Prerequisites  
- .NET 8+  
- SQL Server  
- EF Core  
- LINQ  
- C#

GameZone is designed to be **scalable**, **secure**, and **maintainable**, making it the perfect solution for an online game store. 🎮🚀
