using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentFieldA;
    public TextMeshProUGUI contentFieldB;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string header, string contentA, string contentB = "")
    {
        if (string.IsNullOrEmpty(header)) 
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentFieldA.text = contentA;
        contentFieldB.text = contentB;
    }

    private void Update()
    {
        if (Application.isEditor) 
        {
            int headerLength = headerField.text.Length;
            int contentLengthA = contentFieldA.text.Length;
            int contentLengthB = contentFieldB.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentLengthA > characterWrapLimit || contentLengthB > characterWrapLimit) ? true : false;
        }

        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
