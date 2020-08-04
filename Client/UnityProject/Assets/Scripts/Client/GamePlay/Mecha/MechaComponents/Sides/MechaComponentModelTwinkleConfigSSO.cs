using UnityEngine;

namespace Client
{
    [CreateAssetMenu(menuName = "Model/ModelTwinkleConfig")]
    public class MechaComponentModelTwinkleConfigSSO : ScriptableObject
    {
        public Gradient DamageGradient;
        public AnimationCurve DamageIntensityCurve;
        public float DamageTwinkleDuration;
        //public Gradient PowerGradient;
        public AnimationCurve PowerIntensityCurve;
        public float PowerTwinkleDuration;
    }
}