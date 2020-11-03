using System;
using Core;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
[DefaultExecutionOrder(10)]
public class PlayerPrefValue : MonoBehaviour
{
    [Serializable]
    public enum Value
    {
        Float,
        Int,
        String,
        Bool
    }

    //////////////////////////////////////////////////////////////////////////
    [SerializeField]
    private Value       m_ValueType;
    [SerializeField]
    private string      m_Key;

    [SerializeField] [Tooltip("Set value to the object property value")]
    private bool        m_ApplyProperty;
    [SerializeField] [DrawIf(nameof(m_ApplyProperty), true)]
    private Object      m_Object;
    [SerializeField] [DrawIf(nameof(m_ApplyProperty), true)]
    private string      m_PropertyName = "value";

    private object      m_Data;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        if (PlayerPrefs.HasKey(m_Key) == false)
            return;

        // get value
        switch (m_ValueType)
        {
            case Value.Float:
                m_Data = PlayerPrefs.GetFloat(m_Key);
                break;
            case Value.Int:
                m_Data = PlayerPrefs.GetInt(m_Key);
                break;
            case Value.String:
                m_Data = PlayerPrefs.GetString(m_Key);
                break;
            case Value.Bool:
                m_Data = PlayerPrefs.GetInt(m_Key) != 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // try to set value through reflection
        if (m_ApplyProperty && m_Object != null)
        {
            try
            {
                m_Object.GetType().GetProperty(m_PropertyName).SetValue(m_Object, m_Data);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Can't set value {e.Message}");
            }
        }
    }

    public void Set(bool value)
    {
        PlayerPrefs.SetFloat(m_Key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Set(float value)
    {
        PlayerPrefs.SetFloat(m_Key, value);
        PlayerPrefs.Save();
    }

    public void Set(int value)
    {
        PlayerPrefs.SetInt(m_Key, value);
        PlayerPrefs.Save();
    }

    public void Set(string value)
    {
        PlayerPrefs.SetString(m_Key, value);
        PlayerPrefs.Save();
    }
}