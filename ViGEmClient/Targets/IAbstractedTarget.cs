using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nefarius.ViGEm.Client.Targets
{
    public interface IAbstractedTarget
    {
        void SendReport();

        void SetButtonState(int buttonIndex, bool state);
        void SetAxisState(int axisIndex, int state);
        void SetPovDirectionState(PovDirections direction, bool state);
    }

    public enum PovDirections {Up, Right, Down, Left}
}
