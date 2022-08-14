using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using WavePlayer.WavPlayer;
using WavePlayer.WavPlayer.Management;
namespace WavePlayer
{
    public partial class Form1 : Form
    {
        volatile bool RepeatChecked = false;
        OpenFileDialog mOpenFileDialog = new OpenFileDialog() { Filter = "Supported Files(*.wav;*.mp3;*.lst;*.json;*.bmp)|*.wav;*.mp3;*.lst;*.json;*.bmp;*.gif|Wav File(*.wav)|*.wav|MP3 File(*.mp3)|*.mp3|Image File(*.bmp;*.gif)|*.bmp;*.gif|Data Base List File(*.lst;*.json)|*.lst;*.json", Title = "Open Wave File" };
        OpenFileDialog mOpenMP3FileDialog = new OpenFileDialog() { Filter = "Supported Files(*.wav;*.mp3;*.lst;*.json;*.bmp)|*.wav;*.mp3;*.lst;*.json;*.bmp;*.gif|Wav File(*.wav)|*.wav|MP3 File(*.mp3)|*.mp3|Image File(*.bmp;*.gif)|*.bmp;*.gif|Data Base List File(*.lst;*.json)|*.lst;*.json", Title = "Open Wave File" };
        Interfaces.IPlayer Player;
        CancasImpl mCancasImpl;
        private double PreviousProgress = 0;
        String ListFileName = "";
        Lyrics.LyricParser LyricParser;
        WavPlayer.Management.MusicListItem mCurrentPlayItem;
        IEnumerator<WavPlayer.Management.MusicListItem> PlayItemIterator;
        Enhance.SoundTouchEffect SoundTouchEffect = null;
        Enhance.StreamBufferSharingEffectOperator StreamSharingEffect = null;
        Enhance.SoundVisualizationEffect SoundVision = null;
        public int currentPitch = 0;
        public double tempo = 1;
        FileSystemWatcher mFileSystemWatcher;
        object dataJsonLocker = new object();
        AsyncTask dataJsonUpdator = new AsyncTask();
        class CancasImpl : Interfaces.ICanvas
        {
            Form1 parent;
            public Bitmap BaseImage
            {
                get;
                private set;
            }
            public void UpdateBaseImage(Bitmap bmp)
            {
                this.BaseImage = bmp;
            }
            public Size CanvasSize
            {
                get
                {
                    if (this.parent.InvokeRequired)
                    {
                        return (Size)this.parent.Invoke(new Func<Size>(() => this.parent.pictureBox1.Size));
                    }
                    return this.parent.pictureBox1.Size;
                }
            }
            public void OnBitmapReady(Bitmap bmp)
            {
                try
                {
                    if (this.parent.InvokeRequired)
                    {
                        this.parent.Invoke(new Action(() =>
                        {
                            this.parent.pictureBox1.Image = bmp;
                        }));
                        return;
                    }
                    this.parent.pictureBox1.Image = bmp;
                }
                catch (Exception ee)
                {

                }
            }
            public CancasImpl(Form1 parent)
            {
                this.parent = parent;
            }
        }
        class MediaNodeFileSystemImpl : Interfaces.IFileSystem
        {
            Form1 parent;
            public MediaNodeFileSystemImpl(Form1 parent)
            {
                this.parent = parent;
            }
            public string GetFullFilePath(string filenameKey)
            {
                List<MusicListItem> items = new List<MusicListItem>(parent.musicList1.MusicPlayList.Items);
                for (int i = 0; i < items.Count; ++i)
                {
                    MusicListItem item = items[i];
                    if (Path.GetFileName(item.FileName).Equals(filenameKey))
                    {
                        return item.FileFullPath;
                    }
                }
                return "";
            }

            public int GetFileSize(string filenameKey)
            {
                List<MusicListItem> items = new List<MusicListItem>(parent.musicList1.MusicPlayList.Items);
                for (int i = 0; i < items.Count; ++i)
                {
                    MusicListItem item = items[i];
                    if (Path.GetFileName(item.FileName).Equals(filenameKey))
                    {
                        FileInfo fInfo = new FileInfo(item.FileFullPath);
                        return (int)fInfo.Length;
                    }
                }
                return 0;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            mFileSystemWatcher = new FileSystemWatcher(Application.StartupPath, "*.json");
            mFileSystemWatcher.BeginInit();
            mFileSystemWatcher.IncludeSubdirectories = false;
            mFileSystemWatcher.EnableRaisingEvents = true;
            mFileSystemWatcher.Changed += mFileSystemWatcher_Changed;
            mFileSystemWatcher.EndInit();
            LoadDataJson();
            InitServer();
            if (mMediaNode != null)
            {
                mMediaNode.FileSystem = new MediaNodeFileSystemImpl(this);
            }
            this.tabControl1.TabPages.Remove(tabPage3);
        }
        private void LoadDataJson()
        {
            if (File.Exists("data.json"))
            {
                ListFileName = Path.Combine(Application.StartupPath, "data.json");
                PlayList = FilterPlayList(WavPlayer.Management.MusicPlayList.FromFile(ListFileName));
                musicList1.MusicPlayList = PlayList;
                PlayItemIterator = PlayList.Items.GetEnumerator();
                PlayItemIterator.MoveNext();
            }
        }
        MusicPlayList FilterPlayList(MusicPlayList list)
        {
            MusicPlayList ret = MusicPlayList.FromString("[]");
            for (int i = 0; i < list.Items.Count; ++i)
            {
                MusicListItem item = list.Items[i];
                if (!item.FromRemote)
                {
                    if (!File.Exists(item.FileFullPath))
                    {
                        continue;
                    }
                }
                ret.Items.Add(item);
            }
            return ret;
        }

