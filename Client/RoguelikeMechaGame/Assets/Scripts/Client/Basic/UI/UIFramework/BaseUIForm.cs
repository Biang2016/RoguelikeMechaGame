using UnityEngine;
using UnityEngine.EventSystems;

namespace Client
{
    public class BaseUIForm : MonoBehaviour
    {
        public UIType UIType = new UIType();

        #region  窗体的四种(生命周期)状态

        private bool closeFlag = false;

        void Awake()
        {
        }

        void Update()
        {
            if (UIType.IsESCClose)
            {
                if (ControlManager.Instance.Common_Exit.Down)
                {
                    BaseUIForm peek = UIManager.Instance.GetPeekUIForm();
                    if (peek == null || peek == this)
                    {
                        closeFlag = true;
                        return;
                    }
                }
            }

            if (UIType.IsClickElsewhereClose)
            {
                bool isClickElseWhere = (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetMouseButtonDown(1);
                if (isClickElseWhere)
                {
                    BaseUIForm peek = UIManager.Instance.GetPeekUIForm();
                    if (peek == null || peek == this)
                    {
                        closeFlag = true;
                        return;
                    }
                }
            }

            ChildUpdate();
        }

        private void LateUpdate()
        {
            if (closeFlag)
            {
                CloseUIForm();
                closeFlag = false;
            }
        }

        protected virtual void ChildUpdate()
        {
        }

        public virtual void Display()
        {
            gameObject.SetActive(true);
            UIMaskMgr.Instance.SetMaskWindow(gameObject, UIType.UIForms_Type, UIType.UIForm_LucencyType);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            UIMaskMgr.Instance.CancelAllMaskWindow(UIType.UIForm_LucencyType);
        }

        public virtual void Freeze()
        {
            gameObject.SetActive(true);
        }

        public void CloseUIForm()
        {
            string UIFormName = GetType().Name;
            UIManager.Instance.CloseUIForm(UIFormName);
        }

        #endregion
    }
}
