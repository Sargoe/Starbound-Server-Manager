using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Diagnostics;

namespace Starbound_Server_Manager
{
    public partial class Form1 : Form
    {


        static int    Interval;                                                                                                                      // Used to set the delay between file checks
        static string Data;                                                                                                                          // This is used to store the input from the user and will always get offloaded to another variable such as "Interval"
        static string LineOfText;                                                                                                                    // This is used to store the data from the logs, line by line.

        static bool                HasFailed    = false;                                                                                             // Makes set-up faster, don't wanna deal with newbs.
        static System.Timers.Timer DelayTimer   = new System.Timers.Timer();                                                                         // This is somewhat self explanitory. I use System.Timers.Timer here because "Timer" can refer to both System.Threading.Timer and System.Timers.Timer which is not great.
        static string              TextFilePath = "";                                                                                                // Path to the logfile
        static Process             StarboundExe = Process.Start("starbound_server.exe");
        static int                 TimerTicks   = 0;
        static OpenFolderDialog      Dialogue     = new OpenFileDialog(); // test
        
        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            InitializeServer();
        }

        static void MSG(string Input) 
        {
            MessageBox.Show(Input, "STARBOUND SERVER MANAGER", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static void InitializeServer()
        {

            MSG("Please enter the delay time ( in milliseconds eg. 2500 for 2 and a half seconds ) ");

            CheckInterval();

            CheckDirectory();

            Timer_Start(Interval);

            // Server_Start(); // TODO -- CREATE FUNCTION FOR THIS AND MAKE IT START STARBOUND SERVER.




        }


        static void CheckDirectory() // If you press enter I assume this program is in the root of the server.
        {

            MSG("Please select the starbound server root folder ( the one with the exe in it )");
            Dialogue.ShowDialog();
            if (Data == "")
            {
                TextFilePath += "/starbound_server.txt";
                MSG(TextFilePath);
            }
            else
            {
                TextFilePath = Data + "/starbound_server.txt";
                MSG(TextFilePath);
            }

        }


        static string Input(string title, string defResp) // I'm being lazy.
        {
            string inputData = Microsoft.VisualBasic.Interaction.InputBox(title, defResp);
            return inputData;
        }

        static void CheckInterval()
        {
            Data = Input("Please enter delay time in milliseconds", "2500");


            if (int.TryParse(Data, out Interval))
            {
                Interval = Int32.Parse(Data);
                MSG("Interval set to " + Interval);
            }
            else
            {

                if (HasFailed) // if this is true it means you have failed to set this correctly once and the program is sick of your shit.
                {
                    MSG("ERROR! You are teh noobs. Delay has been set at 2500 milliseconds");
                    Interval = 2500;
                    return;
                }

                HasFailed = true;
                MSG("Please enter a number this time! Fail again and it will be set at 2500");
                CheckInterval();

            }
        }

        static void Timer_Start(int delay) // Duh...
        {
            DelayTimer.Interval = Interval;
            DelayTimer.Elapsed += TimerStuff;
            DelayTimer.Start();
        }

        static void TimerStuff(object sender, ElapsedEventArgs e) // This procs every time the timer hits its delay time. Eventually ProcessText(TextFilePath) will be in here
        {

            if (TimerTicks > 1) 
            {
                return;
                DelayTimer.Stop();
                MSG("TIMER HAS DONE ITS THING");
            }

            StarboundExe.Kill();
            TimerTicks++; // adds one.
        }

        static void ProcessText(string FileDirectory) // This is what does all the hard work, checking through the logs for that networking error.
        {
            var filestream = new FileStream(TextFilePath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite);
            var file = new StreamReader(filestream, Encoding.UTF8, true, 128);

            while ((LineOfText = file.ReadLine()) != null)
            {
                if (LineOfText == "I/O error in RemotePacketSocket::receiveData, closing: (NetworkException) tcp recv error: An established connection was aborted by the software in your host machine.")
                {
                    RestartThatShit();
                }
            }
        }

        static void RestartThatShit() 
        {

            StarboundExe.Kill();
            StarboundExe.Start();

        }

  
    }
}
