using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.Unicode;
using System.Windows;
using System.Collections.ObjectModel;
namespace bmcsdl.Model
{
    public class dataBase
    {
        private static SqlConnection con = new SqlConnection(@"Data Source=TIENHOANGG;Initial Catalog=QLSVNhom;User ID=adminLAB3;Password=123456789;");
        private static dataBase single;
        private static string token { get; set; }
        private dataBase() { }
        public static dataBase get_dataBase()
        {
            if (single == null)
            {
                single = new dataBase();
            }
            return single;
        }
        private void open()
        {
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
        }
        private void close()
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }
        public bool login(string username, string password)
        {
            SqlCommand sqlcmd = new SqlCommand("select dbo.DANGNHAP(@username, @password)", con);
            sqlcmd.Parameters.AddWithValue("@username", username);
            sqlcmd.Parameters.AddWithValue("@password", password);
            open();
            SqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows && reader.Read() && reader[0].ToString() == "400")
            {
                close();
                return false;
            }
            else
            {
                token = reader[0].ToString();
                close();
                return true;
            }
        }
        public ObservableCollection<lopHoc> QuanLiLopHoc()
        {
            ObservableCollection<lopHoc> list = new ObservableCollection<lopHoc>();
            SqlCommand sqlcmd = new SqlCommand("exec dsLop @token", con);
            sqlcmd.Parameters.AddWithValue("@token", token);
            open();
            SqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    lopHoc lop = new lopHoc();
                    lop.malop = reader[0].ToString();
                    lop.tenlop = reader[1].ToString();
                    lop.manv = reader[2].ToString();
                    list.Add(lop);
                }
            }
            close();
            return list;
        }
        public ObservableCollection<sinhvien> detailLop(string malop)
        {
            ObservableCollection<sinhvien> list = new ObservableCollection<sinhvien>();
            SqlCommand sqlcmd = new SqlCommand("exec dsSinhVien @token, @MALOP", con);
            sqlcmd.Parameters.AddWithValue("@token", token);
            sqlcmd.Parameters.AddWithValue("@MALOP", malop);
            open();
            SqlDataReader reader = sqlcmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sinhvien sv = new sinhvien();
                    sv.masv = reader[0].ToString();
                    sv.hoten = reader[1].ToString();
                    sv.ngaysinh = DateTime.Parse(reader[2].ToString());
                    sv.diachi = reader[3].ToString();
                    list.Add(sv);
                }
            }
            close();
            return list;
        }
        public void edit(string masv, string hoten, DateTime ngaysinh, string diachi)
        {
            SqlCommand sqlcmd = new SqlCommand("exec editSinhVien @token, @MASV, @HOTEN, @NGAYSINH, @DIACHI", con);
            sqlcmd.Parameters.AddWithValue("@token", token);
            sqlcmd.Parameters.AddWithValue("@MASV", masv);
            sqlcmd.Parameters.AddWithValue("@HOTEN", hoten);
            sqlcmd.Parameters.AddWithValue("@NGAYSINH", ngaysinh);
            sqlcmd.Parameters.AddWithValue("@DIACHI", diachi);
            open();
            sqlcmd.ExecuteNonQuery();
            close();
        }
        public void addBangDiem(string masv, string hocphan, string diem)
        {
            SqlCommand sqlcmd = new SqlCommand("exec addBangDiem @token, @MASV, @HOCPHAN, @DIEM", con);
            sqlcmd.Parameters.AddWithValue("@token", token);
            sqlcmd.Parameters.AddWithValue("@MASV", masv);
            sqlcmd.Parameters.AddWithValue("@HOCPHAN", hocphan);
            sqlcmd.Parameters.AddWithValue("@DIEM", diem);
            open();
            sqlcmd.ExecuteNonQuery();
            close();
        }
    }
}
