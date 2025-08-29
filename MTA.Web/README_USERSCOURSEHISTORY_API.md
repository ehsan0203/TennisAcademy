# UserCourseHistory API Documentation

## Overview

The UserCourseHistory API provides comprehensive management of user course enrollment and purchase history in the MTA system. This API allows tracking of course purchases, user learning progress, and provides analytics for business intelligence and user experience improvements.

## Base URL

```
http://localhost:5000/api/UserCourseHistory
```

## Authentication

All endpoints require authentication with a valid JWT token. Include the token in the Authorization header:

```
Authorization: Bearer your_jwt_token_here
```

**Required Role:** Any authenticated user (varies by endpoint)

## API Endpoints

### 1. UserCourseHistory CRUD Operations

#### Get All User Course Histories
```http
GET /api/UserCourseHistory?page={page}&pageSize={pageSize}&accountId={accountId}&courseId={courseId}
```

**Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 10)
- `accountId` (optional): Filter by account ID
- `courseId` (optional): Filter by course ID

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "courseId": 1,
      "courseTitle": "Tennis Fundamentals",
      "courseDescription": "Learn the basics of tennis",
      "courseImageIcon": "tennis-icon.png",
      "coursePrice": 49.99,
      "accountId": 3,
      "userFirstName": "John",
      "userLastName": "Doe",
      "userEmail": "john.doe@example.com",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

#### Get User Course History by ID
```http
GET /api/UserCourseHistory/{id}
```

**Response:**
```json
{
  "id": 1,
  "courseId": 1,
  "courseTitle": "Tennis Fundamentals",
  "courseDescription": "Learn the basics of tennis",
  "courseImageIcon": "tennis-icon.png",
  "coursePrice": 49.99,
  "accountId": 3,
  "userFirstName": "John",
  "userLastName": "Doe",
  "userEmail": "john.doe@example.com",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

#### Create User Course History
```http
POST /api/UserCourseHistory
```

**Request Body:**
```json
{
  "courseId": 1,
  "accountId": 3
}
```

**Response:** 201 Created with the created user course history

#### Update User Course History
```http
PUT /api/UserCourseHistory/{id}
```

**Request Body:**
```json
{
  "courseId": 2,
  "accountId": 3
}
```

**Response:** 200 OK with the updated user course history

#### Delete User Course History
```http
DELETE /api/UserCourseHistory/{id}
```

**Response:** 204 No Content

### 2. UserCourseHistory Queries

#### Get User Course Histories by Account
```http
GET /api/UserCourseHistory/account/{accountId}
```

**Response:**
```json
[
  {
    "id": 1,
    "courseId": 1,
    "courseTitle": "Tennis Fundamentals",
    "courseDescription": "Learn the basics of tennis",
    "courseImageIcon": "tennis-icon.png",
    "coursePrice": 49.99,
    "accountId": 3,
    "userFirstName": "John",
    "userLastName": "Doe",
    "userEmail": "john.doe@example.com",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  },
  {
    "id": 2,
    "courseId": 2,
    "courseTitle": "Advanced Tennis Techniques",
    "courseDescription": "Master advanced tennis skills",
    "courseImageIcon": "advanced-tennis-icon.png",
    "coursePrice": 79.99,
    "accountId": 3,
    "userFirstName": "John",
    "userLastName": "Doe",
    "userEmail": "john.doe@example.com",
    "createdAt": "2024-01-20T14:15:00Z",
    "updatedAt": "2024-01-20T14:15:00Z"
  }
]
```

#### Get User Course Histories by Course
```http
GET /api/UserCourseHistory/course/{courseId}
```

**Response:** Array of user course histories for the specified course

#### Check User Has Purchased Course
```http
GET /api/UserCourseHistory/check-purchase?accountId={accountId}&courseId={courseId}
```

**Response:** `true` or `false`

### 3. UserCourseHistory Analytics and Reports

#### Get User Course History Statistics
```http
GET /api/UserCourseHistory/statistics
```

**Response:**
```json
{
  "totalPurchases": 150,
  "totalRevenue": 11250.00,
  "uniqueUsers": 45,
  "uniqueCourses": 8,
  "averageCoursesPerUser": 3.33,
  "averageRevenuePerUser": 250.00,
  "purchasesThisMonth": 25,
  "purchasesLastMonth": 30,
  "revenueThisMonth": 1875.00,
  "revenueLastMonth": 2250.00
}
```

#### Get User Course Histories by Date Range
```http
GET /api/UserCourseHistory/date-range?startDate={startDate}&endDate={endDate}
```

**Response:** Array of user course histories within the specified date range

#### Get Popular Courses
```http
GET /api/UserCourseHistory/popular-courses?count={count}
```

**Response:** Array of popular courses ordered by purchase count

#### Get User Learning Progress
```http
GET /api/UserCourseHistory/learning-progress/{accountId}
```

**Response:**
```json
{
  "accountId": 3,
  "userFirstName": "John",
  "userLastName": "Doe",
  "totalCoursesPurchased": 5,
  "totalCoursesCompleted": 2,
  "totalLessonsCompleted": 15,
  "totalSpent": 299.95,
  "completionRate": 40.0,
  "lastActivityDate": "2024-01-25T16:30:00Z",
  "courseProgress": [
    {
      "courseId": 1,
      "courseTitle": "Tennis Fundamentals",
      "totalLessons": 12,
      "completedLessons": 8,
      "progressPercentage": 66.67,
      "purchaseDate": "2024-01-15T10:30:00Z",
      "lastAccessDate": "2024-01-25T16:30:00Z"
    }
  ]
}
```

## Data Models

### UserCourseHistoryDto
```csharp
public class UserCourseHistoryDto : BaseDto
{
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public string? CourseDescription { get; set; }
    public string? CourseImageIcon { get; set; }
    public decimal CoursePrice { get; set; }
    public int AccountId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserEmail { get; set; }
}
```

### UserCourseHistoryStatisticsDto
```csharp
public class UserCourseHistoryStatisticsDto
{
    public int TotalPurchases { get; set; }
    public decimal TotalRevenue { get; set; }
    public int UniqueUsers { get; set; }
    public int UniqueCourses { get; set; }
    public double AverageCoursesPerUser { get; set; }
    public double AverageRevenuePerUser { get; set; }
    public int PurchasesThisMonth { get; set; }
    public int PurchasesLastMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}
