# Todo App (React + ASP.NET Core)

## Getting Started (Local Dev)

### Prerequisites

- .NET 8 SDK
- Node 18+ / PNPM or NPM
- (Optional) SQLite CLI for inspection

### Run the API

```bash
cd ezra.Server
dotnet restore
dotnet run
# API should be at https://localhost:<port>
# Swagger: https://localhost:<port>/swagger
```

The server writes SQLite to a stable folder (e.g., `/AppData/todo.db`) so your data persists across runs.

### Run the Frontend (Vite)

```bash
cd ezra.client
npm install
npm run dev
# App at https://localhost:49673 (or printed port)
```

- Vite proxies `/api/*` to the ASP.NET server during dev.

## API Endpoints

```
GET    /api/todos
GET    /api/todos/{id}
POST   /api/todos            { "title": "Task name" }
PUT    /api/todos/{id}       { "title": "New title?", "isCompleted": true? }
DELETE /api/todos/{id}
```


## Potential Roadmap
- Add user capabilities
- Dockerize the frontend and backend
- Add `docker-compose` for local parity
- Add GitHub Actions (build FE/BE, push image, optional PR checks)
- **Features**:
    - Search
    - Sort
    - Determine limits for number of ToDos
- **Tests**: Controller/service

## Potential Solutions for Scale

### Frontend
- S3 for hosting the built React app (static files)
- CloudFront in front of S3 for global caching/SSL/HTTP2

### Backend
- ECS service running the ASP.NET API containers
- Application Load Balancer (ALB) → target group (ECS tasks)
- Auto scaling on CPU/RT latency


### Database
- Use a DB that supports sharding or a NoSQL solution
