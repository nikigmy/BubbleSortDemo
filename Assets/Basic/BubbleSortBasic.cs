using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class BubbleSortBasic : MonoBehaviour
{
    [SerializeField] public List<int> elements = new List<int>();

    private int cycle;
    private int index;
    // private bool changed;

    [Button]
    public void Init(int size = 15)
    {
        InitList(size);
        Shuffle();
    }
    
    private void InitList(int size)
    {
        elements = Enumerable.Range(0, size).ToList();
    }

    [Button]
    public bool Step()
    {
        if (elements[index] > elements[index + 1])
        {
            (elements[index], elements[index + 1]) = (elements[index + 1], elements[index]);
            // changed = true;
        }
        
        index++;
        if (index >= elements.Count - 1 - cycle)
        {
            // if (!changed)
            // {
            //     return true;
            // }
            cycle++;
            index = 0;
            // changed = false;
        }

        if (cycle == elements.Count - 2)
        {
            cycle = 0;
            index = 0;
            // changed = false;
            return true;
        }

        return false;
    }
    
    private void Shuffle()
    {
        cycle = 0;
        index = 0;
        System.Random rng = new System.Random();
        int n = elements.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            (elements[k], elements[n]) = (elements[n], elements[k]);
        }  
    }
}
