using UnityEngine;

namespace Core
{
    public class SetCoreMainCamera : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas != null)
                canvas.worldCamera = global::Core.Core.Instance.Camera;
        }
    }
}