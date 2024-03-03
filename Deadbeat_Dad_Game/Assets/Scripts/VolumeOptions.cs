using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeOptions : MonoBehaviour
{
    public Slider VO, Mus, FX;

    public void OnVOChange()
    {
        float value = VO.value;
        SetParam("DialogueLevel", value);
    }

    public void OnMusChange()
    {
        float value = Mus.value;
        SetParam("MusicLevel", value);
    }

    public void OnFXChange()
    {
        float value = FX.value;
        SetParam("FXLevel", value);
    }

    private void SetParam(string paramId, float value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(paramId, value);
    }
}
