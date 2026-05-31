using UnityEngine;

public class PieceSlot : MonoBehaviour
{

    public Piece Piece;
    public CakeLand CakeLand;

    public bool IsEmpty => Piece == null;

    public void Init(CakeLand cakeLand)
    {
        this.CakeLand = cakeLand;
        if(Piece != null)
        {
            Piece.Init(this, cakeLand.IsBlocked);
        }
    }

    public void UpdateColor()
    {
        if (Piece != null)
        {
            Piece.UpdateColor(CakeLand.IsBlocked);
        }
    }

    public void MakeEmpty()
    {
        Piece = null;
    }
}
