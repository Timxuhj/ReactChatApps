using System.Windows.Forms;
using CefSharp.WinForms.Internals;

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
                formMain.Left = formMain.Top = 0;
                formMain.Width = Screen.PrimaryScreen.WorkingArea.Width;
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void DockLeft()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.MaximizeBox = false;
                formMain.Left = 0;
                formMain.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void DockRight()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.MaximizeBox = false;
                formMain.Left = Screen.PrimaryScreen.WorkingArea.Width / 2;
                formMain.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void Quit()
        {
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
