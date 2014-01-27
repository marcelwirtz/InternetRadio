using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;


namespace InternetRadio
{
    public class Player
    {

        private MediaElement _player;
        public PlayerState State { get; set; }
        private Dictionary<string, string> _playlist = new Dictionary<string, string>();
        private static Player _instance;
        public event EventHandler Changed;
 
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
                OnChanged();
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
        }
 
        public void SetSource(int index, string uri)
        {
            SelectedIndex = index;
            _player.Source = new Uri(uri);
            State = PlayerState.Stopped;
            OnChanged();
        }
 
        public void Play()
        {
            _player.Play();
            State = PlayerState.Playing;
            OnChanged();
        }
 
        public void Stop()
        {
            _player.Stop();
            State = PlayerState.Stopped;
            OnChanged();
        }
 
        public void Pause()
        {
            _player.Pause();
            State = PlayerState.Paused;
            OnChanged();
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
            OnChanged();
        }
 
        private void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, null);
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
}
