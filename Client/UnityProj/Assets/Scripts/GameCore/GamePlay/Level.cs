using BiangStudio.GameDataFormat;

namespace GameCore
{
    public class Level
    {
        public uint RandomSeed;
        public SRandom SRandom;

        public void Init(uint randomSeed)
        {
            RandomSeed = randomSeed;
            SRandom = new SRandom(randomSeed);
        }
    }
}