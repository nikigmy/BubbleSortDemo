using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SorterUI : MonoBehaviour
{
    [SerializeField] private InstancedSortController sortController;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider multiplierSlider;
    
    private const int FPS_SAMPLE_COUNT = 20;
    private int[] fpsSamples = new int[FPS_SAMPLE_COUNT];
    private int sampleIndex;
    private int speed;
    private int multiplier;
    public void Start()
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        speedSlider.value = 100;
        SetSpeed(100);
        multiplierSlider.value = 1;
        SetMultiplier(1);
        sortController.Init();
        InvokeRepeating(nameof(UpdateFps), 0, 0.1f);
    }

    private void UpdateFps()
    {
        var sum = 0;
        for (var i = 0; i < FPS_SAMPLE_COUNT; i++)
        {
            sum += fpsSamples[i];
        }

        fpsText.text = $"FPS: {sum / FPS_SAMPLE_COUNT}";
    }
    private void Update()
    {
        fpsSamples[sampleIndex++] = (int)(1.0f / Time.deltaTime);
        if (sampleIndex >= FPS_SAMPLE_COUNT) sampleIndex = 0;
    }

    public void TogglePause()
    {
        sortController.TogglePause();
        UpdatePauseText();
    }

    public void Restart()
    {
        sortController.Restart();
        UpdatePauseText();
    }

    private void UpdatePauseText()
    {
        pauseText.text = sortController.Paused ? "Play" : "Pause";
    }

    public void SetSpeed(float value)
    {
        speed = (int)value;
        speedText.text = speed.ToString();
        UpdateSpeed();
    }
    public void SetMultiplier(float value)
    {
        multiplier = (int)value;
        multiplierText.text = multiplier.ToString();
        UpdateSpeed();
    }

    public void UpdateSpeed()
    {
        sortController.SetSpeed(speed * multiplier);
    }
}
