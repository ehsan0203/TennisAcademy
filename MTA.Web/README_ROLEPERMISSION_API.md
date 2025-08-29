# RolePermission API Documentation

## Overview

The RolePermission API provides comprehensive management of role-permission relationships in the MTA system. This API allows administrators to assign, remove, and manage permissions for different user roles, enabling fine-grained access control across the application.

## Base URL

```
http://localhost:5000/api/RolePermission
```

## Authentication

All endpoints require authentication with a valid JWT token. Include the token in the Authorization header:

```
Authorization: Bearer your_jwt_token_here
```

**Required Role:** Admin

## API Endpoints

### 1. RolePermission CRUD Operations

#### Get All Role Permissions
```http
GET /api/RolePermission?page={page}&pageSize={pageSize}&roleId={roleId}&permissionId={permissionId}
```

**Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 10)
- `roleId` (optional): Filter by role ID
- `permissionId` (optional): Filter by permission ID

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "roleId": 1,
      "roleTitle": "Admin",
      "permissionId": 3,
      "permissionTitle": "User Management",
      "permissionDescription": "Can manage user accounts",
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

#### Get Role Permission by ID
```http
GET /api/RolePermission/{id}
```

**Response:**
```json
{
  "id": 1,
  "roleId": 1,
  "roleTitle": "Admin",
  "permissionId": 3,
  "permissionTitle": "User Management",
  "permissionDescription": "Can manage user accounts",
        "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
}
```

#### Create Role Permission
```http
POST /api/RolePermission
```

**Request Body:**
```json
{
  "roleId": 2,
  "permissionId": 5
}
```

**Response:** 201 Created with the created role permission

#### Update Role Permission
```http
PUT /api/RolePermission/{id}
```

**Request Body:**
```json
{
  "roleId": 2,
  "permissionId": 6
}
```

**Response:** 200 OK with the updated role permission

#### Delete Role Permission
```http
DELETE /api/RolePermission/{id}
```

**Response:** 204 No Content

### 2. RolePermission Queries

#### Get Role Permissions by Role
```http
GET /api/RolePermission/role/{roleId}
```

**Response:**
```json
[
  {
    "id": 1,
    "roleId": 1,
    "roleTitle": "Admin",
    "permissionId": 3,
    "permissionTitle": "User Management",
    "permissionDescription": "Can manage user accounts",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  },
  {
    "id": 2,
    "roleId": 1,
    "roleTitle": "Admin",
    "permissionId": 4,
    "permissionTitle": "Course Management",
    "permissionDescription": "Can manage courses",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  }
]
```

#### Get Role Permissions by Permission
```http
GET /api/RolePermission/permission/{permissionId}
```

**Response:** Array of role permissions that have the specified permission

#### Check Role Has Permission
```http
GET /api/RolePermission/check?roleId={roleId}&permissionId={permissionId}
```

**Response:** `true` or `false`

### 3. RolePermission Management Operations

#### Assign Permission to Role
```http
POST /api/RolePermission/assign?roleId={roleId}&permissionId={permissionId}
```

**Response:** 201 Created with the created role permission

#### Remove Permission from Role
```http
DELETE /api/RolePermission/remove?roleId={roleId}&permissionId={permissionId}
```

**Response:** 204 No Content

#### Bulk Assign Permissions to Role
```http
POST /api/RolePermission/bulk-assign?roleId={roleId}
```

**Request Body:**
```json
[1, 2, 3, 4]
```

**Response:** 201 Created with array of created role permissions

#### Bulk Remove Permissions from Role
```http
DELETE /api/RolePermission/bulk-remove?roleId={roleId}
```

**Request Body:**
```json
[1, 2]
```

**Response:** 204 No Content

### 4. RolePermission Statistics and Reports

#### Get Role Permission Statistics
```http
GET /api/RolePermission/statistics
```

**Response:**
```json
{
  "totalRolePermissions": 150,
  "totalRoles": 8,
  "totalPermissions": 25,
  "averagePermissionsPerRole": 18.75,
  "mostAssignedPermission": "View Dashboard",
  "leastAssignedPermission": "System Configuration",
  "rolesWithMostPermissions": [
    {
      "roleTitle": "Admin",
      "permissionCount": 25
    },
    {
      "roleTitle": "Moderator",
      "permissionCount": 18
    }
  ]
}
```

## Data Models

### RolePermissionDto
```csharp
public class RolePermissionDto : BaseDto
{
    public int RoleId { get; set; }
    public string? RoleTitle { get; set; }
    public int PermissionId { get; set; }
    public string? PermissionTitle { get; set; }
    public string? PermissionDescription { get; set; }
}
```

### RolePermissionStatisticsDto
```csharp
public class RolePermissionStatisticsDto
{
    public int TotalRolePermissions { get; set; }
    public int TotalRoles { get; set; }
    public int TotalPermissions { get; set; }
    public double AveragePermissionsPerRole { get; set; }
    public string? MostAssignedPermission { get; set; }
    public string? LeastAssignedPermission { get; set; }
    public IEnumerable<RolePermissionCountDto> RolesWithMostPermissions { get; set; }
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

1. **Duplicate Prevention**: A role cannot have the same permission assigned multiple times
2. **Role Validation**: The specified role must exist in the system
3. **Permission Validation**: The specified permission must exist in the system
4. **Admin Only**: All operations require Admin role access
5. **Cascade Effects**: Removing a role permission may affect user access immediately

## Usage Examples

### Assign Multiple Permissions to a New Role

```http
POST /api/RolePermission/bulk-assign?roleId=5
Authorization: Bearer admin_token
Content-Type: application/json

[1, 2, 3, 4, 5]
```

### Check User's Role Permissions

```http
GET /api/RolePermission/role/3
Authorization: Bearer admin_token
```

### Remove Specific Permission from Role

```http
DELETE /api/RolePermission/remove?roleId=2&permissionId=7
Authorization: Bearer admin_token
```

## Testing

Use the provided `RolePermission.http` file to test all API endpoints. Make sure to:

1. Replace `your_jwt_token_here` with a valid admin JWT token
2. Test both successful and error scenarios
3. Verify authorization requirements
4. Test pagination and filtering
5. Validate business rule enforcement

## Security Considerations

- All endpoints require Admin role authentication
- JWT tokens should be securely stored and transmitted
- Consider implementing rate limiting for bulk operations
- Audit logging for all role permission changes
- Regular review of role-permission assignments

## Related APIs

- **Roles API**: For managing user roles
- **Permissions API**: For managing system permissions
- **Users API**: For user management and role assignment
- **Auth API**: For authentication and authorization
