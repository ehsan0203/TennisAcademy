# سیستم احراز هویت و مجوزدهی MTA

## خلاصه
این سیستم از `Authorize` استاندارد ASP.NET Core استفاده می‌کند و با JWT Token کار می‌کند. تمام اطلاعات کاربر (ID، ایمیل، نقش، نام و...) در توکن JWT ذخیره می‌شود.

## نحوه استفاده

### 1. احراز هویت ساده (هر کاربر احراز هویت شده)
```csharp
[Authorize]
public IActionResult ProtectedAction()
{
    // فقط کاربران احراز هویت شده می‌توانند به این اکشن دسترسی داشته باشند
}
```

### 2. احراز هویت بر اساس نقش خاص
```csharp
[Authorize(Policy = "RoleAdmin")]
public IActionResult AdminOnly()
{
    // فقط کاربران با نقش Admin می‌توانند به این اکشن دسترسی داشته باشند
}

[Authorize(Policy = "RoleStudent")]
public IActionResult StudentOnly()
{
    // فقط کاربران با نقش Student می‌توانند به این اکشن دسترسی داشته باشند
}

[Authorize(Policy = "RoleCoach")]
public IActionResult CoachOnly()
{
    // فقط کاربران با نقش Coach می‌توانند به این اکشن دسترسی داشته باشند
}
```

### 3. احراز هویت بر اساس چندین نقش
```csharp
[Authorize(Policy = "RolesAdminModerator")]
public IActionResult AdminOrModerator()
{
    // کاربران با نقش Admin یا Moderator می‌توانند به این اکشن دسترسی داشته باشند
}

[Authorize(Policy = "RolesAdminCoach")]
public IActionResult AdminOrCoach()
{
    // کاربران با نقش Admin یا Coach می‌توانند به این اکشن دسترسی داشته باشند
}
```

## دسترسی به اطلاعات کاربر در کنترلر

### Extension Methods موجود:

#### دریافت اطلاعات پایه:
```csharp
var userId = HttpContext.GetCurrentUserId();
var userEmail = HttpContext.GetCurrentUserEmail();
var userRole = HttpContext.GetCurrentUserRole();
var isAuthenticated = HttpContext.IsAuthenticated();
```

#### دریافت اطلاعات اضافی:
```csharp
var fullName = HttpContext.GetCurrentUserFullName();
var skillLevel = HttpContext.GetCurrentUserSkillLevel();
var experience = HttpContext.GetCurrentUserExperience();
var imageUrl = HttpContext.GetCurrentUserImageUrl();
var accountStatus = HttpContext.GetCurrentUserAccountStatus();
```

#### بررسی نقش:
```csharp
if (HttpContext.HasRole("Admin"))
{
    // کد مخصوص ادمین
}

if (HttpContext.HasAnyRole("Admin", "Moderator"))
{
    // کد برای ادمین یا مودریتور
}
```

#### دریافت تمام Claims:
```csharp
var allClaims = HttpContext.GetAllUserClaims();
```

## مثال کامل کنترلر

```csharp
[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return Ok("این اکشن برای همه قابل دسترسی است");
    }

    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult AuthenticatedOnly()
    {
        var userInfo = new
        {
            UserId = HttpContext.GetCurrentUserId(),
            Email = HttpContext.GetCurrentUserEmail(),
            Role = HttpContext.GetCurrentUserRole()
        };
        
        return Ok(userInfo);
    }

    [HttpGet("admin")]
    [Authorize(Policy = "RoleAdmin")]
    public IActionResult AdminOnly()
    {
        return Ok($"سلام ادمین! ID شما: {HttpContext.GetCurrentUserId()}");
    }

    [HttpGet("student")]
    [Authorize(Policy = "RoleStudent")]
    public IActionResult StudentOnly()
    {
        return Ok($"سلام دانشجو! سطح مهارت شما: {HttpContext.GetCurrentUserSkillLevel()}");
    }

    [HttpGet("elevated")]
    [Authorize(Policy = "RolesAdminModerator")]
    public IActionResult ElevatedPrivileges()
    {
        var userRole = HttpContext.GetCurrentUserRole();
        return Ok($"شما دسترسی ویژه دارید. نقش شما: {userRole}");
    }
}
```

## ساختار Policy Names

### برای یک نقش:
- `RoleAdmin` → فقط Admin
- `RoleStudent` → فقط Student
- `RoleCoach` → فقط Coach
- `RoleModerator` → فقط Moderator

### برای چندین نقش:
- `RolesAdminModerator` → Admin یا Moderator
- `RolesAdminCoach` → Admin یا Coach
- `RolesStudentCoach` → Student یا Coach

## نکات مهم

1. **JWT Token**: تمام اطلاعات کاربر در توکن JWT ذخیره می‌شود
2. **Performance**: نیازی به مراجعه به دیتابیس برای بررسی نقش نیست
3. **Security**: توکن قبل از ورود به اکشن اعتبارسنجی می‌شود
4. **Flexibility**: می‌توانید Policy های سفارشی تعریف کنید
5. **Standard**: از استانداردهای ASP.NET Core استفاده می‌کند

## عیب‌یابی

### اگر دسترسی ندارید:
1. مطمئن شوید که توکن JWT معتبر است
2. مطمئن شوید که توکن منقضی نشده است
3. مطمئن شوید که کاربر نقش مناسب را دارد
4. مطمئن شوید که حساب کاربر فعال است

### برای تست:
1. ابتدا با `/api/auth/login` لاگین کنید
2. توکن JWT را دریافت کنید
3. توکن را در هدر `Authorization: Bearer {token}` قرار دهید
4. اکشن مورد نظر را تست کنید
