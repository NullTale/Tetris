using System;
using UnityEngine;

[Serializable]
public class ANode : MonoBehaviour
{
    public ANode     Left;
    public ANode     Right;
    public ANode     Top;
    public ANode     Bottom;
    public bool     Passable  = true;
}