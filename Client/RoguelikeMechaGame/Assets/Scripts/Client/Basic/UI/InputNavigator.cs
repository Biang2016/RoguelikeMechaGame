﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    /// <summary>
    /// UGUI Tab键切换InputField
    /// </summary>
    public class InputNavigator : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private EventSystem system;
        private bool isSelect = false;

        void Start()
        {
            system = EventSystem.current;
        }

        private static bool clickTabInFrame = false;

        void Update()
        {
            if (clickTabInFrame) return;
            if (ControlManager.Instance.Common_Tab.Down && isSelect)
            {
                clickTabInFrame = true;
                Selectable next = null;
                Selectable sec = system.currentSelectedGameObject.GetComponent<Selectable>();

                next = sec.FindSelectableOnDown();

                if (next == null) //Recycle
                {
                    Selectable temp = sec;
                    Selectable temp_notNull = temp;
                    while (temp != null)
                    {
                        temp_notNull = temp;
                        temp = temp.FindSelectableOnUp();
                    }

                    next = temp_notNull;
                }

                InputField inputField = next.GetComponent<InputField>();
                if (inputField == null) return;
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }

        void LateUpdate()
        {
            clickTabInFrame = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            isSelect = true;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            isSelect = false;
        }
    }
}