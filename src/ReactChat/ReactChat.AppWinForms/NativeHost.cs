using System.Windows.Forms;

namespace ReactChat.AppWinForms
{
    public class NativeHost
    {
        public string Platform
        {
            get { return "winforms"; }
        }

        public void Quit()
        {
            Program.Form.Quit();
        }

        public void ShowAbout()
        {
            MessageBox.Show(@"ServiceStack with CefSharp + ReactJS", @"ReactChat.AppWinForms", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ToggleFormBorder()
        {
            Program.Form.ToggleFormBorder();
        }

        public void DockLeft()
        {
            Program.Form.DockLeft();
        }

        public void DockRight()
        {
            Program.Form.DockRight();
        }

        public void Ready()
        {
            Program.Form.Ready();
        }
    }
}
