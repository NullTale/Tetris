using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BlockVisualizer : MonoBehaviour
{
    private static readonly int     k_DestroyTrigger = Animator.StringToHash("Destroy");

    [SerializeField]
    private GameObject              m_Anchor;
    public GameObject               Anchor => m_Anchor;
    [SerializeField]
    private GameObject              m_View;
    public GameObject               View => m_View;

    private ShapeVisualizer         m_Shape;
    [SerializeField]
    private Vector2Int              m_Position;

    public bool                     IsActiveShape => m_Shape.IsActiveShape;

    public Vector2Int               Position
    {
        get => m_Position;
        set
        {
            // save position
            m_Position = value;

            // move anchor
            _UpdatePosition();
        }
    }

    private Vector2                 m_PositionOffset;
    public Vector2                  PositionOffset
    {
        get => m_PositionOffset;
        set
        {
            m_PositionOffset = value;
            // move anchor
            _UpdatePosition();
        }
    }

    //////////////////////////////////////////////////////////////////////////
    public void Init(ShapeVisualizer shape, Vector2Int pos)
    {
        // save shape
        m_Shape = shape;

        // translate position
        m_Position += pos;

        // translate by element position
        m_Anchor.transform.localPosition += transform.localPosition;
        m_Anchor.transform.SetParent(shape.transform.parent, false);
        
        View.transform.localPosition += transform.localPosition;
        View.transform.SetParent(null, false);
        transform.SetParent(View.transform, false);
        
        // translate to world position
        m_Anchor.transform.Translate(new Vector3(pos.x, pos.y, 0));
        View.transform.Translate(new Vector3(pos.x, pos.y, 0));
    }

    public void Seize()
    {
        // remove from shape list
        m_Shape.Blocks.Remove(this);
    }

    public void Destroy()
    {
        Seize();

        if (View != null)
            View.GetComponent<Animator>().SetTrigger(k_DestroyTrigger);
        // spawn dust effect
        /*Instantiate(m_DustEffect, View.transform.position, Quaternion.identity);

        // spawn blob sound
        SoundManager.Sound.Play("s_Blob");

        //
        LeanTween.scale(View.gameObject, Vector3.zero, 0.18f)
            .setEaseInOutElastic()
            .setDestroyOnComplete(true)
            .setOnComplete(() =>
            {
                Destroy(m_Anchor.gameObject);
                Destroy(View.gameObject);
            });*/
    }

    private void OnDestroy()
    {
        Destroy(m_Anchor.gameObject);
    }

    private void _UpdatePosition()
    {
        m_Anchor.transform.localPosition = new Vector3(m_Position.x + 0.5f + m_PositionOffset.x, m_Position.y + 0.5f + m_PositionOffset.y, 0.0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
            return;

        var lp = transform.localPosition;
        transform.localPosition = new Vector3(
            Mathf.Floor(lp.x) + 0.5f,
            Mathf.Floor(lp.y) + 0.5f,
            0.0f);

        var newPos = new Vector2Int(Mathf.FloorToInt(lp.x), Mathf.FloorToInt(lp.y));


#if UNITY_EDITOR
        if (m_Position != newPos)
        {
            m_Position = newPos;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
