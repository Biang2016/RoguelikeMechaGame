public struct MechaComponentPos
{
    public int x;
    public int y;
    public Rotation rotation;

    public MechaComponentPos(int x, int y)
    {
        this.x = x;
        this.y = y;
        rotation = Rotation.None;
    }

    public MechaComponentPos(int x, int y, Rotation rotation)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
    }

    public enum Rotation
    {
        None,
        Clockwise_90,
        Clockwise_180,
        Clockwise_270,
    }
}