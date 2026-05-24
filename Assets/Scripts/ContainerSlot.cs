using UnityEngine;

public class ContainerSlot : MonoBehaviour
{
    public Block Block;

    public bool IsEmpty
    {
        get
        {
            return Block == null;
        }
    }
}
