using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nefarius.ViGEm.Client.Targets
{
    interface IViGEmReport<TButtons, TAxes>
    {
        void SetButtonState(TButtons button, bool state);
        void SetButtonState(int buttonIndex, bool state);

        void SetAxisState(TAxes axis , int state);
        void SetAxisState(int axisIndex, int state);

        void SetPovDirectionState(PovDirections povDirection, bool state);
    }

    public enum PovDirections { Up, Right, Down, Left }
}
