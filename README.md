# 🏨 TripUp — Hotel Booking Microservices Platform

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Ocelot](https://img.shields.io/badge/Ocelot-API_Gateway-00B4D8?style=for-the-badge)

**TripUp** là nền tảng đặt phòng khách sạn được xây dựng theo kiến trúc **Microservices**, sử dụng **.NET 8**, **CQRS**, **Clean Architecture** và **Domain-Driven Design (DDD)**.

</div>

---

## 📋 Mục lục

- [Tổng quan kiến trúc](#-tổng-quan-kiến-trúc)
- [Danh sách Microservices](#-danh-sách-microservices)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Hướng dẫn cài đặt & chạy](#-hướng-dẫn-cài-đặt--chạy)
- [Cấu hình môi trường](#-cấu-hình-môi-trường)
- [API Endpoints](#-api-endpoints)
- [Giao tiếp giữa các Services](#-giao-tiếp-giữa-các-services)
- [Đóng góp](#-đóng-góp)

---

## 🏗 Tổng quan kiến trúc

```
                        ┌─────────────────────────────────────────────────┐
                        │              Client (Frontend / Mobile)          │
                        └──────────────────────┬──────────────────────────┘
                                               │ HTTP
                        ┌──────────────────────▼──────────────────────────┐
                        │           API Gateway  (Port 7000)               │
                        │         Ocelot + SwaggerForOcelot                │
                        └──┬──────────┬──────────┬──────────┬─────────────┘
                           │          │          │          │
              ┌────────────▼─┐  ┌─────▼──────┐  │  ┌───────▼──────┐
              │ AuthService  │  │HotelCatalog│  │  │BookingService│
              │  (Port 7001) │  │(Port 7002) │  │  │ (Port 7003)  │
              └──────────────┘  └────────────┘  │  └──────────────┘
                                                │
                        ┌───────────────────────▼──────────────────────────┐
                        │          PaymentService  (Port 7004)              │
                        └──────────────────────────────────────────────────┘

                        ┌──────────────────────────────────────────────────┐
                        │          SocialService  (Port TBD)               │
                        └──────────────────────────────────────────────────┘

              ┌──────────────────────────────────────────────────────────────┐
              │                  Infrastructure Layer                        │
              │   MySQL 8.0 (Port 3307)   │   RabbitMQ 3 (Port 5672/15672)  │
              └──────────────────────────────────────────────────────────────┘
```

Mỗi microservice có **database riêng biệt** (Database per Service pattern) và giao tiếp với nhau thông qua **RabbitMQ** (async messaging) hoặc **HTTP** (sync calls qua API Gateway).

---

## 📦 Danh sách Microservices

| Service | Port | Database | Mô tả |
|---|---|---|---|
| **API Gateway** | `7000` | — | Ocelot reverse proxy, định tuyến request, tích hợp Swagger UI tổng hợp |
| **AuthService** | `7001` | `Auth.TravelNow.Service` | Xác thực & phân quyền: đăng ký, đăng nhập, JWT, OAuth2 Google, quản lý hồ sơ nhân viên |
| **HotelCatalogService** | `7002` | `HotelCatalog.TravelNow.Service` | Quản lý danh mục khách sạn: phòng, loại phòng, tiện nghi, tầng, block, giá, khuyến mãi, chính sách hủy |
| **BookingService** | `7003` | `Booking.TravelNow.Service` | Quản lý đặt phòng: tạo booking, inventory, phân công phòng, hủy đặt phòng |
| **PaymentService** | `7004` | `Payment.TravelNow.Service` | Xử lý thanh toán: giao dịch, ví điện tử, escrow, hoàn tiền, thanh toán cho chủ khách sạn |
| **SocialService** | — | `Social.TravelNow.Service` | Mạng xã hội: bài đăng, bình luận, like, follow, địa điểm, đánh giá |

---

## 🛠 Công nghệ sử dụng

### Backend
| Công nghệ | Phiên bản | Mục đích |
|---|---|---|
| **.NET / ASP.NET Core** | 8.0 | Framework chính |
| **Entity Framework Core** | 8.0 | ORM |
| **MySQL** | 8.0 | Cơ sở dữ liệu quan hệ |
| **RabbitMQ** | 3 | Message broker (async communication) |
| **Ocelot** | 24.1.0 | API Gateway |
| **MediatR** | — | CQRS Mediator pattern |
| **FluentValidation** | — | Validation pipeline |
| **AutoMapper** | — | Object mapping |
| **JWT Bearer** | 8.0 | Xác thực token |
| **Cloudinary** | — | Lưu trữ & xử lý ảnh |
| **ImageSharp** | — | Xử lý ảnh server-side |
| **Swashbuckle / Swagger** | 6.6.2 | API documentation |
| **DotNetEnv** | 3.1.1 | Quản lý biến môi trường |

### Infrastructure
| Công nghệ | Mục đích |
|---|---|
| **Docker & Docker Compose** | Container hóa toàn bộ hệ thống |
| **RabbitMQ Management UI** | Giám sát message queue (Port 15672) |

### Design Patterns & Architecture
- **Clean Architecture** (Domain → Application → Infrastructure → API)
- **CQRS** (Command Query Responsibility Segregation) với MediatR
- **Domain-Driven Design (DDD)** với Aggregate Root, Domain Events, Value Objects
- **Repository Pattern** + **Unit of Work**
- **Database per Service** (mỗi service có DB riêng)
- **Event-Driven Architecture** (Integration Events qua RabbitMQ)

---

## 📁 Cấu trúc dự án

```
TripUp/
├── ApiGateway/                         # API Gateway (Ocelot)
│   ├── ocelot.Development.json         # Routing config (local)
│   ├── ocelot.Docker.json              # Routing config (Docker)
│   ├── Program.cs
│   └── Dockerfile
│
├── AuthService/                        # Dịch vụ xác thực
│   ├── API/                            # Controllers, Program.cs, appsettings
│   │   └── Controllers/
│   │       └── AuthController.cs
│   ├── Application/                    # CQRS Commands/Queries, DTOs, Validators
│   │   └── Features/
│   │       ├── User/
│   │       ├── StaffProfile/
│   │       └── HotelService/
│   ├── Domain/                         # Entities, Repositories interfaces, Domain Events
│   └── Infrastructure/                 # EF Core, Services (JWT, Email, Cloudinary, OAuth)
│
├── HotelCatalogService/                # Dịch vụ danh mục khách sạn
│   └── src/
│       ├── API/Controllers/            # 13 controllers
│       ├── Application/                # CQRS handlers
│       ├── Domain/Entities/            # Hotel, Room, RoomType, Amenity, Promotion, ...
│       └── Infrastructure/
│
├── BookingService/                     # Dịch vụ đặt phòng
│   └── src/
│       ├── API/Controllers/            # Booking, Inventory, RoomAssignment
│       ├── Application/
│       ├── Domain/Entities/            # Booking, BookingItem, Inventory, RoomAssignment, ...
│       └── Infrastructure/
│
├── PaymentService/                     # Dịch vụ thanh toán
│   └── src/
│       ├── API/Controllers/            # Payment, Wallet, Escrow, Payout, Refund, ...
│       ├── Application/
│       ├── Domain/Entities/            # PaymentTransaction, OwnerWallet, EscrowAccount, ...
│       └── Infrastructure/
│
├── SocialService/                      # Dịch vụ mạng xã hội
│   └── src/
│       ├── API/Controllers/            # Post, Comment, Like, Follow, Location, SavedPost
│       ├── Application/
│       ├── Domain/Entities/            # Post, Comment, Location, Review, UserFollow, ...
│       └── Infrastructure/
│
└── docker-compose.yml                  # Orchestration toàn bộ hệ thống
```

---

## ✅ Yêu cầu hệ thống

Trước khi bắt đầu, hãy đảm bảo bạn đã cài đặt:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (bao gồm Docker Compose)
- [Git](https://git-scm.com/)
- *(Tùy chọn)* [Visual Studio 2022](https://visualstudio.microsoft.com/) hoặc [VS Code](https://code.visualstudio.com/) với C# extension

---

## 🚀 Hướng dẫn cài đặt & chạy

### 1. Clone repository

```bash
git clone https://github.com/PhucDaizz/TripUp.git
cd TripUp
```

### 2. Cấu hình biến môi trường

Tạo file `.env` cho từng service (xem phần [Cấu hình môi trường](#-cấu-hình-môi-trường)).

### 3. Chạy toàn bộ hệ thống với Docker Compose

```bash
docker-compose up --build
```

> Lần đầu chạy sẽ mất vài phút để build image và khởi động các service.

Sau khi khởi động thành công, các service sẽ có sẵn tại:

| Service | URL |
|---|---|
| **API Gateway + Swagger UI** | http://localhost:7000/swagger |
| **AuthService** | http://localhost:7001 |
| **HotelCatalogService** | http://localhost:7002 |
| **BookingService** | http://localhost:7003 |
| **PaymentService** | http://localhost:7004 |
| **RabbitMQ Management** | http://localhost:15672 (user: `ADMIN` / pass: `ADMIN`) |
| **MySQL** | `localhost:3307` |

### 4. Chạy từng service riêng lẻ (Development)

```bash
# Chạy AuthService
cd AuthService/API
dotnet run

# Chạy HotelCatalogService
cd HotelCatalogService/src/API
dotnet run

# Chạy BookingService
cd BookingService/src/API
dotnet run

# Chạy PaymentService
cd PaymentService/src/API
dotnet run

# Chạy SocialService
cd SocialService/src/API
dotnet run

# Chạy API Gateway
cd ApiGateway
dotnet run
```

### 5. Áp dụng Database Migrations

Mỗi service tự động áp dụng migration khi khởi động. Để chạy thủ công:

```bash
# Ví dụ với AuthService
cd AuthService/API
dotnet ef database update
```

---

## ⚙️ Cấu hình môi trường

### AuthService — `AuthService/Config/.env`

```env
# JWT
JWT__KEY=your_super_secret_key_here
JWT__ISSUER=http://localhost:7001
JWT__AUDIENCE=http://localhost:7001
JWT__TOKENEXPIRYMINUTES=60

# Database
CONNECTIONSTRINGS__DEFAULTCONNECTION=Server=localhost;Port=3307;Database=Auth.TravelNow.Service;user=root;password=your_password;

# RabbitMQ
RABBITMQ__HOSTNAME=localhost
RABBITMQ__PORT=5672
RABBITMQ__USERNAME=ADMIN
RABBITMQ__PASSWORD=ADMIN

# Email (SMTP)
EMAILSETTINGS__MAILSERVER=smtp.gmail.com
EMAILSETTINGS__MAILPORT=587
EMAILSETTINGS__FROMEMAIL=your_email@gmail.com
EMAILSETTINGS__PASSWORD=your_app_password

# Google OAuth2
GOOGLE__CLIENTID=your_google_client_id
GOOGLE__CLIENTSECRET=your_google_client_secret

# Cloudinary
CLOUDINARY__CLOUDNAME=your_cloud_name
CLOUDINARY__APIKEY=your_api_key
CLOUDINARY__APISECRET=your_api_secret

# Frontend
FRONTEND__BASEURL=http://localhost:5173
FRONTEND__RESETPASSWORDURL=http://localhost:5173/reset-password
```

### HotelCatalogService — `HotelCatalogService/src/Config/.env`

```env
CONNECTIONSTRINGS__DEFAULTCONNECTION=Server=localhost;Port=3307;Database=HotelCatalog.TravelNow.Service;user=root;password=your_password;
RABBITMQ__HOSTNAME=localhost
RABBITMQ__PORT=5672
RABBITMQ__USERNAME=ADMIN
RABBITMQ__PASSWORD=ADMIN
```

> **Lưu ý bảo mật:** Không commit file `.env` lên repository. Các file này đã được thêm vào `.gitignore`.

---

## 🔌 API Endpoints

Toàn bộ API được tổng hợp và có thể khám phá qua **Swagger UI** tại:
```
http://localhost:7000/swagger
```

### AuthService (`/api/auth`)
| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/auth/register` | Đăng ký tài khoản mới |
| `POST` | `/api/auth/login` | Đăng nhập, nhận JWT token |
| `POST` | `/api/auth/refresh-token` | Làm mới access token |
| `POST` | `/api/auth/google-login` | Đăng nhập bằng Google OAuth2 |
| `POST` | `/api/auth/forgot-password` | Gửi email đặt lại mật khẩu |
| `POST` | `/api/auth/reset-password` | Đặt lại mật khẩu |
| `GET` | `/api/auth/profile` | Lấy thông tin hồ sơ người dùng |

### HotelCatalogService
| Controller | Mô tả |
|---|---|
| `HotelController` | CRUD khách sạn, tìm kiếm, lọc |
| `HotelRoomTypesController` | Quản lý loại phòng |
| `HotelRoomsController` | Quản lý phòng cụ thể |
| `RoomPricesController` | Quản lý giá phòng |
| `HotelPromotionsController` | Quản lý khuyến mãi |
| `CancellationPoliciesController` | Chính sách hủy phòng |
| `AmenityController` | Danh mục tiện nghi |
| `HotelAmenityController` | Tiện nghi của khách sạn |
| `HotelImagesController` | Ảnh khách sạn |
| `HotelBlocksController` | Quản lý block/khu vực |
| `HotelFloorsController` | Quản lý tầng |
| `HousekeepingController` | Quản lý dọn phòng |
| `RoomTypeImagesController` | Ảnh loại phòng |

### BookingService
| Controller | Mô tả |
|---|---|
| `BookingController` | Tạo, xem, hủy đặt phòng |
| `InventoryController` | Quản lý tồn kho phòng |
| `RoomAssignmentController` | Phân công phòng cho booking |

### PaymentService
| Controller | Mô tả |
|---|---|
| `PaymentController` | Khởi tạo & xác nhận thanh toán |
| `WalletController` | Quản lý ví điện tử |
| `EscrowController` | Quản lý tài khoản escrow |
| `PayoutController` | Thanh toán cho chủ khách sạn |
| `RefundController` | Xử lý hoàn tiền |
| `SettlementPeriodController` | Kỳ thanh toán định kỳ |
| `OwnerBankAccountController` | Tài khoản ngân hàng chủ khách sạn |

### SocialService
| Controller | Mô tả |
|---|---|
| `PostController` | Tạo, xem, xóa bài đăng |
| `CommentController` | Bình luận bài đăng |
| `PostLikeController` | Like/unlike bài đăng |
| `UserFollowController` | Follow/unfollow người dùng |
| `LocationController` | Quản lý địa điểm |
| `SavedPostController` | Lưu bài đăng yêu thích |

---

## 📡 Giao tiếp giữa các Services

### Synchronous (HTTP)
- **BookingService** → **HotelCatalogService**: Kiểm tra thông tin phòng, giá, tình trạng
- **BookingService** → **PaymentService**: Khởi tạo giao dịch thanh toán
- **PaymentService** → **HotelCatalogService**: Xác minh thông tin khách sạn

### Asynchronous (RabbitMQ)
- **AuthService** → *: Phát sự kiện khi người dùng đăng ký/cập nhật
- **BookingService** → **PaymentService**: Sự kiện booking được tạo/hủy
- **PaymentService** → **BookingService**: Sự kiện thanh toán thành công/thất bại
- **HotelCatalogService** → **BookingService**: Sự kiện cập nhật inventory

---

## 🤝 Đóng góp

Mọi đóng góp đều được hoan nghênh! Vui lòng:

1. Fork repository
2. Tạo branch mới: `git checkout -b feature/ten-tinh-nang`
3. Commit thay đổi: `git commit -m 'feat: thêm tính năng X'`
4. Push lên branch: `git push origin feature/ten-tinh-nang`
5. Tạo Pull Request

---

## 📄 License

Dự án này được phân phối dưới giấy phép **MIT**. Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

---

<div align="center">

Made with ❤️ by **PhucDaizz**

</div>
