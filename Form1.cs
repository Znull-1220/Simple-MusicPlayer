using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MusicPlayer
{
    public partial class Form1 : Form
    {
        /* AppDomain.CurrentDomain.BaseDirectory获取到的是当前解决方案
         * 下的bin/debug目录获取解决方案的目录需要获取两次父目录. */
        static string basePath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
        
        // 
        string playingImagePath = Path.Combine(basePath, "pictures\\CD_rotating.jpg");
        string stopImagePath = Path.Combine(basePath, "pictures\\CD_static.jpg");
        string muteImagePath = Path.Combine(basePath, "pictures\\mute.jpg");
        string unmuteImagePath = Path.Combine(basePath, "pictures\\unmute.jpg");
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //程序加载时关闭自动播放功能
            MusicPlayer.settings.autoStart = false;
            pictureBox2.Image = Image.FromFile(stopImagePath);
        }

        bool play = true;
        private void PlayorPause_Click(object sender, EventArgs e)
        {
            if(PlayorPause.Text == "播放")
            {
                if(play)
                {   //获得选中的歌曲,让音乐从头播放
                    try
                    {
                        MusicPlayer.URL = listpath[listBox1.SelectedIndex];
                    }
                    catch{

                        MessageBox.Show("音乐未载入!");
                    }
                    
                }              
                MusicPlayer.Ctlcontrols.play();
                IsExistLrc(listpath[listBox1.SelectedIndex]);
                // 如绝对路径, 可以用 @"C:\\PATH\\..."
                pictureBox2.Image = Image.FromFile(playingImagePath);
                PlayorPause.Text = "暂停";
            }
            else if (PlayorPause.Text == "暂停")
            {
                MusicPlayer.Ctlcontrols.pause();
                pictureBox2.Image = Image.FromFile(stopImagePath);
                PlayorPause.Text = "播放";
                play=false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MusicPlayer.Ctlcontrols.stop();
            pictureBox2.Image = Image.FromFile(stopImagePath);
        }

        //存储音乐文件的全路径
        List<string>listpath = new List<string>();

        private void button2_Click(object sender, EventArgs e)
            //打开对话框
        {
            OpenFileDialog ofd = new OpenFileDialog();
            // 自定义启动路径
            ofd.InitialDirectory = @"C:\\YOUR\\PATH\\";
            ofd.Filter = "音乐文件|*.wav|MP3文件|*.mp3|所有文件|*.*";
            ofd.Title = "请选择音乐文件";
            ofd.Multiselect = true;
            ofd.ShowDialog();

            //获得文本框中选择文件的全路径
            string[] path = ofd.FileNames;
            for(int i = 0; i < path.Length; i++)
            {
                listpath.Add(path[i]);//将音乐文件的全路径存储到泛型集合中

                //将音乐文件的文件名存储到ListBox
                listBox1.Items.Add(Path.GetFileName(path[i]));
            }

        }

        //双击播放对应音乐
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {           
            try
            { 
            MusicPlayer.URL = listpath[listBox1.SelectedIndex];
            MusicPlayer.Ctlcontrols.play();
            PlayorPause.Text = "暂停";
            pictureBox2.Image = Image.FromFile(playingImagePath);
            IsExistLrc(listpath[listBox1.SelectedIndex]);
            }
            catch { }

        }


        //点击下一曲
        private void button5_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            //清空所有选中项的索引
            listBox1.SelectedIndices.Clear();
            index++;
            //最后一首歌返回第一首
            if(index == listBox1.Items.Count)
            {
                index=0;
            }
            //改变后的索引重新赋值给当前选中项的索引
            listBox1.SelectedIndex = index;
            MusicPlayer.URL = listpath[index];
            IsExistLrc(listpath[index]);
            MusicPlayer.Ctlcontrols.play();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //获得当前选中项的索引
            int index = listBox1.SelectedIndex;
            //清空所有选中项的索引
            listBox1.SelectedIndices.Clear();
            index--;
            //最后一首歌返回第一首
            if (index < 0)
            {
                index = listBox1.Items.Count-1;
            }
            //改变后的索引重新赋值给当前选中项的索引
            listBox1.SelectedIndex = index;
            MusicPlayer.URL = listpath[index];
            IsExistLrc(listpath[index]);
            MusicPlayer.Ctlcontrols.play();
        }


        //点击删除选中项删除
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //删除列表中的选中项
            
            //首先获得要删除的歌曲数量
            int count = listBox1.SelectedItems.Count;
            for(int i = 0; i < count; i++)
            {
                //先删集合
                listpath.RemoveAt(listBox1.SelectedIndex);
                //再删列表
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);

            }
        }


        //点击放音或静音
        private void label1_Click(object sender, EventArgs e)
        {
            if(label1.Tag.ToString() == "1")
            {
                //静音
                MusicPlayer.settings.mute = true;
                label1.Image = Image.FromFile(muteImagePath);
                label1.Tag = "2";
                

            }
            else if(label1.Tag.ToString() == "2")
            {
                //放音
                MusicPlayer.settings.mute = false;
                label1.Image = Image.FromFile(unmuteImagePath);
                label1.Tag = "1";
            }
        }


        //增大音量
        private void button6_Click(object sender, EventArgs e)
        {
            MusicPlayer.settings.volume += 5;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MusicPlayer.settings.volume -= 5;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //如果播放器正在播放中
            if(MusicPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                Information.Text =  "当前播放时间：" + MusicPlayer.Ctlcontrols.currentPositionString+ "\r\n" + "歌曲总时间：" + MusicPlayer.currentMedia.durationString;

                double t1 = double.Parse(MusicPlayer.currentMedia.duration.ToString());
                double t2 = double.Parse(MusicPlayer.Ctlcontrols.currentPosition.ToString()) + 1;

                //如果当前播放时间等于歌曲总时间，则播放下一曲
                if (t1 <= t2)
                {
                    int index = listBox1.SelectedIndex;
                    //清空所有选中项的索引
                    listBox1.SelectedIndices.Clear();
                    index++;
                    //最后一首歌返回第一首
                    if (index == listBox1.Items.Count)
                    {
                        index = 0;
                    }
                    //改变后的索引重新赋值给当前选中项的索引
                    listBox1.SelectedIndex = index;
                    MusicPlayer.URL = listpath[index];
                    MusicPlayer.Ctlcontrols.play();
                }
            }

        }


        //存储时间
        List<double> listTime = new List<double>();
        List<string> listLrcText = new List<string>();
        //歌词
        void IsExistLrc(string songPath)
        {
            //清空两个集合的内容
            listTime.Clear();
            listLrcText.Clear();

            songPath += ".lrc";
            if(File.Exists(songPath))
            {
                string[] LrcText = File.ReadAllLines(songPath);
                //格式化歌词
                FormatLrc(LrcText);
            }
            //不存在歌词时
            else
            {
                label2.Text = "--------未找到歌词--------";
            }
        }




        //格式化歌词
        void FormatLrc(string[] LrcText)
        {
            for(int i = 0; i < LrcText.Length; i++)
            {
                string[] LrcTemp = LrcText[i].Split(new char[] {'[',']'}, StringSplitOptions.RemoveEmptyEntries);
                string[] lrcNewTemp = LrcTemp[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                double time = double.Parse(lrcNewTemp[0]) * 60 + double.Parse(lrcNewTemp[1]);
                //把截取出来的时间加入泛型集合
                listTime.Add(time);
                //这个时间对应的歌词存储到泛型集合中
                listLrcText.Add(LrcTemp[1]);
            }
        }


        //播放歌词
        private void timer2_Tick(object sender, EventArgs e)
        {
            for(int j = 0; j < listTime.Count; j++)
            {
                if(j == listTime.Count-1)  
                    break;

                if ((MusicPlayer.Ctlcontrols.currentPosition >= listTime[j]) && (MusicPlayer.Ctlcontrols.currentPosition < listTime[j+1]))
                {
                    label2.Text = listLrcText[j];
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }



        private void pictureBox2_Click(object sender, EventArgs e)
        {
           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void MusicPlayer_Enter(object sender, EventArgs e)
        {

        }
    }
}
