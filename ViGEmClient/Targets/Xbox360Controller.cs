using System;
using System.Collections.Generic;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace Nefarius.ViGEm.Client.Targets
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an emulated wired Microsoft Xbox 360 Controller.
    /// </summary>
    public class Xbox360Controller : ViGEmTarget
    {
        public Xbox360Report Report { get; } = new Xbox360Report();

        private ViGEmClient.PVIGEM_X360_NOTIFICATION _notificationCallback;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Nefarius.ViGEm.Client.Targets.Xbox360Controller" /> class bound to a
        ///     <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" />.
        /// </summary>
        /// <param name="client">The <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> this device is attached to.</param>
        public Xbox360Controller(ViGEmClient client) : base(client)
        {
            NativeHandle = ViGEmClient.vigem_target_x360_alloc();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Nefarius.ViGEm.Client.Targets.Xbox360Controller" /> class bound to a
        ///     <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> overriding the default Vendor and Product IDs with the provided
        ///     values.
        /// </summary>
        /// <param name="client">The <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> this device is attached to.</param>
        /// <param name="vendorId">The Vendor ID to use.</param>
        /// <param name="productId">The Product ID to use.</param>
        public Xbox360Controller(ViGEmClient client, ushort vendorId, ushort productId) : this(client)
        {
            VendorId = vendorId;
            ProductId = productId;
        }

        /// <summary>
        ///     Submits an <see cref="Xbox360Report"/> to this device which will update its state.
        /// </summary>
        /// <param name="report">The <see cref="Xbox360Report"/> to submit.</param>
        public void SendReport(Xbox360Report report)
        {
            // Convert managed to unmanaged structure
            var submit = new ViGEmClient.XUSB_REPORT
            {
                wButtons = report.Buttons,
                bLeftTrigger = report.LeftTrigger,
                bRightTrigger = report.RightTrigger,
                sThumbLX = report.LeftThumbX,
                sThumbLY = report.LeftThumbY,
                sThumbRX = report.RightThumbX,
                sThumbRY = report.RightThumbY
            };

            var error = ViGEmClient.vigem_target_x360_update(Client.NativeHandle, NativeHandle, submit);

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
            _notificationCallback = (client, target, largeMotor, smallMotor, number) => FeedbackReceived?.Invoke(this,
                new Xbox360FeedbackReceivedEventArgs(largeMotor, smallMotor, number));

            var error = ViGEmClient.vigem_target_x360_register_notification(Client.NativeHandle, NativeHandle,
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
            ViGEmClient.vigem_target_x360_unregister_notification(NativeHandle);

            base.Disconnect();
        }

        public override void SendReport()
        {
            SendReport(Report);
        }

        private readonly List<Xbox360Buttons> _buttonFlags = new List<Xbox360Buttons>
        {
            Xbox360Buttons.A, Xbox360Buttons.B, Xbox360Buttons.X, Xbox360Buttons.Y,
            Xbox360Buttons.LeftShoulder, Xbox360Buttons.RightShoulder, Xbox360Buttons.LeftThumb, Xbox360Buttons.RightThumb,
            Xbox360Buttons.Back, Xbox360Buttons.Start
        };

        public override void SetButtonState(int buttonIndex, bool state)
        {
            var button = _buttonFlags[buttonIndex];
            SetButtonState(button, state);
        }

        public void SetButtonState(Xbox360Buttons button, bool state)
        {
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
            var value = (short) state;
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
                    Report.LeftTrigger = (byte)value;
                    break;
                case 5:
                    Report.RightTrigger = (byte)value;
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

        public override void SetPovDirectionState(PovDirections direction, bool state)
        {
            SetButtonState(_povDirectionMappings[direction], state);
        }

        public event Xbox360FeedbackReceivedEventHandler FeedbackReceived;
    }

    public delegate void Xbox360FeedbackReceivedEventHandler(object sender, Xbox360FeedbackReceivedEventArgs e);
}