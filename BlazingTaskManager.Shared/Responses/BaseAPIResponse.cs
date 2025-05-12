
using BlazingTaskManager.Shared.Domain;
using BlazingTaskManager.Shared.Domain.DTO.User;

namespace BlazingTaskManager.Shared.Responses
{
    public record BaseAPIResponse
    (
        bool Success = false,
        string Message = null!
    );
    // Authentication
    public record APIResponseAuthentication(
    bool Success = false,
    string Message = null!,
    BTUserDTO? User = null!,
    string? JwtToken = "",
    string? RefreshToken = ""
    ) : BaseAPIResponse(Success, Message);
    //  User
    public record APIResponseBTUserDTO(
    bool Success = false,
    string Message = null!,
    BTUser? User = null!
    ) : BaseAPIResponse(Success, Message);
    // Roles
    public record APIResponseRole(
    bool Success = false,
    string Message = null!,
    Role? Role = null!
    ) : BaseAPIResponse(Success, Message);

}
