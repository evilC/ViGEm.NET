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
            //var controller = new DualShock4Controller(_client);
            var controller = new Xbox360Controller(_client);
            controller.Connect();

            //controller.SetButtonState(1, true);
            //controller.SendReport();
            ////Thread.Sleep(1000);
            //controller.SetButtonState(1, false);
            //controller.SendReport();

            //controller.SetAxisState(0, -30000);
            //controller.SendReport();

            controller.SetPovDirectionState(PovDirections.Up, true);
            controller.SendReport();
            controller.SetPovDirectionState(PovDirections.Right, true);
            controller.SendReport();
            controller.SetPovDirectionState(PovDirections.Up, false);
            controller.SendReport();
            controller.SetPovDirectionState(PovDirections.Right, false);
            controller.SendReport();
        }
    }
}
