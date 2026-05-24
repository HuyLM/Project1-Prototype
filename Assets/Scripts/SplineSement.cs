using TMPro;
using UnityEngine;

public class SplineSement : MonoBehaviour
{
    public TextMeshPro TextMeshPro;

    public void ato(int ato)
    {
        TextMeshPro.text = ato.ToString();
    }
}
