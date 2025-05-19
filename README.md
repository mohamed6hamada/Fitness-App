
# 💪 FitnessAPPMVCSol

**A scalable, layered fitness management system built with ASP.NET Core MVC.**


## 📚 Table of Contents

- [🧭 Overview](#-overview)
- [🧩 Key Components](#-key-components)
  - [🔹 FitnessAPPMVC (Web Application)](#-fitnessappmvc-web-application)
  - [🔹 FitnessApp.DAL (Data Access Layer)](#-fitness-appdal-data-access-layer)
  - [🔹 FitnessApp.BL (Business Logic Layer)](#-fitness-appbl-business-logic-layer)
- [🛠 Technologies Used](#-technologies-used)
- [📁 Project Structure](#-project-structure)
- [✨ Key Features](#-key-features)
- [🚀 Getting Started](#-getting-started)
- [🤝 Contributing](#-contributing)
- [📬 Contact](#-contact)

---

## 🧭 Overview

**FitnessAPPMVCSol** is a robust fitness application built using **ASP.NET Core MVC** and follows a **clean layered architecture** to ensure scalability, separation of concerns, and maintainability.



## 🧩 Key Components

### 🔹 FitnessAPPMVC (Web Application Layer)

- **Controllers**:
  - AdminController – Admin functionality
  - CoachController – Coach tools & views
  - ClientController – Client interfaces
  - AccountController – User registration & login
  - ProfileController – Profile management
  - HomeController – Application entry point
- **Views** – Razor UI components and templates
- **Models** – Includes view models like "ErrorViewModel"
- **Program.cs** – Configures:
  - Entity Framework Core
  - ASP.NET Identity
  - Routing & services
- **appsettings.json** – Application & DB settings



### 🔹 Fitness App.DAL (Data Access Layer)

- **Entities**:
  - ApplicationUser, Coach, WorkoutPlan, ClientCoachSubscription
  - ProgressLog, Blogs, Comment, Feedback, Exercises, DietPlans
- **DbContext** – Manages database communication
- **Repositories** – Handles CRUD operations
- **Interfaces** – Abstractions for services & repositories



### 🔹 Fitness App.BL (Business Logic Layer)

- **Services**:
  - ClientCoachSubscriptionService, WorkoutPlanService
  - BlogService, CommentService, DietPlanService, and more
- **ViewModels** – Data models for the presentation layer
- **Validations** – Business rule enforcement
- **MappingProfiles** – Uses **AutoMapper** for efficient mapping



## 🛠 Technologies Used

- ✅ **.NET 8.0**
- 🗃️ **Entity Framework Core**
- 🔐 **ASP.NET Core Identity**
- 🔄 **AutoMapper**
- 💳 **Stripe.NET** – Payment integration
- 🔧 **Newtonsoft.Json** – JSON processing



## 📁 Project Structure

```plaintext
FitnessAPPMVCSol/
├── FitnessAPPMVC/
│   ├── Controllers/
│   ├── Views/
│   ├── Models/
│   ├── Program.cs
│   └── appsettings.json
├── Fitness App.DAL/
│   ├── Models/
│   ├── DbContext/
│   ├── Repositories/
│   └── Interfaces/
└── Fitness App.BL/
    ├── Services/
    ├── ViewModels/
    ├── Validations/
    └── MappingProfiles/
```




## ✨ Key Features

- 🔐 **Secure Authentication & Authorization** with ASP.NET Identity
- 🧑‍🤝‍🧑 **Role Management** for Admin, Coach, and Client
- 💳 **Stripe Payment Integration** for subscriptions
- 🏋️‍♂️ **Workout & Diet Plan Management**
- 📈 **Progress Tracking & Logs**
- ✍️ **Blogging & Comment System**



## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server / LocalDB
- Visual Studio 2022 (recommended)

### 🔧 Installation Steps

1. **Clone the Repository**  
   bash
   git clone (https://github.com/mohamed6hamada/Fitness-App.git)


2. **Navigate to Project Directory**

   bash
   cd FitnessAPPMVCSol
   

3. **Restore Dependencies**

   bash
   dotnet restore
   

4. **Configure the Connection String**
   Update `appsettings.json` as needed.

5. **Run the Application**

   bash
   dotnet run
   



## 🤝 Contributing

Contributions are welcome! Please fork the repo and submit a pull request. Let’s make this project better together 💡



## 📬 Contact

For questions, suggestions, or feedback:
📧 [zeyadgamal2003@gmail.com](mailto:zeiadgamal2003@gmail.com)

