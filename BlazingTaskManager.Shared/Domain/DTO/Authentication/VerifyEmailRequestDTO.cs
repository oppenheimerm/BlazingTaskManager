
using System.ComponentModel.DataAnnotations;

namespace BlazingTaskManager.Shared.Domain.DTO.Authentication
{
    public class VerifyEmailRequestDTO
    {
        [Required]
        public string? Token { get; set; }
    }
}
