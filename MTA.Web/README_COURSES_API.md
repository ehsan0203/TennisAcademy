# 🎾 Courses API Documentation

## Overview
The Courses API provides comprehensive management of tennis courses in the MTA (Modern Tennis Academy) system. This API supports CRUD operations, advanced filtering, search, recommendations, and statistics.

## 🔐 Authentication
Most endpoints require authentication. Use the `Authorization: Bearer {token}` header with a valid JWT token.

**Public Endpoints:** These can be accessed without authentication
- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get course by ID
- `GET /api/courses/search` - Search courses
- `GET /api/courses/filter` - Advanced filtering
- `GET /api/courses/level/{levelId}` - Get courses by level
- `GET /api/courses/popular` - Get popular courses
- `GET /api/courses/free` - Get free courses

**Protected Endpoints:** These require Admin or Coach role
- All POST, PUT, DELETE, and PATCH operations
- Statistics and management endpoints

## 📚 API Endpoints

### 1. Course CRUD Operations

#### Get All Courses
```http
GET /api/courses?page=1&pageSize=10&searchTerm=tennis&levelId=1&statusId=2&minPrice=0&maxPrice=100
```

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 10)
- `searchTerm` (string): Search in title and description
- `levelId` (int): Filter by skill level
- `statusId` (int): Filter by status
- `minPrice` (decimal): Minimum price filter
- `maxPrice` (decimal): Maximum price filter

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Tennis Fundamentals",
      "description": "Learn basic tennis techniques",
      "price": 49.99,
      "levelId": 1,
      "levelTitle": "Beginner",
      "statusId": 2,
      "statusValue": "Active",
      "lessonCount": 12,
      "purchaseCount": 45
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

#### Get Course by ID
```http
GET /api/courses/{id}
```

#### Create Course
```http
POST /api/courses
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Advanced Tennis Techniques",
  "description": "Master advanced tennis techniques",
  "imageIcon": "https://example.com/icon.jpg",
  "poster": "https://example.com/poster.jpg",
  "price": 99.99,
  "levelId": 2,
  "statusId": 1
}
```

#### Update Course
```http
PUT /api/courses/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Updated Title",
  "description": "Updated description",
  "price": 89.99
}
```

#### Delete Course
```http
DELETE /api/courses/{id}
Authorization: Bearer {token}
```

#### Toggle Course Status
```http
PATCH /api/courses/{id}/toggle-status
Authorization: Bearer {token}
```

### 2. Advanced Filtering and Search

#### Advanced Filtering
```http
POST /api/courses/filter
Content-Type: application/json

{
  "page": 1,
  "pageSize": 20,
  "searchTerm": "tennis",
  "levelId": 1,
  "statusId": 2,
  "minPrice": 0,
  "maxPrice": 100,
  "freeOnly": false,
  "sortBy": "title",
  "sortDirection": "asc"
}
```

**Filter Parameters:**
- `searchTerm`: Text search in title and description
- `levelId`: Filter by skill level
- `statusId`: Filter by status
- `minPrice`/`maxPrice`: Price range filter
- `freeOnly`: Show only free courses
- `sortBy`: Sort field (title, price, createdAt)
- `sortDirection`: Sort direction (asc, desc)

#### Text Search
```http
GET /api/courses/search?searchTerm=tennis&page=1&pageSize=10
```

### 3. Course Recommendations and Statistics

#### Get Recommended Courses
```http
GET /api/courses/recommended?userId=1&count=5
Authorization: Bearer {token}
```

#### Get Course Statistics
```http
GET /api/courses/statistics
Authorization: Bearer {token}
```

**Response:**
```json
{
  "totalCourses": 25,
  "activeCourses": 20,
  "draftCourses": 3,
  "archivedCourses": 2,
  "totalEnrollments": 150,
  "activeEnrollments": 120,
  "completedCourses": 30,
  "totalRevenue": 7500.00,
  "completionRate": 20.0,
  "mostPopularLevel": "Beginner",
  "mostEnrolledCourse": "Tennis Fundamentals"
}
```

