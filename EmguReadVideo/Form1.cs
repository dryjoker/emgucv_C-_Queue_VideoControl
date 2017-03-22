using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EmguReadVideo
{
    public partial class Form1 : Form
    {
        //Emgucv3.1 官方字典使用說明書Link:
        // http://www.emgu.com/wiki/index.php/Version_History#Emgu.CV-3.1.0
        // http://www.emgu.com/wiki/files/3.1.0/document/html/8dee1f02-8c8a-4e37-87f4-05e10c39f27d.htm

        # region variable 
        Mat currentFrame;
        Capture grabber;
        int videoFps = 0;//frame rate
        int frameNumber=0;
        bool isJump = false;
        #endregion

        //C# Queue 官方使用 link:
        //https://msdn.microsoft.com/en-us/library/7977ey2c(v=vs.110).aspx
        //
        //Stackoverflow上有人曾經將Queue跟emgucv結合
        //http://stackoverflow.com/questions/11612046/parallel-image-processing-artifacts
        //

        #region datastructure variable
        Queue<Mat> inQueue = new Queue<Mat>(); //物件尚未實體化就呼叫它的屬性或方法  記得先實體化
        //inQueue 是 global variable 
        #endregion

        

        public Form1()
        {
            InitializeComponent();
        }



        private void btnRead_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "3gpp文件|*.3gpp|MP4文件|*.mp4|AVI文件|*.avi|RMVB文件|*.rmvb|WMV文件|*.wmv|MKV文件|*.mkv|所有文件|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                grabber = new Capture(openFileDialog.FileName);
                Application.Idle += new EventHandler(updateFrame);
                //新版的3.x  記得預設為double 必須 使用  cast運算
                //videoFps = (int)CvInvoke.cveVideoCaptureGet(grabber, CapProp.Fps);

                //舊版的2.x以前 在 新版的已經不適用
                //videoFps = (int)CvInvoke.cvGetCaptureProperty(grabber, Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
            }
        }

        private void updateFrame(object sender, EventArgs e)
        {
            currentFrame = grabber.QueryFrame();
            grabber.Stop();
            if (currentFrame != null)
            {
                grabber.Stop();
                inQueue.Enqueue(currentFrame);
                label2.Text = inQueue.Count.ToString();
                //label4.Text = videoFps.ToString();
                label4.Text = CvInvoke.cveVideoCaptureGet(grabber, CapProp.Fps).ToString();
                pictureBox1.Image = currentFrame.ToImage<Bgr, byte>().ToBitmap();
                
                /*
                int i = 0;
                //存影格用的   可以存下檢查  Queue 中  是否有影格
                foreach (Mat img in inQueue)
                {
                    i++;
                    try
                    {
                        img.Save(@"C:\frame\img" + i.ToString() + ".jpg");
                        //為了避免出現 Additional information: 在 GDI+ 中發生泛型錯誤。 加上一組try... catch...
                        //錯誤說明link : https://dotblogs.com.tw/atowngit/2010/01/13/13003
                    }
                    catch
                    {

                    }
                }
                */
            }
            if (isJump)
            {
                currentFrame = inQueue.ElementAt(frameNumber);
                pictureBox1.Image = currentFrame.ToImage<Bgr, byte>().ToBitmap();
            }
        }

        private void btnQueueClear_Click(object sender, EventArgs e)
        {
            inQueue.Clear();
            label2.Text = inQueue.Count.ToString();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(updateFrame);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(updateFrame);
        }

        private void btnJump_Click(object sender, EventArgs e)
        {
            frameNumber = int.Parse(textBox1.Text);
            //inQueue.ElementAt(frameNumber);
            isJump = true;
        }
    }
}
