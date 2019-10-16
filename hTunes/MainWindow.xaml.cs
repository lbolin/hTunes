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
using System.Data;

namespace hTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer;
        private MusicLib musicLib;
        private List<Song> displayedSongs;

        public MainWindow()
        {
            InitializeComponent();

            musicLib = new MusicLib();
            mediaPlayer = new MediaPlayer();

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

        private void playlist_Selected(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox == null) return;

            var playlist = listbox.SelectedItem.ToString();

            if (playlist == "All Music")
            {
                displayedSongs.Clear();
                foreach (var songId in musicLib.SongIds)
                {
                    var song = musicLib.GetSong(int.Parse(songId));
                    displayedSongs.Add(song);
                }
            }
            else
            {
                if (!musicLib.PlaylistExists(playlist)) return;

                displayedSongs.Clear();

                foreach (DataRow row in musicLib.SongsForPlaylist(playlist).Rows)
                {
                    Song song = new Song();
                    song.Id = int.Parse(row["id"].ToString());
                    song.Title = row["title"].ToString();
                    song.Artist = row["artist"].ToString();
                    song.Album = row["album"].ToString();
                    song.Genre = row["genre"].ToString();

                    displayedSongs.Add(song);
                }
            }
            dataGrid.Items.Refresh();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Media Files | *.mp3;*.m4a;*.wma;*.wav ";

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            //if (result == true)
            //{
            //    FileNameTextBox.Text = openFileDlg.FileName;
            //    TextBlock1.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            //}
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void infoBtn_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void playbtn_click(object sender, RoutedEventArgs e)
        {
            var song = dataGrid.SelectedItem as Song;
            if (song == null) return;
                        
            mediaPlayer.Open(new Uri(song.Filename));
            mediaPlayer.Play();
        }

        private void stopbtn_click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void removebtn_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
