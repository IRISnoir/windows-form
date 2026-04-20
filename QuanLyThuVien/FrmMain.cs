using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuanLyThuVien.FrmLogin;

namespace QuanLyThuVien
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        public void UpdateUserUI()
        {
            DangNhap.Text = CurrentUser.HoTen;
        }

        private void DangNhapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.MdiParent = this;
            frmLogin.Show();
        }

        private void HSNhanVien_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Ban Giám Đốc") //Oke
            {
                FrmLogin frmLogin = new FrmLogin();
                frmLogin.MdiParent = this;
                frmLogin.Show();
                return;
            }
        }

        private void TheDocGia_Click(object sender, EventArgs e)
        {

            if (CurrentUser.BoPhan == "Thủ Thư") //Oke
            {
                FrmReaderCard frmReaderCard = new FrmReaderCard();
                frmReaderCard.MdiParent = this;
                frmReaderCard.Show();
                return;
            }
        }

        private void PhieuMuonSach_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Thủ Thư") //Oke
            {
                FrmBorrow frmBorrow = new FrmBorrow();
                frmBorrow.MdiParent = this;
                frmBorrow.Show();
                return;
            }
        }

        private void PhieuTraSach_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Thủ Thư") //Oke
            {
                FrmReturn frmReturn = new FrmReturn();
                frmReturn.MdiParent = this;
                frmReturn.Show();
                return;
            }
        }

        private void TiepNhanSachMoi_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Thủ Kho") //Oke
            {
                FrmBookEntry frmBookEntry = new FrmBookEntry();
                frmBookEntry.MdiParent = this;
                frmBookEntry.Show();
                return;
            }
        }

        private void TraCuuSach_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Thủ Kho" || CurrentUser.BoPhan == "Ban Giám Đốc" || CurrentUser.BoPhan == "Thủ Thư" || CurrentUser.BoPhan == "Thủ Quỹ") //Oke
            {
                FrmSearchBook frmSearchBook = new FrmSearchBook();
                frmSearchBook.MdiParent = this;
                frmSearchBook.Show();
                return;
            }
        }

        private void ThuTienPhat_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Thủ Quỹ")//Oke
            {
                FrmFineCollection frmFineCollection = new FrmFineCollection();
                frmFineCollection.MdiParent = this;
                frmFineCollection.Show();
                return;
            }
        }

        private void ThanhLy_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Thủ Kho") //Oke
            {
                FrmLiquidation frmLiquidation = new FrmLiquidation();
                frmLiquidation.MdiParent = this;
                frmLiquidation.Show();
                return;
            }

        }

        private void BaoCaoThongKe_Click(object sender, EventArgs e)//Oke   
        {
            if (CurrentUser.BoPhan == "Ban Giám Đốc")
            {
                FrmReports frmReports = new FrmReports();
                frmReports.MdiParent = this;
                frmReports.Show();
                return;
            }

        }

        private void CaiDat_Click(object sender, EventArgs e)
        {
            if (CurrentUser.BoPhan == "Ban Giám Đốc") //Oke
            {   
                FrmSettings frmSettings = new FrmSettings();
                frmSettings.MdiParent = this;
                frmSettings.Show(); 
            }
        }
        

        private void TroGiupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Quản Lý Thư Viện:\n\n\n" +
                "1. Nguyễn Đăng Khoa - 2311554033\n\n" +
                "2. Nguyễn Lê Khánh Hoàng - 2311552947\n\n" +
                "3. Đỗ Văn Hiệp - 2311553289\n\n" +
                "4. Nguyễn Lê Văn Dũng - 2311555475\n\n" +
                "5. Nguyễn Hữu Giàu - 2311553450", "Thông tin nhóm 3", MessageBoxButtons.OK);
        }
    }
}
