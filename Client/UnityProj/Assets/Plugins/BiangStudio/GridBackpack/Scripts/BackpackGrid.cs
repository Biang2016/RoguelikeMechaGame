using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

namespace BiangStudio.GridBackpack
{
    /// <summary>
    /// The class control the display and the background of a grid in a backpack
    /// </summary>
    public class BackpackGrid : PoolObject
    {
        public BackpackGridInfo Data;

        [SerializeField] private Image Image;
        [SerializeField] private Text GridPosText;
        [SerializeField] private Color LockedColor;
        [SerializeField] private Color AvailableColor;
        [SerializeField] private Color UnavailableColor;
        [SerializeField] private Color TempUnavailableColor;
        [SerializeField] private Color PreviewColor;

        internal bool Available => Data.Available;

        internal bool Locked => Data.Locked;

        internal BackpackGridInfo.States State
        {
            get => Data.State;
            set => Data.State = value;
        }

        internal void Init(BackpackGridInfo bgi, GridPos gp)
        {
            Data = bgi;
            Data.SetStateHandler = OnSetState;
#if UNITY_EDITOR
            GridPosText.text = gp.ToString();
#else
            GridPosText.text = "";
#endif
        }

        internal void OnSetState(BackpackGridInfo.States newValue)
        {
            switch (newValue)
            {
                case BackpackGridInfo.States.Locked:
                {
                    Image.color = LockedColor;
                    break;
                }
                case BackpackGridInfo.States.Unavailable:
                {
                    Image.color = UnavailableColor;
                    break;
                }
                case BackpackGridInfo.States.TempUnavailable:
                {
                    Image.color = TempUnavailableColor;
                    break;
                }
                case BackpackGridInfo.States.Available:
                {
                    Image.color = AvailableColor;
                    break;
                }
                case BackpackGridInfo.States.Preview:
                {
                    Image.color = PreviewColor;
                    break;
                }
            }
        }
    }
}