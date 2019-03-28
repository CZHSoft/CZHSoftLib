using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CZHSoft.Controls
{
    public partial class Waitting : Form
    {

        public delegate void LoadingCircleCloseDelegate();
        public event LoadingCircleCloseDelegate OnLoadingCircleClose;

        private Thread waittingThread;
        private Waitting waitting;

        private Random random;
        private int R,G,B;
        private Thread colorThread;

        public Waitting()
        {
            InitializeComponent();
        }

        private void Waitting_Load(object sender, EventArgs e)
        {
            loadingCircle.Active = true;

            random = new Random();
        }

        public void Start()
        {
            waittingThread = new Thread(LoadingCircleShow); ;
            waittingThread.IsBackground = true;
            waittingThread.Start();

            colorThread = new Thread(ColorChange);
            colorThread.IsBackground = true;
            colorThread.Start();

        }

        private void LoadingCircleShow()
        {
            waitting = new Waitting();
            waitting.OnLoadingCircleClose += new LoadingCircleCloseDelegate(WaittingClose);
            waitting.ShowDialog();
        }

        private void ColorChange()
        {
            int a = random.Next(-5,5);

            if(R+a<0 || R+a>255)
            {
                R = 255 -Math.Abs(a);
            }
            if (G + a < 0 || G + a > 255)
            {
                G = 255 - Math.Abs(a);
            }
            if (B + a < 0 || B + a > 255)
            {
                B = 255 - Math.Abs(a);
            }

            loadingCircle.Color = Color.FromArgb(R, G, B);
        }

        public void WaittingClose()
        {
            if (OnLoadingCircleClose != null)
            {
                OnLoadingCircleClose();
            }

            while (true)
            {
                if (waitting.Opacity > 0.0)
                {
                    waitting.Invoke((EventHandler)(delegate
                    {
                        waitting.Opacity -= 0.05;
                    }));

                    Thread.Sleep(50);
                }
                else
                {
                    break;
                }
            }

            if (waittingThread != null)
            {
                if (waittingThread.IsAlive)
                {
                    waittingThread.Abort();
                }
            }

            this.Hide();
        }

        private void Waitting_DoubleClick(object sender, EventArgs e)
        {
            //this.WaittingClose();
        }

        private void Waitting_Click(object sender, EventArgs e)
        {
            //this.WaittingClose();
        }

        private void Waitting_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WaittingClose();
        }
    }
}
