using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.Unicode;
using System.Windows;
namespace bmcsdl.Model
{
    public class dataBase
    {
        private static SqlConnection con = new SqlConnection(@"Data Source=TIENHOANGG;Initial Catalog=QLSVNhom;User ID=adminLAB3;Password=123456789;");
        private static dataBase single;
        private static string _token;
        private string token { get { return _token; } set { _token = value; listLop = QuanLiLopHoc(); } }
        public List<lopHoc> listLop { get; set; }
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
        private List<lopHoc> QuanLiLopHoc()
        {
            List<lopHoc> list = new List<lopHoc>();
            SqlCommand sqlcmd = new SqlCommand("select dbo.dsLop(@token)", con);
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
    }
}
