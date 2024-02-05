using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Taken from: https://www.youtube.com/watch?v=yG4dYBfeC0g
public class WobbleEffect : MonoBehaviour
{
    public Material wobbleEffectMaterial;

    private bool wobbleActive = false;
    private float frequency = 4f;
    private float shift = 0f;
    private float amplitude = 0f;
    private float maxAmplitude = 0.05f;
    private float shiftSpeed = 5f;
    private float amplitudeSpeed = 0.025f;

    void Start()
    {
        StopWobble();
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
        if (!wobbleActive)
        {
            wobbleActive = true;
            StartCoroutine(WobbleCoroutine());
        }
    }

    public void StopWobble()
    {
        wobbleActive = false;
        SetAmplitude(0f);
    }

    private IEnumerator WobbleCoroutine()
    {
        SetFrequency(frequency);
        SetShift(shift);

        while (amplitude < maxAmplitude)
        {
            if (wobbleActive)
            {
                amplitude += amplitudeSpeed * Time.deltaTime;
                shift += shiftSpeed * Time.deltaTime;
                shift %= Mathf.PI * 2f;

                SetAmplitude(amplitude);
                SetShift(shift);

                yield return null;
            }
            else
            {
                break;
            }
        }

        if (wobbleActive)
        {
            amplitude = maxAmplitude;
            SetAmplitude(amplitude);
        }

        while(wobbleActive)
        {
            shift += shiftSpeed * Time.deltaTime;
            shift %= Mathf.PI * 2f;
            SetShift(shift);
            yield return null;
        }

        shift = 0f;
        enabled = false;
    }
}
