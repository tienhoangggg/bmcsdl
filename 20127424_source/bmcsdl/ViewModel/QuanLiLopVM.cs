using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bmcsdl.Model;
namespace bmcsdl.ViewModel
{
    public class QuanLiLopVM: BaseViewModel
    {
        public ICommand detail { get; set; }
        public static ObservableCollection<lopHoc> listLop { get; set; }
        private dataBase data;
        public QuanLiLopVM()
        {
            data = dataBase.get_dataBase();
            detail = new RelayCommand<object>((p) => { return p == null ? false : true; }, (p) => { cmdDetail(p as btPra); });
        }
        private void cmdDetail(btPra p)
        {
            Window w = GetWindowParent(p.p) as Window;
            Grid qllh = w.FindName("quanlilophoc") as Grid;
            Grid detailLop = w.FindName("detailLop") as Grid;
            qllh.Visibility = Visibility.Collapsed;
            detailLop.Visibility = Visibility.Visible;
            ObservableCollection<sinhvien> temp = data.detailLop(p.maLop);
            detailLopVM.list.Clear();
            for (int i = 0;  i < temp.Count; i++)
            {
                detailLopVM.list.Add(temp[i]);
            }
        }
        public class btPra
        {
            public string maLop { get; set; }
            public UserControl p { get; set; }
        }
    }
}
