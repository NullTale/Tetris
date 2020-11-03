using System;
using Core;
using Malee;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [Serializable]
    public class Data
    {
        public float    GravityPerSecond;
    }

    //////////////////////////////////////////////////////////////////////////
    [SerializeField]
    [Reorderable(elementNameOverride = "Level")]
    private ReorderableArrayT<Data>         m_Data;
    public ReorderableArrayT<Data>          DataList => m_Data;
}