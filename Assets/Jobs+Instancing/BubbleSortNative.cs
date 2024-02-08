using System.Linq;
using Unity.Collections;
using UnityEngine;

public class BubbleSortNative
{
    public NativeArray<int> elements;
    public NativeArray<int> posMap;

    private int cycle;
    private int index;
    // private bool changed;
    
    
    public void Init(int size = 15)
    {
        InitList(size);
        Shuffle();
    }
    
    private void InitList(int size)
    {
        elements = new NativeArray<int>(size, Allocator.Persistent);
        var array = Enumerable.Range(0, size).ToArray();

        for (int i = 0; i < size; i++)
        {
            elements[i] = array[i];
        }
    }

    public bool Step()
    {
        if (elements[index] > elements[index + 1])
        {
            (elements[index], elements[index + 1]) = (elements[index + 1], elements[index]);
            posMap[elements[index]] = index;
            posMap[elements[index + 1]] = index + 1;
            // changed = true;
        }
        
        index++;
        if (index >= elements.Length - 1 - cycle)
        {
            // if (!changed)
            // {
            //     return true;
            // }
            cycle++;
            index = 0;
            // changed = false;
        }

        if (cycle == elements.Length - 2)
        {
            cycle = 0;
            index = 0;
            // changed = false;
            return true;
        }

        return false;
    }
    
    public void Shuffle()
    {
        cycle = 0;
        index = 0;
        System.Random rng = new System.Random();
        int n = elements.Length;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            (elements[k], elements[n]) = (elements[n], elements[k]);
        }  
    }
}