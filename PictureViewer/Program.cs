using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using OpenCvSharp;


namespace PictureViewer
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, "pv1"))
            {
                if (!mutex.WaitOne(0,false))
                {
                    using (Mutex mutex2 = new Mutex(false, "pv2"))
                    {
                        if (!mutex2.WaitOne(0, false))
                        {
                            // 既に起動されている
                            return;
                        }
                    }
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
