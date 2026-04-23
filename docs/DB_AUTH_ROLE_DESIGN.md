# Thiết kế DB: Role, Đăng nhập, Đăng ký (Vua Thợ)

Tài liệu này mô tả **thiết kế cơ sở dữ liệu** cho các phần:

- Role/quyền (ASP.NET Identity)
- Đăng nhập (JWT + Refresh token)
- Đăng ký + OTP (SĐT/Email)
- Mở rộng cho tài khoản **thợ** (Worker)

> Ghi chú: Dự án đang dùng **ASP.NET Identity + EF Core** với `Guid` (`IdentityDbContext<User, IdentityRole<Guid>, Guid>`). Vì vậy các bảng `AspNet*` là **chuẩn Identity**, còn các bảng nghiệp vụ (`OtpCodes`, `RefreshTokens`, `WorkerProfiles`, `WorkerRegistrationDocuments`...) là **bổ sung**.

---

## 1) Tổng quan mô hình (quan hệ chính)

### Identity (chuẩn)

- `AspNetUsers` (người dùng) 1—N `AspNetUserRoles`
- `AspNetRoles` (role) 1—N `AspNetUserRoles`

### Nghiệp vụ Auth

- `AspNetUsers` 1—N `RefreshTokens`
- OTP dùng để xác thực trước khi đăng ký: `OtpCodes` (độc lập, gắn theo **PhoneNumber** và/hoặc **Email**)

### Nghiệp vụ Worker

- `AspNetUsers` 1—0/1 `WorkerProfiles`
- `WorkerProfiles` 1—N `WorkerRegistrationDocuments`

---

## 2) Bảng & cột chi tiết

## 2.1 `AspNetUsers` (Identity + mở rộng bởi dự án)

**Nguồn**: `Domain/Entity/User.cs` kế thừa `IdentityUser<Guid>`.

### Cột quan trọng (Identity)
- `Id` (uniqueidentifier, PK)
- `UserName` (nvarchar(256), nullable)
- `NormalizedUserName` (nvarchar(256), index unique nếu không null)
- `Email`, `NormalizedEmail` (index)
- `EmailConfirmed` (bit)
- `PhoneNumber`, `PhoneNumberConfirmed` (bit)
- `PasswordHash` (nvarchar(max))
- `SecurityStamp`, `ConcurrencyStamp`
- `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`

### Cột mở rộng (dự án)
- `FullName` (nvarchar, NOT NULL)
- `IsActive` (bit, default 1)
- `CreatedDate` (datetime2, UTC, NOT NULL)
- `CreatedBy` (nvarchar, NULL)
- `UpdatedDate` (datetime2, UTC, NULL)
- `UpdatedBy` (nvarchar, NULL)
- `IsDeleted` (bit, default 0)
- `DeletedDate` (datetime2, UTC, NULL)
- `DeletedBy` (nvarchar, NULL)

### Index/constraint khuyến nghị
- **Unique** (phụ thuộc business): nếu hệ thống coi `PhoneNumber` là định danh đăng nhập chính, nên đảm bảo:
  - unique trên `PhoneNumber` (lọc `IS NOT NULL`) hoặc validate ở tầng service.

---

## 2.2 `AspNetRoles` / `AspNetUserRoles` (Identity)

### `AspNetRoles`
- `Id` (uniqueidentifier, PK)
- `Name` (nvarchar(256))
- `NormalizedName` (nvarchar(256), unique index)

### `AspNetUserRoles`
- `UserId` (uniqueidentifier, FK → `AspNetUsers.Id`)
- `RoleId` (uniqueidentifier, FK → `AspNetRoles.Id`)
- PK: (`UserId`, `RoleId`)

### Role name chuẩn (theo code)
Khai báo tại `Application/Constants/RoleNames.cs`:
- `Admin`
- `Customer`
- `WorkerExternal`
 
> Backend đã được chuẩn hoá để **chỉ còn thợ**: hệ thống chỉ seed và cấp role `WorkerExternal` cho user thợ mới.

---

## 2.3 `RefreshTokens` (cấp lại access token)

**Nguồn**: `Domain/Entity/RefreshToken.cs`

Mục tiêu: lưu refresh token dạng **hash + salt**, có thể revoke.

### Cột
- `Id` (uniqueidentifier, PK) *(từ `BaseAuditableEntity`)*
- `UserId` (uniqueidentifier, NOT NULL, FK → `AspNetUsers.Id`)
- `TokenHash` (nvarchar, NOT NULL)
- `Salt` (nvarchar, NOT NULL)
- `ExpiresAtUtc` (datetime2, UTC, NOT NULL)
- `RevokedAtUtc` (datetime2, UTC, NULL)
- `RevokeReason` (nvarchar, NULL)
- `CreatedIp` (nvarchar, NULL)
- `RevokedIp` (nvarchar, NULL)
- Audit/Soft delete (từ base):
  - `CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`
  - `IsDeleted`, `DeletedDate`, `DeletedBy`

