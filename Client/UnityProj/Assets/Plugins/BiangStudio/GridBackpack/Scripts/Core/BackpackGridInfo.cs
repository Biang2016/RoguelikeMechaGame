﻿using System;

namespace BiangStudio.GridBackpack
{
    [Serializable]
    public class BackpackGridInfo
    {
        internal delegate void SetStateDelegate(States newValue);

        internal SetStateDelegate SetStateHandler;

        public enum States
        {
            Locked,
            Unavailable,
            TempUnavailable,
            Available,
            Preview
        }

        private States state;

        public States State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    SetStateHandler?.Invoke(value);
                    state = value;
                }
            }
        }

        public bool Available => State == States.TempUnavailable || State == States.Available || State == States.Preview;

        public bool Locked => State == States.Locked;
    }
}