using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using bmcsdl.Model;
namespace bmcsdl.ViewModel
{
    public class QuanLiLopVM: BaseViewModel
    {
        public ICommand detail { get; set; }
        public List<lopHoc> listLop { get { return data.listLop; } set { } }
        private dataBase data;
        public QuanLiLopVM()
        {
            data = dataBase.get_dataBase();
            detail = new RelayCommand<object>((p) => { return p == null ? false : true; }, (p) => { cmdDetail(p as UserControl); });
        }
        private void cmdDetail(UserControl p)
        {

        }
    }
}
