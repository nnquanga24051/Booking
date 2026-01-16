/* =========================================
   XÓA & TẠO LẠI DATABASE
   ========================================= */
USE master;
GO

IF DB_ID('booking_system') IS NOT NULL
BEGIN
    ALTER DATABASE booking_system
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE booking_system;
END
GO

CREATE DATABASE booking_system;
GO
USE booking_system;
GO

/* =========================================
   BẢNG USERS
   ========================================= */
CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    role VARCHAR(10)
        CHECK (role IN ('ADMIN','CUSTOMER'))
        DEFAULT 'CUSTOMER',
    created_at DATETIME DEFAULT GETDATE()
);
GO

/* =========================================
   BẢNG BRANCHES
   ========================================= */
CREATE TABLE branches (
    branch_id INT IDENTITY(1,1) PRIMARY KEY,
    branch_name NVARCHAR(100) NOT NULL UNIQUE,
    description NVARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE()
);
GO

/* =========================================
   BẢNG ROOMS
   ========================================= */
CREATE TABLE rooms (
    room_id INT IDENTITY(1,1) PRIMARY KEY,
    branch_id INT NOT NULL,
    room_name NVARCHAR(100) NOT NULL,
    room_type NVARCHAR(50),
    price_per_night DECIMAL(10,2) NOT NULL,
    status VARCHAR(15)
        CHECK (status IN ('available','booked','maintenance'))
        DEFAULT 'available',
    description NVARCHAR(MAX),
    CONSTRAINT fk_branch
        FOREIGN KEY (branch_id) REFERENCES branches(branch_id)
);
GO

/* =========================================
   BẢNG BOOKINGS
   ========================================= */
CREATE TABLE bookings (
    booking_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    room_id INT NOT NULL,
    check_in DATE NOT NULL,
    check_out DATE NOT NULL,
    total_price DECIMAL(10,2),
    status VARCHAR(15)
        CHECK (status IN ('booked','cancelled','completed'))
        DEFAULT 'booked',
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT fk_room FOREIGN KEY (room_id) REFERENCES rooms(room_id)
);
GO

/* =========================================
   INSERT DỮ LIỆU MẪU (15 RECORDS)
   ========================================= */

-- 1️⃣ Branches (3)
INSERT INTO branches (branch_name, description) VALUES
(N'Chi nhánh Cần Thơ', N'Khách sạn trung tâm Cần Thơ'),
(N'Chi nhánh TP.HCM', N'Khách sạn Quận 1'),
(N'Chi nhánh Hà Nội', N'Khách sạn khu vực Hồ Gươm');
GO

-- 2️⃣ Users (4)
INSERT INTO users (full_name, email, password, role) VALUES
(N'Nguyễn Văn A', 'a@gmail.com', '123456', 'CUSTOMER'),
(N'Trần Thị B', 'b@gmail.com', '123456', 'CUSTOMER'),
(N'Lê Văn C', 'c@gmail.com', '123456', 'CUSTOMER'),
(N'Admin', 'admin@gmail.com', 'admin123', 'ADMIN');
GO

-- 3️⃣ Rooms (5)
INSERT INTO rooms (branch_id, room_name, room_type, price_per_night, status, description) VALUES
(1, N'Phòng 101', N'Standard', 500000, 'available', N'Phòng tiêu chuẩn'),
(1, N'Phòng 102', N'Deluxe', 700000, 'available', N'Phòng cao cấp'),
(2, N'Phòng 201', N'Standard', 600000, 'maintenance', N'Đang bảo trì'),
(2, N'Phòng 202', N'Deluxe', 800000, 'available', N'Phòng sang trọng'),
(3, N'Phòng 301', N'Suite', 1000000, 'available', N'Phòng VIP');
GO

-- 4️⃣ Bookings (3)
INSERT INTO bookings (user_id, room_id, check_in, check_out, total_price, status) VALUES
(1, 1, '2026-02-01', '2026-02-03', 1000000, 'booked'),
(2, 2, '2026-02-05', '2026-02-07', 1400000, 'completed'),
(3, 4, '2026-02-10', '2026-02-12', 1600000, 'cancelled');
GO
