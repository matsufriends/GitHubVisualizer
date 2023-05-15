using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GitHubRowUIMono : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _labelText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Slider _slider;
    private const float Offset = -10;
    private const float Height = 40;
    private const float Space = 10;
    private const int SpawnRank = 12;
    private int _rank = SpawnRank;
    public string Key { get; private set; }
    public float Value { get; private set; }

    private void Awake()
    {
        var pos = _rect.anchoredPosition;
        pos.y = CalcAimY();
        _rect.anchoredPosition = pos;
    }

    public void SetIcon(Texture2D texture)
    {
        if (texture == null)
        {
            return;
        }

        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        _image.sprite = sprite;
    }

    public void SetData(string key, float value)
    {
        _labelText.text = key;
        _valueText.text = $"{value}";
        Key = key;
        Value = value;
    }

    public void SetRank(int rank, float maxValue)
    {
        _rank = Value == 0 ? SpawnRank : rank;
        _slider.value = Value / maxValue;
    }

    private void Update()
    {
        var pos = _rect.anchoredPosition;
        pos.y = Mathf.Lerp(pos.y, CalcAimY(), Time.deltaTime * 50);
        _rect.anchoredPosition = pos;
    }

    private float CalcAimY()
    {
        return Offset - (_rank - 1) * (Height + Space);
    }
}
