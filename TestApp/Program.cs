using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestApp();
            Console.ReadLine();
        }
    }

    class TestApp
    {
        private readonly ViGEmClient _client = new ViGEmClient();

        public TestApp()
        {
            var ds4 = new DualShock4Controller(_client);
            ds4.Connect();

            //ds4.SetButtonState(1, true);
            //ds4.SendReport();
            ////Thread.Sleep(1000);
            //ds4.SetButtonState(1, false);
            //ds4.SendReport();

            //ds4.SetAxisState(0, 65535);
            //ds4.SendReport();

            ds4.SetPovDirectionState(PovDirections.Up, true);
            ds4.SendReport();
            ds4.SetPovDirectionState(PovDirections.Right, true);
            ds4.SendReport();
            ds4.SetPovDirectionState(PovDirections.Up, false);
            ds4.SendReport();
            ds4.SetPovDirectionState(PovDirections.Right, false);
            ds4.SendReport();
        }
    }
}
