using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.PostFX;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostPrecessControl : MonoBehaviour
{
    [SerializeField]
    private VolumeProfile       Profile;

    public float    Contrast
    {
        set
        {
            if (Profile.TryGet(out ColorAdjustments colorAdjustments))
                colorAdjustments.contrast.value = value;
        }
    }

    public float    HueShift
    {
        set
        {
            if (Profile.TryGet(out ColorAdjustments colorAdjustments))
                colorAdjustments.hueShift.value = value;
        }
    }
}
