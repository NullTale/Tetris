using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[DefaultExecutionOrder(10)]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraMimic : MonoBehaviour
{
    private Camera      m_SelfCamera;
    [SerializeField]
    private Camera      m_Camera;

    [SerializeField]
    private bool		    m_CopyProjection = true;
    public bool				CopyProjection => m_CopyProjection;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        m_SelfCamera = GetComponent<Camera>();
    }

    public void Update()
    {
        transform.position = m_Camera.transform.position;
        transform.rotation = m_Camera.transform.rotation;

        if (m_CopyProjection)
        {
            m_SelfCamera.orthographic = m_Camera.orthographic;
            m_SelfCamera.orthographicSize = m_Camera.orthographicSize;
            m_SelfCamera.aspect = m_Camera.aspect;
        }
    }
}
