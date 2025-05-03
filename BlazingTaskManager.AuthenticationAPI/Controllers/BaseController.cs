using BlazingTaskManager.Shared.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BlazingTaskManager.AuthenticationAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the current user account <see cref="BTUser"/> from the HTTP context.
        /// </summary>
        public BTUser? Account => (BTUser?)HttpContext.Items["Account"];
    }
}
