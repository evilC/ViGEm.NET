using System;
using System.Collections.Generic;

namespace Nefarius.ViGEm.Client.Targets.DualShock4
{
    [Flags]
    public enum DualShock4Buttons : ushort
    {
        ThumbRight = 1 << 15,
        ThumbLeft = 1 << 14,
        Options = 1 << 13,
        Share = 1 << 12,
        TriggerRight = 1 << 11,
        TriggerLeft = 1 << 10,
        ShoulderRight = 1 << 9,
        ShoulderLeft = 1 << 8,
        Triangle = 1 << 7,
        Circle = 1 << 6,
        Cross = 1 << 5,
        Square = 1 << 4
    }

    [Flags]
    public enum DualShock4SpecialButtons : byte
    {
        Ps = 1 << 0,
        Touchpad = 1 << 1
    }

    public enum DualShock4Axes
    {
        LeftTrigger,
        RightTrigger,
        LeftThumbX,
        LeftThumbY,
        RightThumbX,
        RightThumbY
    }

    public enum DualShock4DPadValues
    {
        None = 0x8,
        Northwest = 0x7,
        West = 0x6,
        Southwest = 0x5,
        South = 0x4,
        Southeast = 0x3,
        East = 0x2,
        Northeast = 0x1,
        North = 0x0
    }

    public class DualShock4Report : IViGEmReport<DualShock4Buttons, DualShock4Axes>
    {
        public DualShock4Report()
        {
            Buttons &= unchecked((ushort)~0xF);
            Buttons |= 0x08;
            LeftThumbX = 0x80;
            LeftThumbY = 0x80;
            RightThumbX = 0x80;
            RightThumbY = 0x80;
        }

        public ushort Buttons { get; set; }

        public byte SpecialButtons { get; set; }

        public byte LeftTrigger { get; set; }

        public byte RightTrigger { get; set; }

        public byte LeftThumbX { get; set; }

        public byte LeftThumbY { get; set; }

        public byte RightThumbX { get; set; }

        public byte RightThumbY { get; set; }



        private readonly List<DualShock4Buttons> _buttonFlags = new List<DualShock4Buttons>
        {
            DualShock4Buttons.Cross, DualShock4Buttons.Circle, DualShock4Buttons.Square, DualShock4Buttons.Triangle,
            DualShock4Buttons.ShoulderLeft, DualShock4Buttons.ShoulderRight, DualShock4Buttons.ThumbLeft, DualShock4Buttons.ThumbRight,
            DualShock4Buttons.Options, DualShock4Buttons.Share,
            DualShock4Buttons.TriggerLeft, DualShock4Buttons.TriggerRight
        };

        public void SetButtonState(DualShock4Buttons button, bool state)
        {
            if (state)
            {
                Buttons |= (ushort)button;
            }
            else
            {
                Buttons &= (ushort)~button;
            }
        }

        public void SetButtonState(int buttonIndex, bool state)
        {
            var button = _buttonFlags[buttonIndex];
            SetButtonState(button, state);
        }

        public void SetAxisState(DualShock4Axes axis, int state)
        {
            // ToDo: Normalize values?
            var value = (byte)state;
            switch (axis)
            {
                case DualShock4Axes.LeftTrigger:
                    LeftTrigger = value;
                    break;
                case DualShock4Axes.RightTrigger:
                    RightTrigger = value;
                    break;
                case DualShock4Axes.LeftThumbX:
                    LeftThumbX = value;
                    break;
                case DualShock4Axes.LeftThumbY:
                    LeftThumbY = value;
                    break;
                case DualShock4Axes.RightThumbX:
                    RightThumbX = value;
                    break;
                case DualShock4Axes.RightThumbY:
                    RightThumbY = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public void SetAxisState(int axisIndex, int state)
        {
            // ToDo: Normalize values?
            var value = (byte)state;
            switch (axisIndex)
            {
                case 0:
                    LeftThumbX = value;
                    break;
                case 1:
                    LeftThumbY = value;
                    break;
                case 2:
                    RightThumbX = value;
                    break;
                case 3:
                    RightThumbY = value;
                    break;
                case 4:
                    LeftTrigger = value;
                    break;
                case 5:
                    RightTrigger = value;
                    break;
                default:
                    throw new Exception($"Unkown axis index {axisIndex}");
            }

        }

        private static readonly Dictionary<PovDirections, PovVector> IndexToVector = new Dictionary<PovDirections, PovVector>()
        {
            {PovDirections.Up, new PovVector("y", -1)},
            {PovDirections.Right, new PovVector("x", 1)},
            {PovDirections.Down, new PovVector("y", 1)},
            {PovDirections.Left, new PovVector("x", -1)}
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

        // ToDo: Remove, calculate current state from state of report
        private readonly Dictionary<string, int> _povAxisStates = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"x", 0 }, {"y", 0}
        };

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


        public void SetPovDirectionState(PovDirections direction, bool state)
        {
            var mapping = IndexToVector[direction];
            var axisState = _povAxisStates[mapping.Axis];
            var newState = state ? mapping.Direction : 0;
            if (axisState == newState) return;
            _povAxisStates[mapping.Axis] = newState;

            var buttons = (int)Buttons;
            // Clear all the Dpad bits
            buttons &= ~15;

            // Set new Dpad bits
            buttons |= (int)AxisStatesToDpadValue[new PovAxes(_povAxisStates["x"], _povAxisStates["y"])];

            Buttons = (ushort)buttons;

        }
    }
}
