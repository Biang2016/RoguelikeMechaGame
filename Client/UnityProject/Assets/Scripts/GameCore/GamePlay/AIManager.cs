using System.Collections.Generic;
using BiangStudio.Singleton;
using NodeCanvas.BehaviourTrees;

namespace GameCore
{
    public class AIManager : TSingletonBaseManager<AIManager>
    {
        private float AITickInterval;

        private List<BehaviourTreeOwner> BehaviourTreeOwners = new List<BehaviourTreeOwner>();

        public void Clear()
        {
        }

        public void Init(float aiTickInterval)
        {
            AITickInterval = aiTickInterval;
            aiTick = 0;
        }

        public override void Awake()
        {
        }

        public override void Start()
        {
        }

        public void AddBehaviourTreeOwner(BehaviourTreeOwner bto)
        {
            BehaviourTreeOwners.Add(bto);
        }

        public void RemoveBehaviourTreeOwner(BehaviourTreeOwner bto)
        {
            BehaviourTreeOwners.Remove(bto);
        }

        private float aiTick = 0;

        public override void Update(float deltaTime)
        {
            aiTick += deltaTime;
            if (aiTick > AITickInterval)
            {
                aiTick -= AITickInterval;
                AITick(AITickInterval);
            }
        }

        public void AITick(float aiTickInterval)
        {
            //foreach (BehaviourTreeOwner bto in BehaviourTreeOwners)
            //{
            //    bto.graph.UpdateGraph(aiTickInterval);
            //}
        }
    }
}