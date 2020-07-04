using BiangStudio.GameDataFormat;
using UnityEngine;

public class Level : MonoBehaviour
{
    public uint RandomSeed;
    public SRandom SRandom;

    public void Init(uint randomSeed)
    {
        RandomSeed = randomSeed;
        SRandom = new SRandom(randomSeed);
    }
}