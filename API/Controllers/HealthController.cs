using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Endpoint kiểm tra trạng thái hệ thống.
    /// </summary>
    public class HealthController : ApiController
    {
        /// <summary>
        /// Kiểm tra API đang hoạt động.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                timeUtc = DateTime.UtcNow
            });
        }
    }
}

