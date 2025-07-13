using UnityEngine;

public class ColorSwitch : MonoBehaviour
{
    public Color color1 = new Color(1f, 0.898f, 0f); // FFE500
    public Color color2 = new Color(0f, 0.717f, 1f); // 00B7FF
    public float duration = 2f;

    private float t = 0f;
    private bool reverse = false;
    private Renderer render;

    void Start()
    {
        render = GetComponent<Renderer>();
    }

    void Update()
    {
        t += (reverse ? -1 : 1) * Time.deltaTime / duration;
        render.material.color = Color.Lerp(color1, color2, Mathf.PingPong(Time.time / duration, 1));
    }
}
