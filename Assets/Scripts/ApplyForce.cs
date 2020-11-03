using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class ApplyForce : MonoBehaviour
{
    [Serializable]
    public enum Method
    {
        Random,
        Vector
    }

    //////////////////////////////////////////////////////////////////////////
    [SerializeField]
    private Method       m_Method;
    [SerializeField]
    private ForceMode2D  m_Mode = ForceMode2D.Impulse;
    [SerializeField]
    private float        m_Force;

    [DrawIf(nameof(m_Method), Method.Vector)]
    public Vector2      m_Vector;

    //////////////////////////////////////////////////////////////////////////
    [Button]
    public void Apply()
    {
        Apply(m_Force);
    }

    public void Apply(float force)
    {
        switch (m_Method)
        {
            case Method.Random:
                GetComponent<Rigidbody2D>().AddForce(UnityRandom.Normal2D() * force, m_Mode);
                break;
            case Method.Vector:
                Apply(m_Vector * force, m_Force);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Apply(Vector2 normal, float force)
    {
        GetComponent<Rigidbody2D>().AddForce(normal * force, m_Mode);
    }

    public void Apply(Vector2 normal)
    {
        GetComponent<Rigidbody2D>().AddForce(normal * m_Force, m_Mode);
    }
}