        void mFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Path.GetFileName(e.Name).Equals("data.json"))
            {
                dataJsonUpdator.AddAfterFinishJob(new Action(() =>
                {
                    MediaServer.MediaNode.TryAccessFile(e.FullPath, (path) =>
                    {
                        this.Invoke(new Action(LoadDataJson));
                    });
                }));
                dataJsonUpdator.FlushJob(false);
            }
        }
        public Form1()
        {
            InitializeComponent();
            mCancasImpl = new Form1.CancasImpl(this);
            musicList1.ItemDoubleClicked += musicList1_ItemDoubleClicked;
        }
        private void SetItemByMusicListItem(WavPlayer.Management.MusicListItem item)
        {
            if (item == null) return;
            mCurrentPlayItem = item;
            if (item.FromRemote)
            {
                SetSource(item.RemoteLocation.Split(';')[0], true, item.FromRemote);
            }
            else
            {
                SetSource(item.FileFullPath, true, item.FromRemote);
            }
            String album = "";
            String artist = "";
            String name = "";
            if (item.Album != "NONE")
            {
                album = "[" + item.Album + "]";
            }
            if (item.Artist != "NONE")
            {
                artist = item.Artist;
            }
            else
            {
                artist = "<Unknown Artist>";
            }
            if (item.Name == "NONE")
            {
                name = item.Name;
            }
            else
            {
                name = Path.GetFileNameWithoutExtension(item.FileFullPath);
            }
            labelTitle.Text = String.Join(" ", album, artist, "-", name);
            if (File.Exists(item.CoverPictureFullPath))
            {
                Bitmap bmp = Form1.GetResizedBitmap(Bitmap.FromFile(item.CoverPictureFullPath), pictureBox1.Size, Color.Black, true, true);
                if (this.mCancasImpl != null)
                {
                    this.mCancasImpl.UpdateBaseImage(bmp);
                }
                pictureBox1.Image = bmp;
                tabPage1.BackgroundImage = null;
                tabPage1.BackColor = Color.Black;
                pictureBox1.Visible = true;
                pictureBox1.BackColor = Color.Transparent;
            }
            else
            {
                pictureBox1.ImageLocation = "";
                pictureBox1.Visible = false;
                tabPage1.BackColor = Color.Transparent;
            }
        }
        private String GetDownloadFolder()
        {
            String dir = Path.Combine(Application.StartupPath, "Downloads");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
        private void HandleRemoteFile(WavPlayer.Management.MusicListItem item, EventHandler<Tuple<MusicListItem, String>> onFinished = null)
        {
            String dir = GetDownloadFolder();
            mMediaNode.PerformP2PParallelDownload(item, dir, onFinished);
        }
        private void OnRemoteFileDownloadFinished(object sender, Tuple<MusicListItem, String> arg)
        {
            MusicListItem item = arg.Item1;
            String filename = arg.Item2;
            item.FileFullPath = filename;
            item.RemoteLocation += ";localhost";
            item.FromRemote = false;

            this.Invoke(new Action(() =>
            {
                this.musicList1.MusicPlayList.SaveToJSONFile("data.json");
                this.musicList1.MusicPlayList = MusicPlayList.FromFile("data.json");
                this.UpdateWithGatheredMusicList();
                if (DialogResult.Yes == MessageBox.Show(Path.GetFileName(filename) + " has been downloaded" + Environment.NewLine + "Do you want to play it?", "Download Finished", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    SetItemByMusicListItem(item);
                    if (Player == null)
                    {
                        return;
                    }
                    Player.Play();
                    SoundTouchEffect.SetPitchSemiTones(currentPitch);
                    SoundTouchEffect.SetTempo(tempo);
                    SoundTouchEffect.SetPlayRate(1.0);
                    buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
                }
            }));

        }
        void musicList1_ItemDoubleClicked(object sender, int e)
        {
            MusicPlayList playList = this.musicList1.MusicPlayList;

            if (PlayList != null && e >= 0 && e < playList.Items.Count)
            {
                WavPlayer.Management.MusicListItem item = playList.Items[e];
                {
                    if (item.FromRemote)
                    {
                        HandleRemoteFile(item, OnRemoteFileDownloadFinished);
                        return;
                    }
                    SetItemByMusicListItem(item);
                    if (Player == null)
                    {
                        return;
                    }
                    Player.Play();
                    SoundTouchEffect.SetPitchSemiTones(currentPitch);
                    SoundTouchEffect.SetTempo(tempo);
                    SoundTouchEffect.SetPlayRate(1.0);
                    buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
                }
            }
        }
        public static Bitmap GetResizedBitmap(Image bmp, Size size, Color backColor, bool keepRatio = false, bool bAlignCenter = true)
        {

            Bitmap ret = new Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height));
            using (Graphics ctx = Graphics.FromImage(ret))
            {
                ctx.Clear(backColor);
                ctx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                ctx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                ctx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                ctx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                if (!keepRatio)
                {
                    ctx.DrawImage(bmp, 0, 0, size.Width, size.Height);
                }
                else
                {
                    double ratiox = ((double)size.Width) / bmp.Width;
                    double ratioy = ((double)size.Height) / bmp.Height;
                    double ratio = Math.Min(ratiox, ratioy);
                    int newWidth = (int)(ratio * bmp.Width);
                    int newHeight = (int)(ratio * bmp.Height);
                    if (newWidth <= 0)
                    {
                        newWidth = 1;
                    }
                    if (newHeight <= 0)
                    {
                        newHeight = 1;
                    }
                    if (bAlignCenter)
                        ctx.DrawImage(bmp, size.Width / 2 - newWidth / 2, size.Height / 2 - newHeight / 2, newWidth, newHeight);
                    else
                        ctx.DrawImage(bmp, 0, size.Height / 2 - newHeight / 2, newWidth, newHeight);
                }
            }
            return ret;
        }

