# 🏨 TripUp — Nền Tảng Quản Lý Khách Sạn, Đặt Phòng & Mạng Xã Hội Trải Nghiệm Du Lịch

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![Qdrant](https://img.shields.io/badge/Qdrant-Vector_DB-FF4B4B?style=for-the-badge&logo=qdrant&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-Realtime-orange?style=for-the-badge)
![Ocelot](https://img.shields.io/badge/Ocelot-API_Gateway-00B4D8?style=for-the-badge)

**TripUp** là một hệ sinh thái Travel-Tech toàn diện được xây dựng trên kiến trúc **Microservices** hướng sự kiện (Event-Driven), sử dụng **.NET 8**, **Clean Architecture**, **Domain-Driven Design (DDD)** và **CQRS**. 

Không chỉ dừng lại ở một nền tảng đặt phòng khách sạn thông thường, TripUp tích hợp sâu hệ thống quản lý vận hành khách sạn (PMS), mạng xã hội chia sẻ trải nghiệm du lịch (Travel Social Network), hệ thống chat & hỗ trợ khách hàng thời gian thực (SignalR + AI Assistant) và động cơ gợi ý thông minh dựa trên tìm kiếm ngữ nghĩa (RAG + Qdrant Vector DB).

</div>

---

## 📋 Mục lục

- [Tổng quan kiến trúc](#-tổng-quan-kiến-trúc)
- [Danh sách Microservices](#-danh-sách-microservices)
- [Tính năng cốt lõi](#-tính-năng-cốt-lõi)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cấu trúc thư mục dự án](#-cấu-trúc-thư-mục-dự-án)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Hướng dẫn cài đặt & chạy](#-hướng-dẫn-cài-đặt--chạy)
- [Cấu hình môi trường](#-cấu-hình-môi-trường)
- [API Endpoints](#-api-endpoints)
- [Giao tiếp giữa các Services](#-giao-tiếp-giữa-các-services)
- [Đóng góp](#-đóng-góp)
- [Bản quyền](#-bản-quyền)

---

## 🏗 Tổng quan kiến trúc

```
                               ┌────────────────────────────────────────┐
                               │     Client App (Web/Mobile/SignalR)    │
                               └───────────────────┬────────────────────┘
                                                   │ HTTP / WebSockets
                               ┌───────────────────▼────────────────────┐
                               │     API Gateway - Ocelot (Port 7000)   │
                               └─┬─────────┬─────────┬─────────┬──────┬─┘
                                 │         │         │         │      │
     ┌───────────────────────────┼─────────┼─────────┼─────────┼──────┴────────────────────────────┐
     │                           │         │         │         │                                   │
┌────▼──────┐              ┌─────▼─────┐ ┌─▼───────┐┌▼────────┐┌▼─────────┐                     ┌──▼───────┐
│AuthService│              │HotelWeekly│ │Booking  ││Payment  ││Social    │                     │ChatHub   │
│(Port 7001)│              │Catalog    │ │(7003)   ││(7004)   ││(7005)    │                     │& Chat    │
└───────────┘              │(7002)     │ └─────────┘└─────────┘└──────────┘                     │(7006)    │
                           └───────────┘                                                        └──────────┘
                                                                                                      │
┌───────────┐                                                                                         │
│NotifHub & │◄────────────────────────────────────────────────────────────────────────────────────────┘
│Notification│
│(7007)     │              ┌────────────────────────────────────────┐
└───────────┘              │          RecommendationService         │
                           │   - Gợi ý & Tìm kiếm ngữ nghĩa (7008)  │
                           │   - gRPC / HTTP / Ollama / OpenAI      │
                           └───────────────────┬────────────────────┘
                                               │
                                               ▼
                                   ┌──────────────────────┐
                                   │ Qdrant Vector DB     │
                                   │ (Ollama/OpenAI Embed)│
                                   └──────────────────────┘

       ┌────────────────────────────────────────────────────────────────────────────────────────┐
       │                                  Infrastructure Layer                                  │
       │  MySQL 8.0 (Port 3307)   │   RabbitMQ 3 (Event Broker - Port 5672/15672)              │
       └────────────────────────────────────────────────────────────────────────────────────────┘
```

Mỗi microservice áp dụng nguyên lý **Database per Service** để đảm bảo tính độc lập và đóng gói dữ liệu. Hệ thống sử dụng **RabbitMQ** để truyền tin bất đồng bộ qua các Integration Events và **gRPC/HTTP** để thực hiện các cuộc gọi đồng bộ cần hiệu năng cao.

---

## 📦 Danh sách Microservices

| Service | Port | Database | Mô tả |
|---|---|---|---|
| **[ApiGateway](file:///c:/Workspace/csharp/TripNow/ApiGateway)** | `7000` | — | Định tuyến yêu cầu, quản lý CORS, tổng hợp Swagger UI |
| **[AuthService](file:///c:/Workspace/csharp/TripNow/AuthService)** | `7001` | `Auth.TravelNow.Service` | Xác thực JWT, Google OAuth2, phân quyền người dùng và hồ sơ nhân viên |
| **[HotelCatalogService](file:///c:/Workspace/csharp/TripNow/HotelCatalogService)** | `7002` | `HotelCatalog.TravelNow.Service` | Quản lý khách sạn, loại phòng, buồng phòng, tiện nghi, giá động, khuyến mãi |
| **[BookingService](file:///c:/Workspace/csharp/TripNow/BookingService)** | `7003` | `Booking.TravelNow.Service` | Xử lý đặt phòng, cập nhật tồn kho (inventory), tự động gán phòng |
| **[PaymentService](file:///c:/Workspace/csharp/TripNow/PaymentService)** | `7004` | `Payment.TravelNow.Service` | Thanh toán, ví điện tử, tài khoản escrow trung gian, thanh toán định kỳ, hoàn tiền |
| **[SocialService](file:///c:/Workspace/csharp/TripNow/SocialService)** | `7005` | `Social.TravelNow.Service` | MXH du lịch: đăng bài viết/ảnh, bình luận, tương tác, follow, bản đồ địa điểm |
| **[ChatService](file:///c:/Workspace/csharp/TripNow/ChatService)** | `7006` | `Chat.TravelNow.Service` | Chat thời gian thực qua SignalR và tích hợp Chatbot AI hỗ trợ từng khách sạn |
| **[NotificationService](file:///c:/Workspace/csharp/TripNow/NotificationService)** | `7007` | `Notification.TravelNow.Service` | Đẩy thông báo thời gian thực qua SignalR Hub tới người dùng/nhân viên |
| **[RecommendationService](file:///c:/Workspace/csharp/TripNow/RecommendationService)** | `7008` | `Recommendation.TravelNow.Service` | Gợi ý cá nhân hóa & tìm kiếm RAG sử dụng Qdrant Vector DB và Ollama/OpenAI |

---

## ✨ Tính năng cốt lõi

### 1. Quản lý Khách sạn & Vận hành (PMS)
*   **Quản lý sơ đồ khách sạn**: Thiết lập phòng theo tầng, block, cấu hình loại phòng và tiện nghi đi kèm.
*   **Giá động & Khuyến mãi**: Công cụ tính giá linh hoạt theo thời gian thực và quản lý các chương trình ưu đãi, giảm giá.
*   **Quản lý buồng phòng (Housekeeping)**: Phân công dọn dẹp và cập nhật trạng thái phòng tức thời.
*   **Chính sách hủy phòng**: Tự động hóa chính sách hoàn hủy phòng theo thời hạn đã cam kết.

### 2. Đặt phòng & Thanh toán Đảm bảo (Escrow)
*   **Đặt phòng tối ưu**: Cơ chế tự động kiểm tra tồn kho (inventory) và tự động gán phòng trống phù hợp khi đặt thành công.
*   **Tài khoản trung gian (Escrow)**: Tiền thanh toán của khách được giữ ở tài khoản trung gian của hệ thống và chỉ chuyển cho chủ khách sạn (payout) sau khi check-in hoặc hoàn thành kỳ nghỉ.
*   **Ví điện tử (Wallet)**: Nạp tiền, thanh toán nội bộ và quản lý số dư của chủ khách sạn/khách hàng.
*   **Hoàn tiền tự động**: Xử lý hoàn tiền về ví điện tử hoặc cổng thanh toán dựa theo chính sách hủy phòng của khách sạn.

### 3. Mạng xã hội trải nghiệm (Travel Social Network)
*   **Khoảnh khắc trải nghiệm (Posts)**: Đăng tải hình ảnh, mô tả hành trình và chia sẻ đánh giá thực tế của bản thân.
*   **Địa điểm xung quanh (Locations)**: Gợi ý các địa điểm ăn uống, giải trí, check-in lý tưởng xung quanh khu vực khách sạn.
*   **Tương tác cộng đồng**: Thích (like), bình luận (comment), theo dõi (follow) những người dùng có chung gu du lịch.

### 4. Trực tuyến Chat & Trợ lý ảo AI
*   **Chat thời gian thực**: Kết nối tức thì giữa khách lưu trú và bộ phận lễ tân/chủ khách sạn qua SignalR Hub (`/chathub`).
*   **Chatbot AI RAG**: Sử dụng mô hình ngôn ngữ lớn (tích hợp qua OpenRouter) kết hợp với dữ liệu ngữ cảnh (chính sách, tiện ích, dịch vụ của từng khách sạn cụ thể) để giải đáp thông tin tự động cho khách hàng 24/7.

### 5. Cá nhân hóa & Tìm kiếm ngữ nghĩa (AI Recommendation)
*   **Tìm kiếm ngữ nghĩa (Semantic Search/RAG)**: Tìm kiếm các thông tin liên quan đến khách sạn theo ý nghĩa câu hỏi thay vì từ khóa truyền thống.
*   **Gợi ý thông minh**: Thu thập lịch sử tìm kiếm và lịch sử xem của người dùng để cá nhân hóa danh sách khách sạn đề xuất.
*   **Khách sạn tương đồng**: Đề xuất các khách sạn có đặc điểm, tiện nghi hoặc vị trí tương tự nhờ vào Vector Embeddings được lưu trữ trên **Qdrant**.

---

## 🛠 Công nghệ sử dụng

### Backend
*   **Framework chính**: .NET 8.0, ASP.NET Core Web API.
*   **ORM**: Entity Framework Core 8.0.
*   **Cơ sở dữ liệu chính**: MySQL 8.0 (lưu trữ dữ liệu có cấu trúc).
*   **Cơ sở dữ liệu Vector**: Qdrant (lưu trữ vector embeddings phục vụ tìm kiếm RAG và gợi ý).
*   **Message Broker**: RabbitMQ 3 (Event-Driven Architecture).
*   **gRPC**: Giao tiếp hiệu năng cao giữa các service nội bộ.
*   **Realtime**: ASP.NET Core SignalR WebSockets.
*   **AI/LLM**: Ollama (`bge-m3` embedding), OpenAI, OpenRouter API.
*   **API Gateway**: Ocelot Gateway.
*   **Khác**: MediatR (CQRS), FluentValidation, AutoMapper, Cloudinary (Lưu trữ ảnh).

### Infrastructure
*   **Docker & Docker Compose**: Đóng gói và điều phối toàn bộ các container dịch vụ.
*   **RabbitMQ Management UI**: Giám sát hàng đợi sự kiện (Port `15672`).
*   **Qdrant Dashboard**: Quản trị vector collections (Port `6333`).

---

## 📁 Cấu trúc thư mục dự án

```
TripNow/
├── ApiGateway/                         # API Gateway (Ocelot)
├── AuthService/                        # Dịch vụ xác thực & phân quyền
├── HotelCatalogService/                # Dịch vụ danh mục khách sạn & PMS
├── BookingService/                     # Dịch vụ đặt phòng & tồn kho
├── PaymentService/                     # Dịch vụ thanh toán, ví & escrow
├── SocialService/                      # Mạng xã hội du lịch
├── ChatService/                        # Dịch vụ chat realtime & AI chatbot
├── NotificationService/                # Dịch vụ đẩy thông báo realtime
├── RecommendationService/              # Dịch vụ gợi ý & Vector RAG search
└── docker-compose.yml                  # File điều phối Docker Compose toàn hệ thống
```

Mỗi thư mục service đều tuân thủ cấu trúc **Clean Architecture**:
*   **API**: Controller, Hub, Program.cs.
*   **Application**: CQRS Commands/Queries, Handlers, DTOs, Interfaces, Validators.
*   **Domain**: Entities, Value Objects, Aggregates, Domain Events.
*   **Infrastructure**: DbContext, Migrations, External Integrations.

---

## ✅ Yêu cầu hệ thống

Đảm bảo máy tính của bạn đã được cài đặt đầy đủ các công cụ sau:
*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (bao gồm Docker Compose)
*   [Git](https://git-scm.com/)
*   *(Tùy chọn)* [Ollama](https://ollama.com/) (nếu chạy local embedding model)

---

## 🚀 Hướng dẫn cài đặt & chạy

### 1. Clone repository

```bash
git clone https://github.com/PhucDaizz/TripUp.git
cd TripUp
```

### 2. Cấu hình các biến môi trường
Tạo các file `.env` cho từng dịch vụ tương ứng dựa trên các file mẫu `.env.example.*` trong thư mục gốc của dự án:
*   Sao chép `.env.example.auth` thành `.env.auth` (hoặc đặt ở `AuthService/Config/.env`)
*   Sao chép `.env.example.catalog` thành `.env.hotel` (hoặc đặt ở `HotelCatalogService/src/Config/.env`)
*   Sao chép `.env.example.booking` thành `.env.booking` (hoặc đặt ở `BookingService/src/Config/.env`)
*   Sao chép `.env.example.payment` thành `.env.payment` (hoặc đặt ở `PaymentService/src/Config/.env`)
*   Sao chép `.env.example.social` thành `.env.social` (hoặc đặt ở `SocialService/src/Config/.env`)
*   Sao chép `.env.example.chat` thành `.env.chat` (hoặc đặt ở `ChatService/src/Config/.env`)
*   Sao chép `.env.example.notification` thành `.env.notification` (hoặc đặt ở `NotificationService/src/Config/.env`)
*   Sao chép `.env.example.recommendation` thành `.env.recommendation` (hoặc đặt ở `RecommendationService/src/Config/.env`)

*(Xem thêm phần [Cấu hình môi trường](#-cấu-hình-môi-trường) bên dưới)*

### 3. Khởi chạy toàn bộ hệ thống với Docker Compose

Chạy lệnh sau tại thư mục gốc của dự án:
```bash
docker-compose up --build
```

Sau khi hệ thống khởi động thành công, các dịch vụ sẽ sẵn sàng tại:

| Dịch vụ | URL |
|---|---|
| **API Gateway + Swagger UI** | [http://localhost:7000/swagger](http://localhost:7000/swagger) |
| **AuthService** | [http://localhost:7001](http://localhost:7001) |
| **HotelCatalogService** | [http://localhost:7002](http://localhost:7002) |
| **BookingService** | [http://localhost:7003](http://localhost:7003) |
| **PaymentService** | [http://localhost:7004](http://localhost:7004) |
| **SocialService** | [http://localhost:7005](http://localhost:7005) |
| **ChatService** | [http://localhost:7006](http://localhost:7006) |
| **NotificationService** | [http://localhost:7007](http://localhost:7007) |
| **RecommendationService** | [http://localhost:7008](http://localhost:7008) |
| **RabbitMQ Management** | [http://localhost:15672](http://localhost:15672) (User/Pass: `ADMIN` / `ADMIN`) |
| **Qdrant Dashboard** | [http://localhost:6333/dashboard](http://localhost:6333/dashboard) |
| **MySQL Connection** | `localhost:3307` |

### 4. Khởi chạy riêng lẻ để phát triển (Development Mode)

Trong môi trường local, bạn cần khởi động MySQL và RabbitMQ trước bằng Docker:
```bash
docker compose up mysql rabbitmq -d
```

Sau đó chạy từng dịch vụ từ terminal:
```bash
# Api Gateway
cd ApiGateway && dotnet run

# AuthService
cd AuthService/API && dotnet run

# HotelCatalogService
cd HotelCatalogService/src/API && dotnet run

# BookingService
cd BookingService/src/API && dotnet run

# PaymentService
cd PaymentService/src/API && dotnet run

# SocialService
cd SocialService/src/API && dotnet run

# ChatService
cd ChatService/src/API && dotnet run

# NotificationService
cd NotificationService/src/API && dotnet run

# RecommendationService
cd RecommendationService/src/API && dotnet run
```

---

## ⚙️ Cấu hình môi trường

Dưới đây là cấu hình mẫu cho các file `.env` của các dịch vụ mới:

### ChatService — `.env.chat`
```env
# JWT Verification
Jwt__Key=ChuaNghiRaKeyNuaNeChucBanGiaiDuoc8386A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5
Jwt__Issuer=http://localhost:7001
Jwt__Audience=http://localhost:7001
Jwt__TokenExpiryMinutes=60

# Database
ConnectionStrings__DefaultConnection=Server=localhost;Port=3307;Database=Chat.TravelNow.Service;user=root;password=phucdai011;

# RabbitMQ
RabbitMQ__HostName=localhost
RabbitMQ__Port=5672
RabbitMQ__Username=ADMIN
RabbitMQ__Password=ADMIN
RabbitMQ__VirtualHost=/

# External Service Endpoints
ServiceUrls__HotelCatalog=http://localhost:7002
ServiceUrls__Recommendation=http://localhost:7008

# AI Config
OpenRouter__ApiKey=your_openrouter_api_key
```

### NotificationService — `.env.notification`
```env
Jwt__Key=ChuaNghiRaKeyNuaNeChucBanGiaiDuoc8386A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5
Jwt__Issuer=http://localhost:7001
Jwt__Audience=http://localhost:7001

ConnectionStrings__DefaultConnection=Server=localhost;Port=3307;Database=Notification.TravelNow.Service;user=root;password=phucdai011;

RabbitMQ__HostName=localhost
RabbitMQ__Port=5672
RabbitMQ__Username=ADMIN
RabbitMQ__Password=ADMIN
RabbitMQ__VirtualHost=/
```

### RecommendationService — `.env.recommendation`
```env
Jwt__Key=ChuaNghiRaKeyNuaNeChucBanGiaiDuoc8386A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5
Jwt__Issuer=http://localhost:7001
Jwt__Audience=http://localhost:7001

ConnectionStrings__DefaultConnection=Server=localhost;Port=3307;Database=Recommendation.TravelNow.Service;user=root;password=phucdai011;

RabbitMQ__HostName=localhost
RabbitMQ__Port=5672
RabbitMQ__Username=ADMIN
RabbitMQ__Password=ADMIN
RabbitMQ__VirtualHost=/

# AI Provider: Ollama hoặc OpenAI
AI_Provider=Ollama

# Qdrant Vector DB
Qdrant__Host=localhost
Qdrant__Port=6334
Qdrant__Https=false
Qdrant__ApiKey=

# Ollama local embeddings
Ollama__Url=http://localhost:11434/api/embeddings
Ollama__Model=bge-m3
Ollama__VectorSize=1024

# Hoặc OpenAI embeddings
OpenAI__ApiKey=sk-proj-your_openai_api_key
OpenAI__EmbeddingModel=text-embedding-3-small
OpenAI__VectorSize=1536
```

> [!WARNING]
> Tuyệt đối không commit các file `.env` lên Github để tránh rò rỉ mã bảo mật. Các file này đã được định cấu hình bỏ qua trong `.gitignore`.

---

## 🔌 API Endpoints

Khám phá toàn bộ API thông qua tài liệu Swagger UI của API Gateway tại: `http://localhost:7000/swagger`

### 1. SocialService (`/api/` qua Gateway hoặc Port `7005`)
*   `POST /api/Post`: Tạo bài viết mới kèm ảnh và định vị địa điểm.
*   `GET /api/Post`: Lấy danh sách các bài viết (feed) trải nghiệm.
*   `DELETE /api/Post/{id}`: Xóa bài viết.
*   `POST /api/Comment`: Bình luận vào bài viết.
*   `POST /api/PostLike`: Thích hoặc bỏ thích một bài viết.
*   `POST /api/UserFollow`: Theo dõi người dùng khác.
*   `POST /api/SavedPost`: Lưu bài viết để xem lại.

### 2. ChatService (`/api/` hoặc Port `7006`)
*   `POST /api/Conversations/start`: Khởi tạo cuộc hội thoại giữa khách hàng và nhân viên khách sạn.
*   `GET /api/Conversations/all`: Xem danh sách tất cả các cuộc hội thoại hiện có.
*   `GET /api/Conversations/{conversationId}/messages`: Tải lịch sử tin nhắn trong cuộc hội thoại (phân trang).
*   `POST /api/ChatBot/hotel/{hotelId}/ask`: Đặt câu hỏi cho Trợ lý ảo AI của một khách sạn cụ thể.
*   **SignalR Hub**: Kết nối tại `/chathub` để nhắn tin thời gian thực.

### 3. RecommendationService (`/api/` hoặc Port `7008`)
*   `GET /api/Recommendation/for-user`: Gợi ý danh sách khách sạn được cá nhân hóa dựa trên hành vi người dùng.
*   `GET /api/Recommendation/hotel/{hotelId}/similar`: Tìm kiếm danh sách các khách sạn tương đồng về đặc tính.
*   `POST /api/Rag/hotel/{hotelId}/context`: Tìm kiếm ngữ cảnh ngữ nghĩa (RAG context retrieval) của khách sạn.
*   `POST /api/UserSearchHistory`: Theo dõi hành vi tìm kiếm của người dùng.
*   `POST /api/UserViewedHotel`: Ghi nhận sự kiện người dùng xem khách sạn.

---

## 📡 Giao tiếp giữa các Services

TripUp kết hợp linh hoạt cả 2 phương thức giao tiếp để tối ưu hóa hiệu năng:

### 1. Đồng bộ (Synchronous)
*   **BookingService** ➔ **HotelCatalogService** (HTTP): Kiểm tra tính khả dụng của phòng trước khi xác nhận đặt.
*   **BookingService** ➔ **PaymentService** (HTTP): Khởi tạo giao dịch thanh toán tạm giữ.
*   **ChatService** ➔ **RecommendationService** (HTTP): Gọi API lấy ngữ cảnh RAG (`/api/Rag/hotel/{id}/context`) để đưa dữ liệu vào LLM.
*   **RecommendationService** ➔ **HotelCatalogService** (gRPC): Giao tiếp hiệu năng cao lấy dữ liệu khách sạn để thực hiện tính toán vector embedding.

### 2. Bất đồng bộ (Asynchronous qua RabbitMQ)
*   **AuthService** phát sự kiện `UserCreatedIntegrationEvent` ➔ **NotificationService** & **RecommendationService** nhận để thiết lập hồ sơ người dùng ban đầu.
*   **BookingService** phát sự kiện `BookingPlacedIntegrationEvent` ➔ **PaymentService** bắt đầu giữ tiền & **NotificationService** gửi tin báo.
*   **PaymentService** phát sự kiện `PaymentCompletedIntegrationEvent` ➔ **BookingService** cập nhật trạng thái đã thanh toán & **NotificationService** báo thành công.
*   **SocialService** phát sự kiện tương tác (`PostLiked`, `CommentCreated`) ➔ **NotificationService** đẩy thông báo đẩy cho chủ bài viết.

---

## 🤝 Đóng góp

Mọi đóng góp nhằm cải thiện TripUp đều được hoan nghênh. Hãy thực hiện theo các bước:
1.  Fork repository dự án.
2.  Tạo nhánh tính năng mới: `git checkout -b feature/AmazingFeature`
3.  Commit các thay đổi của bạn: `git commit -m 'Add some AmazingFeature'`
4.  Push lên nhánh vừa tạo: `git push origin feature/AmazingFeature`
5.  Mở một Pull Request để chúng tôi duyệt mã.

---

## 📄 Bản quyền

Dự án này được cấp phép hoạt động theo Giấy phép **MIT** - Xem chi tiết tại tệp [LICENSE](LICENSE).

<div align="center">

Được thực hiện với ❤️ bởi **PhucDaizz**

</div>
