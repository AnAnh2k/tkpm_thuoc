using System;
using System.Threading.Tasks;
using CNPM.Models;

namespace CNPM.Services
{
    public interface IDeXuatService
    {
        Task<DeXuatToiUu> TaoDuThaoAsync(DateTime tuNgay, DateTime denNgay, string maNVTao);
        Task<OperationResult> ProcessDecisionAsync(string maDeXuat, string quyetDinh, string lyDo, DateTime? ngayXemLai, string maNVDuyet);
    }
}
