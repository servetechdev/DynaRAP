﻿using DynaRAP.Forms;
using DynaRAP.TEST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynaRAP
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            Application.Run(new MainForm());
#else
            MainStarter main = new MainStarter();
            main.Run();
#endif
        }
    }
}
