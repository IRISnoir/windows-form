using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyThuVien.Enums;
using QuanLyThuVien.Models;
using QuanLyThuVien.Helpers;
using System.Data.SqlClient;

namespace QuanLyThuVien
{
    public partial class FrmBookEntry : Form
    {
        public event EventHandler BookSaved;
        private string _editingId = null;
        public FrmBookEntry()
        {
            InitializeComponent();
            Load += FrmBookEntry_Load;
            btnLuu.Click += btnLuu_Click;
            btnDong.Click += btnDong_Click;
        }

        private void FrmBookEntry_Load(object sender, EventArgs e)
        {
            InitSampleData();
            LoadCategories();
        }

        private void InitSampleData()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) as cnt FROM TheLoai", null);
                var count = Convert.ToInt32(dt.Rows[0]["cnt"]);
                if (count == 0)
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM DauSach", null);
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM TheLoai", null);
                    
                    string[] ids = { "TL001", "TL002", "TL003", "TL004", "TL005", "TL006", "TL007", "TL008", "TL009", "TL010", "TL011", "TL012", "TL013" };
                    string[] names = { "Toán", "Lý", "Hóa", "Sử", "Địa", "Tiếng Anh", "Ngữ Văn", "Sinh", "Tiếng Aslat", "Truyện", "Truyện Ngôn Tình", "Truyện Phiêu Lưu", "Anime" };
                    
