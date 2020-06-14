namespace Client
{
    public class Player
    {
        public PlayerNumber PlayerNumber;

        public void Initialize(PlayerNumber playerNumber)
        {
            PlayerNumber = playerNumber;
        }
    }

    public enum PlayerNumber
    {
        Player1 = 0,
        Player2 = 1,
        Player3 = 2,
        Player4 = 3,
        AnyPlayer = 15
    }
}