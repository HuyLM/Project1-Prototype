using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public SplineComputer splineComputer;
    public MovingSlot MovingSlotPrefab;
    public ConveyorSegment[] Segments;

    public ConveyorSegment[] TopSegments;

    private double[] _slotPercents;
    public List<MovingSlot> _slots = new();
    public Map map;

    private void Start()
    {
        SpawnMovingSlots();
    }

    public void SpawnMovingSlots()
    {
        int count = Segments.Length;
        if (count == 0) return;

        float totalLength = splineComputer.CalculateLength();
        float spacing = totalLength / count;

        // Tính percent của từng Slot trên spline
        _slotPercents = new double[count];
        for (int i = 0; i < count; i++)
        {
            SplineSample sample = splineComputer.Project(Segments[i].transform.position);
            _slotPercents[i] = sample.percent;
        }

        for (int i = 0; i < count; i++)
        {
            float distance = spacing * i;
            double percent = splineComputer.Travel(0.0, distance);
            SplineSample sample = splineComputer.Evaluate(percent);

            MovingSlot movingSlot = Instantiate(MovingSlotPrefab, sample.position, sample.rotation);
            movingSlot.Follower.spline = splineComputer;

            movingSlot.RegisterSlotTriggers(_slotPercents);
            movingSlot.OnPassSlot.AddListener(OnPassSegmentTrigger);

            _slots.Add(movingSlot);

            StartCoroutine(InitFollower(movingSlot.Follower, percent));
        }
    }

    IEnumerator InitFollower(SplineFollower follower, double percent)
    {
        yield return null; // ⏳ đợi 1 frame để Awake() + Start() chạy xong
        follower.SetPercent(percent);
    }

    private void OnPassSegmentTrigger(MovingSlot movingSlot, int index)
    {
        var segment = Segments[index];

        if (movingSlot.IsEmpty())
        {
            if(segment.Unloader != null)
            {
                segment.Unloader.RemoveBlock(movingSlot);
            }
        }
        else
        {
            Block block = movingSlot.Block;

            var container = segment.GetContainer(block.Color, movingSlot.Block.Number);
            if (container == null || container.Count == 0)
            {
                return;
            }
            block.RemoveNumber(container.Count);
            if (block.Number <= 0)
            {
                movingSlot.MakeEmpty();
            }

            for (int i = 0; i < container.Count; i++)
            {
                container[i].Draw(movingSlot.transform, i * 0.1f);
            }

           
        }
    }

    public int FilledMovingSlotCount()
    {
        return _slots.Count(slot => slot.IsEmpty() == false);
    }

    public void LinkSegments()
    {
        int Overlap = 1;
        int m = map.needFillPixels.Count;
        int n = TopSegments.Length;
        if (m == 0 || n == 0) return;


        foreach (var seg in TopSegments)
            seg.Pixels.Clear();

        int startIndex = 0;

        for (int i = 0; i < n; i++)
        {
            // Chia đều, phần dư được cộng vào các segment đầu
            int count = m / n + (i < m % n ? 1 : 0);

            // Mở rộng biên trái (share với segment trước)
            int from = Mathf.Max(0, startIndex - (i > 0 ? Overlap : 0));

            // Mở rộng biên phải (share với segment sau)
            int to = Mathf.Min(m, startIndex + count + (i < n - 1 ? Overlap : 0));

            for (int j = from; j < to; j++)
                TopSegments[i].Pixels.Add(map.needFillPixels[j]);

            startIndex += count;
        }
    }
}
