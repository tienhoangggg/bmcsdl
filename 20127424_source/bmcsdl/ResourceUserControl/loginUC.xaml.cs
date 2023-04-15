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
    /// Interaction logic for loginUC.xaml
    /// </summary>
    public partial class loginUC : UserControl
    {
        public loginVM login { get; set; }
        public loginUC()
        {
            InitializeComponent();
            this.DataContext = login = new loginVM();
        }
    }
}
