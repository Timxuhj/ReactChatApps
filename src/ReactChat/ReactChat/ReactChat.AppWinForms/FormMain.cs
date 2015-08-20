using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace ReactChat.AppWinForms
{
    public partial class FormMain : Form
    {
        private readonly ChromiumWebBrowser chromiumBrowser;

        public FormMain()
        {
            InitializeComponent();
            this.VerticalScroll.Visible = false;
            chromiumBrowser = new ChromiumWebBrowser(Program.HostUrl)
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(chromiumBrowser);

            this.FormClosing += (sender, args) =>
            {
                //Make closing feel more responsive.
                this.Visible = false;
            };

            this.FormClosed += (sender, args) =>
            {
                Cef.Shutdown();
            };

            chromiumBrowser.RegisterJsObject("aboutDialog", new AboutDialogJsObject(), camelCaseJavascriptNames: true);
            chromiumBrowser.RegisterJsObject("winForm",new WinFormsApp(this));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Left = Top = 0;
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            Height = Screen.PrimaryScreen.WorkingArea.Height;
        }
    }

    public class AboutDialogJsObject
    {
        public void Show()
        {
            MessageBox.Show("ServiceStack with CefSharp + ReactJS", "React_Chat_Gap.AppWinForms", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class WinFormsApp
    {
        public FormMain Form { get; set; }

        public WinFormsApp(FormMain form)
        {
            Form = form;
        }

        public void Close()
        {
            Form.InvokeOnUiThreadIfRequired(() =>
            {
                Form.Close();  
            });
        }
    }
}
