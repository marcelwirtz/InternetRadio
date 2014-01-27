using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Windows;


namespace InternetRadio
{
    public class Player
    {

        private MediaElement _player;
        public PlayerState State { get; set; }
        private Dictionary<string, string> _playlist = new Dictionary<string, string>();
        private static Player _instance;
        public event EventHandler StateChanged;
        public event EventHandler<SongEventArgs> SongChanged;
 
        public Dictionary<string, string> Playlist
        {
            get
            {
                return _playlist;
            }
        }
 
        public double Volume
        {
            get
            {
                return _player.Volume;
            }
            set
            {
                _player.Volume = value;
                RaiseStateChanged();
            }
        }
 
        public static Player Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Player();
                }
                return _instance;
            }
        }
 
        public int SelectedIndex { get; set; }
 
        private Player()
        {
            _player = new MediaElement();
            _player.LoadedBehavior = MediaState.Manual;
            _player.UnloadedBehavior = MediaState.Manual;
            _player.ScriptCommand += new EventHandler<MediaScriptCommandRoutedEventArgs>(RaiseSongChanged);
        }
 
        public void SetSource(int index, string uri)
        {
            SelectedIndex = index;
            _player.Source = new Uri(uri);
            State = PlayerState.Stopped;
            RaiseStateChanged();
        }

        public string GetCurrentTitle(string uri)
        {
            return "not implemented yet";
        }
 
        public void Play()
        {
            _player.Play();
            State = PlayerState.Playing;
            RaiseStateChanged();
        }
 
        public void Stop()
        {
            _player.Stop();
            State = PlayerState.Stopped;
            RaiseStateChanged();
        }
 
        public void Pause()
        {
            _player.Pause();
            State = PlayerState.Paused;
            RaiseStateChanged();
        }
 
        public void Unload()
        {
            if (State != PlayerState.Stopped)
            {
                _player.Stop();
            }
            State = PlayerState.Unknown;
        }

        public void LoadPlaylist(DataTable table)
        {
            _playlist.Clear();
            table.DefaultView.Sort = "Fav DESC"; // Favorite-Sort
            DataView clone = table.DefaultView;
            table = clone.ToTable();
            foreach (DataRow line in table.Rows)
            {
                string plus ="";
                if (line[2].ToString() == "True")
                    plus = "*";
                else
                    plus = "";
                _playlist.Add( plus+line[0].ToString().Trim(), line[1].ToString().Trim() );
            }
            RaiseStateChanged();
        }
 
        private void RaiseStateChanged()
        {
            if (StateChanged != null)
            {
                // Raise Event
                StateChanged(this, null);
            }
        }

        protected virtual void RaiseSongChanged(object Sender, MediaScriptCommandRoutedEventArgs eArgs)
        {

            SongEventArgs e = new SongEventArgs((string)eArgs.OriginalSource, eArgs.ParameterType, eArgs.ParameterValue);

            // Temporary copy, to prevent a race condition
            EventHandler<SongEventArgs> eventHandler = SongChanged;

            if (eventHandler != null)
            {
                // Raise Event
                eventHandler(this, e);
            }
        }
    }
 
    public enum PlayerState
    {
        Unknown,
        Stopped,
        Paused,
        Playing
    }

    public class SongEventArgs : EventArgs
    {
        public string title  { get; set; }
        public string artist { get; set; }
        public string album  {get; set; }        

        public SongEventArgs(string title, string artist, string album)
        {
            this.title  = title;
            this.artist = artist;
            this.album  = album;
        }
    }
}
