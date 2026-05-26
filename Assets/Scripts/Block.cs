using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameColor Color;
    public int Number;
    public bool IsBlocked;
    [SerializeField] private Block[] neighbours;

    [SerializeField] private TextMeshPro txtNumber;
    [SerializeField] private Renderer _renderers;

    private void Start()
    {
        UpdateColor();
    }

    private void OnValidate()
    {
        UpdateColor();
    }

    public void JumpToContainer(Container container)
    {
        if (container.IsFull()) return;
        var containerSlot = container.GetEmptySlot();
        if(containerSlot == null) return;

       

        transform.parent = containerSlot.transform;
        containerSlot.Block = this;
        transform.DOLocalJump(Vector3.zero, 1.7f, 1, 0.25f).OnComplete(() => {
            container.CheckFull();
        });
    }

    public void UpdateColor(int overNumber = -1)
    {
        _renderers.sharedMaterial = DataConfigs.instance.GetColorMaterial(Color, IsBlocked);
        txtNumber.text = overNumber < 0 ? Number.ToString() : overNumber.ToString();
        txtNumber.alpha = IsBlocked ? 0.5f : 1;
    }

    public void Unblock()
    {
        IsBlocked = false;
        UpdateColor();
    }

    public void UnblockNeighbours()
    {
        foreach (var n in neighbours)
        {
            n.Unblock();
        }
    }

    public void RemoveNumber(int number)
    {
        int pre = Number;
        Number -= number;

        DOVirtual.Int(pre, Number, 0.1f * number, (value) =>
        {
            UpdateColor(value);
        }).OnComplete(() => {
            if (Number <= 0)
            {
                gameObject.SetActive(false);
            }
        });

        
    }

    [Button]
    private void Random()
    {
        Number = UnityEngine.Random.Range(1, 7);
        UpdateColor();
    }
    Tween moving;

    public void MoveToPos(Vector3 target)
    {
        moving?.Kill();
        moving = transform.DOLocalMove(target, 0.25f).SetEase(Ease.OutQuad);
    }
}

public enum GameColor
{
    None,
    Red,
    Green,
    Blue, 
    Yellow,
}
