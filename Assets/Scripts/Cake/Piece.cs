using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Renderer renderer;
    public GameColor Color;

    public bool IsBlocked;

    public PieceSlot Slot;

    public void Init(PieceSlot slot, bool isBlocked)
    {
        this.Slot = slot;
        IsBlocked = isBlocked;
        UpdateColor(isBlocked);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked();
    }

    public void Clicked()
    {
       // Debug.LogError("Piece");
    }

    [Button]
    private void UpdateColorr()
    {
        UpdateColor(false);
    }

    public void UpdateColor(bool isBlocked)
    {
        IsBlocked = isBlocked;
        renderer.sharedMaterial = DataConfigs.instance.GetColorMaterial(Color, IsBlocked);
    }

    public void JumpToContainer(CakeLand container)
    {
        if (container.IsFull) return;
        var containerSlot = container.GetEmptySlot();
        if (containerSlot == null) return;



        transform.parent = containerSlot.transform;
        containerSlot.Piece = this;
        this.Slot = containerSlot;
        transform.DOLocalRotateQuaternion(Quaternion.identity, 0.25f);
        transform.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() => {
            container.CheckFull();
        });
    }

    Tween moving;
    public void MoveToPos(Vector3 target)
    {
        moving?.Kill();
        moving = transform.DOLocalMove(target, 0.25f).SetEase(Ease.OutQuad);
    }

    public void MakeEmptySlot()
    {
        if (Slot != null)
        {
            Slot.MakeEmpty();
        }
        Slot = null;
    }
}
