# TÀI LIỆU ĐẶC TẢ HỆ THỐNG WEB API QUẢN LÝ BÁN HÀNG ORDER TAOBAO

## 1. Tổng quan

Xây dựng hệ thống Web API phục vụ hoạt động bán hàng order Taobao. Hệ thống cho phép quản lý người dùng, quản lý đơn hàng và thống kê tình trạng đơn hàng.

Mục tiêu:

* Theo dõi toàn bộ vòng đời đơn hàng.
* Quản lý thông tin khách hàng và sản phẩm order.
* Hỗ trợ người bán cập nhật trạng thái đơn hàng.
* Tổng hợp thống kê nhanh tình hình kinh doanh.

---

## 2. Công nghệ đề xuất

### Backend

* RESTful Web API
* JWT Authentication
* Swagger/OpenAPI
* ORM hỗ trợ migration

* ASP.NET Core + PostgreSQL

### Database

PostgreSQL

---

## 3. Phân quyền người dùng

### 3.1 Vai trò

#### Admin

Quyền đầy đủ:

* Quản lý người dùng.
* Tạo/sửa/xóa tài khoản.
* Xem tất cả đơn hàng.
* Tạo/sửa/xóa đơn hàng.
* Thay đổi trạng thái đơn hàng.
* Xem toàn bộ thống kê.

#### Người bán hàng (Seller)

Quyền hạn:

* Đăng nhập hệ thống.
* Xem danh sách đơn hàng.
* Tạo đơn hàng mới.
* Cập nhật thông tin đơn hàng.
* Cập nhật trạng thái đơn hàng.
* Xem thống kê.

Không được:

* Quản lý người dùng.
* Phân quyền.

---

## 4. Chức năng xác thực

### Đăng nhập

API:

POST /api/auth/login

Input:

```json
{
  "username": "seller01",
  "password": "123456"
}
```

Output:

```json
{
  "accessToken": "jwt-token",
  "user": {
    "id": 1,
    "username": "seller01",
    "fullName": "Nguyen Van A",
    "role": "SELLER"
  }
}
```

---

### Đăng xuất

API:

POST /api/auth/logout

---

### Lấy thông tin người dùng hiện tại

API:

GET /api/auth/me

---

## 5. Quản lý người dùng

Chỉ Admin được sử dụng.

### Danh sách người dùng

GET /api/users

Hỗ trợ:

* Phân trang.
* Tìm kiếm theo tên.
* Lọc theo vai trò.

---

### Tạo người dùng

POST /api/users

Input:

```json
{
  "username": "seller01",
  "password": "123456",
  "fullName": "Nguyen Van A",
  "role": "SELLER"
}
```

---

### Cập nhật người dùng

PUT /api/users/{id}

---

### Xóa người dùng

DELETE /api/users/{id}

---

## 6. Quản lý đơn hàng

### 6.1 Trạng thái đơn hàng

Sử dụng Enum:

```text
DRAFT           : Khởi tạo tạm
NEW             : Tạo mới
ORDERED         : Đã order
ARRIVED         : Đã về
CANCELLED       : Đã hủy
DELETED         : Đã xóa
```

Lưu ý:

* DELETED là xóa mềm (soft delete).
* Không xóa vật lý dữ liệu.

---

### 6.2 Danh sách đơn hàng

GET /api/orders

Hỗ trợ:

* Phân trang.
* Tìm kiếm theo khách hàng.
* Tìm kiếm theo sản phẩm.
* Lọc theo trạng thái.
* Lọc theo khoảng ngày.
* Sắp xếp theo ngày tạo.

---

### 6.3 Chi tiết đơn hàng

GET /api/orders/{id}

---

### 6.4 Tạo đơn hàng

POST /api/orders

---

### 6.5 Cập nhật đơn hàng

PUT /api/orders/{id}

---

### 6.6 Chuyển trạng thái đơn hàng

PATCH /api/orders/{id}/status

Input:

```json
{
  "status": "ORDERED"
}
```

---

### 6.7 Xóa đơn hàng

DELETE /api/orders/{id}

Thực hiện:

* Cập nhật trạng thái thành DELETED.

---

## 7. Thông tin đơn hàng

Mỗi đơn hàng bao gồm các trường sau:

