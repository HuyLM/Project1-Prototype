using UnityEngine;

namespace Lean.Touch
{
    public interface IDragOverHandler
    {
        void HandleOver(GameObject droppedGameObject, LeanFinger finger);
    }
}
