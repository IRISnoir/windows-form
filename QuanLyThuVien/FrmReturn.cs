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
    public partial class FrmReturn : Form
    {
        private const decimal FINE_PER_DAY = 1000;

        public FrmReturn()
        {
            InitializeComponent();
            
            this.btnSearchBorrowing.Click += BtnSearchBorrowing_Click;
            this.btnConfirmReturn.Click += BtnConfirmReturn_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.dtpReturnDate.ValueChanged += DtpReturnDate_ValueChanged;
            this.chkIsLost.CheckedChanged += (s, ev) => CalculateFine();
            
            txtPricePerMonth.Text = FINE_PER_DAY.ToString();
            txtPricePerMonth.ReadOnly = true;
            txtLateDays.ReadOnly = true;
            txtTotalFine.ReadOnly = true;
        }

        private void BtnSearchBorrowing_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReaderID.Text))
            {
                MessageBox.Show("Vui lòng nhập mã độc giả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "SELECT CT.IDChiTietMuon, CT.IDPhieuMuon, CT.IDSach, S.TenSach, CT.NgayMuon, CT.HanTra " +
                           "FROM ChiTietMuon CT " +
                           "JOIN PhieuMuon PM ON CT.IDPhieuMuon = PM.IDPhieuMuon " +
                           "JOIN ThongTinSach S ON CT.IDSach = S.IDSach " +
                           "WHERE PM.IDNguoiMuon = @ReaderID AND CT.NgayTra IS NULL";
            
            SqlParameter[] parameters = { new SqlParameter("@ReaderID", txtReaderID.Text) };
            
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvBorrowingList.DataSource = dt;
                
                if (dt.Rows.Count > 0)
                {
                    dtpBorrowDate.Value = Convert.ToDateTime(dt.Rows[0]["NgayMuon"]);
                    CalculateFine();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sách đang mượn của độc giả này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm phiếu mượn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DtpReturnDate_ValueChanged(object sender, EventArgs e)
        {
            CalculateFine();
        }

        private void CalculateFine()
        {
            DateTime dueDate = dtpBorrowDate.Value.AddDays(14);
            DateTime returnDate = dtpReturnDate.Value;
            decimal fine = 0;

            if (returnDate > dueDate)
            {
                int lateDays = (returnDate - dueDate).Days;
                txtLateDays.Text = lateDays.ToString();
                fine = lateDays * FINE_PER_DAY;
            }
            else
            {
                txtLateDays.Text = "0";
            }

            if (chkIsLost.Checked)
            {
                fine += 100000;
            }

            txtTotalFine.Text = fine.ToString();
        }

        private void BtnConfirmReturn_Click(object sender, EventArgs e)
        {
            if (dgvBorrowingList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần trả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                foreach (DataGridViewRow row in dgvBorrowingList.SelectedRows)
                {
                    string ctID = row.Cells["IDChiTietMuon"].Value.ToString();
                    string sachID = row.Cells["IDSach"].Value.ToString();
                    decimal fine = decimal.Parse(txtTotalFine.Text);

                    if (chkIsLost.Checked)
                    {
                        // 1. Record in GhiNhanMatSach
                        string matSachID = "MS" + DateTime.Now.Ticks.ToString().Substring(10);
                        string queryMat = "INSERT INTO GhiNhanMatSach (IDPhieuMatSach, IDNguoiMuon, IDSach, NgayGhiNhan, TienPhat) " +
                                          "VALUES (@MSID, @ReaderID, @SachID, @Ngay, @Fine)";
                        SqlParameter[] paramsMat = {
                            new SqlParameter("@MSID", matSachID),
                            new SqlParameter("@ReaderID", txtReaderID.Text),
                            new SqlParameter("@SachID", sachID),
                            new SqlParameter("@Ngay", dtpReturnDate.Value),
                            new SqlParameter("@Fine", fine)
                        };
                        DatabaseHelper.ExecuteNonQuery(queryMat, paramsMat);

                        // 2. Update Book status to 'Đã mất' or similar
                        string queryUpdateBook = "UPDATE ThongTinSach SET TinhTrang = N'Đã mất' WHERE IDSach = @SachID";
                        DatabaseHelper.ExecuteNonQuery(queryUpdateBook, new SqlParameter[] { new SqlParameter("@SachID", sachID) });
                    }
                    else
                    {
                        // Update ChiTietMuon with return info
                        string queryUpdateCT = "UPDATE ChiTietMuon SET NgayTra = @ReturnDate, TienPhat = @Fine, TinhTrangTra = N'Đã trả' " +
                                               "WHERE IDChiTietMuon = @CTID";
                        SqlParameter[] paramsUpdate = {
                            new SqlParameter("@ReturnDate", dtpReturnDate.Value),
                            new SqlParameter("@Fine", fine),
                            new SqlParameter("@CTID", ctID)
                        };
                        DatabaseHelper.ExecuteNonQuery(queryUpdateCT, paramsUpdate);

                        // Update Book status back to 'Sẵn sàng'
                        string queryUpdateBook = "UPDATE ThongTinSach SET TinhTrang = N'Sẵn sàng' WHERE IDSach = @SachID";
                        DatabaseHelper.ExecuteNonQuery(queryUpdateBook, new SqlParameter[] { new SqlParameter("@SachID", sachID) });
                    }
                }

                string message = chkIsLost.Checked ? "Đã ghi nhận mất sách và thu phí bồi thường." : "Đã nhận trả sách thành công!";
                MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý trả sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
