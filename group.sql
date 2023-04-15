/*----------------------------------------------------------
MASV: 20127072 - 20127091 - 20127102 - 20127424
HO TEN CAC THANH VIEN NHOM: Lê Võ Huỳnh Thanh - Lê Trọng Anh Tú - Hoàng Hữu Minh An - Trần Tiến Hoàng
LAB: 03 - NHOM 09
----------------------------------------------------------*/
-- CAU LENH TAO DB
USE master
GO
IF DB_ID('QLSVNhom') IS NOT NULL
	DROP DATABASE QLSVNhom
CREATE DATABASE QLSVNhom
GO
USE QLSVNhom
GO

-- CAC CAU LENH TAO TABLE
CREATE TABLE SINHVIEN
(
	MASV NVARCHAR(20),
	HOTEN NVARCHAR(100) NOT NULL,
	NGAYSINH DATETIME,
	DIACHI NVARCHAR(200),
	MALOP VARCHAR(20),
	TENDN NVARCHAR(100) NOT NULL,
	MATKHAU VARBINARY(MAX) NOT NULL

		CONSTRAINT PK_SV
	PRIMARY KEY(MASV)
)

CREATE TABLE NHANVIEN
(
	MANV VARCHAR(20),
	HOTEN NVARCHAR(100) NOT NULL,
	EMAIL VARCHAR(20),
	LUONG VARBINARY(MAX),
	TENDN NVARCHAR(100) NOT NULL,
	MATKHAU VARBINARY(MAX) NOT NULL,
	PUBKEY VARCHAR(20)

		CONSTRAINT PK_NV
	PRIMARY KEY(MANV)
)

CREATE TABLE LOP
(
	MALOP VARCHAR(20),
	TENLOP NVARCHAR(100) NOT NULL,
	MANV VARCHAR(20)

		CONSTRAINT PK_LOP
	PRIMARY KEY(MALOP)
)

CREATE TABLE HOCPHAN
(
	MAHP VARCHAR(20),
	TENHP NVARCHAR(100) NOT NULL,
	SOTC INT

		CONSTRAINT PK_HP
	PRIMARY KEY(MAHP)
)

CREATE TABLE BANGDIEM
(
	MASV VARCHAR(20),
	MAHP VARCHAR(20),
	DIEMTHI VARBINARY(MAX)

		CONSTRAINT PK_BD
	PRIMARY KEY(MASV, MAHP)
)

USE QLSVnhom
GO

-- 1. Tạo master key
IF NOT EXISTS
(
	SELECT *
FROM sys.symmetric_keys
WHERE symmetric_key_id = 101
)
begin
	CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'BMCSDL'
end
-- 2. Tạo certificate
IF NOT EXISTS
(
	SELECT *
FROM sys.certificates
WHERE name = 'MyCert'
)
begin
	CREATE CERTIFICATE MyCert WITH SUBJECT = 'MyCert'
end
-- CAU LENH TAO STORED PROCEDURE
-- (i). Insert vào NHANVIEN, mã hoá mật khẩu bằng SHA1, mã hoá lương bằng RSA
go
if OBJECT_ID('SP_INS_PUBLIC_NHANVIEN') is not null
DROP PROCEDURE SP_INS_PUBLIC_NHANVIEN
GO
CREATE PROCEDURE SP_INS_PUBLIC_NHANVIEN
	@MaNV VARCHAR(20),
	@HoTen NVARCHAR(100),
	@email VARCHAR(20),
	@LuongCB INT,
	@TenDN NVARCHAR(100),
	@Matkhau VARCHAR(MAX)
AS
-- Mã hoá mật khẩu
DECLARE @hash_pass VARBINARY(MAX)
SET @hash_pass = HASHBYTES('SHA1', @Matkhau)

