using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using WavePlayer.MediaServer;
using WavePlayer.WavPlayer.Management;

namespace WavePlayer
{
    public partial class Form1
    {
        MediaNode mMediaNode;
        private void InitServer()
        {
            mMediaNode = new MediaNode();
            mMediaNode.MediaServerListUpdated += mMediaNode_MediaServerListUpdated;
            mMediaNode.MediaNodeLoaded += mMediaNode_MediaNodeLoaded;
            mMediaNode.Start();
        }

        void mMediaNode_MediaNodeLoaded(object sender, EventArgs e)
        {
            UpdateWithGatheredMusicList();
        }
        MusicPlayList GetRemotePlayList(String key, bool useSocket=false)
        {
            String cacheJson = "";
            if (!useSocket)
            {
                cacheJson = (String)mMediaNode.GetServerAttribute(key, "MEDIA_NODE_GET_MUSICLIST_ACK");
            }
            else
            {
                cacheJson = mMediaNode.SendServerGetMusicListRequest(key);
            }
            if (cacheJson != null)
            {
                return MusicPlayList.FromString(cacheJson, key);
            }
            return MusicPlayList.FromString("");
        }
        void MediaNodeSelected(int idx)
        {
            String key = ListViewRemoteNode.Items[idx].Text;
            String cacheJson = (String)mMediaNode.GetServerAttribute(key, "MEDIA_NODE_GET_MUSICLIST_ACK");
            if (cacheJson != null)
            {
                MusicPlayList playList = MusicPlayList.FromString(cacheJson,key);
                this.musicListRemote.MusicPlayList = playList;
            }

            AsyncTask task = new AsyncTask(() => {
                mMediaNode.SendServerGetMusicListRequest(key);
                cacheJson = (String)mMediaNode.GetServerAttribute(key, "MEDIA_NODE_GET_MUSICLIST_ACK");
            });
            task.AddAfterFinishJob(this, () => {
                if (cacheJson != null)
                {
                    MusicPlayList playList = MusicPlayList.FromString(cacheJson,key);
                    
                    this.musicListRemote.MusicPlayList = playList;
                }
            });
            task.Start(false);
        }
        void mMediaNode_MediaServerListUpdated(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => {
                    mMediaNode_MediaServerListUpdated(sender, e);
                }));
                return;
            }
            String selectedKey = "";
            int newSelection = -1;
            if (this.ListViewRemoteNode.SelectedIndices.Count > 0)
            {
                selectedKey = this.ListViewRemoteNode.Items[this.ListViewRemoteNode.SelectedIndices[0]].Text;
            }
            this.ListViewRemoteNode.Items.Clear();
            List<String> list = mMediaNode.ServerList;
            for (int i = 0; i < list.Count; ++i)
            {
                String s = list[i];
                ListViewRemoteNode.Items.Add(new ListViewItem(s));
                if (s.Equals(selectedKey))
                {
                    newSelection = ListViewRemoteNode.Items.Count - 1;
                }
            }
            if (newSelection != -1)
            {
                ListViewRemoteNode.Items[newSelection].Selected = true;
            }
            else
            {
                this.musicListRemote.MusicPlayList = MusicPlayList.FromString("");
            }
            ListViewRemoteNode.Invalidate();
            UpdateWithGatheredMusicList();
            if (list.Count == 0)
            {
                if (TabPage3Visible)
                {
                    TabPage3Visible = false;
                    this.tabControl1.TabPages.Remove(this.tabPage3);
                }
            }
            else
            {
                if (!TabPage3Visible)
                {
                    TabPage3Visible = true;
                    this.tabControl1.TabPages.Insert(2, this.tabPage3);
                }
            }
        }
        Locked<bool> TabPage3Visible = false;
        void UpdateWithGatheredMusicList()
        {
            MusicPlayList list = null;
            AsyncTask task = new AsyncTask(() =>
            {
                List<String> servers = new List<string>(this.mMediaNode.ServerList);
                Dictionary<String, MusicListItem> NameTable = new Dictionary<string, MusicListItem>();
                List<MusicListItem> Ordered = new List<MusicListItem>();
                list = this.PlayList;
                foreach (var item in list.Items)
                {
                    String name = Path.GetFileName(item.FileName);
                    if (!NameTable.ContainsKey(name))
                    {
                        NameTable[name] = item;
                        Ordered.Add(item);
                        item.RemoteLocation = "localhost";
                    }
                }
                for (int i = 0; i < servers.Count; ++i)
                {
                    String key = servers[i];
                    list = GetRemotePlayList(key, true);
                    foreach (var item in list.Items)
                    {
                        String name = Path.GetFileName(item.FileName);
                        if (!NameTable.ContainsKey(name))
                        {
                            NameTable[name] = item;
                            Ordered.Add(item);
                            item.RemoteLocation = key;
                        }
                        else
                        {
                            NameTable[name].RemoteLocation += ";" + key + " ";
                        }
                    }
                }
                for (int i = 0; i < Ordered.Count; ++i)
                {
                    MusicListItem item = Ordered[i];
                    if (item.RemoteLocation.IndexOf("localhost") < 0)
                    {
                        item.FromRemote = true;
                    }
                }
                list.Items.Clear();
                list.Items.AddRange(Ordered);
            });
            task.AddAfterFinishJob(this, () =>
            {
                int selectIdx = this.musicList1.SelectedIndex;
                this.musicList1.MusicPlayList = list;
                PlayItemIterator = this.musicList1.MusicPlayList.Items.GetEnumerator();
                for (int i = 0; i < selectIdx; ++i)
                {
                    if (!PlayItemIterator.MoveNext()) break;
                }
            });
            task.Start(false);
        }
    }
}
