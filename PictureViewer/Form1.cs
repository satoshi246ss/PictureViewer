using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Blob;
using VideoInputSharp;
using System.Diagnostics; 
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;

namespace PictureViewer
{
    public partial class Form1 : Form
    {
        //状態を表す定数
        const int TRUE  = 0;
        const int FALSE = 1;
        //上の2つ状態を保持します
        int ImgSaveFlag = FALSE ;

        //状態を表す定数
        const int STOP  = 0;
        const int START = 1;
        const int SAVE  = 2;  // MT3導入動作中
        //上の2つ状態を保持します
        int States = 0;

        int MT3Status = STOP;


        //状態を表す定数
        const int LOST        = 0;
        const int FISH_DETECT = 1;  // Fisheye等他で検出
        const int MyDETECT    = 2;  // 自分で検出
        const int PID_TEST    = 3;
        //上の2つ状態を保持します
        int FishMode     = LOST ;
        int PvMode    = LOST;

        int DarkMode = FALSE;

        // 時刻基準（BCB互換）
        DateTime TBASE = new DateTime(1899, 12, 30, 0, 0, 0);

        // メイン装置光軸座標
        int xoa_mes = 320 ;
        int yoa_mes = 240 ;
        int roa =  5;   //中心マーク長さ
        int row = 60;   //width  半値幅
        int roh = 40;   //Height 半値幅
        double xoad, yoad;

        int xoa = 334;
        int yoa = 223;
        // test
        double test_start_id = 0;
        double xoa_test_start = 70;
        double yoa_test_start = 70;
        double xoa_test_step = 1;
        double yoa_test_step = 1;

        // 観測開始からのフレーム番号
        int id = 0;

        // 検出条件
        const int Low_Limit = 40;
        double gx, gy, max_area;

        const int MaxFrame = 256;
        const int WIDTH    = 640;
        const int HEIGHT   = 480 ;
        ImageData imgdata  = new ImageData(WIDTH, HEIGHT);
        CircularBuffer fifo = new CircularBuffer(MaxFrame,WIDTH,HEIGHT);

        private BackgroundWorker worker;
        private BackgroundWorker worker_udp;

        IplImage imgLabel = new IplImage(WIDTH, HEIGHT, CvBlobLib.DepthLabel, 1);
        CvBlobs blobs = new CvBlobs();
        int threshold_blob = 64; // 検出閾値（０－２５５）
        double threshold_min_area = 0.25; // 最小エリア閾値（最大値ｘ0.25)
        CvPoint2D64f max_centroid;
        uint max_label;
        CvBlob maxBlob;
        double distance, distance_min, d_val;

        FSI_PID_DATA pid_data = new FSI_PID_DATA();
        MT_MONITOR_DATA mtmon_data = new MT_MONITOR_DATA();
        int mmFsiUdpPortMT3PV     = 24404;            // MT3PV  （受信）
        int mmFsiUdpPortMT3PVs    = 24405;            // MT3PVS （送信）
        int mmFsiUdpPortMTmonitor = 24415;
        string mmFsiCore_i5 = "192.168.1.211";
        int mmFsiUdpPortSpCam = 24410;   // SpCam（受信）
        string mmFsiSC440   = "192.168.1.206";
        System.Net.Sockets.UdpClient udpc3 = null;
         DriveInfo cDrive = new DriveInfo("C");
        long diskspace;

        public Form1()
        {
            InitializeComponent();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            worker_udp = new BackgroundWorker();
            worker_udp.WorkerReportsProgress = true;
            worker_udp.WorkerSupportsCancellation = true;
            worker_udp.DoWork += new DoWorkEventHandler(worker_udp_DoWork);
            worker_udp.ProgressChanged += new ProgressChangedEventHandler(worker_udp_ProgressChanged);
            Pid_Data_Send_Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.worker_udp.RunWorkerAsync();

        }

