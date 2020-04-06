using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Rotary;
using System;

namespace Meadow.Foundation.Sensors.Rotary
{
    /// <summary>
    /// Digital rotary encoder that uses two-bit Gray Code to encode rotation.
    /// </summary>
    public class RotaryEncoder : IRotaryEncoder
    {
        #region Properties

        /// <summary>
        /// Returns the pin connected to the A-phase output on the rotary encoder.
        /// </summary>
        public IDigitalInputPort APhasePort { get; }

        /// <summary>
        /// Returns the pin connected to the B-phase output on the rotary encoder.
        /// </summary>
        public IDigitalInputPort BPhasePort { get; }

        /// <summary>
        /// Raised when the rotary encoder is rotated and returns a RotaryTurnedEventArgs object which describes the direction of rotation.
        /// </summary>
        public event RotaryTurnedEventHandler Rotated = delegate { };

        #endregion

        #region Member variables / fields

        /// <summary>
        /// Whether or not we're processing the gray code (encoding of rotational information)
        /// </summary>
        protected bool _processing = false;

        /// <summary>
        /// Two sets of gray code results to determine direction of rotation 
        /// </summary>
        protected TwoBitGrayCode _results = new TwoBitGrayCode();

        public TwoBitGrayCode Results { get { return _results; } }
        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate a new RotaryEncoder on the specified pins.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="aPhasePin"></param>
        /// <param name="bPhasePin"></param>
        public RotaryEncoder(IIODevice device, IPin aPhasePin, IPin bPhasePin)
        {
           APhasePort =  device.CreateDigitalInputPort(aPhasePin, InterruptMode.EdgeBoth, ResistorMode.PullUp, 200);
           BPhasePort =  device.CreateDigitalInputPort(bPhasePin, InterruptMode.EdgeBoth, ResistorMode.PullUp, 200);

           APhasePort.Changed += PhasePinAChanged; 
           BPhasePort.Changed += PhasePinBChanged;
        }
        /// <summary>
        /// Instantiate a new RotaryEncoder on the specified ports
        /// </summary>
        /// <param name="aPhasePort"></param>
        /// <param name="bPhasePort"></param>
        public RotaryEncoder(IDigitalInputPort aPhasePort, IDigitalInputPort bPhasePort)
        {
            APhasePort = aPhasePort;
            BPhasePort = bPhasePort;

            APhasePort.Changed += PhasePinAChanged; // only need to monitor one of the ports
            BPhasePort.Changed += PhasePinBChanged;
        }

        #endregion

        #region Methods
        private void PhasePinAChanged(object sender, DigitalInputPortEventArgs e)
        {
            //Console.WriteLine((!_processing ? "1st result: " : "2nd result: ") + "A{" + (APhasePin.Read() ? "1" : "0") + "}, " + "B{" + (BPhasePin.Read() ? "1" : "0") + "}");
            //Console.WriteLine($"Current Port State - A={APhasePort.State} ; B={BPhasePort.State}");
            // the first time through (not processing) store the result in array slot 0.
            // second time through (is processing) store the result in array slot 1.
            _results.APhase = APhasePort.State;
            _results.BPhase = BPhasePort.State;
            //_results[_processing ? 1 : 0].APhase = APhasePort.State;
            //_results[_processing ? 1 : 0].BPhase = BPhasePort.State;

            // if this is the second result that we're reading, we should now have 
            // enough information to know which way it's turning, so process the
            // gray code

            //if (_processing) This if statement made it so it was only reading every 2nd turn.
            ProcessRotationResults();


            // toggle our processing flag
            // _processing = !_processing; This misses reading. 
        }
        private void PhasePinBChanged(object sender, DigitalInputPortEventArgs e)
        {
            //Console.WriteLine((!_processing ? "1st result: " : "2nd result: ") + "A{" + (APhasePin.Read() ? "1" : "0") + "}, " + "B{" + (BPhasePin.Read() ? "1" : "0") + "}");
            //Console.WriteLine($"Current Port State - A={APhasePort.State} ; B={BPhasePort.State}");
            // the first time through (not processing) store the result in array slot 0.
            // second time through (is processing) store the result in array slot 1.
            _results.APhase = APhasePort.State;
            _results.BPhase = BPhasePort.State;
            //_results[_processing ? 1 : 0].APhase = APhasePort.State;
            //_results[_processing ? 1 : 0].BPhase = BPhasePort.State;

            // if this is the second result that we're reading, we should now have 
            // enough information to know which way it's turning, so process the
            // gray code

            //if (_processing) This if statement made it so it was only reading every 2nd turn.
            ProcessRotationResults();


            // toggle our processing flag
            // _processing = !_processing; This misses reading. 
        }

        /// <summary>
        /// Determines the direction of rotation when the PhasePinChanged event is triggered
        /// </summary>
        protected void ProcessRotationResults()
        {
            //If BPhase is equal to APhase then the A Phase has changed first and it is rotating clockwise
            OnRaiseRotationEvent(_results.APhase == _results.BPhase ? RotationDirection.Clockwise : RotationDirection.CounterClockwise);


            //// if there hasn't been any change, then it's a garbage reading. so toss it out.
            //if (_results[0].APhase == _results[1].APhase &&
            //    _results[0].BPhase == _results[1].BPhase)
            //    //Console.WriteLine("Garbage");
            //    return;
            // Console.WriteLine($"Result[0] APhase - {_results[0].APhase}; BPhase - {_results[0].BPhase}");
            // Console.WriteLine($"Result[1] APhase - {_results[1].APhase}; BPhase - {_results[1].BPhase}");
            // start by reading the a phase pin. if it's High


            //if (_results[0].APhase)
            //{
            //    // if b phase went down, then it spun counter-clockwise
            //    OnRaiseRotationEvent(_results[1].BPhase ? RotationDirection.Clockwise : RotationDirection.CounterClockwise);
            //}
            //// if a phase is low
            //else
            //{
            //    // if b phase went up, then it spun counter-clockwise
            //    OnRaiseRotationEvent(_results[1].BPhase ? RotationDirection.CounterClockwise : RotationDirection.Clockwise);
            //}
        }

        /// <summary>
        /// Invokes the RotaryTurnedEventHandler, passing the direction in the RotaryTurnedEventArgs
        /// </summary>
        /// <param name="direction"></param>
        protected void OnRaiseRotationEvent(RotationDirection direction)
        {
            Rotated?.Invoke(this, new RotaryTurnedEventArgs(direction));
        }

        #endregion
    }
}