using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
using System.Net;
using System.Xml;
using System.IO;

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

        private Point startPoint;

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

            //disable sorting
            foreach (DataGridColumn column in dataGrid.Columns)
            {
                column.CanUserSort = false;
            }
        }

        private void playlist_Selected(object sender, RoutedEventArgs e)
        {
            RefreshSongs();
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
            if (result == true)
            {
                Song s = musicLib.AddSong(openFileDlg.FileName);

                displayedSongs.Clear();
                foreach (DataRow row in musicLib.Songs.Rows)
                {
                    Song song = musicLib.GetSong(int.Parse(row["id"].ToString()));
                    displayedSongs.Add(song);

                    if (song.Id == s.Id)
                    {
                        dataGrid.SelectedItem = song;
                    }
                }

                dataGrid.Items.Refresh();
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            NewPlaylist np = new NewPlaylist();
            np.ShowDialog();
            string name = np.NewPlayListName;
            if (name != "")
            {
                musicLib.AddPlaylist(name);

                RefreshPlaylists();
            }
        }

        private void infoBtn_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void playbtn_click(object sender, RoutedEventArgs e)
        {
            DataRowView song = dataGrid.SelectedItem as DataRowView;
            if (song == null) return;

            Song s = musicLib.GetSong(int.Parse(song["id"].ToString()));
            mediaPlayer.Open(new Uri(s.Filename));
            mediaPlayer.Play();
        }

        private void stopbtn_click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void removebtn_click(object sender, RoutedEventArgs e)
        {
            string msgtext = "Are you sure you want to remove this song?";
            string txt = "Confirmation";
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBoxButton button = MessageBoxButton.YesNo;

            if (MessageBox.Show(msgtext, txt, button, icon) == MessageBoxResult.Yes)
            { 
                var playlist = playlistListBox.SelectedItem?.ToString();
                Song s = dataGrid.SelectedItem as Song;
                if (playlist == "All Music" || playlist == null)
                {
                    musicLib.DeleteSong(s.Id);
                }
                else
                {
                    //musicLib.RemoveSongFromPlaylist()
                    //remove from playlist
                }

                RefreshSongs();
            }
        }

        private void playlistListBox_DragOver(object sender, DragEventArgs e)
        {
            Label playlist = sender as Label;
            if (playlist != null)
            {
                Song s = (Song)e.Data.GetData(e.Data.GetFormats()[0]);
                musicLib.AddSongToPlaylist(s.Id, playlist.Content.ToString());

                playlist_Selected(playlistListBox, null);
            }
        }

        private void dataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void dataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                DragDrop.DoDragDrop(dataGrid, dataGrid.SelectedItem, DragDropEffects.Copy);
            }
        }

        private void RefreshSongs()
        {
            var playlist = playlistListBox.SelectedItem?.ToString();

            if (playlist == "All Music" || playlist == null)
            {
                displayedSongs.Clear();
                foreach (DataRow row in musicLib.Songs.Rows)
                {
                    Song s = musicLib.GetSong(int.Parse(row["id"].ToString()));
                    displayedSongs.Add(s);
                }
            }
            else
            {
                if (!musicLib.PlaylistExists(playlist)) return;

                displayedSongs.Clear();

                foreach (DataRow row in musicLib.SongsForPlaylist(playlist).Rows)
                {
                    Song s = musicLib.GetSong(int.Parse(row["id"].ToString()));
                    displayedSongs.Add(s);
                }
            }
            dataGrid.Items.Refresh();
        }

        private void RefreshPlaylists()
        {
            playlistListBox.Items.Clear();
            playlistListBox.Items.Add("All Music");
            foreach (var playlist in musicLib.Playlists)
            {
                playlistListBox.Items.Add(playlist);
            }
        }
        //private void ShowMessageBox_Click(object sender, RoutedEventArgs e)
        //{
        //    string msgtext = "Are you sure you want to remove this song?";
        //    string txt = "Confirmation";
        //    MessageBoxImage icon = MessageBoxImage.Question;

        //    MessageBoxButton button = MessageBoxButton.YesNo;
        //    MessageBoxResult result = MessageBox.Show(msgtext, txt, button, icon);





        //}
    }
}
