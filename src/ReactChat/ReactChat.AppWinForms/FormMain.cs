using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace ReactChat.AppWinForms
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            VerticalScroll.Visible = false;
            var chromiumBrowser = new ChromiumWebBrowser(Program.HostUrl)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(chromiumBrowser);

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

            FormClosed += (sender, args) =>
            {
                Cef.Shutdown();
            };

            chromiumBrowser.RegisterJsObject("aboutDialog", new AboutDialogJsObject());
            chromiumBrowser.RegisterJsObject("winForm",new WinFormsApp(this, splashPanel));
            chromiumBrowser.RegisterJsObject("formMain", this);
        }

        public void ToggleFormBorder()
        {
            Invoke((MethodInvoker)delegate {
                FormBorderStyle = FormBorderStyle == FormBorderStyle.None
                    ? FormBorderStyle.Sizable
                    : FormBorderStyle.None;
            });
        }

        public void DockLeft()
        {
            Invoke((MethodInvoker)delegate {
                Left = 0;
                Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void DockRight()
        {
            Invoke((MethodInvoker)delegate {
                Left = Screen.PrimaryScreen.WorkingArea.Width / 2;
                Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }
    }

    public class AboutDialogJsObject
    {
        public void Show()
        {
            MessageBox.Show(@"ServiceStack with CefSharp + ReactJS", @"ReactChat.AppWinForms", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class WinFormsApp
    {
        private FormMain _form;
        private Panel _splashPanel;

        public WinFormsApp(FormMain form, Panel splashPanel)
        {
            _form = form;
            _splashPanel = splashPanel;
        }

        public void Close()
        {
            _form.InvokeOnUiThreadIfRequired(() =>
            {
                _form.Close();  
            });
        }

        public void Ready()
        {
            _form.InvokeOnUiThreadIfRequired(() =>
            {
                _form.Controls.Remove(_splashPanel);
            });
        }
    }
}
