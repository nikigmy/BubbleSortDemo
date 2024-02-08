using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CubeVisualizerBasic : MonoBehaviour
{
    [SerializeField] private BubbleSortBasic sortBasic;
    [SerializeField] private List<MeshRenderer> currentObjects;
    
    [SerializeField] Color startColor = Color.red;
    [SerializeField] Color endColor = Color.green;
    
    float interval_R;
    float interval_G;
    float interval_B;

    public int offset = 2; 
    public int cols = 10; 
    
    [Button]
    public void VisualizeList(bool sameColor = false)
    {
        for (var index = 0; index < sortBasic.elements.Count; index++)
        {
            var el = sortBasic.elements[index];
            var color = sameColor ? Color.white : GetColor(el);
            currentObjects[index].material.color = color;
        }

    }

    private Color GetColor(int value)
    {
        return new Color(startColor.r + interval_R * value, startColor.g + interval_G * value, startColor.b + interval_B * value);
    }
    
    public void InitObjects()
    {
        if (currentObjects == null)
        {
            currentObjects = new List<MeshRenderer>(sortBasic.elements.Count);
        }
        if (currentObjects.Count != sortBasic.elements.Count)
        {
            interval_R = (endColor.r - startColor.r) / sortBasic.elements.Count;
            interval_G = (endColor.g - startColor.g) / sortBasic.elements.Count;
            interval_B = (endColor.b - startColor.b) / sortBasic.elements.Count;
            ClearObjects();
            for (var index = 0; index < sortBasic.elements.Count; index++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var row = index / cols;
                var col = index - (row * cols);
                cube.transform.position = new Vector3(col * offset, 0, row * -offset);
                currentObjects.Add(cube.GetComponent<MeshRenderer>());
            }
        }

    }
    private void ClearObjects()
    {
        foreach (var currentObject in currentObjects)
        {
            Destroy(currentObject.gameObject);
        }
        currentObjects.Clear();
    }
}

