# Task Management System

## Getting Started

## Table of Contents
- [Prerequisites](#prerequisites)
- [Backend Setup](#backend-setup)
- [Frontend Setup](#frontend-setup)
- [Tech Stack](#tech-stack)
- [Additional Information](#additional-information)

## Prerequisites
Make Sure You Have The Following Installed On Your Machine:
- .NET SDK (Version 6.0 or Later)
- Node.js (Version 14.0 or Later)
- NPM (Node Package Manager)
- SQL Server (Or Other Preferred Database)

## Backend Setup
### 1. Clone The Repository:
```bash
git clone https://github.com/UmairNadeem76/Task-Management-System
cd Task-Management-System
```

### 2. Configure The Database:
- Update The `appsettings.json` File With Your Database Connection String.
- Example `appsettings.json`:
  ```json
  {
    "ConnectionStrings": {
      "Default": "Server=GAMINGPC\\SQLEXPRESS; Database=CustomAuth; Trusted_Connection=True; MultipleActiveResultSets=true; TrustServerCertificate=True;"
    },
    ...
  }
  ```

### 3. Run Entity Framework Migrations:
```bash
cd CustomAuth
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Install Dependencies:
```bash
dotnet restore
```

### 5. Running The Backend
```bash
dotnet run
```
The Backend API Will Be Available At `http://localhost:5191`


## Frontend Setup
### 1. Navigate To The Frontend Directory:
```bash
cd ../new-frontend
```

### 2. Install Dependencies:
```bash
npm install
```


### 3. Running The Frontend
```bash
npm start
```
The Frontend Will Be Available At `http://localhost:3000/`


# Tech Stack:
Following Tech Stack Is Being Implemented:
- React + Typescript For Frontend
- ASP.NET Core Web Api
- SQL Server Management Studio For Database
- Redux For State Management In React
- Serilog For Application Logging
- xUnit For Unit Testing
- SonarQube For Analyzing Code Quality (To Be Implemented)

## Additional Information
TBD