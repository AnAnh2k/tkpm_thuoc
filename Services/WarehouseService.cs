using CNPM.Infrastructure.Repositories;
using CNPM.Models;
using CNPM.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public class WarehouseService : IWarehouseService
{
    private readonly PharmacyDbContext _context;
    private readonly IRepository<TblPhieuYeuCau> _phieuYeuCauRepository;
    private readonly IRepository<TblCtphieuYeuCau> _ctPhieuYeuCauRepository;
    private readonly IRepository<TblPhieuNhapKho> _phieuNhapRepository;
    private readonly IRepository<TblCtphieuNhapKho> _ctPhieuNhapRepository;
    private readonly IRepository<TblPhieuGiaoHang> _phieuGiaoHangRepository;
    private readonly IRepository<TblBienBanKiemKe> _bienBanKiemKeRepository;
    private readonly IRepository<TblCtbienBanKiemKe> _ctBienBanKiemKeRepository;
    private readonly IRepository<TblBienBanHuy> _bienBanHuyRepository;
    private readonly IRepository<TblCtbienBanHuy> _ctBienBanHuyRepository;
    private readonly IRepository<TblNhanVien> _nhanVienRepository;
    private readonly IRepository<TblSanPham> _sanPhamRepository;

    public WarehouseService(
        PharmacyDbContext context,
        IRepository<TblPhieuYeuCau> phieuYeuCauRepository,
        IRepository<TblCtphieuYeuCau> ctPhieuYeuCauRepository,
        IRepository<TblPhieuNhapKho> phieuNhapRepository,
        IRepository<TblCtphieuNhapKho> ctPhieuNhapRepository,
        IRepository<TblPhieuGiaoHang> phieuGiaoHangRepository,
        IRepository<TblBienBanKiemKe> bienBanKiemKeRepository,
        IRepository<TblCtbienBanKiemKe> ctBienBanKiemKeRepository,
        IRepository<TblBienBanHuy> bienBanHuyRepository,
        IRepository<TblCtbienBanHuy> ctBienBanHuyRepository,
        IRepository<TblNhanVien> nhanVienRepository,
        IRepository<TblSanPham> sanPhamRepository)
    {
        _context = context;
        _phieuYeuCauRepository = phieuYeuCauRepository;
        _ctPhieuYeuCauRepository = ctPhieuYeuCauRepository;
        _phieuNhapRepository = phieuNhapRepository;
        _ctPhieuNhapRepository = ctPhieuNhapRepository;
        _phieuGiaoHangRepository = phieuGiaoHangRepository;
        _bienBanKiemKeRepository = bienBanKiemKeRepository;
        _ctBienBanKiemKeRepository = ctBienBanKiemKeRepository;
        _bienBanHuyRepository = bienBanHuyRepository;
        _ctBienBanHuyRepository = ctBienBanHuyRepository;
        _nhanVienRepository = nhanVienRepository;
        _sanPhamRepository = sanPhamRepository;
    }

    public async Task<OperationResultDto> CreatePhieuYeuCauAsync(CreatePhieuYeuCauDto request)
    {
        if (request.Items.Count == 0 || request.Items.Any(x => string.IsNullOrWhiteSpace(x.FkSMaSp) || x.ISl <= 0))
        {
            return new OperationResultDto { Success = false, Message = "Danh sách sản phẩm yêu cầu không hợp lệ." };
        }

        var maPyc = await GenerateCodeAsync("PYC", code => _phieuYeuCauRepository.AnyAsync(x => x.PkSMaPyc == code));

        await _phieuYeuCauRepository.AddAsync(new TblPhieuYeuCau
        {
            PkSMaPyc = maPyc,
            DTgLap = DateTime.Now,
            FkSMaNv = request.FkSMaNguoiLap,
            BTrangThai = false
        });

        _ctPhieuYeuCauRepository.AddRange(request.Items.Select(i => new TblCtphieuYeuCau
        {
            PkFkSMaPyc = maPyc,
            PkFkSMaSp = i.FkSMaSp,
            ISlcan = i.ISl,
            ISlduyet = 0
        }));

        await _context.SaveChangesAsync();
        return new OperationResultDto { Success = true, Message = "Lập phiếu yêu cầu thành công." };
    }

    public async Task<OperationResultDto> CreatePhieuNhapAsync(CreatePhieuNhapDto request)
    {
        if (request.Items.Count == 0 || request.Items.Any(x => string.IsNullOrWhiteSpace(x.FkSMaSp) || x.ISl <= 0))
        {
            return new OperationResultDto { Success = false, Message = "Danh sách sản phẩm nhập không hợp lệ." };
        }

        if (!await _phieuGiaoHangRepository.AnyAsync(x => x.PkSMaPgh == request.FkSMaPgh))
        {
            return new OperationResultDto { Success = false, Message = "Không tìm thấy phiếu giao hàng." };
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var maPn = await GenerateCodeAsync("PN", code => _phieuNhapRepository.AnyAsync(x => x.PkSMaPn == code));
            await _phieuNhapRepository.AddAsync(new TblPhieuNhapKho
            {
                PkSMaPn = maPn,
                FkSMaPgh = request.FkSMaPgh,
                DTgLap = DateTime.Now,
                FkSMaNv = request.FkSMaNguoiLap
            });

            foreach (var item in request.Items)
            {
                await _ctPhieuNhapRepository.AddAsync(new TblCtphieuNhapKho
                {
                    PkFkSMaPn = maPn,
                    PkFkSMaSp = item.FkSMaSp,
                    ISl = item.ISl,
                    SGhiChu = item.SGhiChu
                });

                var sanPham = await _sanPhamRepository.FirstOrDefaultAsync(sp => sp.PkSMaSp == item.FkSMaSp);
                if (sanPham == null)
                {
                    await transaction.RollbackAsync();
                    return new OperationResultDto { Success = false, Message = $"Không tìm thấy sản phẩm {item.FkSMaSp}." };
                }

                sanPham.ISl = (sanPham.ISl ?? 0) + item.ISl;
                _sanPhamRepository.Update(sanPham);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new OperationResultDto { Success = true, Message = "Nhập kho thành công - tồn kho đã được cập nhật." };
        }
        catch
        {
            await transaction.RollbackAsync();
            return new OperationResultDto { Success = false, Message = "Nhập kho thất bại." };
        }
    }

    public async Task<OperationResultDto> CreateBienBanKiemKeAsync(CreateBienBanKiemKeDto request)
    {
        if (request.Items.Count == 0)
        {
            return new OperationResultDto { Success = false, Message = "Chưa có dữ liệu kiểm kê." };
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var maBbk = await GenerateCodeAsync("BBK", code => _bienBanKiemKeRepository.AnyAsync(x => x.PkSMaBbk == code));
            var members = await _nhanVienRepository.Query()
                .Where(nv => request.ThanhVienIds.Contains(nv.PkSMaNv))
                .ToListAsync();

            var bbk = new TblBienBanKiemKe
            {
                PkSMaBbk = maBbk,
                DTgLap = DateTime.Now,
                FkSMaNv = request.FkSMaNguoiLap,
                SDiaDiemKiem = request.SDiaDiemKiem,
                DTgBatDau = request.DTgBatDau,
                DTgKetThuc = request.DTgKetThuc,
                BTrangThai = true,
                PkFkSMaNguoiKiems = members
            };

            await _bienBanKiemKeRepository.AddAsync(bbk);

            foreach (var item in request.Items)
            {
                await _ctBienBanKiemKeRepository.AddAsync(new TblCtbienBanKiemKe
                {
                    PkFkSMaBbk = maBbk,
                    PkFkSMaSp = item.FkSMaSp,
                    ISl = item.ISl
                });

                if (request.DieuChinhTonKho)
                {
                    var sp = await _sanPhamRepository.FirstOrDefaultAsync(x => x.PkSMaSp == item.FkSMaSp);
                    if (sp != null)
                    {
                        sp.ISl = item.ISl;
                        _sanPhamRepository.Update(sp);
                    }
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new OperationResultDto { Success = true, Message = "Lưu biên bản kiểm kê thành công." };
        }
        catch
        {
            await transaction.RollbackAsync();
            return new OperationResultDto { Success = false, Message = "Lưu biên bản kiểm kê thất bại." };
        }
    }

    public async Task<OperationResultDto> CreateBienBanHuyAsync(CreateBienBanHuyDto request)
    {
        if (request.Items.Count == 0)
        {
            return new OperationResultDto { Success = false, Message = "Chưa có sản phẩm cần hủy." };
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var maBbh = await GenerateCodeAsync("BBH", code => _bienBanHuyRepository.AnyAsync(x => x.PkSMaBbh == code));
            var members = await _nhanVienRepository.Query()
                .Where(nv => request.ThanhVienIds.Contains(nv.PkSMaNv))
                .ToListAsync();

            var bbh = new TblBienBanHuy
            {
                PkSMaBbh = maBbh,
                DTgLap = DateTime.Now,
                FkSMaNv = request.FkSMaNguoiLap,
                SDiaDiemHuy = request.SDiaDiemHuy,
                DTgBatDau = request.DTgBatDau,
                DTgKetThuc = request.DTgKetThuc,
                SPhuongThucHuy = request.SPhuongThucHuy,
                BTrangThai = true,
                PkFkSMaNguoiHuys = members
            };

            await _bienBanHuyRepository.AddAsync(bbh);

            foreach (var item in request.Items)
            {
                var sp = await _sanPhamRepository.FirstOrDefaultAsync(x => x.PkSMaSp == item.FkSMaSp);
                if (sp == null)
                {
                    await transaction.RollbackAsync();
                    return new OperationResultDto { Success = false, Message = $"Không tìm thấy sản phẩm {item.FkSMaSp}." };
                }

                if ((sp.ISl ?? 0) < item.ISl)
                {
                    await transaction.RollbackAsync();
                    return new OperationResultDto { Success = false, Message = "Số lượng hủy vượt quá tồn kho hiện có." };
                }

                await _ctBienBanHuyRepository.AddAsync(new TblCtbienBanHuy
                {
                    PkFkSMaBbh = maBbh,
                    PkFkSMaSp = item.FkSMaSp,
                    ISl = item.ISl,
                    SLyDo = item.SLyDo
                });

                sp.ISl = (sp.ISl ?? 0) - item.ISl;
                _sanPhamRepository.Update(sp);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new OperationResultDto { Success = true, Message = "Lập biên bản hủy thành công - tồn kho đã được điều chỉnh." };
        }
        catch
        {
            await transaction.RollbackAsync();
            return new OperationResultDto { Success = false, Message = "Lập biên bản hủy thất bại." };
        }
    }

    public Task<List<PhieuGiaoHangItemDto>> GetChiTietPhieuGiaoHangAsync(string maPgh)
    {
        return _context.TblCtphieuGiaoHangs
            .AsNoTracking()
            .Where(ct => ct.PkFkSMaPgh == maPgh)
            .Include(ct => ct.PkFkSMaSpNavigation)
            .Select(ct => new PhieuGiaoHangItemDto
            {
                MaSp = ct.PkFkSMaSp,
                TenSp = ct.PkFkSMaSpNavigation.STenSp,
                SoLuongGiao = ct.ISl ?? 0
            })
            .ToListAsync();
    }

    public Task<List<TonKhoSnapshotDto>> GetTonKhoHienTaiAsync()
    {
        return _sanPhamRepository.Query()
            .AsNoTracking()
            .OrderBy(sp => sp.STenSp)
            .Select(sp => new TonKhoSnapshotDto
            {
                MaSp = sp.PkSMaSp,
                TenSp = sp.STenSp,
                SoLuongHeThong = sp.ISl ?? 0
            })
            .ToListAsync();
    }

    private static async Task<string> GenerateCodeAsync(string prefix, Func<string, Task<bool>> existedCheck)
    {
        while (true)
        {
            var candidate = $"{prefix}{DateTime.Now:yyyyMMddHHmmssfff}";
            if (!await existedCheck(candidate))
            {
                return candidate;
            }

            await Task.Delay(2);
        }
    }
}
