# Code-First Database Approach with EF Core

This project demonstrates a complete code-first approach to database development using Entity Framework Core with PostgreSQL.

## 🎯 What Was Implemented

### 1. Entity Models
- **Product**: Main entity with validation attributes
- **Category**: Supporting entity with one-to-many relationship
- Both entities include proper data annotations and navigation properties

### 2. Database Context (`ProductsDbContext`)
- Configured entity relationships using Fluent API
- Added indexes for better query performance
- Implemented data seeding with both entities and relationships
- Proper constraint configuration (foreign keys, cascading deletes)

### 3. Entity Framework Migrations
- **InitialCreate**: Creates Products table with seed data
- **AddCategoriesAndRelationships**: Adds Categories table and relationships
- Proper migration history tracking

### 4. Database Seeding Strategy
- **Migration-based seeding**: Initial seed data in `OnModelCreating`
- **Service-based seeding**: Additional data via `DatabaseSeedingService`
- Smart seeding logic that checks for existing data

### 5. RESTful API Endpoints

#### Products
- `GET /products` - Get all products with categories
- `GET /products/{id}` - Get specific product
- `GET /products/category/{categoryId}` - Get products by category
- `POST /products` - Create new product
- `PUT /products/{id}` - Update product
- `DELETE /products/{id}` - Delete product

#### Categories
- `GET /categories` - Get all categories with products
- `GET /categories/{id}` - Get specific category
- `POST /categories` - Create new category
- `PUT /categories/{id}` - Update category
- `DELETE /categories/{id}` - Delete category

## 🏗️ Code-First Benefits Demonstrated

1. **Schema Evolution**: Changes to entities automatically generate migrations
2. **Type Safety**: Strong typing in C# translates to proper database constraints
3. **Relationship Management**: Navigation properties handle foreign key relationships
4. **Validation**: Data annotations provide both API and database validation
5. **Indexing**: Performance optimizations defined in code
6. **Seeding**: Consistent data population across environments

## 🚀 Running the Application

1. **Start the containers**:
   ```bash
   docker compose up -d
   ```

2. **Run the microservice**:
   ```bash
   cd Microservice
   dotnet run
   ```

3. **Test the API**:
   ```bash
   ./test-api.sh
   ```

## 📁 File Structure

```
Microservice/
├── Models/
│   ├── Product.cs          # Product entity with validations
│   └── Category.cs         # Category entity
├── Data/
│   ├── ProductsDbContext.cs # DbContext with configurations
│   └── DatabaseSeedingService.cs # Advanced seeding logic
├── Migrations/             # EF Core migrations
│   ├── InitialCreate.cs
│   └── AddCategoriesAndRelationships.cs
├── Program.cs              # API endpoints and configuration
└── appsettings.json       # Database connection settings
```

## 🔄 Migration Workflow

1. **Make model changes** in entity classes
2. **Generate migration**: `dotnet ef migrations add MigrationName`
3. **Review migration** files for accuracy
4. **Apply migration**: Happens automatically on app startup
5. **Update seeding** if needed in `DatabaseSeedingService`

## 💾 Database Schema

### Categories Table
- `Id` (Primary Key, Auto-increment)
- `Name` (Required, Max 100 chars, Unique)
- `Description` (Optional, Max 500 chars)
- `CreatedAt` (Timestamp)

### Products Table
- `Id` (Primary Key, Auto-increment)
- `Name` (Required, Max 200 chars, Indexed)
- `Price` (Decimal 18,2, Required)
- `CategoryId` (Foreign Key, Optional)
- `CreatedAt` (Timestamp)

### Relationships
- Category → Products (One-to-Many)
- Product → Category (Many-to-One, Optional)

## 🎯 Key Features

- ✅ **Complete CRUD operations** for both entities
- ✅ **Relationship-aware queries** with Include()
- ✅ **Data validation** at multiple levels
- ✅ **Smart seeding** that avoids duplicates
- ✅ **Proper error handling** and HTTP status codes
- ✅ **Performance optimizations** with indexes
- ✅ **Migration-based schema evolution**

This implementation showcases professional-grade code-first development practices with Entity Framework Core.