# Message API Documentation

## Overview
The Message API provides comprehensive functionality for managing messages in the MTA system. Messages are typically associated with support tickets and can include text content and media files.

## Base URL
```
https://localhost:7001/api/message
```

## Authentication
All endpoints require authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

## API Endpoints

### 1. Message CRUD Operations

#### Get All Messages
```http
GET /api/message
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Page size (default: 10)
- `searchTerm` (optional): Search term for text content
- `ticketId` (optional): Filter by ticket ID
- `senderId` (optional): Filter by sender ID
- `isRead` (optional): Filter by read status

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "text": "Message text content",
      "isRead": false,
      "ticketId": 1,
      "ticketTopic": "Support Request",
      "senderId": 1,
      "senderFirstName": "John",
      "senderLastName": "Doe",
      "senderImage": "profile.jpg",
      "mediaFileCount": 2,
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

#### Get Message by ID
```http
GET /api/message/{id}
```

**Response:**
```json
{
  "id": 1,
  "text": "Message text content",
  "isRead": false,
  "ticketId": 1,
  "ticketTopic": "Support Request",
  "senderId": 1,
  "senderFirstName": "John",
  "senderLastName": "Doe",
  "senderImage": "profile.jpg",
  "mediaFileCount": 2,
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": null
}
```

#### Create New Message
```http
POST /api/message
```

**Request Body:**
```json
{
  "text": "New message content",
  "ticketId": 1,
  "senderId": 1,
  "isRead": false
}
```

**Response:** 201 Created with the created message

#### Update Message
```http
PUT /api/message/{id}
```

**Request Body:**
```json
{
  "text": "Updated message content",
  "ticketId": 1,
  "senderId": 1,
  "isRead": false
}
```

**Response:** 200 OK with the updated message

#### Delete Message
```http
DELETE /api/message/{id}
```

**Response:** 204 No Content on success

### 2. Message Status Operations

#### Mark Message as Read
```http
PATCH /api/message/{id}/mark-read
```

**Response:** 200 OK with the updated message

#### Mark Message as Unread
```http
PATCH /api/message/{id}/mark-unread
```

**Response:** 200 OK with the updated message

#### Mark All Messages in Ticket as Read
```http
PATCH /api/message/ticket/{ticketId}/mark-all-read
```

**Response:** 200 OK with the count of messages marked as read

### 3. Message Queries

#### Get Messages by Ticket ID
```http
GET /api/message/ticket/{ticketId}
```

**Response:** Array of messages for the specified ticket

#### Get Messages by Sender ID
```http
GET /api/message/sender/{senderId}
```

**Response:** Array of messages from the specified sender

#### Get All Unread Messages
```http
GET /api/message/unread
```

**Response:** Array of all unread messages

#### Get Unread Messages by Ticket ID
```http
GET /api/message/ticket/{ticketId}/unread
```

**Response:** Array of unread messages for the specified ticket

#### Get Messages by Date Range
```http
GET /api/message/date-range?startDate={startDate}&endDate={endDate}
```

**Query Parameters:**
- `startDate`: Start date (ISO format: YYYY-MM-DD)
- `endDate`: End date (ISO format: YYYY-MM-DD)

**Response:** Array of messages within the specified date range

### 4. Message Statistics

#### Get Message Statistics
```http
GET /api/message/statistics
```

**Response:**
```json
{
  "totalMessages": 100,
  "readMessages": 75,
  "unreadMessages": 25,
  "messagesWithMedia": 30,
  "averageMediaFilesPerMessage": 1.5,
  "messagesThisMonth": 15,
  "messagesLastMonth": 20,
  "averageResponseTime": 2.5
}
```

## Data Models

### MessageDto
```csharp
public class MessageDto : BaseDto
{
    public required string Text { get; set; }
    public bool IsRead { get; set; }
    public int TicketId { get; set; }
    public string? TicketTopic { get; set; }
    public int SenderId { get; set; }
    public string? SenderFirstName { get; set; }
    public string? SenderLastName { get; set; }
    public string? SenderImage { get; set; }
    public int MediaFileCount { get; set; }
}
```

### MessageStatisticsDto
```csharp
public class MessageStatisticsDto
{
    public int TotalMessages { get; set; }
    public int ReadMessages { get; set; }
    public int UnreadMessages { get; set; }
    public int MessagesWithMedia { get; set; }
    public double AverageMediaFilesPerMessage { get; set; }
    public int MessagesThisMonth { get; set; }
    public int MessagesLastMonth { get; set; }
    public double AverageResponseTime { get; set; }
}
```

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
// Get all messages
const response = await fetch('/api/message?page=1&pageSize=10', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
const messages = await response.json();

// Create a new message
const newMessage = await fetch('/api/message', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    text: 'Hello, I need help!',
    ticketId: 1,
    senderId: 1,
    isRead: false
  })
});

// Mark message as read
await fetch('/api/message/1/mark-read', {
  method: 'PATCH',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
```

### cURL
```bash
# Get all messages
curl -X GET "https://localhost:7001/api/message" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json"

# Create a message
curl -X POST "https://localhost:7001/api/message" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Support request",
    "ticketId": 1,
    "senderId": 1,
    "isRead": false
  }'

# Mark message as read
curl -X PATCH "https://localhost:7001/api/message/1/mark-read" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json"
```

## Testing
Use the provided `Message.http` file in VS Code with the REST Client extension to test all endpoints.

## Notes
- Messages are automatically associated with tickets and senders
- Media files can be attached to messages (counted in MediaFileCount)
- Read status is tracked for each message individually
- Bulk operations are available for marking multiple messages as read
- Date range queries support ISO date format
- Statistics provide insights into message usage patterns
