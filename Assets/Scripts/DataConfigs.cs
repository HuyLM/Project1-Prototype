using Dreamteck;
using System.Collections.Generic;
using UnityEngine;

public class DataConfigs : Singleton<DataConfigs>
{
    [SerializeField] private List<ColorConfig> colorConfigs = new List<ColorConfig>();
    [SerializeField] private Material defaultMaterial;

    public ColorConfig GetColorConfig(GameColor colorType)
    {
        return colorConfigs.Find(config => config.Color == colorType);
    }

    public Material GetColorMaterial(GameColor colorType, bool isBlocked = false)
    {
        var colorConfig = GetColorConfig(colorType);
        if (colorConfig != null)
        {
            return isBlocked ? colorConfig.blockedMat : colorConfig.material;
        }
        return defaultMaterial;
    }

    public Color GetColor(GameColor colorType)
    {
        var colorConfig = GetColorConfig(colorType);
        if (colorConfig != null)
        {
            return colorConfig.BaseColor;
        }
        return Color.black;
    }
}

[System.Serializable]
public class ColorConfig
{
    public GameColor Color;

    public Material material;
    public Material blockedMat;
    public Color BaseColor;
}
