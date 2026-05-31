using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    [SerializeField] private float clickCooldownDuration = 0.5f;
    [SerializeField] private Unloader unloader;

    private float lastCharacterClickTime = -999f;
    private int blockCharacterClick; // if > 0 => block input
    private bool blockInputTrigger;


    private bool isGameRunning = true;

    private void Update()
    {
        // Chỉ xử lý input khi game đang chạy
        if (isGameRunning == true)
        {
            HandleInput();
        }
    }
    private int lastFrame;
    private bool lastCheckResult;
    private List<GraphicRaycaster> raycasters = new List<GraphicRaycaster>();

    public bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (lastFrame == Time.frameCount)
        {
            return lastCheckResult;
        }
        lastFrame = Time.frameCount;

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();

        if (raycasters.Count == 0)
        {
            raycasters.AddRange(FindObjectsOfType<GraphicRaycaster>());
        }
        foreach (var raycaster in raycasters)
        {
            if (raycaster == null)
                continue;
            raycaster.Raycast(pointerData, results);
            if (results.Count > 0)
            {
                lastCheckResult = true;
                return true;

            }
        }
        lastCheckResult = false;
        return false;
    }


    /// <summary>
    /// Xử lý input từ người chơi
    /// </summary>
    private void HandleInput()
    {
        if (blockInputTrigger == true)
        {
            return;
        }
#if UNITY_EDITOR || UNITY_STANDALONE

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null)
            {
                if (EventSystem.current == null)
                {
                    return;
                }
                if (EventSystem.current.firstSelectedGameObject != null)
                {
                }

                if (IsPointerOverUI(Input.mousePosition))
                {
                    return;
                }

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                ChechRaycastHit(Input.mousePosition);
            }

        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(EventSystem.current == null)
            {
                return;
            }
            if (IsPointerOverUI(touch.position))
            {
                return;
            }

            if(touch.phase == TouchPhase.Began)
            {
                ChechRaycastHit(touch.position);
            }
        }
#endif

    }

    private void ChechRaycastHit(Vector2 screenPoint)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Block"))
            {
                SelectPieceHitbox hitbox = hit.collider.GetComponent<SelectPieceHitbox>();
                if (hitbox != null)
                {
                    var block = hitbox.Piece;
                    if (block.IsBlocked)
                    {
                        return;
                    }
                    if (blockCharacterClick > 0)
                    {
                        //GameLog.Log($"[LevelController] Click block by - blockCharacterClick ({blockCharacterClick})");
                        return;
                    }
                    // Check cooldown before processing any character click
                    float timeSinceLastClick = Time.time - lastCharacterClickTime;
                    if (timeSinceLastClick < clickCooldownDuration)
                    {
                        // Still in cooldown, ignore this click
                        //GameLog.Log($"[LevelController] Click ignored - cooldown ({clickCooldownDuration:F2}s). Time remaining: {clickCooldownDuration - timeSinceLastClick:F2}s");
                        return;
                    }
                    var cake = block.Slot.CakeLand;
                    var pieces = cake.GetPieces(block);
                    StartCoroutine(Ijump(pieces));
                }
            }
        }
    }

    private IEnumerator Ijump(List<Piece> pieces)
    {
        foreach(var  piece in pieces)
        {
            bool canAdd = unloader.AddBlock(piece);
            if(canAdd == false)
            {
                yield break;
            }
            piece.MakeEmptySlot();
            yield return new WaitForSeconds(0.25f);
        }
    }

}