-- 3. Tạo key
DECLARE @create_key VARCHAR(MAX)
SET @create_key = 'CREATE ASYMMETRIC KEY ' + @MaNV + ' WITH ALGORITHM = RSA_2048 ENCRYPTION BY PASSWORD = ''' + @Matkhau + ''''
IF NOT EXISTS
	(
		SELECT *
FROM sys.asymmetric_keys
WHERE name = @MaNV
	)
	begin
	EXEC(@create_key)
end
-- 4. Mã hoá sử dụng khoá công khai
DECLARE @AsymID INT
SET @AsymID = ASYMKEY_ID(@MaNV)
DECLARE @luongEnc VARBINARY(MAX)
SET @luongEnc = ENCRYPTBYASYMKEY(@AsymID, CONVERT(VARCHAR(MAX), @luongCB))

INSERT NHANVIEN
VALUES(@MaNV, @HoTen, @email, @luongEnc, @TenDN, @hash_pass, @MaNV)
GO

-- (ii). Truy vấn dữ liệu nhân viên sau khi giải mã
if OBJECT_ID('SP_SEL_PUBLIC_NHANVIEN') is not null
DROP PROCEDURE SP_SEL_PUBLIC_NHANVIEN
GO
CREATE PROCEDURE SP_SEL_PUBLIC_NHANVIEN
	@TenDN NVARCHAR(100),
	@Matkhau NVARCHAR(MAX)
AS
SELECT MANV, HOTEN, EMAIL, CAST(CONVERT(VARCHAR(MAX), DECRYPTBYASYMKEY(ASYMKEY_ID(@TenDN), LUONG, @Matkhau)) AS INT) as LUONGCB
FROM NHANVIEN
go
-- d
-- tạo 2 nhân viên, 3 sinh viên
--delete from NHANVIEN
--delete from SINHVIEN
EXEC SP_INS_PUBLIC_NHANVIEN 'NV01', N'NGUYEN VAN A', 'nva@yahoo.com', 3000000, N'NVA', '123456'
EXEC SP_INS_PUBLIC_NHANVIEN 'NV02', N'NGUYEN VAN B', 'nvb@yahoo.com', 3000000, N'NVB', '1234567'
INSERT into SINHVIEN
	(MASV,HOTEN,MALOP,TENDN,MATKHAU)
VALUES
	(N'SV01', N'Sinh Vien A', 'LOP01', N'sva', HASHBYTES('MD5','1'));
INSERT into SINHVIEN
	(MASV,HOTEN,MALOP,TENDN,MATKHAU)
VALUES
	(N'SV02', N'Sinh Vien B', 'LOP01', N'svb', HASHBYTES('MD5','1'));
INSERT into SINHVIEN
	(MASV,HOTEN,MALOP,TENDN,MATKHAU)
VALUES
	(N'SV03', N'Sinh Vien C', 'LOP02', N'svc', HASHBYTES('MD5','1'));
-- tạo lớp học
INSERT into LOP
	(MALOP,TENLOP,MANV)
VALUES
	(N'LOP01', N'Lop 1', 'NV01');
INSERT into LOP
	(MALOP,TENLOP,MANV)
VALUES
	(N'LOP02', N'Lop 2', 'NV02');
-- tạo acc admin, đầy đủ quyền
EXEC SP_INS_PUBLIC_NHANVIEN 'admin', N'admin', 'admin@yahoo.com', 1, N'admin', 'admin'
go
-- tạo acc admin cho lab 3
USE [master]
GO
CREATE LOGIN [adminLAB3] WITH PASSWORD=N'123456789', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=ON, CHECK_POLICY=ON
GO
USE [QLSVNhom]
GO
CREATE USER [adminLAB3] FOR LOGIN [adminLAB3]
GO
USE [QLSVNhom]
GO
ALTER ROLE [db_owner] ADD MEMBER [adminLAB3]
GO

-- xử lí yêu cầu đăng nhập, nếu đúng thì trả về token, sai thì thông báo 400
if OBJECT_ID('DANGNHAP') is not null
	drop function DANGNHAP
go
create function DANGNHAP (@tendangnhap nvarchar(100), @matkhau varchar(MAX))
returns varchar(MAX)
as
begin
	DECLARE @res VARCHAR(MAX)
	IF EXISTS(select *
	from NHANVIEN
	WHERE TENDN=@tendangnhap and MATKHAU=HASHBYTES('SHA1',@matkhau))
	BEGIN
		DECLARE @time as VARCHAR(MAX) = CONVERT( VARCHAR(MAX) , GETDATE(), 120)
		set @res = CONCAT(
						CONVERT(VARCHAR(MAX), CAST(@tendangnhap AS VARBINARY(MAX)), 1),
						'.',
						CONVERT(VARCHAR(MAX), CAST(@time AS VARBINARY(MAX)), 1),
						'.',
						CONVERT(varchar(MAX), HASHBYTES('SHA1',CONCAT(@tendangnhap, @time, N'passLab3')), 1))
	END
	ELSE
	BEGIN
		set @res = '400'
	END
	RETURN @res
END
GO

-- middleware check token
if OBJECT_ID('checkToken') is not null
	drop function checkToken
go
create function checkToken (@token varchar(MAX))
returns VARCHAR(20)
as
BEGIN
	DECLARE @res VARCHAR(20)
	DECLARE @tendangnhap NVARCHAR(MAX)
	DECLARE @time VARCHAR(MAX)
	DECLARE @hash VARCHAR(MAX)
	set @tendangnhap = CAST(CONVERT(VARBINARY(MAX), SUBSTRING(@token, 1, CHARINDEX('.', @token) - 1) , 1) AS NVARCHAR(MAX))
	set @time = CAST(CONVERT(VARBINARY(MAX), SUBSTRING(@token, CHARINDEX('.', @token) + 1, CHARINDEX('.', @token, CHARINDEX('.', @token) + 1) - CHARINDEX('.', @token) - 1) , 1) AS VARCHAR(MAX))
	set @hash = SUBSTRING(@token, CHARINDEX('.', @token, CHARINDEX('.', @token) + 1) + 1, LEN(@token) - CHARINDEX('.', @token, CHARINDEX('.', @token) + 1))
	IF (@hash = CONVERT(varchar(MAX), HASHBYTES('SHA1',CONCAT(@tendangnhap, @time, N'passLab3')), 1) and DATEDIFF(MINUTE, @time, GETDATE()) < 60)
	BEGIN
		select @res = MANV
		from NHANVIEN
		WHERE TENDN = @tendangnhap
	END
	ELSE
	BEGIN
		set @res = ''
	END
	RETURN @res
END
GO

if OBJECT_ID('dsLop') is not null
	drop proc dsLop
go
CREATE proc dsLop
	@token varchar(MAX)
as
begin
	declare @MaNV as NVARCHAR(20)
	set @MaNV = dbo.checkToken(@token)
	if @MaNV = 'admin'
		begin
		select *
		from LOP
	end
		else
		begin
		select *
		from LOP
		where MANV = @MaNV
	end
end
go

if OBJECT_ID('dsSinhVien') is not null
	drop proc dsSinhVien
go
CREATE proc dsSinhVien @token varchar(MAX), @MALOP VARCHAR(20)
as
BEGIN
	declare @MaNV as NVARCHAR(20)
	set @MaNV = dbo.checkToken(@token)
	select MASV,HOTEN,NGAYSINH,DIACHI from SINHVIEN where MALOP in (select MALOP from LOP where MALOP = @MALOP and MANV = @MaNV)
END
go