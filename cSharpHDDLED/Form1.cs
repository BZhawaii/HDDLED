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
        NotifyIcon hddLedIcon;
        Icon activeIcon;
        Icon idleIcon;
        Thread hddLedWorker;

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


            //  Hide the form becuase we don't need it, this is a notification tray app
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;


        }
#endregion

    }
}
