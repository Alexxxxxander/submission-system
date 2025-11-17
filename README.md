# Form Submission System

Web application for submitting forms with dynamic data structure. Backend is schema-agnostic — any form with any fields can be connected without code changes.

## What's Implemented

**Order form** with 7 fields of different types (text, dropdown, number, date, radio buttons, checkbox) and validation.

**REST API** for creating, retrieving, and searching submissions. Form data is stored as JSON.

**Submissions list** with content search.

**Part 2** - architecture for handling large files (~100MB) is described in `docs/large-files-architecture.md`.

## Technologies

- **Backend**: ASP.NET Core 9 + EF Core In-Memory
- **Frontend**: Vue 3 + Vite
- **Tests**: xUnit (83 backend), Vitest (frontend)
- **Docker**: multi-stage builds, nginx

## Running

### Docker (recommended)

```bash
docker-compose up --build
```

- Frontend: http://localhost:8080
- Backend: http://localhost:5000

### Locally

**Backend:**
```bash
cd backend/FormSubmissionSystem.Api
dotnet run --urls "http://localhost:5000"
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```

Opens at http://localhost:5173

## API

### Create Submission
```http
POST http://localhost:5000/api/submissions
Content-Type: application/json

{
  "formType": "order",
  "data": {
    "fullName": "John Doe",
    "product": "laptop",
    "quantity": 2,
    "deliveryDate": "2024-12-01",
    "address": "123 Main St, New York, NY 10001",
    "paymentMethod": "card",
    "agreeTerms": true
  }
}
```

### Get All Submissions
```http
GET http://localhost:5000/api/submissions
```

### Search Submissions
```http
GET http://localhost:5000/api/submissions?search=laptop
```

### Get Submission by ID
```http
GET http://localhost:5000/api/submissions/{id}
```

## Tests

**Backend:**
```bash
cd backend
dotnet test
```
83 tests - entities, value objects, use cases, repository.

**Frontend:**
```bash
cd frontend
npm test
```
Form validation tests.
