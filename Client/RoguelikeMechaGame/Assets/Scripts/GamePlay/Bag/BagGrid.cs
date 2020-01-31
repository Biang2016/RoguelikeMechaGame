using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// The class control the display and the background of a grid in a bag
/// </summary>
public class BagGrid : PoolObject
{
    [SerializeField] private Image Image;
    [SerializeField] private Color LockedColor;
    [SerializeField] private Color AvailableColor;
    [SerializeField] private Color UnavailableColor;
    [SerializeField] private Color TempUnavailableColor;
    [SerializeField] private Color PreviewColor;

    public enum States
    {
        Locked,
        Unavailable,
        TempUnavailable,
        Available,
        Preview
    }

    private States _state;

    public States State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                switch (value)
                {
                    case States.Locked:
                    {
                        Image.color = LockedColor;
                        break;
                    }
                    case States.Unavailable:
                    {
                        Image.color = UnavailableColor;
                        break;
                    }
                    case States.TempUnavailable:
                    {
                        Image.color = TempUnavailableColor;
                        break;
                    }
                    case States.Available:
                    {
                        Image.color = AvailableColor;
                        break;
                    }
                    case States.Preview:
                    {
                        Image.color = PreviewColor;
                        break;
                    }
                }

                _state = value;
            }
        }
    }

    public bool Available => _state == States.TempUnavailable || _state == States.Available || _state == States.Preview;

    public bool Locked => _state == States.Locked;
}