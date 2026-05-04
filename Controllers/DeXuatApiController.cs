using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CNPM.Services;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeXuatApiController : ControllerBase
    {
        private readonly IDeXuatService _deXuatService;

        public DeXuatApiController(IDeXuatService deXuatService)
        {
            _deXuatService = deXuatService;
        }

        // POST: api/dexuatapi/taoduthao
        // Test SUC-Z01.1
        [HttpPost("taoduthao")]
        public async Task<IActionResult> TaoDuThao()
        {
            try
            {
                // Giả lập Input từ UI
                var tuNgay = DateTime.Now.AddDays(-40); // 40 ngày (hợp lệ)
                var denNgay = DateTime.Now;
                var maNVTao = "NV01";

                var draft = await _deXuatService.TaoDuThaoAsync(tuNgay, denNgay, maNVTao);
                return Ok(new { Success = true, Message = "Tạo dự thảo thành công", Data = draft });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // POST: api/dexuatapi/duyet
        // Test SUC-Z01.3
        [HttpPost("duyet")]
        public async Task<IActionResult> DuyetDeXuat(string maDeXuat, string quyetDinh, string lyDo)
        {
            try
            {
                var maNVDuyet = "QL01"; // Giả lập người duyệt
                var result = await _deXuatService.ProcessDecisionAsync(maDeXuat, quyetDinh, lyDo, null, maNVDuyet);
                
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
