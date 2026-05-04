using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CNPM.Models;
using CNPM.Infrastructure.Repositories;

namespace CNPM.Services
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class DeXuatService : IDeXuatService
    {
        private readonly IRepository<DeXuatToiUu> _deXuatRepo;
        private readonly IRepository<NhatKyHeThong> _auditRepo;
        private readonly PharmacyDbContext _context;

        public DeXuatService(
            IRepository<DeXuatToiUu> deXuatRepo, 
            IRepository<NhatKyHeThong> auditRepo,
            PharmacyDbContext context)
        {
            _deXuatRepo = deXuatRepo;
            _auditRepo = auditRepo;
            _context = context;
        }

        public async Task<DeXuatToiUu> TaoDuThaoAsync(DateTime tuNgay, DateTime denNgay, string maNVTao)
        {
            if ((denNgay - tuNgay).TotalDays < 30)
            {
                throw new Exception("Dữ liệu không đủ 30 ngày hợp lệ.");
            }

            // Giả lập logic phát hiện bất thường
            string hanhDong = "Cảnh báo tồn kho: Cần giảm giá 10% để đẩy hàng.";

            var draft = new DeXuatToiUu
            {
                ma_de_xuat = "SCI-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                tu_ngay = tuNgay,
                den_ngay = denNgay,
                danh_sach_hanh_dong = hanhDong,
                trang_thai = "Chờ duyệt",
                phien_ban = "v1.0",
                ngay_tao = DateTime.Now
            };

            await _deXuatRepo.AddAsync(draft);
            await _context.SaveChangesAsync();
            return draft;
        }

        public async Task<OperationResult> ProcessDecisionAsync(string maDeXuat, string quyetDinh, string lyDo, DateTime? ngayXemLai, string maNVDuyet)
        {
            // 1. Kiểm tra độ dài Rationale
            if (string.IsNullOrWhiteSpace(lyDo) || lyDo.Length < 20)
            {
                return new OperationResult { Success = false, Message = "Vui lòng nhập Lý do (Rationale) >= 20 ký tự." };
            }

            var deXuat = await _deXuatRepo.FirstOrDefaultAsync(x => x.ma_de_xuat == maDeXuat);
            if (deXuat == null || deXuat.trang_thai != "Chờ duyệt")
                return new OperationResult { Success = false, Message = "SCI không tồn tại hoặc sai trạng thái." };

            // 2. Kiểm tra nhánh "Trì hoãn"
            if (quyetDinh == "Trì hoãn")
            {
                deXuat.trang_thai = "Trì hoãn";
                deXuat.ngay_xem_lai = ngayXemLai;
                
                _deXuatRepo.Update(deXuat);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Đã lưu trạng thái Trì hoãn." };
            }

            // Bắt đầu khối Transaction ACID
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                deXuat.quyet_dinh = quyetDinh;
                deXuat.ly_do = lyDo;
                deXuat.ma_nv_thuc_hien = maNVDuyet; 
                deXuat.trang_thai = "Đã hoàn tất";
                deXuat.is_baseline = true; 

                _deXuatRepo.Update(deXuat);

                // 3. Kiểm tra nhánh "Chấp nhận"
                if (quyetDinh == "Chấp nhận")
                {
                    // Mock inventory update
                    // await _inventoryService.UpdateThresholdAsync(deXuat.danh_sach_hanh_dong);
                }

                // Ghi Audit Trail
                var log = new NhatKyHeThong
                {
                    ma_log = Guid.NewGuid().ToString(),
                    ma_de_xuat = deXuat.ma_de_xuat,
                    ma_nv = maNVDuyet,
                    hanh_dong = $"Duyệt đề xuất: {quyetDinh}",
                    chi_tiet = lyDo,
                    thoi_gian = DateTime.Now
                };
                await _auditRepo.AddAsync(log);

                await _context.SaveChangesAsync();
                
                // 4. Commit thành công
                await transaction.CommitAsync();
                return new OperationResult { Success = true, Message = "SCI đã được lưu trữ, Baseline đã xác lập." };
            }
            catch (Exception ex)
            {
                // Rollback khi lỗi
                await transaction.RollbackAsync();
                return new OperationResult { Success = false, Message = "Lưu trữ thất bại. Giao dịch đã bị Rollback." };
            }
        }
    }
}