```

### UserLearningProgressDto
```csharp
public class UserLearningProgressDto
{
    public int AccountId { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public int TotalCoursesPurchased { get; set; }
    public int TotalCoursesCompleted { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public decimal TotalSpent { get; set; }
    public double CompletionRate { get; set; }
    public DateTime LastActivityDate { get; set; }
    public List<CourseProgressDto> CourseProgress { get; set; }
}
```

### CourseProgressDto
```csharp
public class CourseProgressDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; }
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public double ProgressPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? LastAccessDate { get; set; }
}
```

## Error Handling

### Common HTTP Status Codes

- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **204 No Content**: Request successful, no content to return
- **400 Bad Request**: Invalid request data or business rule violation
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

### Error Response Format
```json
{
  "message": "Error description",
  "statusCode": 400
}
```

## Business Rules

1. **Duplicate Prevention**: A user cannot purchase the same course multiple times
2. **Course Validation**: The specified course must exist in the system
3. **Account Validation**: The specified account must exist in the system
4. **Authentication Required**: All operations require valid JWT authentication
5. **Data Integrity**: Course purchase history is maintained for analytics and access control

## Usage Examples

### Purchase a Course for a User

```http
POST /api/UserCourseHistory
Authorization: Bearer user_token
Content-Type: application/json

{
  "courseId": 1,
  "accountId": 3
}
```

### Check User's Course Purchases

```http
GET /api/UserCourseHistory/account/3
Authorization: Bearer user_token
```

### Get Learning Analytics

```http
GET /api/UserCourseHistory/learning-progress/3
Authorization: Bearer user_token
```

### View Popular Courses

```http
GET /api/UserCourseHistory/popular-courses?count=5
Authorization: Bearer user_token
```

## Testing

Use the provided `UserCourseHistory.http` file to test all API endpoints. Make sure to:

1. Replace `your_jwt_token_here` with a valid JWT token
2. Test both successful and error scenarios
3. Verify authentication requirements
4. Test pagination and filtering
5. Validate business rule enforcement
6. Test date range queries with various date formats

## Security Considerations

- All endpoints require JWT token authentication
- JWT tokens should be securely stored and transmitted
- Consider implementing rate limiting for analytics endpoints
- Audit logging for all course purchase activities
- Regular review of user access patterns

## Related APIs

- **Courses API**: For course information and management
- **Users API**: For user account management
- **Auth API**: For authentication and authorization
- **PackageHistory API**: For support package tracking
- **MediaFile API**: For course content and media files

## Analytics and Business Intelligence

The UserCourseHistory API provides valuable insights for:

- **Revenue Tracking**: Monitor course sales and revenue trends
- **User Engagement**: Track course completion rates and learning progress
- **Course Performance**: Identify popular and successful courses
- **User Behavior**: Understand purchasing patterns and preferences
- **Marketing Insights**: Target promotions and improve course offerings
- **Content Optimization**: Focus on high-performing course content
