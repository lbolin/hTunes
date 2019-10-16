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
        const string API_KEY = "e1d7cac3d825c39c69e1e0f2a73ca7f8";

        private MediaPlayer mediaPlayer;
        private MusicLib musicLib;
        private DataTable table;

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
            table = new DataTable();
            table.Columns.Add(new DataColumn("id", typeof(int)));
            table.Columns.Add(new DataColumn("title", typeof(string)));
            table.Columns.Add(new DataColumn("artist", typeof(string)));
            table.Columns.Add(new DataColumn("album", typeof(string)));
            table.Columns.Add(new DataColumn("filename", typeof(string)));
            table.Columns.Add(new DataColumn("length", typeof(string)));
            table.Columns.Add(new DataColumn("genre", typeof(string)));
            table.Columns.Add(new DataColumn("url", typeof(string)));
            table.Columns.Add(new DataColumn("albumImage", typeof(string)));

            foreach (DataRow row in musicLib.Songs.Rows)
            {
                DataRow newRow = table.NewRow();
                newRow["id"] = row["id"];
                newRow["title"] = row["title"];
                newRow["artist"] = row["artist"];
                newRow["album"] = row["album"];
                newRow["filename"] = row["filename"];
                newRow["length"] = row["length"];
                newRow["genre"] = row["genre"];
                newRow["url"] = row["url"];
                newRow["albumImage"] = row["albumImage"];
                table.Rows.Add(newRow);
            }

            dataGrid.ItemsSource = table.DefaultView;


            //displayedSongs = new List<Song>();
            //dataGrid.ItemsSource = displayedSongs;
            //foreach (var songId in musicLib.SongIds)
            //{
            //    var song = musicLib.GetSong(int.Parse(songId));
            //    displayedSongs.Add(song);
            //}
        }

        private void playlist_Selected(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox == null) return;

            var playlist = listbox.SelectedItem.ToString();

            if (playlist == "All Music")
            {
                table.Rows.Clear();
                foreach (DataRow row in musicLib.Songs.Rows)
                {
                    DataRow newRow = table.NewRow();
                    newRow["id"] = row["id"];
                    newRow["title"] = row["title"];
                    newRow["artist"] = row["artist"];
                    newRow["album"] = row["album"];
                    newRow["filename"] = row["filename"];
                    newRow["length"] = row["length"];
                    newRow["genre"] = row["genre"];
                    newRow["url"] = row["url"];
                    newRow["albumImage"] = row["albumImage"];
                    table.Rows.Add(newRow);
                }
            }
            else
            {
                if (!musicLib.PlaylistExists(playlist)) return;

                table.Rows.Clear();

                foreach (DataRow row in musicLib.SongsForPlaylist(playlist).Rows)
                {
                    DataRow newRow = table.NewRow();
                    newRow["id"] = row["id"];
                    newRow["title"] = row["title"];
                    newRow["artist"] = row["artist"];
                    newRow["album"] = row["album"];
                    newRow["filename"] = "";
                    newRow["length"] = "";
                    newRow["genre"] = row["genre"];
                    newRow["url"] = "";
                    newRow["albumImage"] = "";
                    table.Rows.Add(newRow);
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
            if (result == true)
            {
                musicLib.AddSong(openFileDlg.FileName);

                table.Rows.Clear();
                foreach (DataRow row in musicLib.Songs.Rows)
                {
                    DataRow newRow = table.NewRow();
                    newRow["id"] = row["id"];
                    newRow["title"] = row["title"];
                    newRow["artist"] = row["artist"];
                    newRow["album"] = row["album"];
                    newRow["filename"] = row["filename"];
                    newRow["length"] = row["length"];
                    newRow["genre"] = row["genre"];
                    newRow["url"] = row["url"];
                    newRow["albumImage"] = row["albumImage"];
                    table.Rows.Add(newRow);
                }

                dataGrid.Items.Refresh();
            }            
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
            DataRow song = dataGrid.SelectedItem as DataRow;
            if (song == null) return;
                        
            mediaPlayer.Open(new Uri(song["filename"].ToString()));
            mediaPlayer.Play();
        }

        private void stopbtn_click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void removebtn_click(object sender, RoutedEventArgs e)
        {

        }


        async private Task<string> MakeApiCall(String Artist, String Title)
        {
            String url = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key=" + API_KEY + "&" +
                "artist=" + WebUtility.UrlEncode(Artist) + "&track=" + WebUtility.UrlEncode(Title);
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (WebResponse response = await request.GetResponseAsync())
                {
                    Stream strm = response.GetResponseStream();
                    using (XmlTextReader reader = new XmlTextReader(strm))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (reader.Name == "image")
                                {
                                    if (reader.GetAttribute("size") == "medium")
                                        return reader.ReadString();
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException e)
            {
                // A 400 response is returned when the song is not in their library
                Console.WriteLine("Error: " + e.Message);
            }
            return null;
        }

        async private void songSelection_changed(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView rowView = ((DataGrid)sender).SelectedCells[0].Item as DataRowView;
            if (rowView == null) return;


            string fileName = rowView.Row["albumimage"].ToString();
            if (fileName == "")
            {
                await Task.Run(() =>
                {
                    string result = MakeApiCall(rowView.Row["artist"].ToString(), rowView.Row["album"].ToString()).Result;
                    rowView.Row["filename"] = result;
                    Dispatcher.Invoke((Action)delegate
                    {
                        dataGrid.Items.Refresh();
                    });
                });                
            }
        }
    }
}
