public class Modifier
{
    public int Value;
    public Sign Sign;

    public Modifier(int value, Sign sign)
    {
        Value = value;
        Sign = sign;
    }

    public int Calculate(int target)
    {
        switch (Sign)
        {
            case Sign.Plus:
            {
                return target + Value;
            }
            case Sign.Subtract:
            {
                return target - Value;
            }
            case Sign.Multiply:
            {
                return target * Value;
            }
            case Sign.Divide:
            {
                return target / Value;
            }
        }

        return 0;
    }

    public float Calculate(float target)
    {
        switch (Sign)
        {
            case Sign.Plus:
            {
                return target + Value;
            }
            case Sign.Subtract:
            {
                return target - Value;
            }
            case Sign.Multiply:
            {
                return target * Value;
            }
            case Sign.Divide:
            {
                return target / Value;
            }
        }

        return 0;
    }
}