using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;

public class MovingSlot : MonoBehaviour
{
    public SplineFollower Follower;
    public float Speed = 1f;
    public Transform Container;

    [System.Serializable]
    public class SlotPassEvent : UnityEvent<MovingSlot, int> { }

    public SlotPassEvent OnPassSlot;

    public Piece Block;

    private double[] _slotPercents;
    private double _prevPercent;

    public void RegisterSlotTriggers(double[] slotPercents)
    {
        _slotPercents = slotPercents;
    }

    void Update()
    {
        if (_slotPercents == null || _slotPercents.Length == 0) return;

        double current = Follower.result.percent;

        for (int i = 0; i < _slotPercents.Length; i++)
        {
            if (HasCrossed(_prevPercent, current, _slotPercents[i]))
            {
                OnPassSlot?.Invoke(this, i);
            }
        }

        _prevPercent = current;
    }

    /// <summary>
    /// Kiểm tra có vượt qua target trong frame này không, kể cả khi loop (0.99 → 0.01)
    /// </summary>
    bool HasCrossed(double prev, double current, double target)
    {
        if (prev < current)
        {
            // Di chuyển bình thường
            return target > prev && target <= current;
        }
        else
        {
            // Vừa loop qua điểm 0
            return target > prev || target <= current;
        }
    }

    void Start()
    {
        Follower.followSpeed = Speed;
    }

    public bool IsEmpty()
    {
        return Block == null;
    }

    public void MakeEmpty()
    {
        Block = null;
        Unloader.ontextupdate.Invoke();
    }

    public void AddBlock(Piece block)
    {
        Block = block;
        block.transform.SetParent(Container, false);
        block.transform.localPosition = Vector3.zero;
        block.transform.localRotation = Quaternion.identity;
        Unloader.ontextupdate.Invoke();
    }


}
