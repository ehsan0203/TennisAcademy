# MTA Tennis Academy - Quick Start Guide

## 🚀 Getting Started

### Prerequisites
- .NET Core 8.0 SDK
- SQL Server (LocalDB recommended)
- Visual Studio 2022 or VS Code

### Quick Setup

1. **Clone and Navigate**
   ```bash
   cd "MTA Project\MTA"
   ```

2. **Build the Project**
   ```bash
   dotnet build
   ```

3. **Run the Application**
   ```bash
   dotnet run --project MTA.Web
   ```

4. **Access the API**
   - Swagger UI: `https://localhost:7044/swagger`
   - Test Endpoint: `https://localhost:7044/api/test`
   - Health Check: `https://localhost:7044/api/test/health`

## 🔐 Default Credentials

- **Admin User**: `admin@mta.com` / `Admin123!`
- **Role**: Admin with full permissions

## 📚 API Endpoints

### Public Endpoints
- `GET /api/test` - Basic functionality test
- `GET /api/test/health` - Health check

### Authentication Endpoints
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User authentication

### Protected Endpoints
- `GET /api/levels` - View skill levels (Admin/Coach)
- `GET /api/roles` - View roles (Admin only)

## 🛠️ Troubleshooting

### Common Issues

1. **Port Already in Use**
   - Change port in `launchSettings.json`
   - Kill existing processes: `netstat -ano | findstr :7044`

2. **Database Connection Error**
   - Ensure SQL Server is running
   - Check connection string in `appsettings.json`

3. **Build Errors**
   - Clean solution: `dotnet clean`
   - Restore packages: `dotnet restore`
   - Rebuild: `dotnet build`

### Database Reset
If you encounter database issues:
1. Stop the application
2. Delete the database from SQL Server
3. Restart the application (database will be recreated)

## 📖 Next Steps

1. **Test Authentication**: Use Swagger to test login/register
2. **Explore API**: Check all available endpoints
3. **Frontend Integration**: Use the API with your frontend application

## 🆘 Support

For issues or questions:
- Check the main README.md
- Review error logs in the console
- Ensure all prerequisites are met

---

**Happy Coding! 🎾✨**

