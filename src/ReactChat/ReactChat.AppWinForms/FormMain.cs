using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace ReactChat.AppWinForms
{
    public partial class FormMain : Form
    {
        public ChromiumWebBrowser ChromiumBrowser { get; private set; }
        public Panel SplashPanel { get { return splashPanel; } }

        public FormMain()
        {
            InitializeComponent();
            VerticalScroll.Visible = false;
            ChromiumBrowser = new ChromiumWebBrowser(Program.HostUrl)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(ChromiumBrowser);

            Load += (sender, args) =>
            {
                Left = Top = 0;
                Width = Screen.PrimaryScreen.WorkingArea.Width;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            };

            FormClosing += (sender, args) =>
            {
                //Make closing feel more responsive.
                Visible = false;
            };
#if DEBUG
            ChromiumBrowser.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.F12)
                {
                    ChromiumBrowser.ShowDevTools();
                }
            };
#endif

            FormClosed += (sender, args) =>
            {
                Cef.Shutdown();
            };

            ChromiumBrowser.RegisterJsObject("nativeHost", new NativeHost(this));
        }
    }
}