### 4. Course Management Operations

#### Get Courses by Level
```http
GET /api/courses/level/{levelId}
```

#### Get Courses by Status
```http
GET /api/courses/status/{statusId}
Authorization: Bearer {token}
```

#### Get Popular Courses
```http
GET /api/courses/popular?count=10
```

#### Get Free Courses
```http
GET /api/courses/free
```

### 5. Course Status and Level Management

#### Change Course Status
```http
PATCH /api/courses/{id}/status
Authorization: Bearer {token}
Content-Type: application/json

2
```

#### Change Course Level
```http
PATCH /api/courses/{id}/level
Authorization: Bearer {token}
Content-Type: application/json

3
```

#### Update Course Price
```http
PATCH /api/courses/{id}/price
Authorization: Bearer {token}
Content-Type: application/json

79.99
```

## 🏗️ Data Models

### CourseDto
```csharp
public class CourseDto : BaseDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageIcon { get; set; }
    public string? Poster { get; set; }
    public decimal Price { get; set; }
    public int LevelId { get; set; }
    public string? LevelTitle { get; set; }
    public int StatusId { get; set; }
    public string? StatusValue { get; set; }
    public int LessonCount { get; set; }
    public int PurchaseCount { get; set; }
}
```

### CreateCourseDto
```csharp
public class CreateCourseDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageIcon { get; set; }
    public string? Poster { get; set; }
    public decimal Price { get; set; }
    public int LevelId { get; set; }
    public int StatusId { get; set; } = 1;
}
```

### UpdateCourseDto
```csharp
public class UpdateCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageIcon { get; set; }
    public string? Poster { get; set; }
    public decimal? Price { get; set; }
    public int? LevelId { get; set; }
    public int? StatusId { get; set; }
}
```

### CourseFilterDto
```csharp
public class CourseFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? LevelId { get; set; }
    public int? StatusId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? FreeOnly { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
}
```

## 🔒 Authorization Policies

- **RolesAdminCoach**: Admin and Coach users can create, update, delete, and manage courses
- **Public**: Anyone can view and search courses
- **Authenticated**: Users must be logged in for recommendations and personal features

## 📊 Status Codes

- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **204 No Content**: Request successful, no content returned
- **400 Bad Request**: Invalid request data
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

## 🚀 Usage Examples

### Frontend Integration
```javascript
// Get all courses with pagination
const getCourses = async (page = 1, pageSize = 10) => {
  const response = await fetch(`/api/courses?page=${page}&pageSize=${pageSize}`);
  return await response.json();
};

// Create new course
const createCourse = async (courseData) => {
  const response = await fetch('/api/courses', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(courseData)
  });
  return await response.json();
};

// Advanced filtering
const filterCourses = async (filterData) => {
  const response = await fetch('/api/courses/filter', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(filterData)
  });
  return await response.json();
};
```

### Mobile App Integration
```csharp
// Get recommended courses
public async Task<List<CourseDto>> GetRecommendedCoursesAsync(int userId)
{
    var response = await _httpClient.GetAsync($"/api/courses/recommended?userId={userId}&count=5");
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<CourseDto>>(content);
    }
    return new List<CourseDto>();
}
```

## 🔧 Error Handling

The API returns detailed error messages and appropriate HTTP status codes:

```json
{
  "message": "Course with ID 999 not found",
  "statusCode": 404,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 📈 Performance Considerations

- **Pagination**: All list endpoints support pagination to handle large datasets
- **Filtering**: Use query parameters and advanced filtering to reduce data transfer
- **Caching**: Consider caching frequently accessed course data
- **Search**: Text search is optimized for performance

## 🔍 Testing

Use the provided `Courses.http` file to test all API endpoints. The file includes examples for:
- Authentication
- CRUD operations
- Advanced filtering
- Error scenarios
- All available endpoints

## 📝 Notes

- Course deletion is soft-delete (status changed to archived) if students are enrolled
- Price updates affect future enrollments only
- Status changes are logged for audit purposes
- Recommendations are based on user's learning history and skill level
