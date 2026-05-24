using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Pixel pixelPrefab;
    public Conveyor conveyor;

    public MapData MapData;
    public int curRow = 0;

    public List<Pixel> needFillPixels = new List<Pixel>();
    public List<Pixel> pixels = new List<Pixel>();

    private void Start()
    {
        MapData = new MapData();
        MapData.CreateMap();
        SpawnNewRow();
    }

    private void SpawnNewRow()
    {
        needFillPixels.Clear();
        float enemySpacing = 0.35f;
        for (int c = 0; c < MapData.Col; c++)
        {
            float startX = -((MapData.Col - 1) * enemySpacing) / 2;

            Vector3 spawnPos = new Vector3(
                 startX + c * enemySpacing,
                 0,
                0
              );


            var enemyObj = GameObject.Instantiate(pixelPrefab, transform);
            enemyObj.transform.localPosition = spawnPos;
            enemyObj.transform.rotation = Quaternion.identity;
            enemyObj.transform.localScale = new Vector3(enemySpacing, 0.35f, enemySpacing);
            enemyObj.Initialize(MapData.Pixels[curRow, c].Color);
            enemyObj.OnFilled = OnFilled;
            needFillPixels.Add(enemyObj);
            pixels.Add(enemyObj);
        }

        conveyor.LinkSegments();
    }
    bool movingUp = false;
    private void OnFilled()
    {
        if(movingUp == true)
        {
            return;
        }
        foreach(var p in needFillPixels)
        {
            if(p.IsDrawed == false)
            {
                return;
            }
        }
        movingUp = true;

        DOVirtual.DelayedCall(1, () => {
            // move up
            for (int i = 0; i < pixels.Count; i++)
            {
                pixels[i].MoveUp(0.35f);
            }

            // new row
            curRow++;
            SpawnNewRow();
            movingUp = false;
        });
    }
}

public class MapData
{
    public PixelData[,] Pixels;
    public int Row = 15;
    public int Col  = 20;

    public void CreateMap()
    {
        Pixels = new PixelData[Row, Col];
        for (int c = 0; c < Col; c++)
        {
            for(int r = 0; r < Row; r++)
            {
                if(r == 0)
                {
                    Pixels[r, c] = new PixelData() { Color = GameColor.Red };
                }
                else
                {
                    Pixels[r, c] = new PixelData() { Color = (GameColor)(Random.Range(1, 5)) };
                }
            }
        }

    }
}

public class PixelData
{
    public GameColor Color;
}
