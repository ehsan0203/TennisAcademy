# MediaFile API Documentation

## Overview
The MediaFile API provides comprehensive functionality for managing media files in the MTA system. Media files can be associated with lessons, messages, and have different types (video, audio, document, image). The API supports various operations including CRUD operations, filtering, and statistics.

## Base URL
```
https://localhost:7001/api/mediafile
```

## Authentication
All endpoints require authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

## API Endpoints

### 1. MediaFile CRUD Operations

#### Get All Media Files
```http
GET /api/mediafile
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Page size (default: 10)
- `searchTerm` (optional): Search term for title
- `typeId` (optional): Filter by type ID
- `lessonId` (optional): Filter by lesson ID
- `messageId` (optional): Filter by message ID

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Tennis Lesson Video",
      "url": "https://example.com/videos/tennis-lesson.mp4",
      "typeId": 1,
      "type": "MediaType",
      "typeValue": "Video",
      "lessonId": 1,
      "lessonTitle": "Basic Tennis Techniques",
      "messageId": null,
      "fileSize": 0,
      "fileExtension": ".mp4",
      "createdAt": "2024-01-01T10:00:00Z",
      "updatedAt": null
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

#### Get Media File by ID
```http
GET /api/mediafile/{id}
```

**Response:**
```json
{
  "id": 1,
  "title": "Tennis Lesson Video",
  "url": "https://example.com/videos/tennis-lesson.mp4",
  "typeId": 1,
  "type": "MediaType",
  "typeValue": "Video",
  "lessonId": 1,
  "lessonTitle": "Basic Tennis Techniques",
  "messageId": null,
  "fileSize": 0,
  "fileExtension": ".mp4",
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": null
}
```

#### Create New Media File
```http
POST /api/mediafile
```

**Request Body:**
```json
{
  "title": "Tennis Lesson Video",
  "url": "https://example.com/videos/tennis-lesson.mp4",
  "typeId": 1,
  "lessonId": 1,
  "messageId": null
}
```

**Response:** 201 Created with the created media file

#### Update Media File
```http
PUT /api/mediafile/{id}
```

**Request Body:**
```json
{
  "title": "Updated Tennis Lesson Video",
  "url": "https://example.com/videos/updated-tennis-lesson.mp4",
  "typeId": 1,
  "lessonId": 1,
  "messageId": null
}
```

**Response:** 200 OK with the updated media file

#### Delete Media File
```http
DELETE /api/mediafile/{id}
```

**Response:** 204 No Content on success

### 2. MediaFile Update Operations

#### Update Media File Type
```http
PATCH /api/mediafile/{id}/type
```

**Request Body:** Integer representing the new type ID

**Response:** 200 OK with the updated media file

#### Update Media File URL
```http
PATCH /api/mediafile/{id}/url
```

**Request Body:** String representing the new URL

**Response:** 200 OK with the updated media file

### 3. MediaFile Queries

#### Get Media Files by Type ID
```http
GET /api/mediafile/type/{typeId}
```

**Response:** Array of media files of the specified type

#### Get Media Files by Lesson ID
```http
GET /api/mediafile/lesson/{lessonId}
```

**Response:** Array of media files in the specified lesson

#### Get Media Files by Message ID
```http
GET /api/mediafile/message/{messageId}
```

**Response:** Array of media files in the specified message

#### Get Media Files by Extension
```http
GET /api/mediafile/extension/{extension}
```

**Response:** Array of media files with the specified extension

#### Get Media Files by Date Range
```http
GET /api/mediafile/date-range?startDate={startDate}&endDate={endDate}
```

**Query Parameters:**
- `startDate`: Start date (ISO format: YYYY-MM-DD)
- `endDate`: End date (ISO format: YYYY-MM-DD)

**Response:** Array of media files within the specified date range

#### Get Media Files by File Size Range
```http
GET /api/mediafile/size-range?minSize={minSize}&maxSize={maxSize}
```

**Query Parameters:**
- `minSize`: Minimum file size in bytes
- `maxSize`: Maximum file size in bytes

**Response:** Array of media files within the specified size range

### 4. MediaFile Statistics

#### Get Media File Statistics
```http
GET /api/mediafile/statistics
```

**Response:**
```json
{
  "totalMediaFiles": 100,
  "totalFileSize": 1073741824,
  "averageFileSize": 10737418.24,
  "filesByTypeVideo": 40,
  "filesByTypeAudio": 20,
  "filesByTypeDocument": 25,
  "filesByTypeImage": 10,
  "filesByTypeOther": 5,
  "filesInLessons": 60,
  "filesInMessages": 40,
  "filesThisMonth": 15,
  "filesLastMonth": 20,
  "filesPerExtension": {
    ".mp4": 40,
    ".mp3": 20,
    ".pdf": 25,
    ".jpg": 10,
    ".png": 5
  }
}
```

## Data Models

### MediaFileDto
```csharp
public class MediaFileDto : BaseDto
{
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string Type { get; set; }
    public int TypeId { get; set; }
    public string? TypeValue { get; set; }
    public int? LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public int? MessageId { get; set; }
    public long FileSize { get; set; }
    public string? FileExtension { get; set; }
}
```

### MediaFileStatisticsDto
```csharp
public class MediaFileStatisticsDto
{
    public int TotalMediaFiles { get; set; }
    public long TotalFileSize { get; set; }
    public double AverageFileSize { get; set; }
    public int FilesByTypeVideo { get; set; }
    public int FilesByTypeAudio { get; set; }
    public int FilesByTypeDocument { get; set; }
    public int FilesByTypeImage { get; set; }
    public int FilesByTypeOther { get; set; }
    public int FilesInLessons { get; set; }
    public int FilesInMessages { get; set; }
    public int FilesThisMonth { get; set; }
    public int FilesLastMonth { get; set; }
    public Dictionary<string, int> FilesPerExtension { get; set; }
}
```

## Media File Types
The system supports different media file types based on the `typeId`:

- **Type ID 1**: Video files (e.g., .mp4, .avi, .mov)
- **Type ID 2**: Audio files (e.g., .mp3, .wav, .aac)
- **Type ID 3**: Document files (e.g., .pdf, .doc, .txt)
- **Type ID 4**: Image files (e.g., .jpg, .png, .gif)
- **Other IDs**: Custom or additional file types

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
// Get all media files
const response = await fetch('/api/mediafile?page=1&pageSize=10', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
const mediaFiles = await response.json();

// Create a new media file
const newMediaFile = await fetch('/api/mediafile', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    title: 'Tennis Tutorial Video',
    url: 'https://example.com/videos/tutorial.mp4',
    typeId: 1,
    lessonId: 1,
    messageId: null
  })
});

// Update media file type
await fetch('/api/mediafile/1/type', {
  method: 'PATCH',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(2)
});
```

### cURL
```bash
# Get all media files
curl -X GET "https://localhost:7001/api/mediafile" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json"

# Create a media file
curl -X POST "https://localhost:7001/api/mediafile" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Tennis Tutorial",
    "url": "https://example.com/videos/tutorial.mp4",
    "typeId": 1,
    "lessonId": 1,
    "messageId": null
  }'

# Update media file type
curl -X PATCH "https://localhost:7001/api/mediafile/1/type" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '2'
```

## Testing
Use the provided `MediaFile.http` file in VS Code with the REST Client extension to test all endpoints.

## Notes
- Media files can be associated with lessons, messages, or both
- File size information is currently a placeholder and would need to be implemented with actual file storage
- The API supports various file types and extensions
- Statistics provide insights into media file usage patterns
- All operations require authentication
- File URLs should be valid and accessible
- The system automatically extracts file extensions from URLs
