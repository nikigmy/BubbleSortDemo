using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SortController : MonoBehaviour
{
    public BubbleSortBasic sortBasic;
    public CubeVisualizerBasic visualizerBasic;

    private bool run;

    public int stepDelay = 200;
    public int whiteDelay = 200;
    public int size = 15;
    public int combinedSteps = 10000;

    private Task task;

    [Button]
    public void StartSort()
    {
        sortBasic.Init(size);
        visualizerBasic.InitObjects();
        SortLoop();
    }

    [Button]
    public void OnlySort()
    {
        var sorted = false;
        sortBasic.Init(size);
        while (!sorted)
        {
            sorted = sortBasic.Step();
        }
    }

    [Button]
    public void TogglePause()
    {
        run = !run;
    }

    public async Task SortLoop()
    {
        var startTime = DateTime.Now;
        run = true;
        var sorted = false;
        while (!sorted)
        {
            if (!run)
            {
                await Task.Delay(stepDelay);
                continue;
            }
            for (int i = 0; i < combinedSteps; i++)
            {
                sorted = sortBasic.Step();
                if (sorted)
                {
                    break;
                }
            }

            if (stepDelay > 0)
            {
                visualizerBasic.VisualizeList();
                if (!sorted)
                {
                    await Task.Delay(stepDelay);
                }
            }
        }

        visualizerBasic.VisualizeList(true);
        await Task.Delay(whiteDelay);
        visualizerBasic.VisualizeList();

        var timespan = DateTime.Now - startTime;
        Debug.LogError(timespan.TotalMilliseconds);
    }
}