using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public GameColor Color;

    public ContainerSlot[] Slots;

    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private List<Container> blockContainers;

    private Action<Container> onFullSlot;

    public bool IsBlocked;

    private void Start()
    {
        if (blockContainers != null && blockContainers.Count > 0)
        {
            IsBlocked = true;
            for (int i = 0; i < blockContainers.Count; i++)
            {
                blockContainers[i].onFullSlot += OnRemovedBlockContainer;
            }
        }
        UpdateColor();
    }

    public bool IsFull()
    {
        foreach (ContainerSlot slot in Slots)
        {
            if (slot.IsEmpty) return false;
        }
        return true;
    }

    public ContainerSlot GetEmptySlot()
    {
        for (int i = 0; i < Slots.Length; ++i)
        {
            if(Slots[i].IsEmpty) return Slots[i];
        }
        return null;
    }

    public void CheckFull()
    {
        if(IsFull())
        {
            transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => {
                onFullSlot?.Invoke(this);
                gameObject.SetActive(false);
            });
        }
    }

    private void OnRemovedBlockContainer(Container container)
    {
        blockContainers.Remove(container);
        if(blockContainers.Count == 0)
        {
            IsBlocked = false;
            UpdateColor();
        }
    }

    [Button]
    public void UpdateColor()
    {
        for(int i = 0;i < _renderers.Length; ++i)
        {
            _renderers[i].sharedMaterial = DataConfigs.instance.GetColorMaterial(Color, IsBlocked);
        }
    }
}
