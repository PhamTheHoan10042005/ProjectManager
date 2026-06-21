# Front-end — Hệ thống quản lý dự án & phân công công việc

Vue 3 + TypeScript + Vite, gọi API tới 3 microservice (hoặc qua Ocelot API Gateway).

## Kiến trúc kết nối

| Service | Port mặc định | Chức năng FE |
|---------|---------------|--------------|
| **Notify Service** (N3) | `5286` | JWT login/register, bình luận, thông báo, activity log |
| **Project Service** (N1) | `5116` | CRUD dự án, thành viên, sprint |
| **Task Service** (N2) | `5027` | Kanban board, task, sub-task, time log |
| **API Gateway** (Ocelot) | `5114` | Điểm vào duy nhất (tùy chọn) |

## Chạy development

```bash
npm install
npm run dev
```

FE chạy tại **http://localhost:5173**.

Trong dev, Vite proxy tự động chuyển request:

- `/project-api/*` → Project Service
- `/task-api/*` → Task Service
- `/notify-api/*` → Notify Service

## Kịch bản 2: 3 service trên các máy LAN khác

1. Trên **mỗi máy backend**: chạy service (`dotnet run`), mở firewall cho port tương ứng.
2. Trên **máy FE**: sửa `FE/.env.development` — thay `192.168.1.10/11/12` bằng IP LAN thực (`ipconfig`).
3. Chạy FE: `npm run dev` → mở `http://localhost:5173` hoặc `http://<IP-máy-FE>:5173`.

## Link luôn chạy (Windows)

**Cách 1 — Chạy thủ công, tự restart khi crash** (máy FE):

```powershell
cd D:\BTL\BTL-T3
powershell -ExecutionPolicy Bypass -File scripts\windows\start-fe-always.ps1
```

Link cố định: `http://<IP-LAN-máy-FE>:5173` (sau khi sửa `FE/.env.production`).

Trên máy N1/N2/N3 tương ứng:

```powershell
powershell -ExecutionPolicy Bypass -File scripts\windows\start-project-service.ps1
powershell -ExecutionPolicy Bypass -File scripts\windows\start-task-service.ps1
powershell -ExecutionPolicy Bypass -File scripts\windows\start-notify-service.ps1
```

**Cách 2 — Tự chạy khi Windows bật** (PowerShell **Admin**):

```powershell
cd D:\BTL\BTL-T3\scripts\windows
.\register-autostart.ps1 -Role fe        # máy FE
.\register-autostart.ps1 -Role project   # máy N1
.\register-autostart.ps1 -Role task      # máy N2
.\register-autostart.ps1 -Role notify    # máy N3
```

Dev + proxy (hot-reload): dùng `-Role fe-dev` hoặc `start-fe-dev-always.ps1`.

## Cấu hình (.env)

Copy `.env.example` thành `.env.development`:

```env
VITE_USE_GATEWAY=false
VITE_GATEWAY_API=http://localhost:5114
```

## Build production

```bash
npm run build
npm run preview
```
