﻿using Meadow.Hardware;
using System;

namespace Meadow.Foundation.Sensors.Motion
{
    /// <summary>
    ///     Create a new Parallax PIR object.
    /// </summary>
    public class Hcsens0040
    {
        #region Member variables and fields

        /// <summary>
        ///     Digital input port
        /// </summary>
        private readonly IDigitalInputPort _digitalInputPort;

        #endregion Member variables and fields

        #region Delegates and events

        /// <summary>
        ///     Delgate for the motion start and end events.
        /// </summary>
        public delegate void MotionChange(object sender);

        /// <summary>
        ///     Event raised when motion is detected.
        /// </summary>
        public event MotionChange OnMotionDetected;

        #endregion Delegates and events

        #region Constructors

        /// <summary>
        /// Default constructor is private to prevent it being called.
        /// </summary>
        private Hcsens0040() { }

        /// <summary>
        /// Create a new Parallax PIR object connected to an input pin and IO Device.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="inputPin"></param>        
        public Hcsens0040(IIODevice device, IPin pin) : 
            this (device.CreateDigitalInputPort(pin, InterruptMode.EdgeRising, ResistorMode.PullDown)) { }

        /// <summary>
        /// Create a new Parallax PIR object connected to a interrupt port.
        /// </summary>
        /// <param name="digitalInputPort"></param>        
        public Hcsens0040(IDigitalInputPort digitalInputPort)
        {
            if (digitalInputPort != null)
            {
                _digitalInputPort = digitalInputPort;
                _digitalInputPort.Changed += DigitalInputPortChanged;
            }
            else
            {
                throw new Exception("Invalid pin for the PIR interrupts.");
            }
        }

        #endregion Constructors

        #region Interrupt handlers

        /// <summary>
        ///     Catch the PIR motion change interrupts and work out which interrupt should be raised.
        /// </summary>
        private void DigitalInputPortChanged(object sender, DigitalInputPortEventArgs e)
        {
            if (_digitalInputPort.State == true)
            {
                OnMotionDetected?.Invoke(this);
            }
        }

        #endregion Interrupt handlers
    }
}