        void Player_Finished(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { Player_Finished(sender, e); }));
                return;
            }

            try
            {
                if (!IsPlayListMode)
                {
                    if (RepeatChecked)
                    {
                        if (Player != null)
                        {
                            Player.Play();
                            SoundTouchEffect.SetPitchSemiTones(currentPitch);
                            SoundTouchEffect.SetTempo(tempo);
                            SoundTouchEffect.SetPlayRate(1.0);
                        }
                    }
                    else
                    {
                        buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;
                    }
                }
                else
                {
                    if (PlayList != null)
                    {
                        if (PlayItemIterator == null)
                        {
                            PlayItemIterator = PlayList.Items.GetEnumerator();
                        }

                        bool canMoveNext = false;
                        try
                        {
                            canMoveNext = PlayItemIterator.MoveNext();
                        }
                        catch (Exception ee)
                        {

                        }
                        if (!canMoveNext)
                        {
                            if (RepeatChecked)
                            {
                                PlayItemIterator = PlayList.Items.GetEnumerator();
                                PlayItemIterator.MoveNext();
                                mCurrentPlayItem = PlayItemIterator.Current;
                                SetItemByMusicListItem(mCurrentPlayItem);
                                Player.Play();
                                SoundTouchEffect.SetPitchSemiTones(currentPitch);
                                SoundTouchEffect.SetTempo(tempo);
                                SoundTouchEffect.SetPlayRate(1.0);
                                buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
                            }
                            else
                            {
                                PlayItemIterator = PlayList.Items.GetEnumerator();
                                PlayItemIterator.MoveNext();
                                mCurrentPlayItem = PlayItemIterator.Current;
                                SetItemByMusicListItem(mCurrentPlayItem);
                                SoundTouchEffect.SetPitchSemiTones(currentPitch);
                                SoundTouchEffect.SetTempo(tempo);
                                SoundTouchEffect.SetPlayRate(1.0);
                                buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;
                            }
                        }
                        else
                        {
                            mCurrentPlayItem = PlayItemIterator.Current;
                            SetItemByMusicListItem(mCurrentPlayItem);
                            Player.Play();
                            SoundTouchEffect.SetPitchSemiTones(currentPitch);
                            SoundTouchEffect.SetTempo(tempo);
                            SoundTouchEffect.SetPlayRate(1.0);
                            buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
                        }
                    }

                }
            }
            catch (Exception ee)
            {

            }


        }

        void Player_ProgressUpdated(object sender, double e)
        {
            if (buttonForward.IsDown)
            {
                this.Player.Position += 1;
            }
            else if (buttonBackward.IsDown)
            {
                this.Player.Position -= 1;
            }
            if (Math.Abs(e - PreviousProgress) < 1) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { Player_ProgressUpdated(sender, e); }));
            }
            this.progressBar1.Value = (int)e;
            PreviousProgress = e;
            this.labelElapsed.Text = TimeSpan.FromSeconds(e).ToSimpleString();
            if (LyricParser != null)
            {
                LyricParser.Refresh(e);

                String txt = LyricParser.CurrentLyrics;
                if (txt != null)
                {
                    labelLyric.Text = txt;
                }
                else
                {
                    labelLyric.Text = "";
                    LyricParser.CurrentLyrics = "";
                }
            }
        }
        bool IsPlayListMode
        {
            get
            {
                return !String.IsNullOrEmpty(ListFileName);
            }
        }
        private void SetStreamSource(String server)
        {
            if (Player != null)
            {
                Player.Dispose();
                Player = null;
            }
            Player = new Enhance.RemoteRealtimeStreamPlayer(this, this.mMediaNode);
            Player.ProgressUpdated += Player_ProgressUpdated;
            Player.Finished += Player_Finished;
            SoundTouchEffect = new Enhance.SoundTouchEffect(Player);
            StreamSharingEffect = new Enhance.StreamBufferSharingEffectOperator(this.mMediaNode);
            SoundVision = new Enhance.SoundVisualizationEffect(this.mCancasImpl);
            if (this.mMediaNode != null) SoundVision.Enabled = mMediaNode.EnableSoundVisualization;
            Player.SetDataSource(server);
            Player.Effects.Add(SoundTouchEffect);
            Player.Effects.Add(StreamSharingEffect);
            Player.Effects.Add(SoundVision);
            this.progressBar1.Maximum = (int)Player.Duration;
            this.progressBar1.Value = 0;
            this.labelRemain.Text = "";
            LyricParser = null;
            labelLyric.Text = "";
            labelElapsed.Text = "";
            buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;

            labelTitle.Text = "Streaming From:" + server.Split(':')[0];
            pictureBox1.ImageLocation = "";
            pictureBox1.Visible = false;
            tabPage1.BackgroundImage = global::MusicPlayer.Properties.Resources.bluegradient;
            tabPage1.BackColor = Color.Transparent;
        }
        private void SetSource(String FileName, bool fromPlayList = false, bool streamFromRemote = false)
        {
            if (!fromPlayList)
            {
                ListFileName = "";
            }
            if (Player != null)
            {
                Player.Dispose();
                Player = null;
            }
            if (!streamFromRemote)
            {
                if (Path.GetExtension(FileName).Equals(".wav", StringComparison.InvariantCultureIgnoreCase))
                {
                    Player = new WavPlayer.Player.WavPlayer(this);
                    Player.ProgressUpdated += Player_ProgressUpdated;
                    Player.Finished += Player_Finished;
                }
                else if (Path.GetExtension(FileName).Equals(".mp3", StringComparison.InvariantCultureIgnoreCase))
                {
                    Player = new Enhance.RealtimeStreamPlayer(this);
                    Player.ProgressUpdated += Player_ProgressUpdated;
                    Player.Finished += Player_Finished;
                }
                else if (Path.GetExtension(FileName).Equals(".bmp", StringComparison.InvariantCultureIgnoreCase))
                {
                    Player = new Enhance.BMPPlayer(this);
                    Player.ProgressUpdated += Player_ProgressUpdated;
                    Player.Finished += Player_Finished;
                }
                else if (Path.GetExtension(FileName).Equals(".gif", StringComparison.InvariantCultureIgnoreCase))
                {
                    Player = new Enhance.GIFPlayer(this);
                    Player.ProgressUpdated += Player_ProgressUpdated;
                    Player.Finished += Player_Finished;
                }
            }
            else
            {
                // TODO 
                // get file from remote
                return;
            }
            if (Player.HasVideo)
            {
                Player.AttachCanvas(mCancasImpl);
            }
            SoundTouchEffect = new Enhance.SoundTouchEffect(Player);
            StreamSharingEffect = new Enhance.StreamBufferSharingEffectOperator(this.mMediaNode);
            SoundVision = new Enhance.SoundVisualizationEffect(this.mCancasImpl);
            if (this.mMediaNode != null) SoundVision.Enabled = mMediaNode.EnableSoundVisualization;
            Player.SetDataSource(FileName);
            Player.Effects.Add(SoundTouchEffect);
            Player.Effects.Add(StreamSharingEffect);
            Player.Effects.Add(SoundVision);
            if (!streamFromRemote)
            {
                if (this.mMediaNode != null)
                {
                    this.mMediaNode.Attributes.SetAttribute("Current.Channels", Player.Channels);
                    this.mMediaNode.Attributes.SetAttribute("Current.SampleRate", Player.SampleRate);
                    this.mMediaNode.Attributes.SetAttribute("Current.BitsPerSample", Player.BitsPerSample);
                }
            }
            this.progressBar1.Maximum = (int)Player.Duration;
            this.progressBar1.Value = 0;
            this.labelRemain.Text = TimeSpan.FromSeconds(Player.Duration).ToSimpleString();
            String lrcFileName = Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName) + ".lrc");
            if (File.Exists(lrcFileName))
            {
                LyricParser = new Lyrics.LyricParser(lrcFileName, Encoding.UTF8);
            }
            else
            {
                LyricParser = null;
            }
            if (!fromPlayList)
            {
                labelTitle.Text = Path.GetFileName(FileName);
                tabPage1.BackgroundImage = global::MusicPlayer.Properties.Resources.bluegradient;
                tabPage1.BackColor = Color.Transparent;
            }
            labelLyric.Text = "";
            labelElapsed.Text = "00:00:00";
            buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;
        }
        WavPlayer.Management.MusicPlayList mPlayList = null;
        Locker mListLocker = new Locker();
        WavPlayer.Management.MusicPlayList PlayList
        {
            get
            {
                return mListLocker.Synchronized(() => mPlayList);
            }
            set
            {
                mListLocker.Synchronized(()=>mPlayList=value);
            }
        }
        private void SetCoverFromFileName(String filename)
        {
            String coverFileName = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
            String coverPath = "";
            if (File.Exists(coverFileName + ".png"))
            {
                coverPath = coverFileName + ".png";
            }
            else if (File.Exists(coverFileName + ".jpg"))
            {
                coverPath = coverFileName + ".jpg";
            }
            else if (File.Exists(coverFileName + ".bmp"))
            {
                coverPath = coverFileName + ".bmp";
            }
            if (!String.IsNullOrEmpty(coverPath))
            {
                CoverFullPathWithoutPlayList = coverPath;
                Bitmap bmp = Form1.GetResizedBitmap(Bitmap.FromFile(CoverFullPathWithoutPlayList), pictureBox1.Size, Color.Black, true, true);
                if (this.mCancasImpl != null)
                {
                    this.mCancasImpl.UpdateBaseImage(bmp);
                }
                pictureBox1.Image = bmp;
                tabPage1.BackgroundImage = null;
                tabPage1.BackColor = Color.Black;
                pictureBox1.Visible = true;
                pictureBox1.BackColor = Color.Transparent;
            }
        }
        volatile String CoverFullPathWithoutPlayList = "";
        private void OpenFile()
        {
            CoverFullPathWithoutPlayList = "";
            if (mOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Player != null)
                {
                    Player.Stop();
                    Player.Close();
                    Player = null;
                }
                String extension = Path.GetExtension(mOpenFileDialog.FileName);
                if (extension.IndexOf("wav", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    SetSource(mOpenFileDialog.FileName);
                    pictureBox1.Visible = true;
                    SetCoverFromFileName(mOpenFileDialog.FileName);
                    PlayList = null;
                    PlayItemIterator = null;
                    musicList1.MusicPlayList = null;
                    PlayItemIterator = null;
                    
                }
                else if (
                    extension.IndexOf("lst", StringComparison.InvariantCultureIgnoreCase) != -1
                    || extension.IndexOf("json", StringComparison.InvariantCultureIgnoreCase) != -1
                )
                {
                    ListFileName = mOpenFileDialog.FileName;
                    PlayList = WavPlayer.Management.MusicPlayList.FromFile(ListFileName);
                    musicList1.MusicPlayList = PlayList;
                    pictureBox1.Visible = false;
                    PlayItemIterator = PlayList.Items.GetEnumerator();
                    PlayItemIterator.MoveNext();
                }
                else if (extension.IndexOf("mp3", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    SetSource(mOpenFileDialog.FileName);
                    pictureBox1.Visible = true;
                    SetCoverFromFileName(mOpenFileDialog.FileName);
                    PlayList = null;
                    PlayItemIterator = null;
                    musicList1.MusicPlayList = null;
                    
                }
                else if (extension.IndexOf("bmp", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    SetSource(mOpenFileDialog.FileName);
                    PlayList = null;
                    PlayItemIterator = null;
                    musicList1.MusicPlayList = null;
                    pictureBox1.Visible = true;
                }
                else if (extension.IndexOf("gif", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    SetSource(mOpenFileDialog.FileName);
                    PlayList = null;
                    PlayItemIterator = null;
                    musicList1.MusicPlayList = null;
                    pictureBox1.Visible = true;
                }
            }
        }
        private void CloseFile()
        {
            PlayItemIterator = null;
            PlayList = null;
            mCurrentPlayItem = null;
            Player.Close();
            this.labelRemain.Text = "00:00:00";
            this.labelElapsed.Text = "00:00:00";
            this.progressBar1.Value = 0;
            labelTitle.Text = "";
            labelLyric.Text = "";
            pictureBox1.Visible = false;
            buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;
        }

        private void buttonStartPause_Click(object sender, EventArgs e)
        {
            if (Player != null && Player.Playing)
            {
                Player.Pause();
                buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;
            }
            else
            {
                if (IsPlayListMode)
                {
                    if (PlayList != null)
                    {
                        if (PlayItemIterator == null)
                        {
                            PlayItemIterator = PlayList.Items.GetEnumerator();
                            PlayItemIterator.MoveNext();
                        }
                        if (mCurrentPlayItem == null)
                        {
                            if (PlayItemIterator != null)
                            {
                                mCurrentPlayItem = PlayItemIterator.Current;
                            }
                            else
                            {
                                mCurrentPlayItem = PlayList.Items[0];
                            }
                            SetItemByMusicListItem(mCurrentPlayItem);
                        }
                    }
                }
                if (Player == null)
                {
                    return;
                }
                Player.Play();
                SoundTouchEffect.SetPitchSemiTones(currentPitch);
                SoundTouchEffect.SetTempo(tempo);
                SoundTouchEffect.SetPlayRate(1.0);
                buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
            }
        }

        private void progressBar1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Player == null) return;
            if (MouseIsDown)
            {
                double ratio = ((double)e.X) / progressBar1.Width;
                double target = ratio * this.Player.Duration;
                this.Player.Position = (long)target;
                progressBar1.Value = (int)target;
                this.labelElapsed.Text = TimeSpan.FromSeconds(target).ToSimpleString();
            }
        }

        private void progressBar1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseIsDown = false;
        }
        private volatile bool MouseIsDown = false;
        private void progressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Player == null) return;
            try
            {
                double dur = this.Player.Duration;
                double ratio = ((double)e.X) / progressBar1.Width;
                double target = ratio * dur;
                this.Player.Position = (long)target;
                progressBar1.Value = (int)target;
                this.labelElapsed.Text = TimeSpan.FromSeconds(target).ToSimpleString();
                MouseIsDown = true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        private void progressBar1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Player == null) return;
            try
            {
                if (!MouseIsDown)
                {
                    double dur = this.Player.Duration;
                    double ratio = ((double)e.X) / progressBar1.Width;
                    double target = ratio * dur;
                    this.Player.Position = (long)target;
                    progressBar1.Value = (int)target;
                    this.labelElapsed.Text = TimeSpan.FromSeconds(target).ToSimpleString();
                }
                MouseIsDown = false;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CloseFile();
            this.Close();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (Player == null)
            {
                return;
            }
            this.Player.Stop();
            this.progressBar1.Value = 0;
            this.Player.Effects.Add(SoundTouchEffect);
            this.Player.Effects.Add(this.StreamSharingEffect);
            this.Player.Effects.Add(this.SoundVision);
            buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;
        }

        private void checkBoxRepeat_CheckedChanged(object sender, EventArgs e)
        {
            RepeatChecked = !RepeatChecked;
            if (RepeatChecked)
            {
                checkBoxRepeat.Image = global::MusicPlayer.Properties.Resources.repeat_activated;
            }
            else
            {
                checkBoxRepeat.Image = global::MusicPlayer.Properties.Resources.repeat;
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (this.PlayList != null)
            {
                if (mCurrentPlayItem == null) return;
                if (String.IsNullOrEmpty(mCurrentPlayItem.CoverPictureFullPath)) return;
                if (String.IsNullOrEmpty(mCurrentPlayItem.CoverPictureFullPath)) return;
                if (mCurrentPlayItem != null && pictureBox1.Image != null && pictureBox1.Width > 0 && pictureBox1.Height > 0)
                {
                    Bitmap bmp = Form1.GetResizedBitmap(Bitmap.FromFile(mCurrentPlayItem.CoverPictureFullPath), pictureBox1.Size, Color.Black, true, true);
                    if (this.mCancasImpl != null)
                    {
                        this.mCancasImpl.UpdateBaseImage(bmp);
                    }
                    pictureBox1.Image = bmp;

                }
                pictureBox1.Invalidate();
            }
            else
            {

                if (!String.IsNullOrEmpty(CoverFullPathWithoutPlayList) && pictureBox1.Width > 0 && pictureBox1.Height > 0)
                {
                    Bitmap bmp = Form1.GetResizedBitmap(Bitmap.FromFile(CoverFullPathWithoutPlayList), pictureBox1.Size, Color.Black, true, true);
                    if (this.mCancasImpl != null)
                    {
                        this.mCancasImpl.UpdateBaseImage(bmp);
                    }
                    pictureBox1.Image = bmp;

                }
                pictureBox1.Invalidate();
            }
        }

        private void buttonPitchDown_Click(object sender, EventArgs e)
        {
            if (currentPitch - 1 >= -12)
            {
                --currentPitch;
            }
            SoundTouchEffect.SetPitchSemiTones(currentPitch);
            if (currentPitch != 0 || tempo != 1)
            {
                this.Text = "Wave Player" + String.Format("(Pitch:{0} Tempo:{1})", currentPitch, tempo);
            }
            else
            {
                this.Text = "Wave Player";
            }
        }

        private void buttonPitchUp_Click(object sender, EventArgs e)
        {
            if (currentPitch + 1 <= 12)
            {
                ++currentPitch;
            }
            SoundTouchEffect.SetPitchSemiTones(currentPitch);
            if (currentPitch != 0 || tempo != 1)
            {
                this.Text = "Wave Player" + String.Format("(Pitch:{0} Tempo:{1})", currentPitch, tempo);
            }
            else
            {
                this.Text = "Wave Player";
            }
        }

        private void buttonTempoDown_Click(object sender, EventArgs e)
        {
            if (tempo - 0.1 > 0.1)
            {
                tempo = tempo - 0.1;
            }
            else
            {
                tempo = 0.1;
            }
            tempo = Math.Round(tempo, 1);
            SoundTouchEffect.SetTempo(tempo);
            if (currentPitch != 0 || tempo != 1)
            {
                this.Text = "Wave Player" + String.Format("(Pitch:{0} Tempo:{1})", currentPitch, tempo);
            }
            else
            {
                this.Text = "Wave Player";
            }
        }

        private void buttonTempoUp_Click(object sender, EventArgs e)
        {
            if (tempo + 0.1 <= 2)
            {
                tempo = tempo + 0.1;
            }
            else
            {
                tempo = 2.0;
            }
            tempo = Math.Round(tempo, 1);
            SoundTouchEffect.SetTempo(tempo);
            if (currentPitch != 0 || tempo != 1)
            {
                this.Text = "Wave Player" + String.Format("(Pitch:{0} Tempo:{1})", currentPitch, tempo);
            }
            else
            {
                this.Text = "Wave Player";
            }
        }
        int mSearchIndex = -1;
        AsyncTask taskSearchForMusicPlayList = new AsyncTask();
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (this.musicList1 == null) return;
            String txt = textBoxSearch.Text;
            MusicPlayList playList = this.musicList1.MusicPlayList;
            if (playList == null) return;
            taskSearchForMusicPlayList.AddAfterFinishJob(() =>
            {
                bool found = false;
                for (int i = 0; i < playList.Items.Count; ++i)
                {
                    var item = playList.Items[i];
                    if (item.Name.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase) > -1
                        || item.Album.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase) > -1
                        || item.Artist.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        found = true;
                        mSearchIndex = i;
                        break;
                    }
                }
                if (!found)
                {
                    mSearchIndex = -1;
                }
                this.Invoke(new Action(() =>
                {
                    this.musicList1.Select(mSearchIndex);
                }));
            });
            taskSearchForMusicPlayList.FlushJob(false);
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.musicList1 == null) return;
                String txt = textBoxSearch.Text;
                MusicPlayList playList = this.musicList1.MusicPlayList;
                if (playList == null)
                {
                    return;
                }
                taskSearchForMusicPlayList.AddAfterFinishJob(() =>
                {
                    bool found = false;
                    if (mSearchIndex + 1 >= playList.Items.Count)
                    {
                        mSearchIndex = -1;
                    }
                    for (int i = mSearchIndex + 1; i < playList.Items.Count; ++i)
                    {
                        var item = PlayList.Items[i];
                        if (item.Name.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase) > -1
                       || item.Album.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase) > -1
                       || item.Artist.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase) > -1)
                        {
                            found = true;
                            mSearchIndex = i;
                            break;
                        }
                    }
                    if (!found)
                    {
                        mSearchIndex = -1;
                    }
                    this.Invoke(new Action(() =>
                    {
                        this.musicList1.Select(mSearchIndex);
                    }));
                });
                taskSearchForMusicPlayList.FlushJob(false);
            }
        }



        private void ListViewRemoteNode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListViewRemoteNode.SelectedIndices.Count == 0) return;
            int selectedIdx = ListViewRemoteNode.SelectedIndices[0];
            MediaNodeSelected(selectedIdx);

            // TODO
            // send "GET_MUSIC_LIST" request to selected node.
            // ack "ACK_MUSIC_LIST"

        }

        private void ListViewRemoteNode_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = ListViewRemoteNode.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                String serverName = item.Text;
                try
                {
                    SetStreamSource(serverName);
                    Player.Play();
                    SoundTouchEffect.SetPitchSemiTones(currentPitch);
                    SoundTouchEffect.SetTempo(tempo);
                    SoundTouchEffect.SetPlayRate(1.0);
                    buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
                }
                catch (Exception ee)
                {
                    Player.Stop();
                    this.progressBar1.Value = 0;
                    this.Player.Effects.Add(SoundTouchEffect);
                    this.Player.Effects.Add(this.StreamSharingEffect);
                    this.Player.Effects.Add(this.SoundVision);
                    buttonStartPause.Image = global::MusicPlayer.Properties.Resources.play;
                }
            }
        }

        private void buttonNextSong_Click(object sender, EventArgs e)
        {
            if (PlayList != null)
            {
                if (PlayItemIterator == null)
                {
                    PlayItemIterator = PlayList.Items.GetEnumerator();
                }

                bool canMoveNext = false;
                try
                {
                    canMoveNext = PlayItemIterator.MoveNext();
                }
                catch (Exception ee)
                {

                }
                if (!canMoveNext)
                {
                    PlayItemIterator = PlayList.Items.GetEnumerator();
                    PlayItemIterator.MoveNext();
                    mCurrentPlayItem = PlayItemIterator.Current;
                    SetItemByMusicListItem(mCurrentPlayItem);
                    Player.Play();
                    SoundTouchEffect.SetPitchSemiTones(currentPitch);
                    SoundTouchEffect.SetTempo(tempo);
                    SoundTouchEffect.SetPlayRate(1.0);
                    buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
                }
                else
                {
                    mCurrentPlayItem = PlayItemIterator.Current;
                    SetItemByMusicListItem(mCurrentPlayItem);
                    Player.Play();
                    SoundTouchEffect.SetPitchSemiTones(currentPitch);
                    SoundTouchEffect.SetTempo(tempo);
                    SoundTouchEffect.SetPlayRate(1.0);
                    buttonStartPause.Image = global::MusicPlayer.Properties.Resources.pause;
                }
            }

        }
        volatile bool VolumeMouseIsDown = false;
        private void trackBar1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (!VolumeMouseIsDown)
                {
                    double ratio = ((double)e.X) / trackBar1.Width;
                    double target = ratio * 100;
                    trackBar1.Value = (int)target;
                    SoundTouchEffect.SetVolume(trackBar1.Value / 100.0);
                }
                MouseIsDown = false;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                VolumeMouseIsDown = true;
                double ratio = ((double)e.X) / trackBar1.Width;
                double target = ratio * 100;
                trackBar1.Value = (int)target;
                SoundTouchEffect.SetVolume(trackBar1.Value/100.0);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        private void trackBar1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (VolumeMouseIsDown)
                {
                    double ratio = ((double)e.X) / trackBar1.Width;
                    double target = ratio * 100;
                    trackBar1.Value = (int)target;
                    SoundTouchEffect.SetVolume(trackBar1.Value / 100.0);
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            VolumeMouseIsDown = false;
        }

        MusicPlayList mixPlayList = MusicPlayList.FromFile("MixPlayList");
        private void buttonAddMix_Click(object sender, EventArgs e)
        {
            if (Player == null) return;
            if (!(Player is Enhance.RealtimeStreamPlayer)) return;
            if(mOpenMP3FileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            Enhance.RealtimeStreamPlayer realtimePlayer = (Enhance.RealtimeStreamPlayer)Player;
            realtimePlayer.AddMP3File(mOpenMP3FileDialog.FileName);
            mixPlayList.AddFile(mOpenMP3FileDialog.FileName);
            musicList2.MusicPlayList = mixPlayList;
        }

        private void buttonDeleteMix_Click(object sender, EventArgs e)
        {
            if (Player == null) return;
            if (!(Player is Enhance.RealtimeStreamPlayer)) return;
            if (musicList2.SelectedIndex < 0) return;
            int idx = musicList2.SelectedIndex;
            MusicListItem item = musicList2.MusicPlayList.Items[idx];
            Enhance.RealtimeStreamPlayer realtimePlayer = (Enhance.RealtimeStreamPlayer)Player;
            realtimePlayer.RemoveMP3File(item.FileFullPath);
            musicList2.MusicPlayList.Items.RemoveAt(idx);
            musicList2.MusicPlayList = mixPlayList;
        }

        private void buttonClearMix_Click(object sender, EventArgs e)
        {
            if (Player == null) return;
            if (!(Player is Enhance.RealtimeStreamPlayer)) return;
            Enhance.RealtimeStreamPlayer realtimePlayer = (Enhance.RealtimeStreamPlayer)Player;
            List<MusicListItem> items = new List<MusicListItem>();
            for(int i=0; i< musicList2.MusicPlayList.Items.Count;++i)
            {
                MusicListItem item = musicList2.MusicPlayList.Items[i];
                items.Add(item);
            }
            foreach (MusicListItem item in items)
            {
                realtimePlayer.RemoveMP3File(item.FileFullPath);
            }
            musicList2.MusicPlayList.Items.Clear();
            musicList2.MusicPlayList = mixPlayList;
        }
    }
}
