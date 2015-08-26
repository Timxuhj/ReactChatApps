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
            this.InvokeOnUiThreadIfRequired(() => {
                FormBorderStyle = FormBorderStyle == FormBorderStyle.None
                    ? FormBorderStyle.Sizable
                    : FormBorderStyle.None;
            });
        }

        public void DockLeft()
        {
            this.InvokeOnUiThreadIfRequired(() => {
                Left = 0;
                Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void DockRight()
        {
            this.InvokeOnUiThreadIfRequired(() => {
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
        private FormMain form;
        private Panel splashPanel;

        public WinFormsApp(FormMain form, Panel splashPanel)
        {
            this.form = form;
            this.splashPanel = splashPanel;
        }

        public void Close()
        {
            form.InvokeOnUiThreadIfRequired(() => {
                form.Close();  
            });
        }

        public void Ready()
        {
            form.InvokeOnUiThreadIfRequired(() => {
                form.Controls.Remove(splashPanel);
            });
        }
    }
}
