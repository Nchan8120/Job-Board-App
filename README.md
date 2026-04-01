# JobBoard — Handy Bros Take-Home Project

A full-stack job board application where users can post jobs and express interest in them.

## Tech Stack

- **Backend:** ASP.NET Core 10 Web API
- **Frontend:** React (Vite)
- **Database:** MySQL
- **ORM:** Entity Framework Core
- **Auth:** JWT (JSON Web Tokens)

## Features

- User registration and login with role-based access (Poster / Viewer)
- Posters can create, edit, and delete their own jobs
- Viewers can browse jobs and express interest (toggle)
- Posters can see a list of users interested in their jobs
- Job listings with search and pagination
- Jobs older than 2 months are automatically hidden
- Protected routes on both frontend and backend

## Prerequisites

Before running the project make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org)
- [MySQL](https://dev.mysql.com/downloads/mysql/)

## Getting Started

### 1. Clone the repository
```bash
git clone <your-repo-url>
cd JobBoardApp
```

### 2. Set up the database

Open MySQL and create the database:
```sql
CREATE DATABASE jobboard;
```

### 3. Configure the backend

Open `JobBoard.API/appsettings.json` and update the connection string with your MySQL credentials:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=jobboard;User=root;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "JobBoardApp",
    "Audience": "JobBoardApp"
  }
}
```

### 4. Run the backend
```bash
cd JobBoard.API
dotnet ef database update
dotnet run
```

The API will start on `http://localhost:5142`.

### 5. Configure the frontend

Open `job-board-client/src/services/api.js` and make sure the `baseURL` matches your API port:
```javascript
const api = axios.create({
  baseURL: 'http://localhost:5142/api',
});
```

### 6. Run the frontend
```bash
cd job-board-client
npm install
npm run dev
```

The app will start on `http://localhost:5173`.

## API Endpoints

### Auth
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Log in and receive a JWT |

### Jobs
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| GET | `/api/jobs` | Public | Get all active jobs (supports `?search=` and `?page=`) |
| GET | `/api/jobs/{id}` | Public | Get a single job |
| POST | `/api/jobs` | Poster | Create a new job |
| PUT | `/api/jobs/{id}` | Poster (owner only) | Edit a job |
| DELETE | `/api/jobs/{id}` | Poster (owner only) | Delete a job |

### Interest
| Method | Endpoint | Access | Description |
|--------|----------|--------|-------------|
| POST | `/api/jobs/{id}/interest` | Viewer | Toggle interest on a job |
| GET | `/api/jobs/{id}/interest` | Poster (owner only) | Get list of interested users |

## Project Structure
```
JobBoardApp/
├── JobBoard.API/          # ASP.NET Core backend
│   ├── Controllers/       # API endpoints
│   ├── Services/          # Business logic
│   ├── Repositories/      # Data access
│   ├── Models/            # Database models
│   ├── DTOs/              # Request/response shapes
│   └── Data/              # EF Core DbContext
└── job-board-client/      # React frontend
    └── src/
        ├── pages/         # Page components
        ├── components/    # Shared components
        ├── context/       # Auth context
        └── services/      # API service
```