### Index/constraint khuyến nghị
- Index `IX_RefreshTokens_UserId` để query theo user
- Index `IX_RefreshTokens_ExpiresAtUtc` để cleanup token hết hạn
- (Tuỳ chọn) Unique `TokenHash` nếu muốn tránh trùng token

---

## 2.4 `OtpCodes` (OTP xác thực đăng ký/đăng nhập)

**Nguồn**: `Domain/Entity/OtpCode.cs`

Mục tiêu: lưu OTP dạng **hash + salt**, giới hạn số lần sai, và sinh `VerificationToken` cho bước đăng ký.

### Cột
- `Id` (uniqueidentifier, PK) *(từ `BaseAuditableEntity`)*
- `PhoneNumber` (nvarchar, NOT NULL)
- `Email` (nvarchar, NULL) *(hỗ trợ OTP qua email)*
- `CodeHash` (nvarchar, NOT NULL)
- `Salt` (nvarchar, NOT NULL)
- `ExpiresAtUtc` (datetime2, UTC, NOT NULL)
- `IsVerified` (bit, NOT NULL, default 0)
- `VerifiedAtUtc` (datetime2, UTC, NULL)
- `FailedAttemptCount` (int, NOT NULL, default 0)
- `VerificationToken` (nvarchar, NULL)
- `VerificationTokenExpiresAtUtc` (datetime2, UTC, NULL)
- Audit/Soft delete (từ base):
  - `CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`
  - `IsDeleted`, `DeletedDate`, `DeletedBy`

### Index/constraint khuyến nghị
- Index `IX_OtpCodes_PhoneNumber_ExpiresAtUtc` (tìm OTP gần nhất còn hạn)
- Index `IX_OtpCodes_Email_ExpiresAtUtc` (nếu dùng email)
- Index `IX_OtpCodes_VerificationToken` (phục vụ bước đăng ký)

---

## 2.5 `WorkerProfiles` (hồ sơ thợ)

**Nguồn**: `Domain/Entity/WorkerProfile.cs`

### Cột
- `Id` (uniqueidentifier, PK) *(từ `BaseAuditableEntity`)*
- `UserId` (uniqueidentifier, NOT NULL, FK → `AspNetUsers.Id`)
- `WorkerType` (int, NOT NULL) *(enum `WorkerType`)*
- `WorkerStatus` (int, NOT NULL) *(enum `WorkerStatus`)*
- `Latitude` (decimal, NOT NULL)
- `Longitude` (decimal, NOT NULL)
- `RatingAvg` (decimal, NOT NULL)
- `RatingCount` (int, NOT NULL)
- `BasePrice` (decimal, NOT NULL)
- `ReviewedByUserId` (uniqueidentifier, NULL) *(admin duyệt)*
- `ReviewedAtUtc` (datetime2, UTC, NULL)
- `ReviewNote` (nvarchar, NULL)
- Audit/Soft delete (từ base):
  - `CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`
  - `IsDeleted`, `DeletedDate`, `DeletedBy`

### Index/constraint khuyến nghị
- **Unique** `IX_WorkerProfiles_UserId` (mỗi user tối đa 1 hồ sơ thợ)
- Index `IX_WorkerProfiles_WorkerType`
- Index `IX_WorkerProfiles_WorkerStatus`

> Với yêu cầu “chỉ còn thợ”, bạn vẫn có thể giữ cột `WorkerType` để tương thích dữ liệu cũ, nhưng chuẩn hoá: mọi hồ sơ mới `WorkerType = External`.

---

## 3) DDL gợi ý (SQL Server) cho phần nghiệp vụ (không gồm `AspNet*`)

> Chỉ là bản tham chiếu để review. Thực tế dự án đang dùng EF Core Migrations để tạo schema.

