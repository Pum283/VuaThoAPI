using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Base controller dùng chung cho các API v1 của hệ thống Vua Thợ.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiController : ControllerBase
    {
        /// <summary>
        /// Chuẩn hoá trả về kết quả theo <see cref="OperationResult{T}"/>.
        /// </summary>
        protected IActionResult HandleResult<T>(OperationResult<T> result)
        {
            if (result == null) return NotFound();
            
            if (result.IsSuccess && result.Data != null)
                return Ok(result.Data);
            
            if (result.IsSuccess && result.Data == null)
                return NotFound();
            
            if (result.Errors != null && result.Errors.Any())
                return BadRequest(result.Errors);
            
            return BadRequest(result.Message ?? "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
        }

        /// <summary>
        /// Chuẩn hoá trả về kết quả theo <see cref="OperationResult"/>.
        /// </summary>
        protected IActionResult HandleResult(OperationResult result)
        {
            if (result == null) return NotFound();
            
            if (result.IsSuccess)
                return Ok(new { message = result.Message });
            
            if (result.Errors != null && result.Errors.Any())
                return BadRequest(result.Errors);
            
            return BadRequest(result.Message ?? "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
        }
    }
}

