using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSegment : MonoBehaviour
{
    public CakeLand[] Containers;
    public Unloader Unloader; 
    public SplineComputer spline;

    public CakeLand GetContainer(GameColor color)
    {
        for (int i = 0; i < Containers.Length; i++)
        {
            if (Containers[i].IsBlocked == false && Containers[i].Color == color && Containers[i].IsFull == false)
            {
                return Containers[i];
            }
        }
        return null;
    }

   
}