```sql
-- RefreshTokens
CREATE TABLE [dbo].[RefreshTokens] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [TokenHash] nvarchar(450) NOT NULL,
    [Salt] nvarchar(200) NOT NULL,
    [ExpiresAtUtc] datetime2 NOT NULL,
    [RevokedAtUtc] datetime2 NULL,
    [RevokeReason] nvarchar(500) NULL,
    [CreatedIp] nvarchar(64) NULL,
    [RevokedIp] nvarchar(64) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(256) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(256) NULL,
    [IsDeleted] bit NOT NULL DEFAULT(0),
    [DeletedDate] datetime2 NULL,
    [DeletedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
);
CREATE INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens]([UserId]);
CREATE INDEX [IX_RefreshTokens_ExpiresAtUtc] ON [dbo].[RefreshTokens]([ExpiresAtUtc]);

-- OtpCodes
CREATE TABLE [dbo].[OtpCodes] (
    [Id] uniqueidentifier NOT NULL,
    [PhoneNumber] nvarchar(50) NOT NULL,
    [Email] nvarchar(256) NULL,
    [CodeHash] nvarchar(200) NOT NULL,
    [Salt] nvarchar(200) NOT NULL,
    [ExpiresAtUtc] datetime2 NOT NULL,
    [IsVerified] bit NOT NULL DEFAULT(0),
    [VerifiedAtUtc] datetime2 NULL,
    [FailedAttemptCount] int NOT NULL DEFAULT(0),
    [VerificationToken] nvarchar(200) NULL,
    [VerificationTokenExpiresAtUtc] datetime2 NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(256) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(256) NULL,
    [IsDeleted] bit NOT NULL DEFAULT(0),
    [DeletedDate] datetime2 NULL,
    [DeletedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_OtpCodes] PRIMARY KEY ([Id])
);
CREATE INDEX [IX_OtpCodes_PhoneNumber_ExpiresAtUtc] ON [dbo].[OtpCodes]([PhoneNumber], [ExpiresAtUtc]);
CREATE INDEX [IX_OtpCodes_Email_ExpiresAtUtc] ON [dbo].[OtpCodes]([Email], [ExpiresAtUtc]);
CREATE INDEX [IX_OtpCodes_VerificationToken] ON [dbo].[OtpCodes]([VerificationToken]);

-- WorkerProfiles (rút gọn phần FK)
CREATE TABLE [dbo].[WorkerProfiles] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [WorkerType] int NOT NULL,
    [WorkerStatus] int NOT NULL,
    [Latitude] decimal(18, 6) NOT NULL,
    [Longitude] decimal(18, 6) NOT NULL,
    [RatingAvg] decimal(18, 2) NOT NULL,
    [RatingCount] int NOT NULL,
    [BasePrice] decimal(18, 2) NOT NULL,
    [ReviewedByUserId] uniqueidentifier NULL,
    [ReviewedAtUtc] datetime2 NULL,
    [ReviewNote] nvarchar(1000) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(256) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(256) NULL,
    [IsDeleted] bit NOT NULL DEFAULT(0),
    [DeletedDate] datetime2 NULL,
    [DeletedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_WorkerProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WorkerProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
);
CREATE UNIQUE INDEX [UX_WorkerProfiles_UserId] ON [dbo].[WorkerProfiles]([UserId]);
CREATE INDEX [IX_WorkerProfiles_WorkerType] ON [dbo].[WorkerProfiles]([WorkerType]);
CREATE INDEX [IX_WorkerProfiles_WorkerStatus] ON [dbo].[WorkerProfiles]([WorkerStatus]);
```

---

## 4) Luồng dữ liệu (mapping DB → tính năng)

### 4.1 Đăng ký (Customer / Thợ)
- `OtpCodes`: tạo OTP + lưu `CodeHash/Salt` + `ExpiresAtUtc`
- Verify OTP thành công:
  - cập nhật `OtpCodes.IsVerified = 1`, set `VerifiedAtUtc`
  - sinh `VerificationToken` + `VerificationTokenExpiresAtUtc`
- Đăng ký:
  - tạo `AspNetUsers` (User)
  - gán role trong `AspNetUserRoles`:
    - Customer → `Customer`
    - Thợ → `WorkerExternal`
  - nếu là thợ: tạo `WorkerProfiles` + `WorkerRegistrationDocuments` (nếu có)

### 4.2 Đăng nhập
- Tìm `AspNetUsers` theo định danh (SĐT/email/username tuỳ thiết kế)
- Kiểm tra `PasswordHash`, kiểm tra `IsActive`, `IsDeleted`
- Cấp JWT (access token) + tạo `RefreshTokens` (hash + salt)

### 4.3 Refresh token
- Nhận refresh token (plain) từ client → hash bằng salt tương ứng (hoặc lưu salt theo record) để so khớp `RefreshTokens.TokenHash`
- Kiểm tra `ExpiresAtUtc`, `RevokedAtUtc`, `IsDeleted`
- Nếu hợp lệ: cấp access token mới, có thể rotate refresh token (tạo record mới + revoke record cũ)

---

## 5) Gợi ý seed role (tuỳ môi trường)

Seed tối thiểu:
- `Admin`
- `Customer`
- `WorkerExternal`

