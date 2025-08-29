# PackageHistory API Documentation

## Overview
The PackageHistory API provides comprehensive functionality for managing package history in the MTA system. Package history tracks user purchases, usage, and expiration of service packages including tickets and messages. The API supports various operations including CRUD operations, usage tracking, expiration management, and detailed analytics.

## Base URL
```
https://localhost:7001/api/packagehistory
```

## Authentication
All endpoints require authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

## API Endpoints

### 1. PackageHistory CRUD Operations

#### Get All Package Histories
```http
GET /api/packagehistory
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Page size (default: 10)
- `accountId` (optional): Filter by account ID
- `packageId` (optional): Filter by package ID
- `isExpired` (optional): Filter by expired status (true/false)

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "expiredDate": "2024-12-31T23:59:59Z",
      "remainingTickets": 8,
      "remainingMessages": 18,
      "packageId": 1,
      "packageTitle": "Basic Package",
      "packagePrice": 29.99,
      "totalTickets": 10,
      "totalMessages": 20,
      "accountId": 1,
      "userFirstName": "John",
      "userLastName": "Doe",
      "userEmail": "john.doe@example.com",
      "isExpired": false,
      "createdAt": "2024-01-01T10:00:00Z",
      "updatedAt": "2024-01-15T14:30:00Z"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

#### Get Package History by ID
```http
GET /api/packagehistory/{id}
```

**Response:**
```json
{
  "id": 1,
  "expiredDate": "2024-12-31T23:59:59Z",
  "remainingTickets": 8,
  "remainingMessages": 18,
  "packageId": 1,
  "packageTitle": "Basic Package",
  "packagePrice": 29.99,
  "totalTickets": 10,
  "totalMessages": 20,
  "accountId": 1,
  "userFirstName": "John",
  "userLastName": "Doe",
  "userEmail": "john.doe@example.com",
  "isExpired": false,
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": "2024-01-15T14:30:00Z"
}
```

#### Create New Package History
```http
POST /api/packagehistory
```

**Request Body:**
```json
{
  "expiredDate": "2024-12-31T23:59:59Z",
  "remainingTickets": 10,
  "remainingMessages": 20,
  "packageId": 1,
  "accountId": 1
}
```

**Response:** 201 Created with the created package history

#### Update Package History
```http
PUT /api/packagehistory/{id}
```

**Request Body:**
```json
{
  "expiredDate": "2024-12-31T23:59:59Z",
  "remainingTickets": 8,
  "remainingMessages": 18,
  "packageId": 1,
  "accountId": 1
}
```

**Response:** 200 OK with the updated package history

#### Delete Package History
```http
DELETE /api/packagehistory/{id}
```

**Response:** 204 No Content on success

### 2. PackageHistory Update Operations

#### Update Remaining Tickets
```http
PATCH /api/packagehistory/{id}/remaining-tickets
```

**Request Body:** Integer representing the new remaining tickets count

**Response:** 200 OK with the updated package history

#### Update Remaining Messages
```http
PATCH /api/packagehistory/{id}/remaining-messages
```

**Request Body:** Integer representing the new remaining messages count

**Response:** 200 OK with the updated package history

#### Extend Package Expiration
```http
PATCH /api/packagehistory/{id}/extend-expiration
```

**Request Body:** DateTime string representing the new expiration date

**Response:** 200 OK with the updated package history

### 3. PackageHistory Queries

#### Get Package Histories by Account ID
```http
GET /api/packagehistory/account/{accountId}
```

**Response:** Array of package histories for the specified account

#### Get Package Histories by Package ID
```http
GET /api/packagehistory/package/{packageId}
```

**Response:** Array of package histories for the specified package

#### Get Active Package Histories for Account
```http
GET /api/packagehistory/account/{accountId}/active
```

**Response:** Array of active (non-expired) package histories for the account

#### Get Expired Package Histories for Account
```http
GET /api/packagehistory/account/{accountId}/expired
```

**Response:** Array of expired package histories for the account

#### Check if User Has Active Package
```http
GET /api/packagehistory/check-active?accountId={accountId}&packageId={packageId}
```

**Response:** Boolean indicating if user has an active package

#### Get Package Histories by Date Range
```http
GET /api/packagehistory/date-range?startDate={startDate}&endDate={endDate}
```

**Query Parameters:**
- `startDate`: Start date (ISO format: YYYY-MM-DD)
- `endDate`: End date (ISO format: YYYY-MM-DD)

**Response:** Array of package histories within the specified date range

#### Get Expiring Packages
```http
GET /api/packagehistory/expiring?days={days}
```

**Query Parameters:**
- `days`: Number of days (default: 7)

**Response:** Array of packages expiring within the specified days

### 4. PackageHistory Statistics and Reports

#### Get Package History Statistics
```http
GET /api/packagehistory/statistics
```

**Response:**
```json
{
  "totalPackageHistories": 100,
  "activePackages": 75,
  "expiredPackages": 25,
  "totalRevenue": 2999.99,
  "totalTicketsSold": 1000,
  "totalMessagesSold": 2000,
  "totalTicketsUsed": 800,
  "totalMessagesUsed": 1600,
  "averageTicketsPerPackage": 10.0,
  "averageMessagesPerPackage": 20.0,
  "packagesThisMonth": 15,
  "packagesLastMonth": 20,
  "revenueThisMonth": 449.99,
  "revenueLastMonth": 599.99
}
```

#### Get User Package Usage Summary
```http
GET /api/packagehistory/account/{accountId}/usage-summary
```

**Response:**
```json
{
  "accountId": 1,
  "userFirstName": "John",
  "userLastName": "Doe",
  "totalPackagesPurchased": 5,
  "activePackages": 2,
  "expiredPackages": 3,
  "totalSpent": 149.99,
  "totalTicketsPurchased": 50,
  "totalMessagesPurchased": 100,
  "totalTicketsUsed": 35,
  "totalMessagesUsed": 70,
  "remainingTickets": 15,
  "remainingMessages": 30,
  "nextExpiryDate": "2024-12-31T23:59:59Z",
  "packageUsage": [
    {
      "packageId": 1,
      "packageTitle": "Basic Package",
      "purchaseDate": "2024-01-01T10:00:00Z",
      "expiryDate": "2024-12-31T23:59:59Z",
      "isExpired": false,
      "totalTickets": 10,
      "totalMessages": 20,
      "usedTickets": 7,
      "usedMessages": 14,
      "remainingTickets": 3,
      "remainingMessages": 6,
      "usagePercentage": 70.0
    }
  ]
}
```

## Data Models

### PackageHistoryDto
```csharp
public class PackageHistoryDto : BaseDto
{
    public DateTime ExpiredDate { get; set; }
    public int RemainingTickets { get; set; }
    public int RemainingMessages { get; set; }
    public int PackageId { get; set; }
    public string? PackageTitle { get; set; }
    public decimal PackagePrice { get; set; }
    public int TotalTickets { get; set; }
    public int TotalMessages { get; set; }
    public int AccountId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserEmail { get; set; }
    public bool IsExpired { get; set; }
}
```

### PackageHistoryStatisticsDto
```csharp
public class PackageHistoryStatisticsDto
{
    public int TotalPackageHistories { get; set; }
    public int ActivePackages { get; set; }
    public int ExpiredPackages { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalTicketsSold { get; set; }
    public int TotalMessagesSold { get; set; }
    public int TotalTicketsUsed { get; set; }
    public int TotalMessagesUsed { get; set; }
    public double AverageTicketsPerPackage { get; set; }
    public double AverageMessagesPerPackage { get; set; }
    public int PackagesThisMonth { get; set; }
    public int PackagesLastMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}
```

### UserPackageUsageSummaryDto
```csharp
public class UserPackageUsageSummaryDto
{
    public int AccountId { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public int TotalPackagesPurchased { get; set; }
    public int ActivePackages { get; set; }
    public int ExpiredPackages { get; set; }
    public int TotalSpent { get; set; }
    public int TotalTicketsPurchased { get; set; }
    public int TotalMessagesPurchased { get; set; }
    public int TotalTicketsUsed { get; set; }
    public int TotalMessagesUsed { get; set; }
    public int RemainingTickets { get; set; }
    public int RemainingMessages { get; set; }
    public DateTime? NextExpiryDate { get; set; }
    public List<PackageUsageDto> PackageUsage { get; set; }
}
```

### PackageUsageDto
```csharp
public class PackageUsageDto
{
    public int PackageId { get; set; }
    public string PackageTitle { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public int TotalTickets { get; set; }
    public int TotalMessages { get; set; }
    public int UsedTickets { get; set; }
    public int UsedMessages { get; set; }
    public int RemainingTickets { get; set; }
    public int RemainingMessages { get; set; }
    public double UsagePercentage { get; set; }
}
```

## Package History States

### Active Package
- `ExpiredDate` is in the future
- `RemainingTickets` > 0 or `RemainingMessages` > 0
- User can use the package services

### Expired Package
- `ExpiredDate` is in the past
- Package services are no longer available
- Can be renewed or extended

### Expiring Package
- `ExpiredDate` is within a specified number of days
- User should be notified to renew or extend

## Error Handling

### Common HTTP Status Codes
- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **204 No Content**: Request successful, no content to return
- **400 Bad Request**: Invalid request data
- **401 Unauthorized**: Authentication required
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

### Error Response Format
```json
{
  "message": "Error description"
}
```

## Usage Examples

### JavaScript/TypeScript
```typescript
// Get all package histories
const response = await fetch('/api/packagehistory?page=1&pageSize=10', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
const packageHistories = await response.json();

// Create a new package history
const newPackageHistory = await fetch('/api/packagehistory', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    expiredDate: '2024-12-31T23:59:59Z',
    remainingTickets: 10,
    remainingMessages: 20,
    packageId: 1,
    accountId: 1
  })
});

// Update remaining tickets
await fetch('/api/packagehistory/1/remaining-tickets', {
  method: 'PATCH',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(8)
});

// Extend package expiration
await fetch('/api/packagehistory/1/extend-expiration', {
  method: 'PATCH',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify('2025-01-31T23:59:59Z')
});
```

### cURL
```bash
# Get all package histories
curl -X GET "https://localhost:7001/api/packagehistory" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json"

# Create a package history
curl -X POST "https://localhost:7001/api/packagehistory" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "expiredDate": "2024-12-31T23:59:59Z",
    "remainingTickets": 10,
    "remainingMessages": 20,
    "packageId": 1,
    "accountId": 1
  }'

# Update remaining tickets
curl -X PATCH "https://localhost:7001/api/packagehistory/1/remaining-tickets" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '8'

# Extend expiration
curl -X PATCH "https://localhost:7001/api/packagehistory/1/extend-expiration" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '"2025-01-31T23:59:59Z"'
```

## Testing
Use the provided `PackageHistory.http` file in VS Code with the REST Client extension to test all endpoints.

## Notes
- Package history tracks user purchases and usage of service packages
- Remaining tickets and messages are updated as users consume services
- Expired packages can be extended or renewed
- Statistics provide insights into package usage patterns and revenue
- All operations require authentication
- Date parameters should be in ISO format (YYYY-MM-DD)
- Negative values are not allowed for remaining tickets/messages
- Expiration dates must be in the future
- The system automatically calculates `IsExpired` based on current date
