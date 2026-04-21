using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyThuVien.Helpers;
using System.Data.SqlClient;

namespace QuanLyThuVien
{
    public partial class FrmSearchBook : Form
    {
        public FrmSearchBook()
        {
            InitializeComponent();
            Load += FrmSearchBook_Load;
            btnTimKiem.Click += Button1_Click;
            btnTatCa.Click += Button2_Click;
            btnSanSang.Click += Button3_Click;
            btnDangMuon.Click += Button4_Click;
        }

        private void FrmSearchBook_Load(object sender, EventArgs e)
        {
            LoadCategories();
            SetupGridColumns();
            LoadData();
        }

        private void LoadCategories()
        {
            try
            {
                cboTheLoaiFilter.Items.Clear();
                cboTheLoaiFilter.Items.Add(new CategoryItem("ALL", "Tất cả"));
                var dt = DatabaseHelper.ExecuteQuery("SELECT IDDauSach, TenDauSach FROM DauSach", null);
                foreach (DataRow r in dt.Rows)
                {
                    cboTheLoaiFilter.Items.Add(new CategoryItem(r["IDDauSach"]?.ToString(), r["TenDauSach"]?.ToString()));
                }
                if (cboTheLoaiFilter.Items.Count > 0) cboTheLoaiFilter.SelectedIndex = 0;
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

        private void SetupGridColumns()
        {
            dgvBooks.Columns.Clear();
            dgvBooks.AutoGenerateColumns = false;
            dgvBooks.Columns.Add("IDSach", "Mã sách");
            dgvBooks.Columns.Add("TenSach", "Tên sách");
            dgvBooks.Columns.Add("TacGia", "Tác giả");
            dgvBooks.Columns.Add("TheLoai", "Thể loại");
            dgvBooks.Columns.Add("TinhTrang", "Tình trạng");
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.MultiSelect = false;
            dgvBooks.DoubleClick += DataGridView1_DoubleClick;
        }

        private void LoadData(string whereClause = null, SqlParameter[] parameters = null)
        {
            try
            {
                string sql = "SELECT s.IDSach, s.TenSach, s.TacGia, d.TenDauSach AS TheLoai, s.TinhTrang FROM ThongTinSach s LEFT JOIN DauSach d ON s.IDDauSach = d.IDDauSach";
                
                string finalWhere = "";
                
                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    finalWhere = whereClause;
                }

                if (cboTheLoaiFilter.SelectedItem is CategoryItem cat && cat.Id != "ALL")
                {
                    string categoryFilter = "s.IDDauSach = @categoryId";
                    if (!string.IsNullOrWhiteSpace(finalWhere))
                    {
                        finalWhere += " AND " + categoryFilter;
                    }
                    else
                    {
                        finalWhere = categoryFilter;
                    }
                }

                if (!string.IsNullOrWhiteSpace(finalWhere))
                {
                    sql += " WHERE " + finalWhere;
                }

                List<SqlParameter> paramList = new List<SqlParameter>();
                if (parameters != null) paramList.AddRange(parameters);
                if (cboTheLoaiFilter.SelectedItem is CategoryItem cat2 && cat2.Id != "ALL")
                {
                    paramList.Add(new SqlParameter("@categoryId", cat2.Id));
                }

                var dt = DatabaseHelper.ExecuteQuery(sql, paramList.ToArray());
                dgvBooks.Rows.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    var statusObj = r["TinhTrang"];
                    string statusText = statusObj?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(statusText))
                    {
                        statusText = GetStatusText(statusText);
                    }
                    dgvBooks.Rows.Add(r["IDSach"], r["TenSach"], r["TacGia"], r["TheLoai"], statusText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể truy vấn dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string GetStatusText(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return status;
            var normalized = status.Trim().ToLower();
            switch (normalized)
            {
                case "ok":
                case "sẵn sàng":
                    return "Sẵn sàng";
                case "đang mượn":
                case "muon":
                    return "Đang mượn";
                case "hỏng":
                case "hong":
                    return "Hỏng";
                case "mất":
                case "mat":
                    return "Mất";
                default:
                    return status;
            }
        }

        private void DataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0) return;
            var id = dgvBooks.SelectedRows[0].Cells[0].Value?.ToString();
            if (string.IsNullOrWhiteSpace(id)) return;
            var frm = new FrmLiquidation();
            frm.SelectAndHighlight(id);
            frm.ShowDialog(this);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMaSach.Text))
            {
                var p = new SqlParameter[] { new SqlParameter("@id", txtMaSach.Text.Trim()) };
                LoadData("s.IDSach = @id", p);
            }
            else
            {
                LoadData();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            LoadData("s.TinhTrang = N'OK'");
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            LoadData("s.TinhTrang = N'Đang mượn'");
        }
    }
}
