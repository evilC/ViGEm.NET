using System;
using System.Collections.Generic;

namespace Nefarius.ViGEm.Client.Targets.Xbox360
{
    [Flags]
    public enum Xbox360Buttons : ushort
    {
        Up = 0x0001,
        Down = 0x0002,
        Left = 0x0004,
        Right = 0x0008,
        Start = 0x0010,
        Back = 0x0020,
        LeftThumb = 0x0040,
        RightThumb = 0x0080,
        LeftShoulder = 0x0100,
        RightShoulder = 0x0200,
        Guide = 0x0400,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000
    }

    public enum Xbox360Axes
    {
        LeftTrigger,
        RightTrigger,
        LeftThumbX,
        LeftThumbY,
        RightThumbX,
        RightThumbY
    }

    public class Xbox360Report : IViGEmReport
    {
        /// <summary>
        ///     Bitmask of the device digital buttons.
        ///     A set bit indicates that the corresponding button is pressed.
        /// </summary>
        public ushort Buttons { get; set; }

        /// <summary>
        ///     The current value of the left trigger analog control. The value is between 0 and 255.
        /// </summary>
        public byte LeftTrigger { get; set; }

        /// <summary>
        ///     The current value of the right trigger analog control. The value is between 0 and 255.
        /// </summary>
        public byte RightTrigger { get; set; }

        /// <summary>
        ///     Left thumbstick x-axis value.
        ///     Each of the thumbstick axis members is a signed value between -32768 and 32767 describing the position of the
        ///     thumbstick. A value of 0 is centered. Negative values signify down or to the left. Positive values signify up or to
        ///     the right.
        /// </summary>
        public short LeftThumbX { get; set; }

        /// <summary>
        ///     Left thumbstick y-axis value.
        ///     Each of the thumbstick axis members is a signed value between -32768 and 32767 describing the position of the
        ///     thumbstick. A value of 0 is centered. Negative values signify down or to the left. Positive values signify up or to
        ///     the right.
        /// </summary>
        public short LeftThumbY { get; set; }

        /// <summary>
        ///     Right thumbstick x-axis value.
        ///     Each of the thumbstick axis members is a signed value between -32768 and 32767 describing the position of the
        ///     thumbstick. A value of 0 is centered. Negative values signify down or to the left. Positive values signify up or to
        ///     the right.
        /// </summary>
        public short RightThumbX { get; set; }

        /// <summary>
        ///     Right thumbstick y-axis value.
        ///     Each of the thumbstick axis members is a signed value between -32768 and 32767 describing the position of the
        ///     thumbstick. A value of 0 is centered. Negative values signify down or to the left. Positive values signify up or to
        ///     the right.
        /// </summary>
        public short RightThumbY { get; set; }

        private readonly List<Xbox360Buttons> _buttonFlags = new List<Xbox360Buttons>
        {
            Xbox360Buttons.A, Xbox360Buttons.B, Xbox360Buttons.X, Xbox360Buttons.Y,
            Xbox360Buttons.LeftShoulder, Xbox360Buttons.RightShoulder, Xbox360Buttons.LeftThumb, Xbox360Buttons.RightThumb,
            Xbox360Buttons.Back, Xbox360Buttons.Start
        };

        public void SetButtonState(int buttonIndex, bool state)
        {
            var button = _buttonFlags[buttonIndex];
            SetButtonState(button, state);
        }

        public void SetButtonState(Xbox360Buttons button, bool state)
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

        public void SetAxisState(int axisIndex, int state)
        {
            // ToDo: Normalize values?
            var value = (short)state;
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
                    LeftTrigger = (byte)value;
                    break;
                case 5:
                    RightTrigger = (byte)value;
                    break;
                default:
                    throw new Exception($"Unknown axis {axisIndex}");
            }
        }

        private readonly Dictionary<PovDirections, Xbox360Buttons> _povDirectionMappings = new Dictionary<PovDirections, Xbox360Buttons>
        {
            {PovDirections.Up, Xbox360Buttons.Up }, {PovDirections.Right, Xbox360Buttons.Right},
            { PovDirections.Down, Xbox360Buttons.Down}, {PovDirections.Left, Xbox360Buttons.Left}
        };

        public void SetPovDirectionState(PovDirections direction, bool state)
        {
            SetButtonState(_povDirectionMappings[direction], state);
        }
    }
}