using DG.Tweening;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Unloader : MonoBehaviour
{
    [SerializeField]    Conveyor Conveyor;
    [SerializeField] private Transform queue;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float distance = 0.5f;

    public int MaxQueueSize;

    public List<Piece> blocks = new();

    public static Action ontextupdate;

    private void Start()
    {
        UpdateVisual();
        ontextupdate = UpdateText;
    }

    public void UpdateText()
    {
        text.text = $"{blocks.Count + Conveyor.FilledMovingSlotCount()}/{Conveyor.Segments.Length}";
    }
    
    private void UpdateVisual()
    {
        UpdateText();

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].MoveToPos(CalculatePositionForIndex(i));
        }
    }

    public bool AddBlock(Piece block)
    {
        if(blocks.Count + Conveyor.FilledMovingSlotCount() >= Conveyor.Segments.Length)
        {
            return false;
        }
        blocks.Add(block);
        block.transform.parent = queue;
        block.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.25f);
        UpdateVisual();
        return true;
    }

    public void RemoveBlock(MovingSlot movingSlot)
    {
        if(blocks.Count == 0)
        {
            return;
        }
        var block = blocks[0];
        blocks.RemoveAt(0);
        UpdateVisual();
        movingSlot.AddBlock(block);
    }

    private Vector3 CalculatePositionForIndex(int index)
    {
        return new Vector3(0 , 0, -1 * distance * index);
    }
}
