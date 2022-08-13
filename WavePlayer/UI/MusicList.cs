using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WavePlayer.WavPlayer.Management;
using System.IO;

namespace WavePlayer.UI
{
    public partial class MusicList : UserControl
    {
        public event EventHandler<int> ItemDoubleClicked;
        private MusicPlayList mMusicPlayList;
        private volatile bool mShowLocation=true;
        object locker = new object();
        [Browsable(true)]
        public bool ShowLocation
        {
            get
            {
                return mShowLocation;
            }
            set
            {
                try
                {
                    mShowLocation = value;
                    ListView1.Columns.Remove(columnHeaderLocation);
                    if(mShowLocation)
                    {
                        ListView1.Columns.Insert(0, columnHeaderLocation);
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.ToString());
                }
            }
        }
        public int SelectedIndex
        {
            get
            {
                if (ListView1.SelectedIndices.Count == 0) return -1;
                return ListView1.SelectedIndices[0];
            }
        }
        public void Select(int idx)
        {
            ListView1.SelectedIndices.Clear();
            if (idx != -1 && idx < ListView1.Items.Count)
            {
                ListView1.Items[idx].Selected = true;
            }
        }

        public MusicPlayList MusicPlayList
        {
            get
            {
                lock (locker)
                {
                    return mMusicPlayList;
                }
            }
            set
            {
                lock (locker)
                {
                    mMusicPlayList = value;
                }
                if (mMusicPlayList == null)
                {
                    ListView1.Items.Clear();
                }
                else
                {
                    ListView1.Items.Clear();
                    ListView1.BeginUpdate();
                    for (int i = 0; i < mMusicPlayList.Items.Count; ++i)
                    {
                        MusicListItem item = mMusicPlayList.Items[i];
                        if (item == null) continue;
                        ListViewItem listViewItem = null;
                        if (!item.FromRemote)
                        {
                            if (!File.Exists(item.FileFullPath)) continue;
                        }
                        if (mShowLocation)
                        {
                            String locationString = "";
                            if(item.RemoteLocation != null && item.RemoteLocation.IndexOf("localhost") <0)
                            {
                                locationString="Remote";
                            }
                            else
                            {
                                locationString="Local";
                            }
                            listViewItem = new ListViewItem(new string[]{
                                locationString,
                                item.Name,
                                item.DurationString,
                                item.Artist,
                                item.Album
                            });
                        }
                        else
                        {
                            listViewItem = new ListViewItem(new string[]{
                                item.Name,
                                item.DurationString,
                                item.Artist,
                                item.Album
                            });
                        }
                        listViewItem.Tag = item;
                        ListView1.Items.Add(listViewItem);
                    }
                    ListView1.EndUpdate();
                }
            }
        }

        public MusicList()
        {
            InitializeComponent();
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = ListView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                if (ItemDoubleClicked != null)
                {
                    ItemDoubleClicked(this, item.Index);
                }
            }
        }
    }
}
