using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-1000)]
public sealed class DontDestroyOnLoad : MonoBehaviour
{
    [Serializable]
    public enum HasParentBehavior
    {
        None,
        Unparent,
        MoveParent
    }

    //////////////////////////////////////////////////////////////////////////
    [SerializeField]
    private bool                    m_Enable;
    [SerializeField]
    private HasParentBehavior       m_HasParentBehavior;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        if (m_Enable)
            if (transform.parent == null)
                GameObject.DontDestroyOnLoad(gameObject);
            else
                // implement has parent behavior
                switch (m_HasParentBehavior)
                {
                    case HasParentBehavior.Unparent:
                        // set this object to root
                        gameObject.transform.SetParent(null, true);
                        StartCoroutine(_WaitFrameAndDo());
                        break;
                    case HasParentBehavior.MoveParent:
                        // dont destroy on load root
                        GameObject.DontDestroyOnLoad(gameObject.transform.root);
                        break;
                    case HasParentBehavior.None:
                    // do nothing
                        break;

                    default:
                        break;
                }
    }

    //////////////////////////////////////////////////////////////////////////
    private IEnumerator _WaitFrameAndDo()
    {
        yield return null;
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
