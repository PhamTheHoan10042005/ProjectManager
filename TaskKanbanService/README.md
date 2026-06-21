# Task & Kanban Service - Nhom 2

Backend microservice quan ly task, kanban board, sub-task va log thoi gian.

## Chay bang Docker

```powershell
cd TaskKanbanService
docker compose up --build
```

- API: http://localhost:5002
- Swagger: http://localhost:5002/swagger
- RabbitMQ Management: http://localhost:15672 (guest/guest)

## Chay local (can SQL Server + RabbitMQ)

```powershell
cd TaskKanbanService/TaskKanban.API
dotnet ef database update
dotnet run
```

## API chinh

| Method | Endpoint | Mo ta |
|--------|----------|-------|
| GET | `/api/projects/{projectId}/kanban` | Lay kanban board |
| GET | `/api/projects/{projectId}/tasks` | Danh sach task |
| POST | `/api/projects/{projectId}/tasks` | Tao task |
| PATCH | `/api/projects/{projectId}/tasks/{id}/status` | Doi trang thai |
| PATCH | `/api/projects/{projectId}/tasks/{id}/move` | Keo tha kanban |
| PATCH | `/api/projects/{projectId}/tasks/{id}/assign` | Gan nguoi phu trach |
| GET/POST | `/api/projects/{projectId}/tasks/{id}/subtasks` | Sub-task |
| GET/POST | `/api/projects/{projectId}/tasks/{id}/timelogs` | Log gio lam viec |

## Events (RabbitMQ)

- `TaskStatusChangedEvent` - khi doi trang thai task
- `TaskAssignedEvent` - khi gan nguoi phu trach

Nhom 3 (Comment & Notify Service) consume cac event nay de gui thong bao.

## Luu y tich hop

- JWT key/Issuer/Audience phai trung voi Nhom 3 va API Gateway
- `projectId` tham chieu tu Project Service (Nhom 1), khong luu du lieu project o day