                    for (int i = 0; i < ids.Length; i++)
                    {
                        string tlId = ids[i];
                        string dsId = "DS" + (i + 1).ToString("D3");
                        string name = names[i];
                        DatabaseHelper.ExecuteNonQuery("INSERT INTO TheLoai (IDTheLoai, TenTheLoai) VALUES (@id, @name)", new[] { new SqlParameter("@id", tlId), new SqlParameter("@name", name) });
                        DatabaseHelper.ExecuteNonQuery("INSERT INTO DauSach (IDDauSach, IDTheLoai, TenDauSach) VALUES (@id, @tl, @name)", new[] { new SqlParameter("@id", dsId), new SqlParameter("@tl", tlId), new SqlParameter("@name", name) });
                    }
                    MessageBox.Show("Đã tạo " + ids.Length + " thể loại sách!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void LoadCategories()
        {
            try
            {
                cboTheLoai.Items.Clear();
                var dt = DatabaseHelper.ExecuteQuery("SELECT IDDauSach, TenDauSach FROM DauSach", null);
                foreach (DataRow r in dt.Rows)
                {
                    cboTheLoai.Items.Add(new CategoryItem(r["IDDauSach"]?.ToString(), r["TenDauSach"]?.ToString()));
                }
                if (cboTheLoai.Items.Count > 0) cboTheLoai.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private class CategoryItem
        {
            public string Id { get; }
            public string Name { get; }
            public CategoryItem(string id, string name) { Id = id; Name = name; }
            public override string ToString() => Name ?? "";
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            txtTenSach.Text = string.Empty;
            txtTacGia.Text = string.Empty;
            txtNhaXuatBan.Text = string.Empty;
            txtNamXuatBan.Text = string.Empty;
            txtGiaBan.Text = string.Empty;
            txtGiaThue.Text = string.Empty;
            dtpNgayNhap.Value = DateTime.Now;
            if (cboTheLoai.Items.Count > 0) cboTheLoai.SelectedIndex = 0;
            _editingId = null;
            btnLuu.Text = "Lưu";
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSach.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sách.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSach.Focus();
                return;
            }

            if (!int.TryParse(txtNamXuatBan.Text.Trim(), out int namXB) || namXB < 1000 || namXB > DateTime.Now.Year)
            {
                MessageBox.Show("Năm xuất bản không hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamXuatBan.Focus();
                return;
            }

            if (!int.TryParse(txtGiaBan.Text.Trim(), out int triGia) || triGia < 0)
            {
                MessageBox.Show("Giá bán (trị giá) không hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaBan.Focus();
                return;
            }

            try
            {
                var book = new Book
                {
                    TenSach = txtTenSach.Text.Trim(),
                    TacGia = txtTacGia.Text.Trim(),
                    NhaXuatBan = txtNhaXuatBan.Text.Trim(),
                    NgayNhap = dtpNgayNhap.Value,
                    NamXuatBan = namXB,
                    TriGia = triGia,
                    IDDauSach = (cboTheLoai.SelectedItem is CategoryItem cat) ? cat.Id : null
                };

                if (!string.IsNullOrEmpty(_editingId))
                {
                    string updateSql = "UPDATE ThongTinSach SET TenSach=@TenSach, TacGia=@TacGia, NhaXuatBan=@NhaXuatBan, NamXuatBan=@NamXuatBan, NgayNhap=@NgayNhap, TriGia=@TriGia, IDDauSach=@IDDauSach WHERE IDSach=@id";
                    var updateParams = new SqlParameter[]
                    {
                        new SqlParameter("@TenSach", book.TenSach),
                        new SqlParameter("@TacGia", book.TacGia),
                        new SqlParameter("@NhaXuatBan", book.NhaXuatBan),
                        new SqlParameter("@NamXuatBan", book.NamXuatBan),
                        new SqlParameter("@NgayNhap", book.NgayNhap.Date),
                        new SqlParameter("@TriGia", book.TriGia),
                        new SqlParameter("@IDDauSach", (object)book.IDDauSach ?? DBNull.Value),
                        new SqlParameter("@id", _editingId)
                    };

                    DatabaseHelper.ExecuteNonQuery(updateSql, updateParams);
                    MessageBox.Show("Cập nhật sách thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BookSaved?.Invoke(this, EventArgs.Empty);
                    this.Close();
                }
                else
                {
                    var newId = GenerateNewBookId();
                    string sql = "INSERT INTO ThongTinSach (IDSach, TenSach, TacGia, NhaXuatBan, NamXuatBan, NgayNhap, TriGia, IDDauSach, TinhTrang) " +
                                 "VALUES (@IDSach, @TenSach, @TacGia, @NhaXuatBan, @NamXuatBan, @NgayNhap, @TriGia, @IDDauSach, N'OK');";
                    
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@IDSach", newId),
                        new SqlParameter("@TenSach", book.TenSach),
                        new SqlParameter("@TacGia", book.TacGia),
                        new SqlParameter("@NhaXuatBan", book.NhaXuatBan),
                        new SqlParameter("@NamXuatBan", (object)book.NamXuatBan),
                        new SqlParameter("@NgayNhap", (object)book.NgayNhap.Date),
                        new SqlParameter("@TriGia", (object)book.TriGia),
                        new SqlParameter("@IDDauSach", (object)book.IDDauSach ?? DBNull.Value)
                    };

                    DatabaseHelper.ExecuteNonQuery(sql, parameters);
MessageBox.Show($"Thêm sách thành công. Mã sách: {newId}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    BookSaved?.Invoke(this, EventArgs.Empty);

                    var result = MessageBox.Show("Bạn có muốn tiếp tục nhập thêm sách mới?", "Tiếp tục?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        btnDong_Click(null, null);
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateNewBookId()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT TOP 1 IDSach FROM ThongTinSach ORDER BY IDSach DESC", null);
                if (dt.Rows.Count > 0)
                {
                    var lastId = dt.Rows[0]["IDSach"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(lastId) && lastId.StartsWith("S"))
                    {
                        if (int.TryParse(lastId.Substring(1), out int num))
                        {
                            return "S" + (num + 1).ToString("D3");
                        }
                    }
                }
            }
catch { }
            return "S001";
        }

        public void OpenForEdit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            try
            {
                var p = new SqlParameter[] { new SqlParameter("@id", id) };
                var dt = DatabaseHelper.ExecuteQuery("SELECT TOP 1 * FROM ThongTinSach WHERE IDSach = @id", p);
                if (dt.Rows.Count == 0) return;
                var r = dt.Rows[0];
                _editingId = r["IDSach"]?.ToString();
                txtTenSach.Text = r["TenSach"]?.ToString();
                txtTacGia.Text = r["TacGia"]?.ToString();
                txtNhaXuatBan.Text = r["NhaXuatBan"]?.ToString();
                txtNamXuatBan.Text = r["NamXuatBan"]?.ToString();
                if (DateTime.TryParse(r["NgayNhap"]?.ToString(), out var dn)) dtpNgayNhap.Value = dn;
                txtGiaBan.Text = r["TriGia"]?.ToString();
                txtGiaThue.Text = "";
                var cat = r.Table.Columns.Contains("IDDauSach") ? r["IDDauSach"]?.ToString() : null;
                if (!string.IsNullOrWhiteSpace(cat))
                {
                    for (int i = 0; i < cboTheLoai.Items.Count; i++)
                    {
                        if (cboTheLoai.Items[i] is CategoryItem item && item.Id == cat)
                        {
                            cboTheLoai.SelectedIndex = i;
                            break;
                        }
                    }
                }
                btnLuu.Text = "Cập nhật";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể nạp thông tin sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
