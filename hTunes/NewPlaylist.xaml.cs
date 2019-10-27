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
using System.Windows.Shapes;

namespace hTunes
{
    /// <summary>
    /// Interaction logic for NewPlaylist.xaml
    /// </summary>
    public partial class NewPlaylist : Window
    {
        public string NewPlayListName { get; set; }

        public NewPlaylist()
        {
            InitializeComponent();
        }

        private void OkBtn_click(object sender, RoutedEventArgs e)
        {
            NewPlayListName = txtPlayListName.Text;
            this.Close();
        }

        private void CancelBtn_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
