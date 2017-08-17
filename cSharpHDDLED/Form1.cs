using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Management;
using System.Management.Instrumentation;
using System.Collections.Specialized;
using System.Threading;


namespace cSharpHDDLED
{

    public partial class Form1 : Form
    {
        #region Global Variables

        NotifyIcon hddLedIcon;
        Icon activeIcon;
        Icon idleIcon;
        Thread hddLedWorker;

        #endregion

        #region Form Stuff
        public Form1()
        {
            InitializeComponent();

            // Load icons from files into objects
            activeIcon = new Icon("active.png");
            idleIcon = new Icon("idle.png");

            //  Create notify icons and assign idle icon and show it
            hddLedIcon = new NotifyIcon();
            hddLedIcon.Icon = idleIcon;
            hddLedIcon.Visible = true;

            //  Create all context menu items and add them to notification tray icon
            MenuItem progNameMenuItem = new MenuItem("HDD LED v0.1 BETA by: BZ");
            MenuItem quitMenuItem = new MenuItem("Quit");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(progNameMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);
            hddLedIcon.ContextMenu = contextMenu;

            //  Wire up quit button to close application
            quitMenuItem.Click += quitMenuItem_Click;


            //  Hide the form becuase we don't need it, this is a notification tray app
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            //  Start worker thread that pulls HDD activity
            hddLedWorker = new Thread(new ThreadStart(HddActivityThread));
            hddLedWorker.Start();

        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Close the applicaiton on click of 'quit' button on context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void quitMenuItem_Click(object sender, EventArgs e)
        {
            hddLedWorker.Abort();
            hddLedIcon.Dispose();
            this.Close(); ;
        }  // closes quitMenuItem_Click function



        #endregion


        #region Threads

        /// <summary>
        /// This is the thread that pulls the HDD for activity and updates the notification icon
        /// </summary>
        public void HddActivityThread()
        {
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                //  Main loop where all the magic happens
                while (true)
                {
                    //  Connect to the drive performance instance
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach( ManagementObject obj in driveDataClassCollection)
                    {
                        //  Only process the _Total instance and ignore all the individual instances
                        if( obj["Name"].ToString() == "_Total")
                        {
                            if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                            {
                                //  Show busy icon
                                hddLedIcon.Icon = activeIcon;

                            } else
                            {
                                //  Show idle icon
                                hddLedIcon.Icon = idleIcon;

                            }  // closes else statement

                        }  // closes if statement
                    }  // closes foreach statemnt

                    //  Slows the loop down so it doesn't consume so much cpu
                    Thread.Sleep(100);
                }
            } catch( ThreadAbortException tbe)
            {
                //  Thread was aborted
                driveDataClass.Dispose();

            }  // closes catch
        }  // closes HDDActivityThread

        #endregion
    }  // closes public
}  // closes namespace
