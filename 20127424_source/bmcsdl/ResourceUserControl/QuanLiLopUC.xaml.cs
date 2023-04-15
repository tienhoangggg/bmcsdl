﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using bmcsdl.ViewModel;
namespace bmcsdl.ResourceUserControl
{
    /// <summary>
    /// Interaction logic for QuanLiLopUC.xaml
    /// </summary>
    public partial class QuanLiLopUC : UserControl
    {
        public QuanLiLopUC()
        {
            InitializeComponent();
            this.DataContext = new QuanLiLopVM();
        }
    }
    public class CommandParameterForBt
    {
        public string maLop { get; set; }
        public UserControl p { get; set; }
    }
}
