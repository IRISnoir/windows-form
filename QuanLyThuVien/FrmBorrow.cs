using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyThuVien.Models;
using QuanLyThuVien.Helpers;
using System.Data.SqlClient;

namespace QuanLyThuVien
{
    public partial class FrmBorrow : Form
    {
        public FrmBorrow()
        {
            InitializeComponent();
            
            this.btnSearchReader.Click += BtnSearchReader_Click;
            this.btnBorrow.Click += BtnBorrow_Click;
            this.btnCancel.Click += BtnCancel_Click;
            
            LoadBooksFromDB();
        }

        private void LoadBooksFromDB()
        {
            string query = "SELECT IDSach, TenSach, TacGia, TinhTrang FROM ThongTinSach WHERE TinhTrang = N'Sẵn sàng'";
            try
            {
                dgvBooks.DataSource = DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearchReader_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReaderID.Text))
            {
                MessageBox.Show("Vui lòng nhập mã độc giả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "SELECT HoTen FROM TheDocGia WHERE IDDocGia = @ID";
            SqlParameter[] parameters = { new SqlParameter("@ID", txtReaderID.Text) };
            
            try
            {
                object result = DatabaseHelper.ExecuteScalar(query, parameters);
                if (result != null)
                {
                    MessageBox.Show($"Tìm thấy độc giả: {result} (Mã: {txtReaderID.Text})", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy độc giả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm độc giả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReaderID.Text))
            {
                MessageBox.Show("Vui lòng chọn độc giả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một quyển sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phieuMuonID = "PM" + DateTime.Now.ToString("yyyyMMddHHmmss").Substring(8);
            
            try
            {
                // 1. Insert into PhieuMuon
                string queryPhieu = "INSERT INTO PhieuMuon (IDPhieuMuon, IDNguoiMuon) VALUES (@ID, @ReaderID)";
                SqlParameter[] paramsPhieu = {
                    new SqlParameter("@ID", phieuMuonID),
                    new SqlParameter("@ReaderID", txtReaderID.Text)
                };
                DatabaseHelper.ExecuteNonQuery(queryPhieu, paramsPhieu);

                // 2. Insert into ChiTietMuon for each selected book
                foreach (DataGridViewRow row in dgvBooks.SelectedRows)
                {
                    string sachID = row.Cells["IDSach"].Value.ToString();
                    string ctID = "CT" + DateTime.Now.Ticks.ToString().Substring(10);
                    
                    string queryCT = "INSERT INTO ChiTietMuon (IDChiTietMuon, IDPhieuMuon, IDSach, NgayMuon, HanTra, TinhTrangTra) " +
                                     "VALUES (@CTID, @PMID, @SachID, @NgayMuon, @HanTra, N'Đang mượn')";
                    
                    SqlParameter[] paramsCT = {
                        new SqlParameter("@CTID", ctID),
                        new SqlParameter("@PMID", phieuMuonID),
                        new SqlParameter("@SachID", sachID),
                        new SqlParameter("@NgayMuon", dtpBorrowDate.Value),
                        new SqlParameter("@HanTra", dtpBorrowDate.Value.AddDays(14)),
                    };
                    DatabaseHelper.ExecuteNonQuery(queryCT, paramsCT);

                    // Update Book status
                    string queryUpdateBook = "UPDATE ThongTinSach SET TinhTrang = N'Đang mượn' WHERE IDSach = @SachID";
                    DatabaseHelper.ExecuteNonQuery(queryUpdateBook, new SqlParameter[] { new SqlParameter("@SachID", sachID) });
                }

                MessageBox.Show($"Lập phiếu mượn thành công!\nMã phiếu: {phieuMuonID}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lập phiếu mượn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
