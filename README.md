# 🧺 SurveyBasket

SurveyBasket is a powerful and flexible Web API built with **.NET 9** and **ASP.NET Core**, designed for creating, managing, and analyzing surveys and polls. Whether you're a business, an educational institution, or a researcher, SurveyBasket makes data collection easy, secure, and insightful.

---

## 🎯 Objective

SurveyBasket provides a backend platform that enables:

- ✅ Creating and managing custom surveys and polls.
- 📥 Collecting structured responses.
- 🔐 Ensuring secure access and efficient performance for all users.
- 📊 Generating analytical insights through detailed voting statistics.


---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core API (.NET 9)
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** JWT + Refresh Tokens
- **Mapping:** Mapster
- **Background Jobs:** Hangfire
- **Validation:** FluentValidation
- **Caching:** Hybrid Cache 
- **Documentation:** Scalar
- 

---

## 🔑 Authentication & Security

- 🔐 JWT-based authentication
- 🔁 Refresh Token implementation
- 📧 Email confirmation & password reset flows
- 🧑‍🤝‍🧑 Account and role management
- 🚦 Rate Limiting to prevent abuse

---

## 📚 Key Features

| Feature                   | Description                                                                 |
|---------------------------|-----------------------------------------------------------------------------|
| ✅ User Management         | Register, login, change/reset password, confirm email                      |
| 📊 Surveys & Polls        | Create, update, delete surveys and collect responses                        |
| 📝 Audit Logging          | Track user actions for transparency                                         |
| 🚨 Global Error Handling  | Centralized exception middleware with Result pattern                        |
| 🔍 Search, Filter, Sort   | Query and organize data effectively                                         |
| 📈 Pagination             | Manage large datasets with ease                                             |
| 🛠️ Hangfire Jobs         | Background jobs for emails and scheduled tasks                              |
| 🔄 Mapster                | Clean mapping between DTOs and domain models                                |
| 🩺 Health Checks          | Monitor app health and availability                                         |
| 📦 API Versioning         | Support backward compatibility for evolving APIs                            |
| 🚦 CORS                   | Secure cross-origin requests handling                                       |

---

## 🏗️ Architecture

- ✅ Generic Repository + Unit of Work
- ✅ Result Pattern for consistent responses
- ✅ Separation of Concerns using SOLID principles

---

## 🗃️ Database

- Code-First Migrations via EF Core
- Seeded initial data (roles +  users)
- Proper entity relationships

