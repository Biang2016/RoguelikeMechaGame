using UnityEngine;
using BiangStudio.Singleton;

namespace Client
{
    public class LevelManager : TSingletonBaseManager<LevelManager>
    {
        public Level CurrentLevel;

        public override void Awake()
        {
        }

        public override void Start()
        {
        }

        public override void Update()
        {
           
        }

        public void StartLevel()
        {
        }
    }
}