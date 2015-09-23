﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using ServiceStack;
using ServiceStack.Text;

namespace ReactChat.AppWinForms
{
    static class Program
    {
        public static string HostUrl = "http://127.0.0.1:1337/";
        public static AppHost AppHost;
        public static FormMain Form;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Cef.Initialize(new CefSettings());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppHost = new AppHost();

            try
            {
                AppHost.Init().Start("http://*:1337/");
                "ServiceStack SelfHost listening at {0} ".Fmt(HostUrl).Print();
                Form = new FormMain();
            }
            catch
            {
                "Listening to existing service at {0}".Print(HostUrl);
                Form = new FormMain(startRight: true);
            }

            Application.Run(Form);
        }
    }
}
