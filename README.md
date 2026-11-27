# Web-ChamThi (Online Judge & AI Rubric Grading)

Hệ thống thi và chấm điểm tự động kết hợp **Docker Sandbox** (chấm code C++, Python, Java...) và **AI Rubric** (gợi ý chấm tự luận có kiểm soát của giáo viên).

## Cấu trúc thư mục
- `backend/WebChamThi.API`: ASP.NET Core 9 Web API (quản lý đề thi, Queue/Worker, Docker.DotNet, SignalR, AI Grader).
- `frontend`: ReactJS + TypeScript + Vite (Monaco Editor, SignalR client, giao diện duyệt điểm).
- `docker-images`: Các Dockerfile tối giản, chạy dưới quyền non-root (`runner`) dùng làm Sandbox chấm code.

## Chạy nhanh

### 1. Backend (.NET 9)
```powershell
cd D:\Web-ChamThi\backend\WebChamThi.API
dotnet run
```

### 2. Frontend (React + Vite)
```powershell
cd D:\Web-ChamThi\frontend
npm install
npm run dev
```

### 3. Build Docker Sandbox (tùy chọn khi bắt đầu làm Online Judge)
```powershell
cd D:\Web-ChamThi\docker-images
docker build -f Dockerfile.cpp -t sandbox-cpp .
docker build -f Dockerfile.python -t sandbox-python .
```

