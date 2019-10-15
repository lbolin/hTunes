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

namespace hTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MusicLib musicLib;
        private List<Song> displayedSongs;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                musicLib = new MusicLib();
            }
            catch (Exception e)
            {
                //TODO - show error message
                throw new Exception("Failed here");
            }

            //load playlist list
            playlistListBox.Items.Add("All Music");
            foreach (var playlist in musicLib.Playlists)
            {
                playlistListBox.Items.Add(playlist);
            }

            //load songs
            displayedSongs = new List<Song>();
            dataGrid.ItemsSource = displayedSongs;
            foreach (var songId in musicLib.SongIds)
            {
                var song = musicLib.GetSong(int.Parse(songId));
                displayedSongs.Add(song);
            }
        }
    }
}
