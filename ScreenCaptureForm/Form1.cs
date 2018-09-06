/**
 * ****************************************************************************
 * Copyright (C) 2018 Das Deutsche Institut für Internationale Pädagogische Forschung (DIPF)
 * <p/>
 * This library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * <p/>
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * <p/>
 * You should have received a copy of the GNU Lesser General Public License
 * along with this library.  If not, see <http://www.gnu.org/licenses/>.
 * <p/>
 * Contributors: Jan Schneider
 * ****************************************************************************
 */

using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCaptureForm
{
    public partial class Form1 : Form
    {
        public Timer timer1;

        VideoFileWriter vf;
       

        bool isRecording = false;
        ConnectorHub.ConnectorHub myConnector;
        System.DateTime sartRecordingTime;

        public delegate void startRecording();
        public startRecording myStart;

        public delegate void stopRecording();
        public stopRecording myStop;

        string filename;

        Bitmap bmpScreenShot;

        int i;

        public Form1()
        {
            InitializeComponent();
            this.FormClosed += Form1_FormClosed;
            i = 0;
            try
            {
                myConnector = new ConnectorHub.ConnectorHub();
                myConnector.init();
                myConnector.sendReady();
                myStart = new startRecording(startMethod);
                myStop = new stopRecording(stopMethod);

                myConnector.startRecordingEvent += MyConnector_startRecordingEvent;
                myConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
            }
            catch
            {

            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            myConnector.close();
        }

        private void stopMethod()
        {
            button2_Click(null, null);
        }

        private void startMethod()
        {
            button1_Click(null, null);
        }

        private void MyConnector_stopRecordingEvent(object sender)
        {
            this.Invoke(this.myStop);
        }

        private void MyConnector_startRecordingEvent(object sender)
        {
            this.Invoke(this.myStart);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1 = new Timer();
            timer1.Interval = 20;
            timer1.Tick += timer1_Tick;
            vf = new VideoFileWriter();
            vf.Open("Exported_Video.avi", 800, 600, 25, VideoCodec.Default, 1000000);

            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width * 2;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height * 2;
            bmpScreenShot = new Bitmap(screenWidth, screenHeight);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            captureFunction();
        }

        private void captureFunction()
        {
            try
            {
                int screenWidth = Screen.PrimaryScreen.WorkingArea.Width*2;
                int screenHeight = Screen.PrimaryScreen.WorkingArea.Height*2;
                

                //Bitmap bmpScreenShot = new Bitmap(screenWidth, screenHeight);
                Graphics gfx = Graphics.FromImage((System.Drawing.Image)bmpScreenShot);
                gfx.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenWidth, screenHeight));
                System.TimeSpan diff1 = DateTime.Now.Subtract(sartRecordingTime);
                vf.WriteVideoFrame(bmpScreenShot, diff1);
            }
            catch(Exception e)
            {
                int x = i;
            
            }
            i++;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isRecording = true;
            vf = new VideoFileWriter();
            sartRecordingTime = DateTime.Now;
            string time = DateTime.Now.Hour.ToString();
            time = time + "H" + DateTime.Now.Minute.ToString() + "M" + DateTime.Now.Second.ToString() + "S";
            time = time + ".mp4";
            filename = time;
            // vf.Open(time, (int)System.Windows.SystemParameters.PrimaryScreenWidth * 2, (int)System.Windows.SystemParameters.PrimaryScreenHeight * 2, 25, VideoCodec.MPEG4, 200000);
            //vf.Open(time, Screen.PrimaryScreen.WorkingArea.Width * 2, Screen.PrimaryScreen.WorkingArea.Height * 2, 25, VideoCodec.MPEG4);
            
            vf.Open(time, Screen.PrimaryScreen.WorkingArea.Width * 2, Screen.PrimaryScreen.WorkingArea.Height * 2, 25, VideoCodec.Default, 1000000);
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            timer1.Stop();
            vf.Close();
            try
            {
                myConnector.sendTCPAsync(myConnector.SendFile + filename + myConnector.endSendFile);
            }
            catch (Exception x)
            {
                int i = 0;
                i++;
            }
        }
        public void close()
        {
            myConnector.close();
        }
    }
}