        #region UDP
        // 別スレッド処理（UDP）
        private void worker_udp_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;

            //バインドするローカルポート番号
            int localPort = 24404;
            System.Net.Sockets.UdpClient udpc = null; ;
            try
            {
                udpc = new System.Net.Sockets.UdpClient(localPort);

            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }


            //文字コードを指定する
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //データを送信するリモートホストとポート番号
            //string remoteHost = "localhost";
         //   string remoteHost = "192.168.1.204";
         //   int remotePort = 24404;
            //送信するデータを読み込む
            string sendMsg = "test送信するデータ";
            byte[] sendBytes = enc.GetBytes(sendMsg);
            
            //リモートホストを指定してデータを送信する
           // udpc.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);


            string str;
            MOTOR_DATA_KV_SP kmd3 = new MOTOR_DATA_KV_SP();
            int size = Marshal.SizeOf(kmd3);

            //データを受信する
            System.Net.IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, localPort);
            while (bw.CancellationPending == false)
            {
                byte[] rcvBytes = udpc.Receive(ref remoteEP);
                if (rcvBytes.Length == size)
                {
                    kmd3 = ToStruct(rcvBytes);
                    if (kmd3.cmd == 1) //mmMove:1
                    {
                        FishMode = FISH_DETECT;
                        MT3Status = SAVE;
                        this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveMainTime, START });
                        //timerSaveMainTime.Start();
                        buttonSave_Click(sender, e) ;
                    } else
                    if (kmd3.cmd == 16) //mmLost:16
                    {
                        FishMode = LOST;
                        //ButtonSaveEnd_Click(sender, e);
                        //匿名デリゲートで表示する
                        this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveMainTime, STOP });
                        this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSavePostTime, START });
                        //timerSavePostTime.Start();                   
                    } else
                    if (kmd3.cmd == 17) //mmMoveEnd:17
                    {
                        MT3Status = STOP ;
                    }

                    str = "受信したデータ(kmd3):" + kmd3.cmd + ":" + kmd3.t + ":" +kmd3.az + "\n";
                    this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
                    bw.ReportProgress(0, kmd3);
                }
                else
                {
                    string rcvMsg = enc.GetString(rcvBytes);
                    str = "受信したデータ:[" +rcvMsg +"]\n" ;
                    this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
                }

                str = "送信元アドレス:{0}/ポート番号:{1}/Size:{2}\n" + remoteEP.Address + "/" + remoteEP.Port + "/" + rcvBytes.Length ;
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
            }

            //UDP接続を終了
            udpc.Close();
        }
        //メインスレッドでの処理
        private void worker_udp_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 画面表示
            if ((id % 1) == 0)
            {
                MOTOR_DATA_KV_SP kmd3 = (MOTOR_DATA_KV_SP)e.UserState;
                string s = string.Format("worker_udp_ProgressChanged:[{0} {1} az:{2} alt:{3}]\n", kmd3.cmd ,kmd3.t,kmd3.az, kmd3.alt);
                textBox1.AppendText(s);
            }
        }

        static byte[] ToBytes(MOTOR_DATA_KV_SP obj)
        {
            int size = Marshal.SizeOf(typeof(MOTOR_DATA_KV_SP));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }
        static byte[] ToBytes(FSI_PID_DATA obj)
        {
            int size = Marshal.SizeOf(typeof(FSI_PID_DATA));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        public static MOTOR_DATA_KV_SP ToStruct(byte[] bytes)
        {
            GCHandle gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            MOTOR_DATA_KV_SP result = (MOTOR_DATA_KV_SP)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(MOTOR_DATA_KV_SP));
            gch.Free();
            return result;
        }

        #endregion

        #region キャプチャー
        // 別スレッド処理（キャプチャー）
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;
            Stopwatch sw = new Stopwatch() ;
            string str;
            id = 0;
            
            //PID送信用UDP
            //バインドするローカルポート番号
