using System;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public class MultiControllerManager : MonoSingleton<MultiControllerManager>
    {
        public Dictionary<PlayerNumber, ControllerIndex> PlayerControllerMap = new Dictionary<PlayerNumber, ControllerIndex>();
        public Dictionary<ControllerIndex, Controller> Controllers = new Dictionary<ControllerIndex, Controller>();

        void Awake()
        {
            Init();
        }

        public void Init()
        {
            PlayerControllerMap.Clear();
            Controllers.Clear();
            foreach (object o in Enum.GetValues(typeof(ControllerIndex)))
            {
                ControllerIndex ci = (ControllerIndex) o;
                if ((int) ci <= GameCore.ConfigManager.MaxPlayerNumber_Local)
                {
                    Controllers.Add(ci, new XBoxController());
                    Controllers[ci].Init(ci);
                }

                if ((int) ci > GameCore.ConfigManager.MaxPlayerNumber_Local)
                {
                    Controllers.Add(ci, new KeyBoardController());
                    Controllers[ci].Init(ci);
                }
            }
        }

        private void Update()
        {
            foreach (KeyValuePair<ControllerIndex, Controller> kv in Controllers)
            {
                kv.Value.Update();
            }

            foreach (object o in Enum.GetValues(typeof(ControllerIndex)))
            {
                ControllerIndex ci = (ControllerIndex) o;

                Controller c = Controllers[ci];
                bool addPlayer = false;
                if (c is KeyBoardController kbc)
                {
                    if (kbc.ButtonDown[ControlButtons.LeftStickUp])
                    {
                        addPlayer = true;
                    }
                }

                if (c is XBoxController xbc)
                {
                    if (xbc.ButtonDown[ControlButtons.A])
                    {
                        addPlayer = true;
                    }
                }

                if (addPlayer)
                {
                    if (!PlayerControllerMap.Values.Contains(ci))
                    {
                       
                    }
                }
            }
        }
    }
}