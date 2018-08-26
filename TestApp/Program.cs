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
            var controller = new DualShock4Controller(_client);
            //var controller = new Xbox360Controller(_client);
            controller.Connect();

            controller.Report.SetButtonState(1, true);
            controller.SendReport();
            //Thread.Sleep(1000);
            controller.Report.SetButtonState(1, false);
            controller.SendReport();

            for (var i = 0; i < 6; i++)
            {
                controller.Report.SetAxisState(i, short.MaxValue);
                controller.SendReport();
                Thread.Sleep(250);
                controller.Report.SetAxisState(i, short.MinValue);
                controller.SendReport();
                Thread.Sleep(250);
                controller.Report.SetAxisState(i, 0);
                controller.SendReport();
                Thread.Sleep(250);
            }

            controller.Report.SetPovDirectionState(PovDirections.Up, true);
            controller.SendReport();
            controller.Report.SetPovDirectionState(PovDirections.Right, true);
            controller.SendReport();
            controller.Report.SetPovDirectionState(PovDirections.Up, false);
            controller.SendReport();
            controller.Report.SetPovDirectionState(PovDirections.Right, false);
            controller.SendReport();
        }
    }
}
