using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

namespace BiangStudio.GridBag
{
    /// <summary>
    /// The class control the display and the background of a grid in a bag
    /// </summary>
    public class BagGrid : PoolObject
    {
        public BagGridInfo Data;

        [SerializeField]
        private Image Image;

        [SerializeField]
        private Text GridPosText;

        [SerializeField]
        private Image BanSign;

        [SerializeField]
        private Color LockedColor;

        [SerializeField]
        private Color AvailableColor;

        [SerializeField]
        private Color UnavailableColor;

        [SerializeField]
        private Color TempUnavailableColor;

        [SerializeField]
        private Color PreviewColor;

        public bool Available => Data.Available;

        public bool Locked => Data.Locked;

        public BagGridInfo.States State
        {
            get => Data.State;
            set => Data.State = value;
        }

        public void Init(BagGridInfo bgi, GridPos gp)
        {
            Data = bgi;
            Data.BanHandler = OnBan;
            Data.SetStateHandler = OnSetState;
#if UNITY_EDITOR
            GridPosText.text = gp.ToString();
#else
            GridPosText.text = "";
#endif
        }

        public void OnBan(bool newValue)
        {
            BanSign.enabled = newValue;
        }

        public void OnSetState(BagGridInfo.States newValue)
        {
            switch (newValue)
            {
                case BagGridInfo.States.Locked:
                {
                    Image.color = LockedColor;
                    break;
                }
                case BagGridInfo.States.Unavailable:
                {
                    Image.color = UnavailableColor;
                    break;
                }
                case BagGridInfo.States.TempUnavailable:
                {
                    Image.color = TempUnavailableColor;
                    break;
                }
                case BagGridInfo.States.Available:
                {
                    Image.color = AvailableColor;
                    break;
                }
                case BagGridInfo.States.Preview:
                {
                    Image.color = PreviewColor;
                    break;
                }
            }
        }
    }
}