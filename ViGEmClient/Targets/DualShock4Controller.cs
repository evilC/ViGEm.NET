using System;
using System.Collections.Generic;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace Nefarius.ViGEm.Client.Targets
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an emulated wired Sony DualShock 4 Controller.
    /// </summary>
    public class DualShock4Controller : ViGEmTarget
    {
        private ViGEmClient.PVIGEM_DS4_NOTIFICATION _notificationCallback;

        public DualShock4Report Report { get; } = new DualShock4Report();

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Nefarius.ViGEm.Client.Targets.DualShock4Controller" /> class bound
        ///     to a <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" />.
        /// </summary>
        /// <param name="client">The <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> this device is attached to.</param>
        public DualShock4Controller(ViGEmClient client) : base(client)
        {
            NativeHandle = ViGEmClient.vigem_target_ds4_alloc();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Nefarius.ViGEm.Client.Targets.DualShock4Controller" /> class bound
        ///     to a <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> overriding the default Vendor and Product IDs with the
        ///     provided values.
        /// </summary>
        /// <param name="client">The <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> this device is attached to.</param>
        /// <param name="vendorId">The Vendor ID to use.</param>
        /// <param name="productId">The Product ID to use.</param>
        public DualShock4Controller(ViGEmClient client, ushort vendorId, ushort productId) : this(client)
        {
            VendorId = vendorId;
            ProductId = productId;
        }

        /// <summary>
        ///     Submits an <see cref="DualShock4Report"/> to this device which will update its state.
        /// </summary>
        /// <param name="report">The <see cref="DualShock4Report"/> to submit.</param>
        public void SendReport(DualShock4Report report)
        {
            // Convert managed to unmanaged structure
            var submit = new ViGEmClient.DS4_REPORT
            {
                wButtons = report.Buttons,
                bSpecial = report.SpecialButtons,
                bThumbLX = report.LeftThumbX,
                bThumbLY = report.LeftThumbY,
                bThumbRX = report.RightThumbX,
                bThumbRY = report.RightThumbY,
                bTriggerL = report.LeftTrigger,
                bTriggerR = report.RightTrigger
            };

            var error = ViGEmClient.vigem_target_ds4_update(Client.NativeHandle, NativeHandle, submit);

            switch (error)
            {
                case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
                    throw new VigemBusNotFoundException();
                case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
                    throw new VigemInvalidTargetException();
            }
        }

        public override void Connect()
        {
            base.Connect();

            //
            // Callback to event
            // 
            _notificationCallback = (client, target, motor, smallMotor, color) => FeedbackReceived?.Invoke(this,
                new DualShock4FeedbackReceivedEventArgs(motor, smallMotor,
                    new LightbarColor(color.Red, color.Green, color.Blue)));

            var error = ViGEmClient.vigem_target_ds4_register_notification(Client.NativeHandle, NativeHandle,
                _notificationCallback);

            switch (error)
            {
                case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
                    throw new VigemBusNotFoundException();
                case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
                    throw new VigemInvalidTargetException();
                case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_CALLBACK_ALREADY_REGISTERED:
                    throw new VigemCallbackAlreadyRegisteredException();
            }
        }

        public override void Disconnect()
        {
            ViGEmClient.vigem_target_ds4_unregister_notification(NativeHandle);

            base.Disconnect();
        }

        public override void SendReport()
        {
            SendReport(Report);
        }

        private readonly List<DualShock4Buttons> _buttonFlags = new List<DualShock4Buttons>
        {
            DualShock4Buttons.Cross, DualShock4Buttons.Circle, DualShock4Buttons.Square, DualShock4Buttons.Triangle,
            DualShock4Buttons.ShoulderLeft, DualShock4Buttons.ShoulderRight, DualShock4Buttons.ThumbLeft, DualShock4Buttons.ThumbRight,
            DualShock4Buttons.Options, DualShock4Buttons.Share,
            DualShock4Buttons.TriggerLeft, DualShock4Buttons.TriggerRight
        };

        public override void SetButtonState(int buttonIndex, bool state)
        {
            var button = _buttonFlags[buttonIndex];
            if (state)
            {
                Report.Buttons |= (ushort)button;
            }
            else
            {
                Report.Buttons &= (ushort)~button;
            }
        }

        public override void SetAxisState(int axisIndex, int state)
        {
            var value = (byte) state;
            switch (axisIndex)
            {
                case 0:
                    Report.LeftThumbX = value;
                    break;
                case 1:
                    Report.LeftThumbY = value;
                    break;
                case 2:
                    Report.RightThumbX = value;
                    break;
                case 3:
                    Report.RightThumbY = value;
                    break;
                case 4:
                    Report.LeftTrigger = value;
                    break;
                case 5:
                    Report.RightTrigger = value;
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


        public override void SetPovDirectionState(PovDirections direction, bool state)
        {
            var mapping = IndexToVector[direction];
            var axisState = _povAxisStates[mapping.Axis];
            var newState = state ? mapping.Direction : 0;
            if (axisState == newState) return;
            _povAxisStates[mapping.Axis] = newState;

            var buttons = (int)Report.Buttons;
            // Clear all the Dpad bits
            buttons &= ~15;

            // Set new Dpad bits
            buttons |= (int)AxisStatesToDpadValue[new PovAxes(_povAxisStates["x"], _povAxisStates["y"])];

            Report.Buttons = (ushort)buttons;
        }

        public event DualShock4FeedbackReceivedEventHandler FeedbackReceived;
    }

    public delegate void DualShock4FeedbackReceivedEventHandler(object sender, DualShock4FeedbackReceivedEventArgs e);
}