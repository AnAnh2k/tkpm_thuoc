using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CNPM.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Đã cập nhật thủ công bằng SQL: 
            // ALTER TABLE tbl_KhachHang ADD bTrangThai bit NULL;
            // UPDATE tbl_KhachHang SET bTrangThai = 1;
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nếu muốn rollback thì xóa cột thủ công trong SQL
        }
    }
}
