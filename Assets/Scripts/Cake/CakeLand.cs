using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CakeLand : MonoBehaviour
{
    [SerializeField] private Renderer renderer;

    public GameColor Color;
    public PieceSlot[] Slots;

    [SerializeField] private List<CakeLand> blockCakes;

    private Action<CakeLand> onFullSlot;

    public bool IsCompleted
    {
        get
        {
            foreach (var slot in Slots)
            {
                if (slot.IsEmpty || slot.Piece.Color != Color)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public bool IsFull
    {
        get
        {
            foreach (var slot in Slots)
            {
                if (slot.IsEmpty)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public bool IsBlocked;

    private void Start()
    {
        if (blockCakes != null && blockCakes.Count > 0)
        {
            IsBlocked = true;
            for (int i = 0; i < blockCakes.Count; i++)
            {
                blockCakes[i].onFullSlot += OnRemovedBlockContainer;
            }
        }
        Init();
    }

    public void Init()
    {
        foreach (var slot in Slots)
        {
            slot.Init(this);
        }
        UpdateColor();
    }

    public PieceSlot GetEmptySlot()
    {
        for (int i = 0; i < Slots.Length; ++i)
        {
            if (Slots[i].IsEmpty) return Slots[i];
        }
        return null;
    }


    public void CheckFull()
    {
        if (IsCompleted)
        {
            transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => {
                onFullSlot?.Invoke(this);
                gameObject.SetActive(false);
            });
        }
    }


    private void OnRemovedBlockContainer(CakeLand cake)
    {
        blockCakes.Remove(cake);
        if (blockCakes.Count == 0)
        {
            IsBlocked = false;
            UpdateColor();
        }
    }

    [Button]
    public void UpdateColor()
    {
        renderer.sharedMaterial = DataConfigs.instance.GetColorMaterial(Color, IsBlocked);

        foreach (var slot in Slots)
        {
            slot.UpdateColor();
        }
    }

    public List<Piece> GetPieces(Piece piece)
    {
        List<Piece> pieces = new List<Piece>();
        pieces.Add(piece);
        foreach (var slot in Slots)
        {
            if (slot.IsEmpty == false && slot.Piece != piece && slot.Piece.Color == piece.Color)
            {
                pieces.Add(slot.Piece);
            }
        }
        return pieces;
    }
}
