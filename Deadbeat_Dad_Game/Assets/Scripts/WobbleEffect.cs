using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleEffect : MonoBehaviour
{
    // https://www.youtube.com/watch?v=yG4dYBfeC0g
    public Material wobbleEffectMaterial;

    private bool wobbleActive = false;
    private float frequency = 4f;
    private float shift = 0f;
    private float amplitude = 0.05f;
    private float shiftSpeed = 5f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, wobbleEffectMaterial);
    }

    private void SetFrequency(float freq)
    {
        wobbleEffectMaterial.SetFloat("_frequency", freq);
    }

    private void SetShift(float shift)
    {
        wobbleEffectMaterial.SetFloat("_shift", shift);
    }

    private void SetAmplitude(float amp)
    {
        wobbleEffectMaterial.SetFloat("_amplitude", amp);
    }

    public void StartWobble()
    {
        wobbleActive = true;
        StartCoroutine(WobbleCoroutine());
    }

    public void StopWobble()
    {
        wobbleActive = false;
    }

    private IEnumerator WobbleCoroutine()
    {
        SetFrequency(frequency);
        SetShift(shift);
        SetAmplitude(amplitude);

        while(wobbleActive)
        {
            shift += shiftSpeed * Time.deltaTime;
            SetShift(shift);
            yield return null;
        }

        shift = 0f;
        enabled = false;
    }
}
