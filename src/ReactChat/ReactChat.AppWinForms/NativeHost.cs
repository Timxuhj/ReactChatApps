using System.Threading;
using System.Windows.Forms;
using CefSharp.WinForms.Internals;
using ServiceStack;

namespace ReactChat.AppWinForms
{
    public class NativeHost
    {
        private readonly FormMain formMain;

        public NativeHost(FormMain formMain)
        {
            this.formMain = formMain;
            //Enable Chrome Dev Tools when debugging WinForms
#if DEBUG
            formMain.ChromiumBrowser.KeyboardHandler = new KeyboardHandler();
#endif
        }

        public string Platform
        {
            get { return "winforms"; }
        }

        public void ShowAbout()
        {
            MessageBox.Show(@"ServiceStack with CefSharp + ReactJS", @"ReactChat.AppWinForms", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ToggleFormBorder()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.FormBorderStyle = formMain.FormBorderStyle == FormBorderStyle.None
                    ? FormBorderStyle.Sizable
                    : FormBorderStyle.None;
            });
        }

        public void Shrink()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
            });
        }

        public void Grow()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        struct Position
        {
            public Position(int top, int height, int pause)
            {
                Top = top;
                Height = height;
                Pause = pause;
            }

            public int Top;
            public int Height;
            public int Pause;
        }

        public void Dance()
        {
            var height = Screen.PrimaryScreen.WorkingArea.Height;
            var positions = new[]
            {
                new Position(0, height, 500),
                new Position(height / 16, height / 16 * 15, 500),
                new Position(height / 8, height / 8 * 7, 500),
                new Position(height / 4, height / 4 * 3, 500),
                new Position(height / 2, height / 2, 500),

                new Position(height / 2, height / 2 - 100, 250),
                new Position(height / 2, height / 2 - 200, 250),
                new Position(height / 2, height / 2 - 100, 250),
                new Position(height / 2, height / 2, 200),

                new Position(height / 2, height / 2 - 100, 250),
                new Position(height / 2, height / 2 - 200, 250),
                new Position(height / 2, height / 2 - 100, 250),
                new Position(height / 2, height / 2, 500),

                new Position(height / 4, height / 4 * 3, 500),
                new Position(height / 8, height / 8 * 7, 500),
                new Position(height / 16, height / 16 * 15, 500),
                new Position(0, height, 0),
            };

            foreach (var position in positions)
            {
                formMain.InvokeOnUiThreadIfRequired(() =>
                {
                    formMain.Top = position.Top;
                    formMain.Height = position.Height;
                });
                Thread.Sleep(position.Pause);
            }
        }

        public void Quit()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Hide();
                HostContext.Resolve<IServerEvents>().NotifyChannel("home", "cmd.announce", "Quick follow me, lets get out of here..");
                Thread.Sleep(1000);
                HostContext.Resolve<IServerEvents>().NotifyChannel("home", "formMain.quit", "");
            });

            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Close();
            });
        }

        public void Ready()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Controls.Remove(formMain.SplashPanel);
            });
        }
    }

#if DEBUG
    public class KeyboardHandler : CefSharp.IKeyboardHandler
    {
        public bool OnPreKeyEvent(CefSharp.IWebBrowser browserControl, CefSharp.KeyType type, int windowsKeyCode, int nativeKeyCode,
            CefSharp.CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (windowsKeyCode == (int)Keys.F12)
            {
                Program.Form.ChromiumBrowser.ShowDevTools();
            }
            return false;
        }

        public bool OnKeyEvent(CefSharp.IWebBrowser browserControl, CefSharp.KeyType type, int windowsKeyCode, CefSharp.CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }
    }
#endif
}
