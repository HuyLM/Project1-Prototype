using DG.Tweening;
using System;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public GameColor Color;

    [SerializeField] private Renderer enemyRenderer;    // Renderer để thay đổi material 
    [SerializeField] private Renderer quadRenderer;
    [SerializeField] private SpriteRenderer dotRenderer;
    [SerializeField] private Renderer moveRenderer;

    public Action OnFilled;

    public bool IsDrawed = false;

    public void Initialize(GameColor color)
    {
        IsDrawed = false;
        Color = color;
        UpdateColor();
    }


    public void UpdateColor()
    {
        var colorConfig = DataConfigs.instance.GetColorMaterial(Color);
        enemyRenderer.sharedMaterial = colorConfig;
        moveRenderer.sharedMaterial = colorConfig;

        dotRenderer.color = DataConfigs.instance.GetColor(Color);
    }

    private void DestroyEnemy()
    {
        quadRenderer.gameObject.SetActive(true);
        quadRenderer.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);

        enemyRenderer.gameObject.SetActive(true);
        enemyRenderer.transform.localScale = Vector3.zero;
        enemyRenderer.transform.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                quadRenderer.gameObject.SetActive(false);
                //OnEnemyDestroyed?.Invoke(this);
            });
    }

    public void Draw(Transform moving, float delay)
    {
        IsDrawed = true;
        DOVirtual.DelayedCall(delay, () => {
            moveRenderer.gameObject.SetActive(true);
            moveRenderer.transform.position = moving.position;
            moveRenderer.transform.DOLocalJump(Vector3.zero, 1, 1, 0.2f).OnComplete(() => {
                moveRenderer.gameObject.SetActive(false);
                DestroyEnemy();
                OnFilled?.Invoke();
            });
        });
    }

    public void MoveUp(float upValue)
    {
        transform.DOMoveY(transform.position.y + upValue, 0.15f);
    }
}
