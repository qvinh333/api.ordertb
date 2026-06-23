# Sale Order Management API

A comprehensive ASP.NET Core Web API for managing Taobao order sales, including user management, order tracking, and business analytics.

## Features

### Authentication & Authorization
- JWT-based authentication
- Role-based access control (ADMIN, SELLER)
- Secure password hashing with BCrypt

### User Management
- Create, read, update, delete users
- Role-based user management
- Search and filter by name or role
- Pagination support

### Order Management
- Create, read, update, delete orders
- Order status tracking (DRAFT, NEW, ORDERED, ARRIVED, CANCELLED, DELETED)
- Payment status tracking (UNPAID, PARTIAL, PAID, REFUNDED)
- Soft delete functionality
- Advanced filtering and search:
  - Search by customer name
  - Search by product name
  - Filter by status
  - Filter by payment status
  - Filter by date range
  - Sorting by creation date

### Product Management
- Create, read, update, delete products
- Search by product code or name
- Pagination support

### Customer Management
- Create, read, update, delete customers
- Search by customer code, full name, or phone
- Pagination support

### Dashboard & Statistics
- Order status statistics
- Revenue reporting with profit calculation
- Orders by date statistics

### Currency Rate Configuration
- Configure exchange rate by account
- Keep change history for every update
- Get the latest configured rate
- Data isolation by user account (users cannot see each other's rates)

### Health Check
- Liveness endpoint for process health
- Readiness endpoint with database connectivity check

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL 14+
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT (System.IdentityModel.Tokens.Jwt)
- **Password Hashing**: BCrypt.Net-Next
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Validation**: FluentValidation

## Prerequisites

- .NET 9.0 SDK or later
- PostgreSQL 14 or later
- Visual Studio 2022, JetBrains Rider, or any .NET-compatible IDE

## Installation & Setup

### 1. Clone the Repository
```bash
cd D:\Code\Sale\API.Sale
```

### 2. Configure Database Connection
Edit `API.Sale/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=sale_order_db;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-that-is-at-least-32-characters-long-for-security",
    "Issuer": "SaleOrderAPI",
    "Audience": "SaleOrderAPIUsers",
    "ExpirationDays": 7
  }
}
```

### 3. Install Dependencies
```bash
cd API.Sale
dotnet restore
```

### 4. Apply Database Migrations
```bash
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The API will be available at `https://localhost:5163`
Swagger UI will be available at `https://localhost:5163/`

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login user and get JWT token
- `POST /api/auth/logout` - Logout user
- `GET /api/auth/me` - Get current user information

### User Management (Admin only)
- `GET /api/users` - Get all users with pagination, search, and filtering
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user information
- `DELETE /api/users/{id}` - Delete user

### Order Management
- `GET /api/orders` - Get all orders with advanced filtering
- `GET /api/orders/money-summary` - Get order money summary with the same filters as order list
- `GET /api/orders/{id}` - Get order details
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}` - Update order information
- `PATCH /api/orders/{id}/status` - Update order status
- `PATCH /api/orders/{id}/payment-status` - Update payment status
- `DELETE /api/orders/{id}` - Soft delete order

### Dashboard & Statistics
- `GET /api/dashboard/order-status` - Get order status statistics
- `GET /api/dashboard/revenue` - Get revenue report (with optional date range)
- `GET /api/dashboard/orders-by-date` - Get orders grouped by date

### Product Management
- `GET /api/products` - Get products with pagination and search
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Customer Management
- `GET /api/customers` - Get customers with pagination and search
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

### Currency Rate
- `POST /api/currencyrates` - Configure a new exchange rate (stored as history)
- `GET /api/currencyrates` - Get configured exchange rate history for current account (newest first)
- `GET /api/currencyrates/latest` - Get latest configured exchange rate for current account

### Health Check
- `GET /api/health/live` - Liveness check
- `GET /api/health/ready` - Readiness check (includes DB connection test)

## Authentication

### Getting a Token

1. Create an admin user first:
```bash
POST /api/users
{
  "username": "admin",
  "password": "admin123",
  "fullName": "Administrator",
  "role": "ADMIN"
}
```

2. Login to get JWT token:
```bash
POST /api/auth/login
{
  "username": "admin",
  "password": "admin123"
}
```

3. Use the token in Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

## Database Schema

### Users Table
- `id` - Primary key (bigint)
- `username` - Username (unique, varchar)
- `password_hash` - Hashed password
- `full_name` - Full name (varchar)
- `role` - User role (ADMIN, SELLER)
- `is_active` - Active status (boolean)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

### Orders Table
- `id` - Primary key (bigint)
- `order_code` - Order code (unique, varchar)
- `order_date` - Order creation date
- `customer_name` - Customer name (varchar)
- `product_name` - Product name (varchar)
- `specification` - Product specification (text)
- `quantity` - Quantity (integer)
- `selling_price` - Selling price (decimal)
- `status` - Order status (DRAFT, NEW, ORDERED, ARRIVED, CANCELLED, DELETED)
- `payment_status` - Payment status (UNPAID, PARTIAL, PAID, REFUNDED)
- `yuan_price` - Yuan price (decimal)
- `import_price` - Import price (decimal)
- `supplier` - Supplier name (varchar)
- `warehouse_payment` - Warehouse payment (decimal)
- `shipping_weight_fee` - Shipping weight fee (decimal)
- `shipping_payment_date` - Shipping payment date
- `refund_amount` - Refund amount (decimal)
- `refund_status` - Refund status (varchar)
- `note` - Notes (text)
- `created_by` - Created by user ID (bigint)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp
- `deleted` - Soft delete flag (boolean)

### Products Table
- `id` - Primary key (bigint)
- `product_code` - Product code (varchar)
- `name` - Product name (varchar)
- `specification` - Product specification (text)
- `unit` - Product unit (varchar)
- `default_selling_price` - Default selling price (decimal)
- `note` - Notes (text)
- `created_by` - Created by user ID (bigint)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp
- `deleted` - Soft delete flag (boolean)

### Customers Table
- `id` - Primary key (bigint)
- `customer_code` - Customer code (varchar)
- `full_name` - Customer full name (varchar)
- `phone` - Customer phone (varchar)
- `email` - Customer email (varchar)
- `address` - Customer address (varchar)
- `note` - Notes (text)
- `created_by` - Created by user ID (bigint)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp
- `deleted` - Soft delete flag (boolean)

### CurrencyRateHistories Table
- `id` - Primary key (bigint)
- `rate` - Exchange rate value (decimal 18,4)
- `note` - Notes for each rate change (text)
- `created_by` - Created by user ID (bigint)
- `created_at` - Creation timestamp

## Project Structure

```
API.Sale/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── OrdersController.cs
│   ├── DashboardController.cs
│   ├── ProductsController.cs
│   ├── CustomersController.cs
│   ├── CurrencyRatesController.cs
│   └── HealthController.cs
├── Models/
│   ├── User.cs
│   ├── Order.cs
│   ├── Product.cs
│   ├── Customer.cs
│   ├── CurrencyRateHistory.cs
│   └── Enums/
│       ├── UserRole.cs
│       ├── OrderStatus.cs
│       └── PaymentStatus.cs
├── Services/
│   ├── AuthService.cs
│   ├── TokenService.cs
│   ├── UserService.cs
│   ├── OrderService.cs
│   ├── DashboardService.cs
│   ├── ProductService.cs
│   ├── CustomerService.cs
│   ├── CurrencyRateService.cs
│   └── CurrentUserService.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   ├── ApiResponse.cs
│   ├── Auth/
│   ├── User/
│   ├── Order/
│   ├── Product/
│   ├── Customer/
│   ├── CurrencyRate/
│   └── Dashboard/
├── Utilities/
│   └── PasswordHasher.cs
├── Migrations/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

## API Response Format

All responses follow a standard format:

### Success Response
```json
{
  "success": true,
  "message": "Success",
  "data": {
    // Response data
  }
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error message"
}
```

## Error Handling

- `400 Bad Request` - Invalid input or missing required fields
- `401 Unauthorized` - Invalid credentials or missing token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server-side error

## Testing with VS Code REST Client

Use the `API.Sale.http` file to test all endpoints:
1. Install the "Rest Client" extension in VS Code
2. Open `API.Sale.http` file
3. Click "Send Request" above each endpoint

## Future Enhancements

- Supplier management
- Product image upload
- Chinese shipping tracking
- Vietnam shipping tracking
- Customer debt management
- Detailed profit reporting
- Excel/PDF export
- Zalo/Email notifications
- Analytics dashboard with charts
- Audit logging
- Multi-branch support

## License

This project is licensed under the MIT License.

## Support

For issues or questions, please contact the development team.