//            FSI_PID_DATA pid_data = new FSI_PID_DATA();
            int localPort = mmFsiUdpPortMT3PV;
            System.Net.Sockets.UdpClient udpc2 = null; ;
/*            try
            {
                udpc2 = new System.Net.Sockets.UdpClient(localPort);

            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
            */

            //videoInputオブジェクト
            const int DeviceID = 0;// 0;      // 3 (pro), 4(piccolo)  7(DMK)
            const int CaptureFps = 30;  // 30
            int interval = (int)(1000 / CaptureFps/10);
            const int CaptureWidth = 640;
            const int CaptureHeight = 480;
            // 画像保存枚数
            int mmFsiPostRec = 60;
            int save_counter = mmFsiPostRec;

            using (VideoInput vi = new VideoInput())
            {
                vi.SetIdealFramerate(DeviceID, CaptureFps);
                vi.SetupDevice(DeviceID, CaptureWidth, CaptureHeight);

                int width = vi.GetWidth(DeviceID);
                int height = vi.GetHeight(DeviceID);

                using (IplImage img = new IplImage(width, height, BitDepth.U8, 3))
                using (IplImage img_dark8 = Cv.LoadImage(@"C:\piccolo\MT3V_dark.bmp", LoadMode.GrayScale))
                //using (IplImage img_dark = new IplImage(width, height, BitDepth.U8, 3))
                using (IplImage img_mono = new IplImage(width, height, BitDepth.U8, 1))
                using (IplImage img2 = new IplImage(width, height, BitDepth.U8, 1))
                //                    using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                using (CvFont font = new CvFont(FontFace.HersheyComplex, 0.45, 0.45))
                //using (CvWindow window0 = new CvWindow("FIFO0", WindowMode.AutoSize))
                {
                    //this.Size = new Size(width + 12, height + 148);
                    double min_val, max_val;
                    CvPoint min_loc, max_loc;
                    int size  = 15 ;
                    int size2x = size/2 ;
                    int size2y = size/2 ;
                    int crop   = 20;
                    double sigma = 3;
                    long   elapsed0  =0, elapsed1  =0;
                    double framerate0=0, framerate1=0;
                    double alfa_fr=0.99;
                    sw.Start();      
                    while (bw.CancellationPending == false)
                    {
                        if (vi.IsFrameNew(DeviceID))
                        {
                            DateTime dn = DateTime.Now; //取得時刻
                            vi.GetPixels(DeviceID, img.ImageData, false, true);
                            // 画面time表示
                            str = String.Format("Wide ID:{0:D2} ", id) + dn.ToString("yyyyMMdd_HHmmss_fff");// +String.Format(" ({0,000:F2},{1,000:F2}) ({2,000:0},{3,000:0})({4,0:F1})", gx, gy, max_loc.X, max_loc.Y, max_val);
                            img.PutText(str, new CvPoint(10, 475), font, new CvColor(0, 100, 40));

                            Cv.CvtColor(img, img_mono, ColorConversion.BgrToGray);
                            Cv.Sub(img_mono, img_dark8, imgdata.img); // dark減算
                            imgdata.id = ++id;
                            imgdata.t = dn;
                            imgdata.ImgSaveFlag = !(ImgSaveFlag != 0); //int->bool変換
                            if (fifo.Count == MaxFrame - 1) fifo.EraseLast();
                            fifo.InsertFirst(imgdata);

                            #region 位置検出1  //MinMaxLoc
                            /*// 位置検出
                            Cv.Smooth(imgdata.img, img2, SmoothType.Gaussian, size, 0, sigma, 0);
                            CvRect rect;
                            if (PvMode == MyDETECT)
                            {
                                rect = new CvRect( (int)(gx+0.5) - size, (int)(gy+0.5) - size, size*2, size*2);
                                Cv.SetImageROI(img2, rect);
                                Cv.MinMaxLoc(img2, out  min_val, out  max_val, out  min_loc, out  max_loc, null);
                                Cv.ResetImageROI(img2);
                                max_loc.X += (int)(gx + 0.5) - size; // 基準点が(1,1)のため＋１
                                max_loc.Y += (int)(gy + 0.5) - size;
                            }
                            else
                            {
                                rect = new CvRect(crop, crop, width - (crop + crop), height - (crop + crop));
                                Cv.SetImageROI(img2, rect);
                                Cv.MinMaxLoc(img2, out  min_val, out  max_val, out  min_loc, out  max_loc, null);
                                Cv.ResetImageROI(img2);
                                max_loc.X += crop; // 基準点が(1,1)のため＋１
                                max_loc.Y += crop;
                            }
                            window0.ShowImage(img2);

                            double m00, m10, m01;
                            size2x = size2y = size / 2;
                            if (max_loc.X - size2x < 0) size2x = max_loc.X;
                            if (max_loc.Y - size2y < 0) size2y = max_loc.Y;
                            if (max_loc.X + size2x >= width ) size2x = width  -max_loc.X -1;
                            if (max_loc.Y + size2y >= height) size2y = height -max_loc.Y -1;
                            rect = new CvRect(max_loc.X - size2x, max_loc.Y - size2y, size, size);
                            CvMoments moments;
                            Cv.SetImageROI(img2, rect);
                            Cv.Moments(img2, out moments, false);
                            Cv.ResetImageROI(img2);
                            m00 = Cv.GetSpatialMoment(moments, 0, 0);
                            m10 = Cv.GetSpatialMoment(moments, 1, 0);
                            m01 = Cv.GetSpatialMoment(moments, 0, 1);
                            gx = max_loc.X - size2x + m10 / m00;
                            gy = max_loc.Y - size2y + m01 / m00;
                            */
                            #endregion
                            
                            
                            #region 位置検出2  //Blob
                            Cv.Threshold(imgdata.img, img2, threshold_blob, 255, ThresholdType.Binary); //2ms
                            blobs.Label(img2, imgLabel); //1.4ms
                            max_label = blobs.GreaterBlob();
                            elapsed1 = sw.ElapsedTicks; //1.3ms

                            if (blobs.Count > 1 && gx >= 0)
                            {
                                uint min_area = (uint)(threshold_min_area * blobs[max_label].Area);
                                blobs.FilterByArea(min_area, uint.MaxValue); //0.001ms

                                // 最適blobの選定（area大　かつ　前回からの距離小）
                                double x = blobs[max_label].Centroid.X;
                                double y = blobs[max_label].Centroid.Y;
                                uint area = blobs[max_label].Area;
                                //CvRect rect;
                                distance_min = ((x - gx) * (x - gx) + (y - gy) * (y - gy)); //Math.Sqrt()
                                foreach (var item in blobs)
                                {
                                    //Console.WriteLine("{0} | Centroid:{1} Area:{2}", item.Key, item.Value.Centroid, item.Value.Area);
                                    x = item.Value.Centroid.X;
                                    y = item.Value.Centroid.Y;
                                    //rect = item.Value.Rect;
                                    distance = ((x - gx) * (x - gx) + (y - gy) * (y - gy)); //将来はマハラノビス距離
                                    if (distance < distance_min)
                                    {
                                        d_val = (item.Value.Area) / max_area;
                                        if (distance <= 25) //近距離(5pix)
                                        {
                                            if (d_val >= 0.4)//&& d_val <= 1.2)
                                            {
                                                max_label = item.Key;
                                                distance_min = distance;
                                            }
                                        }
                                        else
                                        {
                                            if (d_val >= 0.8 && d_val <= 1.5)
                                            {
                                                max_label = item.Key;
                                                distance_min = distance;
                                            }
                                        }
                                    }
                                    //w.WriteLine("{0} {1} {2} {3} {4}", dis, dv, i, item.Key, item.Value.Area);
                                }
                                //gx = x; gy = y; max_val = area;
                            }

                            if (max_label > 0)
                            {
                                maxBlob = blobs[max_label];
                                max_centroid = maxBlob.Centroid;
                                gx = max_centroid.X;
                                gy = max_centroid.Y;
                                max_area = maxBlob.Area;
                                if (this.States == SAVE)
                                {
                                    Pid_Data_Send();
                                    timerSavePostTime.Stop();
                                    timerSaveMainTime.Stop();
                                    timerSaveMainTime.Start();
                                }
                            }
                            else
                            {
                                gx = gy = 0;
                                max_area = 0;
                            }
                            #endregion
                          
                          
                            // 画面表示
                            str = String.Format("ID:{0:D2} ", id) + dn.ToString("yyyyMMdd_HHmmss_fff") + String.Format(" ({0,000:F2},{1,000:F2}) ({2,000:0},{3,000:0})({4,0:F1})", gx,gy, xoa, yoa, max_area);
                            if (imgdata.ImgSaveFlag) str += " True";
                            img.PutText(str, new CvPoint(10, 20), font, new CvColor(0, 255, 100));
                            img.Circle(new CvPoint((int)gx,(int)gy), 10, new CvColor(255, 255, 100));
                            bw.ReportProgress(0, img);

                            // 処理速度
                            elapsed0 = sw.ElapsedTicks - elapsed1 ; // 1frameのticks
                            elapsed1 = sw.ElapsedTicks;
                            framerate0 = alfa_fr * framerate1 + (1 - alfa_fr) * (Stopwatch.Frequency / (double)elapsed0);
                            framerate1 = framerate0;

                            str = String.Format("fr time = {0}({1}){2:F1}", sw.Elapsed, id, framerate0); //," ", sw.ElapsedMilliseconds);
                            //匿名デリゲートで現在の時間をラベルに表示する
                            this.Invoke(new dlgSetString(ShowText), new object[] { textBox1, str });
                            //img.ToBitmap(bitmap);
                            //pictureBox1.Refresh();

                        }
                        Application.DoEvents();
                        Thread.Sleep(interval);
                    }
                    this.States = STOP;
                    this.Invoke(new dlgSetColor(SetColor), new object[] { ObsStart, this.States });
                    this.Invoke(new dlgSetColor(SetColor), new object[] { ObsEndButton, this.States });
                    vi.StopDevice(DeviceID);
                  //udpc2.Close();
                }
            }
        }
        //BCB互換TDatetime値に変換
        private double TDateTimeDouble(DateTime t)
        {
            TimeSpan ts = t - TBASE ;   // BCB 1899/12/30 0:0:0 からの経過日数
            return (ts.TotalDays);
        }

        //現在の時刻の表示と、タイマーの表示に使用されるデリゲート
        delegate void dlgSetString(object lbl, string text);
        //ボタンのカラー変更に使用されるデリゲート
        delegate void dlgSetColor(object lbl, int state);

        //デリゲートで別スレッドから呼ばれてラベルに現在の時間又は
        //ストップウオッチの時間を表示する
        private void ShowRText(object sender, string str)
        {
            RichTextBox rtb = (RichTextBox)sender;　//objectをキャストする
            rtb.AppendText(str); 
        }
        private void ShowText(object sender, string str)
        {
            TextBox rtb = (TextBox)sender;　//objectをキャストする
            rtb.Text = str;
        }
        private void SetColor(object sender, int sta)
        {
            Button rtb = (Button)sender;　//objectをキャストする
            if (sta == START)
            {
                rtb.BackColor = Color.Red;
            }
            else if (sta == STOP)
            {
                rtb.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }
        private void SetTimer(object sender, int sta)
        {
            System.Windows.Forms.Timer tim = (System.Windows.Forms.Timer)sender;　//objectをキャストする
            if (sta == START)
            {
                tim.Start();
            }
            else if (sta == STOP)
            {
                tim.Stop();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 画面表示
            if ((id % 1) == 0)
            {
                IplImage image = (IplImage)e.UserState;

                //Cv.Circle(image, new CvPoint(xoa, yoa), roa, new CvColor(0, 255, 0));
                Cv.Rectangle(image, new CvRect(xoa-row, yoa-roh,row+row,roh+roh), new CvColor(0, 255, 0));
                Cv.Line(  image, new CvPoint(xoa + roa, yoa + roa), new CvPoint(xoa - roa, yoa - roa), new CvColor(0, 255, 0));
                Cv.Line(  image, new CvPoint(xoa - roa, yoa + roa), new CvPoint(xoa + roa, yoa - roa), new CvColor(0, 255, 0));
              
                pictureBox1.Image = image.ToBitmap();
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                this.ObsStart.BackColor     = Color.FromKnownColor(KnownColor.Control);
                this.ObsEndButton.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            this.States = STOP;
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void ShowButton_Click(object sender, EventArgs e)
        {
            /*
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(openFileDialog1.FileName);
            }
            */
            Pid_Data_Send();
            return;

            //testルーチン

            double gx = 1;
            double gy = 2;

            //PID送信用UDP
            //バインドするローカルポート番号
            FSI_PID_DATA pid_data = new FSI_PID_DATA();
            int localPort = 24407;
            System.Net.Sockets.UdpClient udpc3 = null; ;
            try
            {
                udpc3 = new System.Net.Sockets.UdpClient(localPort);

            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
            //データを送信するリモートホストとポート番号
            string remoteHost = "192.168.1.206";
            int remotePort = 24410;
            //送信するデータを読み込む
            ++(pid_data.id);
            pid_data.swid = 24402;          // 仮　mmFsiUdpPortFSI2
            pid_data.t = TDateTimeDouble(DateTime.Now);
            pid_data.dx = (float)(gx);
            pid_data.dy = (float)(gy);
            pid_data.vmax = 123 ;
            byte[] sendBytes = ToBytes(pid_data);
            //リモートホストを指定してデータを送信する
            udpc3.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            if (udpc3 != null) udpc3.Close();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            /* if (checkBox1.Checked)
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            else
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
             */
        }

        private void ObsEndButton_Click(object sender, EventArgs e)
        {
            // BackgroundWorkerを停止.
            if (worker.IsBusy)
            {
                this.worker.CancelAsync();
                this.ObsEndButton.BackColor = Color.Red;
            }
        }

        private void ObsStart_Click(object sender, EventArgs e)
        {
            // BackgroundWorkerを開始
            if (!worker.IsBusy)
            {
                this.worker.RunWorkerAsync();
                this.ObsStart.BackColor = Color.Red;
                this.States = START;
            }
        }
            
        private void buttonSave_Click(object sender, EventArgs e)
        {
            ImgSaveFlag = TRUE;
            this.buttonSave.BackColor = Color.Red;
        }

        private void ButtonSaveEnd_Click(object sender, EventArgs e)
        {
            ImgSaveFlag = FALSE;
            this.buttonSave.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

        private void timerSavePostTime_Tick(object sender, EventArgs e)
        {
            timerSaveMainTime.Stop();
            timerSavePostTime.Stop();
            ButtonSaveEnd_Click(sender, e); 
        }

        private void timerSaveMainTime_Tick(object sender, EventArgs e)
        {
            timerSaveMainTime.Stop();
            timerSavePostTime.Start();
        }

        private void buttonMakeDark_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Checked = false;
                DarkMode = TRUE;
                timerMakeDark.Enabled = true;
            }
        }

        private void timerMakeDark_Tick(object sender, EventArgs e)
        {
            timerMakeDark.Enabled = false;
            DarkMode = FALSE;

        }

        private void timerObsOnOff_Tick(object sender, EventArgs e)
        {
            TimeSpan nowtime   = DateTime.Now - DateTime.Today;
            TimeSpan endtime   = new TimeSpan( 7, 0, 0);
            TimeSpan starttime = new TimeSpan(17, 0, 0);

            if (nowtime.CompareTo(endtime) >= 0 && nowtime.CompareTo(starttime) <= 0)
            {
                // DayTime
                if (this.States == START && checkBoxObsAuto.Checked)
                {
                     ObsEndButton_Click(sender, e);
                }
            }
            else
            {
                //NightTime
                if (this.States == STOP && checkBoxObsAuto.Checked)
                {
                    ObsStart_Click(sender, e);
                }
            }
        }
        // PID data送信ルーチン
        private void Pid_Data_Send_Init()
        {
            //PID送信用UDP
            //バインドするローカルポート番号
            //FSI_PID_DATA pid_data = new FSI_PID_DATA();
            int localPort = mmFsiUdpPortMT3PVs; // 24405 mmFsiUdpPortMT3PVs
            //System.Net.Sockets.UdpClient udpc3 = null ;
            try
            {
                udpc3 = new System.Net.Sockets.UdpClient(localPort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
        }
        // PID data送信ルーチン
        private void Pid_Data_Send()
        {
            // PID data send for UDP
            //データを送信するリモートホストとポート番号
            string remoteHost = mmFsiSC440;
            int remotePort = mmFsiUdpPortSpCam; // KV1000SpCam
            //送信するデータを読み込む
            ++(pid_data.id);
            //pid_data.swid = (ushort)mmFsiUdpPortMT3IDS2s;// 24417;  //mmFsiUdpPortMT3WideS
            pid_data.swid = (ushort)id;// mmFsiUdpPortMT3IDS2s;// 24417;  //mmFsiUdpPortMT3WideS
            pid_data.t =TDateTimeDouble(DateTime.Now); //TDateTimeDouble(imageInfo.TimestampSystem);  //LiveStartTime.AddSeconds(CurrentBuffer.SampleEndTime));//(DateTime.Now);
            if (PvMode == PID_TEST)
            {
                xoad = xoa_test_start + xoa_test_step * (pid_data.id - test_start_id);
                yoad = yoa_test_start + yoa_test_step * (pid_data.id - test_start_id);
            }
            else
            {
                xoad = xoa_mes;
                yoad = yoa_mes;
            }
            pid_data.dx = (float)(gx - xoad);
            pid_data.dy = (float)(gy - yoad);

            pid_data.vmax = (ushort)(max_area);
            byte[] sendBytes = ToBytes(pid_data);

            try
            {
                //リモートホストを指定してデータを送信する
                udpc3.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
        }

        /// <summary>
        /// MTmon status 送信ルーチン
        /// </summary>
        /// <remarks>
        /// MTmon status send
        /// </remarks>
        private void MTmon_Data_Send(object sender)
        {
            // MTmon status for UDP
            //データを送信するリモートホストとポート番号
            string remoteHost = mmFsiCore_i5;
            int remotePort = mmFsiUdpPortMTmonitor;
            //送信するデータを読み込む
            mtmon_data.id = 6; //PictureViewer
            mtmon_data.diskspace = (int)(diskspace / (1024 * 1024 * 1024));
            mtmon_data.obs = (byte)this.States;

            //mtmon_data.obs = this.States ; 
            byte[] sendBytes = ToBytes(mtmon_data);

            try
            {
                //リモートホストを指定してデータを送信する
                udpc3.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
        }
        static byte[] ToBytes(MT_MONITOR_DATA obj)
        {
            int size = Marshal.SizeOf(typeof(MT_MONITOR_DATA));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        private void timerMTmonSend_Tick(object sender, EventArgs e)
        {
            MTmon_Data_Send(sender);
        }

        private void timer1min_Tick(object sender, EventArgs e)
        {
            diskspace = cDrive.TotalFreeSpace;
        }
    }
}
