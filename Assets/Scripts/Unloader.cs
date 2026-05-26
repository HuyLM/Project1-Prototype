using DG.Tweening;
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

    public List<Block> blocks = new();

    public static Action ontextupdate;

    private void Start()
    {
        UpdateVisual();
        ontextupdate = UpdateText;
    }

    public void UpdateText()
    {
        text.text = $"{blocks.Count + Conveyor.FilledMovingSlotCount()}/{MaxQueueSize}";
    }
    
    private void UpdateVisual()
    {
        UpdateText();

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].MoveToPos(CalculatePositionForIndex(i));
        }
    }

    public void AddBlock(Block block)
    {
        if(blocks.Count + Conveyor.FilledMovingSlotCount() >= MaxQueueSize)
        {
            return;
        }
        blocks.Add(block);
        block.UnblockNeighbours();
        block.transform.parent = queue;
        UpdateVisual();
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

    [SerializeField] private Block[] addBlock;

    [Button]
    private void TestAdd()
    {
        for(int i = 0; i < addBlock.Length; i++) {
            AddBlock(addBlock[i]);

        }
    }
}
