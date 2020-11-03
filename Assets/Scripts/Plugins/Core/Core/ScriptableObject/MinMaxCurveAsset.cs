using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "MinMaxCurve", menuName = "Curve/MinMaxCurve")]
    public class MinMaxCurveAsset : ScriptableObject
    {
        public ParticleSystem.MinMaxCurve	m_Curve = new ParticleSystem.MinMaxCurve();
    }
}