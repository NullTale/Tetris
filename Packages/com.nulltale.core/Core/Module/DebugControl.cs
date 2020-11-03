using System;
using UnityEngine;

namespace Core.Module
{
    [CreateAssetMenu(fileName = nameof(DebugControl), menuName = Core.c_CoreModuleMenu + nameof(DebugControl))]
    public class DebugControl : Core.Module
    {
        [SerializeField]
        private Core.ProjectSpace   m_ProjectSpace;

        [SerializeField]
        private float               m_MouseScrollScale = 1.0f;
        [SerializeField]
        private float               m_MouseMoveScale = 0.06f;
        [SerializeField]
        private float               m_KeyboardMoveScale = 10.0f;

        private Vector3             m_MousePosLast;

	    private Vector3             m_ForvardVector;
	    private Vector3             m_RightVector;
	    private Vector3             m_UpVector;

        [SerializeField]
        private Core.MouseButton    m_DragMouseButton;

        [SerializeField]
        private bool                m_EnableArrowKeysMovement = true;

	    //////////////////////////////////////////////////////////////////////////
        public override void Init()
	    {
            // init space
		    hideFlags = HideFlags.HideInInspector;
		    switch (m_ProjectSpace)
		    {
			    case Core.ProjectSpace.XY:
				    m_ForvardVector = Vector3.up;
				    m_RightVector = Vector3.right;
				    break;
			    case Core.ProjectSpace.XZ:
				    m_ForvardVector = Vector3.forward;
				    m_RightVector = Vector3.right;
				    break;
			    default:
				    m_ForvardVector = Vector3.up;
				    m_RightVector = Vector3.right;
				    break;
		    }

            // set up vector
		    m_UpVector = Vector3.Cross(m_ForvardVector, m_RightVector);

            // create updater
            Core.Instance.gameObject.AddComponent<OnUpdateCallback>().Action = _Update;
	    }

	    private void _Update()
	    {
            // arrow keys movement
		    if (m_EnableArrowKeysMovement)
		    {
			    var translateVector = Vector3.zero;

                // sum vectors, calculate move normal
			    if (Input.GetKey(KeyCode.UpArrow))
				    translateVector += m_ForvardVector;
			    if (Input.GetKey(KeyCode.DownArrow))
				    translateVector -= m_ForvardVector;
			    if (Input.GetKey(KeyCode.LeftArrow))
				    translateVector -= m_RightVector;
			    if (Input.GetKey(KeyCode.RightArrow))
				    translateVector += m_RightVector;
			    
			    if (Input.GetKey(KeyCode.RightShift))
				    translateVector += m_UpVector;
			    if (Input.GetKey(KeyCode.RightControl))
				    translateVector -= m_UpVector;

                // move by normal if has vector
			    if (translateVector != Vector3.zero)
			    {
				    translateVector.Normalize();
                    Core.Instance.Camera.gameObject.transform.position += (translateVector * m_KeyboardMoveScale * UnityEngine.Time.deltaTime);
                }
		    }

            // implement drag
		    if (m_DragMouseButton != Core.MouseButton.None)
		    {
			    var view = Core.Instance.Camera.ScreenToViewportPoint(Input.mousePosition);

			    if ((view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1) == false)
			    {
				    if (Input.GetMouseButton((int)m_DragMouseButton))
				    {
					    var offset = m_MousePosLast - Input.mousePosition;
					    if (offset.magnitude < 40.0f)
						    switch (m_ProjectSpace)
						    {
							    case Core.ProjectSpace.XY:
								    Core.Instance.Camera.transform.position += ((offset * m_MouseMoveScale).WithZ(0.0f));
								    break;
							    case Core.ProjectSpace.XZ:
								    Core.Instance.Camera.transform.position += ((offset * m_MouseMoveScale).WithZ(0.0f)).XZY();
								    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
				    }

                    // implement scroll
					var scrollImpact = Input.mouseScrollDelta.y * m_MouseScrollScale;

					if (Core.Instance.Camera.orthographic)
						Core.Instance.Camera.orthographicSize = Mathf.Clamp(Core.Instance.Camera.orthographicSize - scrollImpact, 1.0f, int.MaxValue);
					else
					{
						switch (m_ProjectSpace)
						{
							case Core.ProjectSpace.XY:
								Core.Instance.Camera.transform.Translate(scrollImpact.ToVector3Z());
								break;
							case Core.ProjectSpace.XZ:
								Core.Instance.Camera.transform.Translate(scrollImpact.ToVector3Y());
								break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
					}
                }

			    m_MousePosLast = Input.mousePosition;
		    }
	    }
    }
}