
namespace BlazingTaskManager.Shared.Domain.DTO.Authentication
{
    //  Authentication is implemented with JWT access tokens and refresh tokens. On successful authentication
    //  the API returns a short lived JWT access token that expires after 15 minutes, and a refresh token that
    //  expires after 7 days in an HTTP Only cookie. The JWT is used for accessing secure routes on the API and
    //  the refresh token is used for generating new JWT access tokens when (or just before) they expire.
    //  We store these properties in localstorage
    public class AuthLocalStorageDTO
    {
        public string? JWtToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Guid? Id { get; set; }
    }
}
