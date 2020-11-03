using System;
using UnityEngine;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(MousePosition), menuName = Core.c_CoreModuleMenu + nameof(MousePosition))]
    public class MousePosition : Core.Module, IWorldPosition
    {
        public static readonly Plane		c_GroundPlaneXY = new Plane(Vector3.forward, 0.0f);
        public static readonly Plane		c_GroundPlaneXZ = new Plane(Vector3.up, 0.0f);
        
        public static MousePosition     Instance;

        [SerializeField]
        private Core.ProjectSpace       m_ProjectSpace;


        public Plane                    GroundPlane     {get; private set;}
        public Vector3                  ScreenPosition  {get; private set;}
        public Vector3                  WorldPosition   {get; private set;}

        public Ray                      CameraRay;

        //////////////////////////////////////////////////////////////////////////
        public override void Init()
        {
            // set instance
            Instance = this;

            // init ground plane
            switch (m_ProjectSpace)
            {
                case Core.ProjectSpace.XY:
                    GroundPlane = c_GroundPlaneXY;
                    break;
                case Core.ProjectSpace.XZ:
                    GroundPlane = c_GroundPlaneXZ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // create updater
            Core.Instance.gameObject.AddComponent<OnUpdateCallback>().Action = Update;
        }

        public void Update()
        {
            // update screen position
            ScreenPosition = Input.mousePosition;

            // update word position
            WorldPosition = GetWordPosition(GroundPlane);
        }

        public Vector3 GetWordPosition(Plane plane)
        {
            CameraRay = Core.Instance.Camera.ScreenPointToRay(new Vector3(ScreenPosition.x, ScreenPosition.y, Core.Instance.Camera.farClipPlane));

            plane.Raycast(CameraRay, out var d);

            return CameraRay.GetPoint(d);
        }

        public Vector3 GetWordPosition(float distance)
        {
            var ray = Core.Instance.Camera.ScreenPointToRay(new Vector3(ScreenPosition.x, ScreenPosition.y, Core.Instance.Camera.farClipPlane));

            return ray.GetPoint(distance);
        }

        public Vector3 GetWorldPosition()
        {
            return WorldPosition;
        }

        public static implicit operator Vector3(MousePosition mouseWorldPos)
        {
            return mouseWorldPos.WorldPosition;
        }
    }
}