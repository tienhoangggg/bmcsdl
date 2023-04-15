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
        public static UserControl cur { get; set; }
        private dataBase data;
        public QuanLiLopVM()
        {
            data = dataBase.get_dataBase();
            listLop = new ObservableCollection<lopHoc>();
            detail = new RelayCommand<object>((p) => { return p == null ? false : true; }, (p) => { cmdDetail(p as string); });
        }
        private void cmdDetail(string p)
        {
            Window w = GetWindowParent(cur) as Window;
            Grid quanli = w.FindName("quanlilophoc") as Grid;
            Grid detail = w.FindName("detailLop") as Grid;
            quanli.Visibility = Visibility.Collapsed;
            detail.Visibility = Visibility.Visible;
            ObservableCollection<sinhvien> temp = data.detailLop(p);
            detailLopVM.list.Clear();
            for (int i = 0;  i < temp.Count; i++)
            {
                detailLopVM.list.Add(temp[i]);
            }
            detailLopVM.MaLop = p;
        }
    }
}
