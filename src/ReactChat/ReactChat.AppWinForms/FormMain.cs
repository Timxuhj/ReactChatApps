using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace ReactChat.AppWinForms
{
    public partial class FormMain : Form
    {
        public ChromiumWebBrowser ChromiumBrowser { get; private set; }

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

            ChromiumBrowser.RegisterJsObject("nativeHost", new NativeHost());
        }

        public void ToggleFormBorder()
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                FormBorderStyle = FormBorderStyle == FormBorderStyle.None
                    ? FormBorderStyle.Sizable
                    : FormBorderStyle.None;
                Left = Top = 0;
                Width = Screen.PrimaryScreen.WorkingArea.Width;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void DockLeft()
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                MaximizeBox = false;
                Left = 0;
                Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void DockRight()
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                MaximizeBox = false;
                Left = Screen.PrimaryScreen.WorkingArea.Width / 2;
                Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void Quit()
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                Close();
            });
        }

        public void Ready()
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                Controls.Remove(splashPanel);
                //Enable Chrome Dev Tools when debugging WinForms
#if DEBUG
                ChromiumBrowser.KeyboardHandler = new KeyboardHandler();
#endif
            });
        }
    }

#if DEBUG
    public class KeyboardHandler : IKeyboardHandler
    {
        public bool OnPreKeyEvent(IWebBrowser browserControl, KeyType type, int windowsKeyCode, int nativeKeyCode,
            CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (windowsKeyCode == (int)Keys.F12)
            {
                Program.Form.ChromiumBrowser.ShowDevTools();
            }
            return false;
        }

        public bool OnKeyEvent(IWebBrowser browserControl, KeyType type, int windowsKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }
    }
#endif
}
