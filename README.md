# MTA Tennis Academy - Online Learning Platform

## Overview
MTA Tennis Academy is a comprehensive online tennis learning platform that provides structured courses, personalized coaching, and interactive support through a ticket-based system. The platform is built using Clean Architecture principles and follows SOLID design patterns.

## Architecture
This project follows **Clean Architecture** principles with the following layers:

- **Domain Layer**: Core business entities and interfaces
- **Application Layer**: Business logic, DTOs, and application services
- **Infrastructure Layer**: Data access and external service implementations
- **Web Layer**: API controllers and presentation logic

## Technology Stack
- **.NET 8.0**: Latest LTS version with modern C# features
- **Entity Framework Core 8.0**: Modern ORM with SQL Server support
- **Clean Architecture**: Separation of concerns and dependency inversion
- **Repository Pattern**: Generic repository with Unit of Work
- **SOLID Principles**: Single responsibility, Open/Closed, Liskov substitution, Interface segregation, Dependency inversion
- **Swagger/OpenAPI**: Comprehensive API documentation

## Project Structure
```
MTA/
├── MTA.Domain/                 # Domain entities and interfaces
│   ├── Entities/               # Business entities
│   ├── Enums/                  # Domain enums
│   └── Interfaces/             # Repository and service interfaces
├── MTA.Application/            # Application layer
│   └── DTOs/                  # Data Transfer Objects
├── MTA.Infrastructure/        # Infrastructure layer
│   ├── Data/                  # DbContext and configurations
│   └── Repositories/          # Repository implementations
├── MTA.Infrastructure.Persistence/  # Persistence configuration
└── MTA.Web/                   # Web API layer
    └── Controllers/           # API controllers
```

## Core Entities

### User Management
- **Account**: User authentication and basic info
- **UserProfile**: Personal and tennis-related information
- **Role**: User roles (Student, Coach, Admin)
- **Permission**: System permissions
- **RolePermission**: Role-permission mapping

### Learning System
- **Level**: Skill levels (Beginner, Intermediate, Advanced)
- **Course**: Tennis courses with lessons
- **Lesson**: Individual learning units
- **MediaFile**: Course content files (videos, documents, images)

### Support System
- **Package**: Coaching service packages
- **Ticket**: Support tickets for coaching assistance
- **Message**: Communication within tickets

### History Tracking
- **HistoryUserCourse**: Course purchase history
- **HistoryPackage**: Package purchase history

## Key Features

### 1. Generic Repository Pattern
- Type-safe repository operations
- Consistent CRUD operations across all entities
- Async/await support for better performance

### 2. Unit of Work Pattern
- Transaction management
- Multiple repository coordination
- Atomic operations support

### 3. Clean Architecture Benefits
- **Testability**: Easy to unit test business logic
- **Maintainability**: Clear separation of concerns
- **Scalability**: Easy to add new features
- **Independence**: Framework and database agnostic

### 4. Performance Optimizations
- Efficient Entity Framework configurations
- Lazy loading for navigation properties
- Optimized database queries
- Async operations throughout the stack

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB for development)
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Navigate to the MTA.Web directory
3. Update connection string in `appsettings.json`
4. Run the following commands:

```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run database migrations (if needed)
dotnet ef database update

# Run the application
dotnet run
```

### Database Setup
The application uses Entity Framework Core with SQL Server. The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MTATennisAcademy;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## API Documentation
Once the application is running, you can access the Swagger UI at:
- **Development**: `https://localhost:5001` or `http://localhost:5000`
- **Swagger UI**: Available at the root URL for comprehensive API documentation

### Key API Endpoints
- **Levels**: `/api/levels` - Manage tennis skill levels
- **Tickets**: `/api/tickets` - Support ticket management
- **Courses**: `/api/courses` - Course management
- **Accounts**: `/api/accounts` - User account management

## Development Guidelines

### Code Quality
- Follow C# coding conventions
- Use XML documentation for public APIs
- Implement proper error handling
- Write unit tests for business logic

### Database Design
- Use meaningful table and column names
- Implement proper foreign key relationships
- Consider performance implications of indexes
- Use appropriate data types and constraints

### API Design
- RESTful endpoint design
- Consistent response formats
- Proper HTTP status codes
- Comprehensive error messages

## Performance Considerations
- Async/await for I/O operations
- Efficient Entity Framework queries
- Proper database indexing
- Connection pooling
- Caching strategies where appropriate

## Security Features
- Input validation
- SQL injection prevention
- Proper authentication and authorization
- Secure connection strings

## Future Enhancements
- Authentication and authorization system
- File upload and management
- Real-time notifications
- Payment integration
- Mobile app support
- Advanced analytics and reporting

## Contributing
1. Follow the established architecture patterns
2. Write comprehensive tests
3. Update documentation
4. Follow the coding standards
5. Submit pull requests with detailed descriptions

## License
This project is proprietary software developed for MTA Tennis Academy.

## Support
For technical support or questions, please contact the development team at dev@mta-tennis.com.