| Tên trường          | Kiểu dữ liệu | Mô tả                 |
| ------------------- | ------------ | --------------------- |
| id                  | bigint       | Khóa chính            |
| orderCode           | varchar      | Mã đơn hàng           |
| orderDate           | date         | Ngày tạo đơn          |
| customerName        | varchar      | Tên khách             |
| productName         | varchar      | Tên sản phẩm          |
| specification       | text         | Đặc điểm sản phẩm     |
| quantity            | integer      | Số lượng              |
| sellingPrice        | decimal      | Giá bán               |
| status              | enum         | Tình trạng đơn        |
| paymentStatus       | varchar      | Trạng thái thanh toán |
| yuanPrice           | decimal      | Giá tệ                |
| importPrice         | decimal      | Giá nhập              |
| supplier            | varchar      | Nguồn hàng            |
| warehousePayment    | decimal      | Trả tiền kho          |
| shippingWeightFee   | decimal      | Tiền cân              |
| shippingPaymentDate | date         | Ngày thanh toán cân   |
| refundAmount        | decimal      | Tiền hoàn             |
| refundStatus        | varchar      | Tình trạng hoàn       |
| note                | text         | Ghi chú               |
| createdBy           | bigint       | Người tạo             |
| createdAt           | timestamp    | Ngày tạo              |
| updatedAt           | timestamp    | Ngày cập nhật         |
| deleted             | boolean      | Đánh dấu xóa mềm      |

---

## 8. Trạng thái thanh toán

Có thể sử dụng:

```text
UNPAID      : Chưa thanh toán
PARTIAL     : Thanh toán một phần
PAID        : Đã thanh toán
REFUNDED    : Đã hoàn tiền
```

---

## 9. Thống kê đơn hàng

### 9.1 Thống kê theo trạng thái

GET /api/dashboard/order-status

Kết quả:

```json
{
  "draft": 10,
  "new": 25,
  "ordered": 40,
  "arrived": 18,
  "cancelled": 3,
  "deleted": 2
}
```

---

### 9.2 Thống kê doanh thu

GET /api/dashboard/revenue

Tham số:

* fromDate
* toDate

Kết quả:

```json
{
  "totalOrders": 120,
  "totalSellingPrice": 500000000,
  "totalImportPrice": 420000000,
  "estimatedProfit": 80000000
}
```

Trong đó:

```text
estimatedProfit
=
Tổng giá bán
-
Tổng giá nhập
-
Tổng tiền kho
-
Tổng tiền cân
+
Tổng tiền hoàn
```

---

### 9.3 Thống kê theo ngày

GET /api/dashboard/orders-by-date

Kết quả:

```json
[
  {
    "date": "2026-06-01",
    "count": 15
  },
  {
    "date": "2026-06-02",
    "count": 22
  }
]
```

---

## 10. Cấu trúc database đề xuất

### users

```text
id
username
password_hash
full_name
role
is_active
created_at
updated_at
```

---

### orders

```text
id
order_code
order_date
customer_name
product_name
specification
quantity
selling_price
status
payment_status
yuan_price
import_price
supplier
warehouse_payment
shipping_weight_fee
shipping_payment_date
refund_amount
refund_status
note
created_by
created_at
updated_at
deleted
```

---

## 11. Phi chức năng

Yêu cầu hệ thống:

* Xác thực bằng JWT.
* Phân quyền theo Role.
* Ghi log thao tác cập nhật trạng thái đơn hàng.
* Hỗ trợ Swagger để test API.
* Hỗ trợ Docker triển khai.
* Validation dữ liệu đầu vào.
* Soft Delete đối với đơn hàng.
* API trả về theo định dạng JSON thống nhất.

Ví dụ:

```json
{
  "success": true,
  "message": "Success",
  "data": {}
}
```

---

## 12. Định hướng phát triển tương lai

Có thể mở rộng thêm:

* Quản lý khách hàng.
* Quản lý nguồn hàng.
* Upload ảnh sản phẩm.
* Theo dõi vận đơn Trung Quốc.
* Theo dõi vận đơn Việt Nam.
* Quản lý công nợ khách hàng.
* Báo cáo lợi nhuận chi tiết.
* Xuất Excel/PDF.
* Gửi thông báo qua Zalo hoặc Email.
* Dashboard biểu đồ trực quan.
* Nhật ký thao tác người dùng (Audit Log).
* Hỗ trợ đa chi nhánh bán hàng.

```
```
