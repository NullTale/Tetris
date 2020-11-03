using UnityEngine;

namespace Core.Effect
{
    public class RotationDeviationModule : ModuleBase
    {
        public ParticleSystem.MinMaxCurve   m_RotationOffsetX;
        public ParticleSystem.MinMaxCurve   m_RotationOffsetY;
        public ParticleSystem.MinMaxCurve   m_RotationOffsetZ;

        //////////////////////////////////////////////////////////////////////////
        public override void Begin()
        {
            gameObject.transform.rotation *= Quaternion.Euler(m_RotationOffsetX.Evaluate(Random.value, Random.value),
                m_RotationOffsetY.Evaluate(Random.value, Random.value),
                m_RotationOffsetZ.Evaluate(Random.value, Random.value));
        }
    }
}