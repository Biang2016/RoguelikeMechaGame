using System;

namespace GameCore
{
    [Serializable]
    public class BagGridInfo
    {
        public delegate void OnBanDelegate(bool newValue);

        public OnBanDelegate OnBanHandler;

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
                    OnSetStateHandler?.Invoke(value);
                    state = value;
                }
            }
        }

        public delegate void OnSetStateDelegate(States newValue);

        public OnSetStateDelegate OnSetStateHandler;

        public bool Available => State == States.TempUnavailable || State == States.Available || State == States.Preview;

        public bool Locked => State == States.Locked;
    }
}