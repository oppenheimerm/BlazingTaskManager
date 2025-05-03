
using BlazingTaskManager.Shared.Domain;

namespace BlazingTaskManager.Shared.Responses
{
    public record BaseAPIResponse
    (
        bool Success = false,
        string Message = null!
    );
    // Add Roles
    public record APIResponseRole(
    bool Success = false,
    string Message = null!,
    Role? Role = null!
    ) : BaseAPIResponse(Success, Message);
}
