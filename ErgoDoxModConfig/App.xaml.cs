using ErgoDoxModConfig.M;
using ErgoDoxModConfig.VM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ErgoDoxModConfig
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // ここはMainWindowのコンストラクタより先に呼ばれる
            m_model = new Config();
            m_model.LoadConfig();
            m_vm = new VMMain(m_model);
        }

        VMMain m_vm;
        Config m_model;

        public static VMMain GetViewModel()
        {
            var app = Application.Current as App;
            if (app!=null)
                return app.m_vm;
            throw new InvalidOperationException("VM not found...");
        }
        public static Config GetModel()
        {
            var app = Application.Current as App;
            if (app != null)
                return app.m_model;
            throw new InvalidOperationException("Model not found...");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            m_model.SaveConfig();
        }
    }
}
