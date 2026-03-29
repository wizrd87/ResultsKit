# ResultsKit

`ResultsKit` provides a simple result pattern for application code.

`ResultsKit.Http` maps those results to ASP.NET Core HTTP responses.

## ResultsKit

Use `Result` when an operation only needs success or failure.

```csharp
using ResultsKit;

public class UserService
{
    public Result DeleteUser(int id)
    {
        var exists = id > 0;
        if (!exists)
        {
            return new NotFoundError("user", id);
        }

        return Result.Ok();
    }
}
```

Use `Result<T>` when an operation returns data on success.

```csharp
using ResultsKit;

public class UserService
{
    public Result<UserDto> GetUser(int id)
    {
        if (id <= 0)
        {
            return new ValidationError(["A valid user id is required."]);
        }

        if (id == 404)
        {
            return new NotFoundError("user", id);
        }

        return new UserDto(id, "Alex");
    }
}

public record UserDto(int Id, string Name);
```

Available error types:

- `Error(code, message)` for a custom failure
- `NotFoundError(resource, reference)` when a resource does not exist
- `ConflictError(resource, reference, conflictingReference)` when a request conflicts with an existing resource
- `UnauthorizedError(action)` when the caller is not allowed to perform an action
- `ValidationError(messages)` when validation fails

You can also create results explicitly:

```csharp
using ResultsKit;

var success = Result.Ok();
var successWithValue = Result.OkResult(new UserDto(1, "Alex"));
var failure = Result.Fail(new Error("user_disabled", "The user is disabled."));
var partial = Result.Partial(new Error("warning", "The operation completed with warnings."));
```

## ResultsKit.Http

Use `ResultsKit.Http.Process` to translate a `Result` or `Result<T>` into an HTTP response.

### Minimal APIs

```csharp
using ResultsKit.Http;

app.MapGet("/users/{id:int}", (int id, UserService service) =>
{
    var result = service.GetUser(id);
    return Process.ApiResult(result);
});
```

`ApiResult(...)` returns:

- `200 OK` for successful results
- `400 Bad Request` for generic and validation failures
- `401 Unauthorized` for `UnauthorizedError`
- `404 Not Found` for `NotFoundError`
- `409 Conflict` for `ConflictError`

Error responses are returned as problem details payloads with neutral extension fields such as `resource`, `reference`, `conflictingReference`, `messages`, and `action`.

### MVC / Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using ResultsKit.Http;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult<UserDto> Get(int id, [FromServices] UserService service)
    {
        var result = service.GetUser(id);
        return Process.MvcResult(result);
    }
}
```

`MvcResult(...)` applies the same status-code mapping and returns ASP.NET Core `ActionResult` / `ActionResult<T>` values.

## Typical Pattern

Keep business logic in your service layer and return `Result` or `Result<T>` there.

```csharp
using ResultsKit;

public class OrderService
{
    public Result<OrderDto> CreateOrder(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerCode))
        {
            return new ValidationError(["Customer code is required."]);
        }

        if (request.CustomerCode == "blocked")
        {
            return new UnauthorizedError("create-order");
        }

        return new OrderDto(123, request.CustomerCode);
    }
}

public record CreateOrderRequest(string CustomerCode);
public record OrderDto(int Id, string CustomerCode);
```

Then translate that result at the API boundary with `Process.ApiResult(...)` or `Process.MvcResult(...)`.
