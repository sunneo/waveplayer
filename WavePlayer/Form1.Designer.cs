namespace WavePlayer
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.flowLayoutPanel1 = new System.Windows.Forms.Panel();
            this.trackBar1 = new WavePlayer.UI.TrackBar();
            this.buttonTempoDown = new WavePlayer.UI.DoubleBufferedButton();
            this.buttonPitchDown = new WavePlayer.UI.DoubleBufferedButton();
            this.buttonBackward = new WavePlayer.UI.DoubleBufferedButton();
            this.checkBoxRepeat = new System.Windows.Forms.Button();
            this.buttonNextSong = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStartPause = new WavePlayer.UI.DoubleBufferedButton();
            this.buttonTempoUp = new WavePlayer.UI.DoubleBufferedButton();
            this.buttonPitchUp = new WavePlayer.UI.DoubleBufferedButton();
            this.buttonForward = new WavePlayer.UI.DoubleBufferedButton();
            this.labelRemain = new System.Windows.Forms.Label();
            this.labelElapsed = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new WavePlayer.UI.TrackBar();
            this.tabControl1 = new WavePlayer.UI.DoubleBufferedTabControl();
            this.tabPage1 = new WavePlayer.UI.DoubleBufferedTabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelLyric = new System.Windows.Forms.Label();
            this.tabPage2 = new WavePlayer.UI.DoubleBufferedTabPage();
            this.musicList1 = new WavePlayer.UI.MusicList();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.doubleBufferedSplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ListViewRemoteNode = new WavePlayer.UI.DoubleBufferedListView();
            this.columnHeaderServerNode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.musicListRemote = new WavePlayer.UI.MusicList();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.doubleBufferedPanel1 = new Utilities.UI.DoubleBufferedPanel();
            this.musicList2 = new WavePlayer.UI.MusicList();
            this.buttonAddMix = new Utilities.UI.BorderLessButton();
            this.buttonDeleteMix = new Utilities.UI.BorderLessButton();
            this.buttonClearMix = new Utilities.UI.BorderLessButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doubleBufferedSplitContainer1)).BeginInit();
            this.doubleBufferedSplitContainer1.Panel1.SuspendLayout();
            this.doubleBufferedSplitContainer1.Panel2.SuspendLayout();
            this.doubleBufferedSplitContainer1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.doubleBufferedPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.trackBar1);
            this.flowLayoutPanel1.Controls.Add(this.buttonTempoDown);
            this.flowLayoutPanel1.Controls.Add(this.buttonPitchDown);
            this.flowLayoutPanel1.Controls.Add(this.buttonBackward);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxRepeat);
            this.flowLayoutPanel1.Controls.Add(this.buttonNextSong);
            this.flowLayoutPanel1.Controls.Add(this.buttonStop);
            this.flowLayoutPanel1.Controls.Add(this.buttonStartPause);
            this.flowLayoutPanel1.Controls.Add(this.buttonTempoUp);
            this.flowLayoutPanel1.Controls.Add(this.buttonPitchUp);
            this.flowLayoutPanel1.Controls.Add(this.buttonForward);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(21, 50);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(523, 37);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBar1.Location = new System.Drawing.Point(379, 17);
            this.trackBar1.Maximum = 100;
            this.trackBar1.MaximumSize = new System.Drawing.Size(100, 20);
            this.trackBar1.Min = 0;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.ShowThumb = false;
            this.trackBar1.Size = new System.Drawing.Size(100, 17);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.TrackBarBackground = System.Drawing.Color.DimGray;
            this.trackBar1.TrackBarForeColor = System.Drawing.Color.LightCyan;
            this.trackBar1.Value = 100;
            this.trackBar1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseClick);
            this.trackBar1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseDown);
            this.trackBar1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseMove);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // buttonTempoDown
            // 
            this.buttonTempoDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTempoDown.BackColor = System.Drawing.Color.Transparent;
            this.buttonTempoDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonTempoDown.FlatAppearance.BorderSize = 0;
            this.buttonTempoDown.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonTempoDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonTempoDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonTempoDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTempoDown.ForeColor = System.Drawing.Color.Black;
            this.buttonTempoDown.Image = global::WavePlayer.Properties.Resources.tempoDown;
            this.buttonTempoDown.Location = new System.Drawing.Point(291, 0);
            this.buttonTempoDown.Margin = new System.Windows.Forms.Padding(0);
            this.buttonTempoDown.Name = "buttonTempoDown";
            this.buttonTempoDown.Size = new System.Drawing.Size(32, 41);
            this.buttonTempoDown.TabIndex = 0;
            this.buttonTempoDown.UseVisualStyleBackColor = false;
            this.buttonTempoDown.Click += new System.EventHandler(this.buttonTempoDown_Click);
            // 
            // buttonPitchDown
            // 
            this.buttonPitchDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPitchDown.BackColor = System.Drawing.Color.Transparent;
            this.buttonPitchDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonPitchDown.FlatAppearance.BorderSize = 0;
            this.buttonPitchDown.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonPitchDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonPitchDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonPitchDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPitchDown.ForeColor = System.Drawing.Color.Black;
            this.buttonPitchDown.Image = global::WavePlayer.Properties.Resources.Keydown;
            this.buttonPitchDown.Location = new System.Drawing.Point(205, 0);
            this.buttonPitchDown.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPitchDown.Name = "buttonPitchDown";
            this.buttonPitchDown.Size = new System.Drawing.Size(32, 41);
            this.buttonPitchDown.TabIndex = 0;
            this.buttonPitchDown.UseVisualStyleBackColor = false;
            this.buttonPitchDown.Click += new System.EventHandler(this.buttonPitchDown_Click);
            // 
            // buttonBackward
            // 
            this.buttonBackward.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBackward.BackColor = System.Drawing.Color.Transparent;
            this.buttonBackward.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonBackward.FlatAppearance.BorderSize = 0;
            this.buttonBackward.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonBackward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonBackward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonBackward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBackward.ForeColor = System.Drawing.Color.Black;
            this.buttonBackward.Image = global::WavePlayer.Properties.Resources.backward;
            this.buttonBackward.Location = new System.Drawing.Point(0, 0);
            this.buttonBackward.Margin = new System.Windows.Forms.Padding(0);
            this.buttonBackward.Name = "buttonBackward";
            this.buttonBackward.Size = new System.Drawing.Size(32, 41);
            this.buttonBackward.TabIndex = 0;
            this.buttonBackward.UseVisualStyleBackColor = false;
            // 
            // checkBoxRepeat
            // 
            this.checkBoxRepeat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxRepeat.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxRepeat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxRepeat.FlatAppearance.BorderSize = 0;
            this.checkBoxRepeat.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBoxRepeat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.checkBoxRepeat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.checkBoxRepeat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxRepeat.ForeColor = System.Drawing.Color.Black;
            this.checkBoxRepeat.Image = global::WavePlayer.Properties.Resources.repeat;
            this.checkBoxRepeat.Location = new System.Drawing.Point(491, 0);
            this.checkBoxRepeat.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxRepeat.Name = "checkBoxRepeat";
            this.checkBoxRepeat.Size = new System.Drawing.Size(32, 41);
            this.checkBoxRepeat.TabIndex = 0;
            this.checkBoxRepeat.UseVisualStyleBackColor = false;
            this.checkBoxRepeat.Click += new System.EventHandler(this.checkBoxRepeat_CheckedChanged);
            // 
            // buttonNextSong
            // 
            this.buttonNextSong.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNextSong.BackColor = System.Drawing.Color.Transparent;
            this.buttonNextSong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonNextSong.FlatAppearance.BorderSize = 0;
            this.buttonNextSong.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonNextSong.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonNextSong.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonNextSong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNextSong.ForeColor = System.Drawing.Color.Black;
            this.buttonNextSong.Image = global::WavePlayer.Properties.Resources.nextsong;
            this.buttonNextSong.Location = new System.Drawing.Point(160, 0);
            this.buttonNextSong.Margin = new System.Windows.Forms.Padding(0);
            this.buttonNextSong.Name = "buttonNextSong";
            this.buttonNextSong.Size = new System.Drawing.Size(32, 41);
            this.buttonNextSong.TabIndex = 0;
            this.buttonNextSong.UseVisualStyleBackColor = false;
            this.buttonNextSong.Click += new System.EventHandler(this.buttonNextSong_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.BackColor = System.Drawing.Color.Transparent;
            this.buttonStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonStop.FlatAppearance.BorderSize = 0;
            this.buttonStop.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStop.ForeColor = System.Drawing.Color.Black;
            this.buttonStop.Image = global::WavePlayer.Properties.Resources.stop;
            this.buttonStop.Location = new System.Drawing.Point(116, 0);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(0);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(32, 41);
            this.buttonStop.TabIndex = 0;
            this.buttonStop.UseVisualStyleBackColor = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonStartPause
            // 
            this.buttonStartPause.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStartPause.BackColor = System.Drawing.Color.Transparent;
            this.buttonStartPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonStartPause.FlatAppearance.BorderSize = 0;
            this.buttonStartPause.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonStartPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonStartPause.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonStartPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStartPause.ForeColor = System.Drawing.Color.Black;
            this.buttonStartPause.Image = global::WavePlayer.Properties.Resources.play;
            this.buttonStartPause.Location = new System.Drawing.Point(40, 0);
            this.buttonStartPause.Margin = new System.Windows.Forms.Padding(0);
            this.buttonStartPause.Name = "buttonStartPause";
            this.buttonStartPause.Size = new System.Drawing.Size(32, 41);
            this.buttonStartPause.TabIndex = 0;
            this.buttonStartPause.UseVisualStyleBackColor = false;
            this.buttonStartPause.Click += new System.EventHandler(this.buttonStartPause_Click);
            // 
            // buttonTempoUp
            // 
            this.buttonTempoUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTempoUp.BackColor = System.Drawing.Color.Transparent;
            this.buttonTempoUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonTempoUp.FlatAppearance.BorderSize = 0;
            this.buttonTempoUp.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonTempoUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonTempoUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonTempoUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTempoUp.ForeColor = System.Drawing.Color.Black;
            this.buttonTempoUp.Image = global::WavePlayer.Properties.Resources.tempoUp;
            this.buttonTempoUp.Location = new System.Drawing.Point(333, 0);
            this.buttonTempoUp.Margin = new System.Windows.Forms.Padding(0);
            this.buttonTempoUp.Name = "buttonTempoUp";
            this.buttonTempoUp.Size = new System.Drawing.Size(32, 41);
            this.buttonTempoUp.TabIndex = 0;
            this.buttonTempoUp.UseVisualStyleBackColor = false;
            this.buttonTempoUp.Click += new System.EventHandler(this.buttonTempoUp_Click);
            // 
            // buttonPitchUp
            // 
            this.buttonPitchUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPitchUp.BackColor = System.Drawing.Color.Transparent;
            this.buttonPitchUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonPitchUp.FlatAppearance.BorderSize = 0;
            this.buttonPitchUp.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonPitchUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonPitchUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonPitchUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPitchUp.ForeColor = System.Drawing.Color.Black;
            this.buttonPitchUp.Image = global::WavePlayer.Properties.Resources.Keyup;
            this.buttonPitchUp.Location = new System.Drawing.Point(247, 0);
            this.buttonPitchUp.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPitchUp.Name = "buttonPitchUp";
            this.buttonPitchUp.Size = new System.Drawing.Size(32, 41);
            this.buttonPitchUp.TabIndex = 0;
            this.buttonPitchUp.UseVisualStyleBackColor = false;
            this.buttonPitchUp.Click += new System.EventHandler(this.buttonPitchUp_Click);
            // 
            // buttonForward
            // 
            this.buttonForward.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonForward.BackColor = System.Drawing.Color.Transparent;
            this.buttonForward.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonForward.FlatAppearance.BorderSize = 0;
            this.buttonForward.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonForward.ForeColor = System.Drawing.Color.Black;
            this.buttonForward.Image = global::WavePlayer.Properties.Resources.fastforward;
            this.buttonForward.Location = new System.Drawing.Point(78, 0);
            this.buttonForward.Margin = new System.Windows.Forms.Padding(0);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(32, 41);
            this.buttonForward.TabIndex = 0;
            this.buttonForward.UseVisualStyleBackColor = false;
            // 
            // labelRemain
            // 
            this.labelRemain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRemain.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelRemain.Location = new System.Drawing.Point(482, 10);
            this.labelRemain.Name = "labelRemain";
            this.labelRemain.Size = new System.Drawing.Size(71, 14);
            this.labelRemain.TabIndex = 2;
            this.labelRemain.Text = "00:00";
            this.labelRemain.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelElapsed
            // 
            this.labelElapsed.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelElapsed.Location = new System.Drawing.Point(7, 10);
            this.labelElapsed.Name = "labelElapsed";
            this.labelElapsed.Size = new System.Drawing.Size(86, 14);
            this.labelElapsed.TabIndex = 2;
            this.labelElapsed.Text = "00:00";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(556, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem2});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(106, 22);
            this.toolStripMenuItem2.Text = "&Exit";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.labelElapsed);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.labelRemain);
            this.panel1.Location = new System.Drawing.Point(0, 380);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 100);
            this.panel1.TabIndex = 4;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.progressBar1.Location = new System.Drawing.Point(7, 28);
            this.progressBar1.Maximum = 100;
            this.progressBar1.Min = 0;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.ShowThumb = true;
            this.progressBar1.Size = new System.Drawing.Size(546, 19);
            this.progressBar1.TabIndex = 1;
            this.progressBar1.TrackBarBackground = System.Drawing.Color.Gray;
            this.progressBar1.TrackBarForeColor = System.Drawing.Color.Cyan;
            this.progressBar1.Value = 0;
            this.progressBar1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.progressBar1_MouseClick);
            this.progressBar1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.progressBar1_MouseDown);
            this.progressBar1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.progressBar1_MouseMove);
            this.progressBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.progressBar1_MouseUp);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 31);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(553, 345);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Black;
            this.tabPage1.BackgroundImage = global::WavePlayer.Properties.Resources.bluegradient;
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.labelTitle);
            this.tabPage1.Controls.Add(this.labelLyric);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(545, 318);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Playing";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(57, 37);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(430, 240);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(3, 3);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(539, 30);
            this.labelTitle.TabIndex = 5;
            // 
            // labelLyric
            // 
            this.labelLyric.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLyric.BackColor = System.Drawing.Color.Transparent;
            this.labelLyric.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelLyric.ForeColor = System.Drawing.Color.White;
            this.labelLyric.Location = new System.Drawing.Point(3, 281);
            this.labelLyric.Name = "labelLyric";
            this.labelLyric.Size = new System.Drawing.Size(523, 30);
            this.labelLyric.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.musicList1);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(545, 318);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // musicList1
            // 
            this.musicList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.musicList1.Location = new System.Drawing.Point(3, 38);
            this.musicList1.MusicPlayList = null;
            this.musicList1.Name = "musicList1";
            this.musicList1.ShowLocation = true;
            this.musicList1.Size = new System.Drawing.Size(539, 277);
            this.musicList1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxSearch);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(539, 35);
            this.panel2.TabIndex = 1;
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(62, 6);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(466, 22);
            this.textBoxSearch.TabIndex = 1;
            this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
            this.textBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.doubleBufferedSplitContainer1);
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(545, 318);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Network";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // doubleBufferedSplitContainer1
            // 
            this.doubleBufferedSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferedSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.doubleBufferedSplitContainer1.Name = "doubleBufferedSplitContainer1";
            // 
            // doubleBufferedSplitContainer1.Panel1
            // 
            this.doubleBufferedSplitContainer1.Panel1.Controls.Add(this.ListViewRemoteNode);
            // 
            // doubleBufferedSplitContainer1.Panel2
            // 
            this.doubleBufferedSplitContainer1.Panel2.Controls.Add(this.musicListRemote);
            this.doubleBufferedSplitContainer1.Size = new System.Drawing.Size(545, 318);
            this.doubleBufferedSplitContainer1.SplitterDistance = 181;
            this.doubleBufferedSplitContainer1.TabIndex = 0;
            // 
            // ListViewRemoteNode
            // 
            this.ListViewRemoteNode.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderServerNode});
            this.ListViewRemoteNode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListViewRemoteNode.FullRowSelect = true;
            this.ListViewRemoteNode.GridLines = true;
            this.ListViewRemoteNode.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListViewRemoteNode.HideSelection = false;
            this.ListViewRemoteNode.Location = new System.Drawing.Point(0, 0);
            this.ListViewRemoteNode.MultiSelect = false;
            this.ListViewRemoteNode.Name = "ListViewRemoteNode";
            this.ListViewRemoteNode.Size = new System.Drawing.Size(181, 318);
            this.ListViewRemoteNode.TabIndex = 0;
            this.ListViewRemoteNode.UseCompatibleStateImageBehavior = false;
            this.ListViewRemoteNode.View = System.Windows.Forms.View.Details;
            this.ListViewRemoteNode.SelectedIndexChanged += new System.EventHandler(this.ListViewRemoteNode_SelectedIndexChanged);
            this.ListViewRemoteNode.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListViewRemoteNode_MouseDoubleClick);
            // 
            // columnHeaderServerNode
            // 
            this.columnHeaderServerNode.Text = "Node";
            this.columnHeaderServerNode.Width = 177;
            // 
            // musicListRemote
            // 
            this.musicListRemote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.musicListRemote.Location = new System.Drawing.Point(0, 0);
            this.musicListRemote.MusicPlayList = null;
            this.musicListRemote.Name = "musicListRemote";
            this.musicListRemote.ShowLocation = false;
            this.musicListRemote.Size = new System.Drawing.Size(360, 318);
            this.musicListRemote.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.doubleBufferedPanel1);
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(545, 318);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "MixMusics";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // doubleBufferedPanel1
            // 
            this.doubleBufferedPanel1.Controls.Add(this.buttonClearMix);
            this.doubleBufferedPanel1.Controls.Add(this.buttonDeleteMix);
            this.doubleBufferedPanel1.Controls.Add(this.buttonAddMix);
            this.doubleBufferedPanel1.Controls.Add(this.musicList2);
            this.doubleBufferedPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferedPanel1.Location = new System.Drawing.Point(0, 0);
            this.doubleBufferedPanel1.Name = "doubleBufferedPanel1";
            this.doubleBufferedPanel1.Size = new System.Drawing.Size(545, 318);
            this.doubleBufferedPanel1.TabIndex = 0;
            // 
            // musicList2
            // 
            this.musicList2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.musicList2.Location = new System.Drawing.Point(3, 42);
            this.musicList2.MusicPlayList = null;
            this.musicList2.Name = "musicList2";
            this.musicList2.ShowLocation = true;
            this.musicList2.Size = new System.Drawing.Size(539, 278);
            this.musicList2.TabIndex = 1;
            // 
            // buttonAddMix
            // 
            this.buttonAddMix.Location = new System.Drawing.Point(3, 13);
            this.buttonAddMix.Name = "buttonAddMix";
            this.buttonAddMix.Size = new System.Drawing.Size(75, 23);
            this.buttonAddMix.TabIndex = 2;
            this.buttonAddMix.Text = "Add";
            this.buttonAddMix.UseVisualStyleBackColor = true;
            this.buttonAddMix.Click += new System.EventHandler(this.buttonAddMix_Click);
            // 
            // buttonDeleteMix
            // 
            this.buttonDeleteMix.Location = new System.Drawing.Point(84, 13);
            this.buttonDeleteMix.Name = "buttonDeleteMix";
            this.buttonDeleteMix.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteMix.TabIndex = 2;
            this.buttonDeleteMix.Text = "Delete";
            this.buttonDeleteMix.UseVisualStyleBackColor = true;
            this.buttonDeleteMix.Click += new System.EventHandler(this.buttonDeleteMix_Click);
            // 
            // buttonClearMix
            // 
            this.buttonClearMix.Location = new System.Drawing.Point(165, 13);
            this.buttonClearMix.Name = "buttonClearMix";
            this.buttonClearMix.Size = new System.Drawing.Size(75, 23);
            this.buttonClearMix.TabIndex = 2;
            this.buttonClearMix.Text = "Clear";
            this.buttonClearMix.UseVisualStyleBackColor = true;
            this.buttonClearMix.Click += new System.EventHandler(this.buttonClearMix_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(556, 482);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Wave Player";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.doubleBufferedSplitContainer1.Panel1.ResumeLayout(false);
            this.doubleBufferedSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.doubleBufferedSplitContainer1)).EndInit();
            this.doubleBufferedSplitContainer1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.doubleBufferedPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel flowLayoutPanel1;
        private UI.DoubleBufferedButton buttonBackward;
        private UI.DoubleBufferedButton buttonStartPause;
        private UI.DoubleBufferedButton buttonForward;
        private UI.TrackBar progressBar1;
        private System.Windows.Forms.Label labelRemain;
        private System.Windows.Forms.Label labelElapsed;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button checkBoxRepeat;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelLyric;
        private UI.DoubleBufferedTabControl tabControl1;
        private UI.DoubleBufferedTabPage tabPage1;
        private UI.DoubleBufferedTabPage tabPage2;
        private UI.MusicList musicList1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UI.DoubleBufferedButton buttonPitchDown;
        private UI.DoubleBufferedButton buttonPitchUp;
        private UI.DoubleBufferedButton buttonTempoDown;
        private UI.DoubleBufferedButton buttonTempoUp;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.SplitContainer doubleBufferedSplitContainer1;
        private UI.DoubleBufferedListView ListViewRemoteNode;
        private UI.MusicList musicListRemote;
        private System.Windows.Forms.ColumnHeader columnHeaderServerNode;
        private System.Windows.Forms.Button buttonNextSong;
        private UI.TrackBar trackBar1;
        private System.Windows.Forms.TabPage tabPage4;
        private Utilities.UI.DoubleBufferedPanel doubleBufferedPanel1;
        private Utilities.UI.BorderLessButton buttonClearMix;
        private Utilities.UI.BorderLessButton buttonDeleteMix;
        private Utilities.UI.BorderLessButton buttonAddMix;
        private UI.MusicList musicList2;
    }
}

