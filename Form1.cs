using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLBanHang
{
    public partial class frmMatHang : Form
    {
        //Tạo local var
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter dap;
        DataSet ds;
        public frmMatHang()
        {
            InitializeComponent();
        }

        private void frmMatHang_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLBanHangDataSet.tblMatHang' table. You can move, or remove it, as needed.
            this.tblMatHangTableAdapter.Fill(this.qLBanHangDataSet.tblMatHang);
            // load tất cả dữ liệu từ tblMatHang
            conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\repos\\QLBanHang\\QLBanHang.mdf;Integrated Security=True");
            LoadDuLieu("select * from tblMatHang");
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            HienChiTiet(false);
        }
        private void LoadDuLieu(string sql)
        {
            ds = new DataSet();
            dap = new SqlDataAdapter(sql, conn);
            dap.Fill(ds);
            dgvKetQua.DataSource = ds.Tables[0];
        }
        private void HienChiTiet(bool hien)
        {
            txtMaSP.Enabled = hien;
            txtTenSP.Enabled = hien;
            dtpNgayHH.Enabled = hien;
            dtpNgaySX.Enabled = hien;
            txtDonVi.Enabled = hien;    
            txtDonGia.Enabled = hien;
            txtGhiChu.Enabled = hien;
            btnLuu.Enabled = hien;
            btnHuy.Enabled = hien;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            lblTieuDe.Text = "Tìm Kiếm Mặt Hàng";
            btnSua.Enabled=false;
            btnXoa.Enabled=false;
            string sql = "select * from tblMatHang";
            string dk = "";
            
            if (txtTKMaSP.Text.Trim() != "") {
                dk += " MaSP like '%"+ txtTKMaSP.Text +"%'";
            }

            if(txtTKTenSP.Text.Trim() != "" && dk != "")
            {
                dk += " AND TenSP like N'%"+txtTKTenSP.Text+"%'";
            }

            if (txtTKTenSP.Text.Trim() != "" && dk == "")
            {
                dk += " TenSP like N'%" + txtTKTenSP.Text + "%'";
            }

            if(dk != "")
            {
                sql += " WHERE" + dk;
            }

            LoadDuLieu(sql);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            lblTieuDe.Text = "Thêm mới sản phẩm";
            XoaTrangChiTiet();
            btnSua.Enabled = false; 
            btnXoa.Enabled=false;
            HienChiTiet(true);
        }

        private void XoaTrangChiTiet()
        {
            txtMaSP.Text = "";
            txtTenSP.Text = "";
            txtDonVi.Text = "";
            txtDonGia.Text = "";
            txtGhiChu.Text = "";
        }

        

        private void btnSua_Click(object sender, EventArgs e)
        {
            lblTieuDe.Text = "Cập Nhật Mặt Hàng";
            btnThem.Enabled = false;
            btnXoa.Enabled=false;
            HienChiTiet(true);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xoá mặt hàng " + txtMaSP.Text +
                " không? (Lưu/Không)", "Xoá Sản Phẩm", MessageBoxButtons.YesNo) == DialogResult.Yes) 
            {
                lblTieuDe.Text = "Xoá Mặt Hàng";
                btnThem.Enabled=false;
                btnSua.Enabled=false;

                HienChiTiet(true);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string sql = "";
            if (conn.State != ConnectionState.Open)
                conn.Open();

            if (txtTenSP.Text.Trim() == "")
            {
                errChiTiet.SetError(txtTenSP, "Bạn không để trống tên sản phẩm!");
                return;
            }
            else
            {
                errChiTiet.Clear();
            }

            if (dtpNgaySX.Value > DateTime.Now)
            {
                errChiTiet.SetError(dtpNgaySX, "Ngày Sản Xuất không hợp lệ");
                return;
            }
            else
            {
                errChiTiet.Clear();
            }

            if (dtpNgayHH.Value < dtpNgaySX.Value)
            {
                errChiTiet.SetError(dtpNgayHH, "Ngày hết hạn nhỏ hơn ngày sản xuất");
                return;
            }
            else
            {
                errChiTiet.Clear();
            }

            if (txtDonVi.Text.Trim() == "")
            {
                errChiTiet.SetError(txtDonVi, "Bạn không để trống đơn vị!");
                return;
            }
            else
            {
                errChiTiet.Clear();
            }

            if (txtDonGia.Text.Trim() == "")
            {
                errChiTiet.SetError(txtDonGia, "Bạn không để trống đơn giá!");
                return;
            }
            else
            {
                errChiTiet.Clear();
            }

            if (btnThem.Enabled == true)
            {
                if (txtMaSP.Text.Trim() == "")
                {
                    errChiTiet.SetError(txtMaSP, "Bạn không để trống mã sản phẩm!");
                    return;
                }
                else
                {
                    sql = "select Count(*) from tblMatHang where MaSP = @MaSP";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaSP", txtMaSP.Text);
                    int val = (int)cmd.ExecuteScalar();

                    if (val > 0)
                    {
                        errChiTiet.SetError(txtMaSP, "Mã sản phẩm đã tồn tại trong cơ sở dữ liệu.");
                        return;
                    }
                    errChiTiet.Clear();
                }

                sql = "INSERT INTO tblMatHang (MaSP, TenSP, NgaySX, NgayHH, DonVi, DonGia, GhiChu) VALUES (@MaSP, @TenSP, @NgaySX, @NgayHH, @DonVi, @DonGia, @GhiChu)";
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaSP", txtMaSP.Text);
                cmd.Parameters.AddWithValue("@TenSP", txtTenSP.Text);
                cmd.Parameters.AddWithValue("@NgaySX", dtpNgaySX.Value);
                cmd.Parameters.AddWithValue("@NgayHH", dtpNgayHH.Value);
                cmd.Parameters.AddWithValue("@DonVi", txtDonVi.Text);
                cmd.Parameters.AddWithValue("@DonGia", Convert.ToSingle(txtDonGia.Text));
                cmd.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text);
            }

            if (btnSua.Enabled == true)
            {
                sql = "UPDATE tblMatHang SET TenSP = @TenSP, NgaySX = @NgaySX, NgayHH = @NgayHH, DonVi = @DonVi, DonGia = @DonGia, GhiChu = @GhiChu WHERE MaSP = @MaSP";
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaSP", txtMaSP.Text);
                cmd.Parameters.AddWithValue("@TenSP", txtTenSP.Text);
                cmd.Parameters.AddWithValue("@NgaySX", dtpNgaySX.Value);
                cmd.Parameters.AddWithValue("@NgayHH", dtpNgayHH.Value);
                cmd.Parameters.AddWithValue("@DonVi", txtDonVi.Text);
                cmd.Parameters.AddWithValue("@DonGia", Convert.ToSingle(txtDonGia.Text));
                cmd.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text);
            }

            if (btnXoa.Enabled == true)
            {
                sql = "DELETE FROM tblMatHang WHERE MaSP = @MaSP";
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaSP", txtMaSP.Text);
            }

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            sql = "select * from tblMatHang";
            LoadDuLieu(sql);
            conn.Close();
            HienChiTiet(false);
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            btnSua.Enabled=false;
            btnXoa.Enabled=false;
            btnThem.Enabled=false;

            XoaTrangChiTiet();

            HienChiTiet(false);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        private void dgvKetQua_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            btnThem.Enabled = false;
            try
            {

                txtMaSP.Text = dgvKetQua[0, e.RowIndex].Value.ToString();
                txtTenSP.Text = dgvKetQua[1, e.RowIndex].Value.ToString();
                dtpNgaySX.Value = (DateTime)dgvKetQua[2, e.RowIndex].Value;
                dtpNgayHH.Value = (DateTime)dgvKetQua[3, e.RowIndex].Value;
                txtDonVi.Text = dgvKetQua[4, e.RowIndex].Value.ToString();
                txtDonGia.Text = dgvKetQua[5, e.RowIndex].Value.ToString();
                txtGhiChu.Text = dgvKetQua[6, e.RowIndex].Value.ToString();
            }
            catch (Exception ex)
            {

            }
        }

        private void dgvKetQua_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            btnThem.Enabled = false;
            try
            {

                txtMaSP.Text = dgvKetQua[0, e.RowIndex].Value.ToString();
                txtTenSP.Text = dgvKetQua[1, e.RowIndex].Value.ToString();
                dtpNgaySX.Value = (DateTime)dgvKetQua[2, e.RowIndex].Value;
                dtpNgayHH.Value = (DateTime)dgvKetQua[3, e.RowIndex].Value;
                txtDonVi.Text = dgvKetQua[4, e.RowIndex].Value.ToString();
                txtDonGia.Text = dgvKetQua[5, e.RowIndex].Value.ToString();
                txtGhiChu.Text = dgvKetQua[6, e.RowIndex].Value.ToString();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
