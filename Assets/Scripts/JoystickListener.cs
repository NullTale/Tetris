using System;
using System.Linq;
using Core;
using Ludiq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class JoystickListener : MonoBehaviour
{
    public static bool JoystickConnected { get; private set; }
    public        bool HasJoystick       => JoystickConnected;

    [SerializeField]
    private UnityEvent m_OnConnected;
    [SerializeField]
    private UnityEvent m_OnDisconnected;

    //////////////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        var connected = string.IsNullOrWhiteSpace(Input.GetJoystickNames().FirstOrDefault()) == false;
        if (connected != JoystickConnected)
        {
            JoystickConnected = connected;
            if (connected)
                m_OnConnected.Invoke();
            else
                m_OnDisconnected.Invoke();
        }
        else if (JoystickConnected && EventSystem.current.currentSelectedGameObject == null)
            m_OnConnected.Invoke();
    }
}