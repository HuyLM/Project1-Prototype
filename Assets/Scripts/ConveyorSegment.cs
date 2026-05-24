using System.Collections.Generic;
using UnityEngine;

public class ConveyorSegment : MonoBehaviour
{
    public List<Pixel> Pixels;
    public Unloader Unloader;

    public List<Pixel> GetContainer(GameColor color, int number)
    {
        if(number <= 0)
        {
            return null;
        }
        List<Pixel> pixels = new List<Pixel>();
        for(int i = 0; i < Pixels.Count; i++)
        {
            if(Pixels[i].IsDrawed == false && Pixels[i].Color == color)
            {
                pixels.Add(Pixels[i]);
                number--;
                if(number == 0)
                {
                    return pixels;
                }
            }
        }
        return pixels;
    }

   
}
