using UnityEngine;
using UnityEngine.UI;

public class NeedBars : MonoBehaviour
{
    [SerializeField] private RectTransform needBarRect;
    [SerializeField] private float duration = 100f;

    private Vector2 originalSize;
    private float currentTime;

    private void Start()
    {
        currentTime = duration;
        originalSize = needBarRect.sizeDelta; 
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        float percentage = Mathf.Clamp01(currentTime / duration);

        needBarRect.sizeDelta = new Vector2(originalSize.x * percentage, originalSize.y);

        if (currentTime <= 0f)
        {
            enabled = false;
        }
    }
}
