using bmcsdl.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmcsdl.ViewModel
{
    public class detailLopVM: BaseViewModel
    {
        public static ObservableCollection<sinhvien> list { get; set; }
        public detailLopVM()
        { }
    }
}
