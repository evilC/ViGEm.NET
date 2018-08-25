using System;
using System.Collections.Generic;

namespace Nefarius.ViGEm.Client.Targets.DualShock4
{
    /*
    public static class DualShock4ReportExtensions
    {
        private static readonly Dictionary<string, int> _povAxisStates = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"x", 0 }, {"y", 0}
        };

        private struct PovAxes
        {
            public PovAxes(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        private struct PovVector
        {
            public PovVector(string axis, int direction)
            {
                Axis = axis;
                Direction = direction;
            }

            public string Axis { get; }
            public int Direction { get; }
        }

        private static readonly Dictionary<PovAxes, DualShock4DPadValues> AxisStatesToDpadValue = new Dictionary<PovAxes, DualShock4DPadValues>()
        {
            {new PovAxes(0, 0), DualShock4DPadValues.None},
            {new PovAxes(0, -1), DualShock4DPadValues.North},
            {new PovAxes(1, -1), DualShock4DPadValues.Northeast},
            {new PovAxes(1, 0), DualShock4DPadValues.East},
            {new PovAxes(1, 1), DualShock4DPadValues.Southeast},
            {new PovAxes(0, 1), DualShock4DPadValues.South},
            {new PovAxes(-1, 1), DualShock4DPadValues.Southwest},
            {new PovAxes(-1, 0), DualShock4DPadValues.West},
            {new PovAxes(-1, -1), DualShock4DPadValues.Northwest}
        };

        private static readonly Dictionary<int, PovVector> IndexToVector = new Dictionary<int, PovVector>()
        {
            {0, new PovVector("y", -1)},
            {1, new PovVector("x", 1)},
            {2, new PovVector("y", 1)},
            {3, new PovVector("x", -1)}
        };

        public static void SetButtons(this DualShock4Report report, params DualShock4Buttons[] buttons)
        {
            foreach (var button in buttons)
            {
                report.Buttons |= (ushort)button;
            }
        }

        public static void SetButtonState(this DualShock4Report report, DualShock4Buttons button, bool state)
        {
            if (state)
            {
                report.Buttons |= (ushort)button;
            }
            else
            {
                report.Buttons &= (ushort)~button;
            }
        }

        public static void SetDPad(this DualShock4Report report, DualShock4DPadValues value)
        {
            report.Buttons &= unchecked((ushort)~0xF);
            report.Buttons |= (ushort)value;
        }

        public static void SetSpecialButtons(this DualShock4Report report, params DualShock4SpecialButtons[] buttons)
        {
            foreach (var button in buttons)
            {
                report.SpecialButtons |= (byte)button;
            }
        }

        public static void SetSpecialButtonState(this DualShock4Report report, DualShock4SpecialButtons button, bool state)
        {
            if (state)
            {
                report.SpecialButtons |= (byte)button;
            }
            else
            {
                report.SpecialButtons &= (byte)~button;
            }
        }

        public static void SetAxis(this DualShock4Report report, DualShock4Axes axis, byte value)
        {
            switch (axis)
            {
                case DualShock4Axes.LeftTrigger:
                    report.LeftTrigger = value;
                    break;
                case DualShock4Axes.RightTrigger:
                    report.RightTrigger = value;
                    break;
                case DualShock4Axes.LeftThumbX:
                    report.LeftThumbX = value;
                    break;
                case DualShock4Axes.LeftThumbY:
                    report.LeftThumbY = value;
                    break;
                case DualShock4Axes.RightThumbX:
                    report.RightThumbX = value;
                    break;
                case DualShock4Axes.RightThumbY:
                    report.RightThumbY = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        /// Sets a POV cardinal direction (N/S/E/W) to either Pressed or Released
        /// </summary>
        /// <param name="report"></param>
        /// <param name="direction">0 = N, 1 = E, 2 = S, 3 = W</param>
        /// <param name="state">true = pressed, false = released</param>
        public static void SetPovDirectionState(this DualShock4Report report, int direction, bool state)
        {
            var mapping = IndexToVector[direction];
            var axisState = _povAxisStates[mapping.Axis];
            var newState = state ? mapping.Direction : 0;
            if (axisState == newState) return;
            _povAxisStates[mapping.Axis] = newState;

            var buttons = (int)report.Buttons;
            // Clear all the Dpad bits
            buttons &= ~15;

            // Set new Dpad bits
            buttons |= (int)AxisStatesToDpadValue[new PovAxes(_povAxisStates["x"], _povAxisStates["y"])];

            report.Buttons = (ushort)buttons;
        }
    }
    */
}