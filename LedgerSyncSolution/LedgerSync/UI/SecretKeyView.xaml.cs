using LedgerSyncViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
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

namespace LedgerSync.UI
{
    /// <summary>
    /// SecretKeyView.xaml 的交互逻辑
    /// </summary>
    public partial class SecretKeyView : Page
    {
        public SecretKeyView()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService<SecretKeyViewModel>();
        }
    }
}
