using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Shared.Domain.DTO.Authentication
{
    public class AuthenticateRequestDTO
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [MinLength(7, ErrorMessage = "Passwordd must be a minimum of 7 characters long.")]
        public string? Password { get; set; }
    }
}
