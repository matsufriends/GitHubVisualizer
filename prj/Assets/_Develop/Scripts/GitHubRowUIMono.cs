using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GitHubRowUIMono : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _labelText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Slider _slider;
    private int _displayRank;
    private int _lastRank;
    private const float Offset = -10;
    public const float Height = 40;
    public const float Space = 10;
    private const int FadeOutRankDif = 2;
    private const float MoveSpeed = 20;
    private const float FadeSpeed = 15;
    public bool IsActive { get; private set; }
    public string Key { get; private set; }
    public float Value { get; private set; }

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
        if (IsActive == false)
        {
            UpdatePos(_lastRank + FadeOutRankDif, false);
        }

        IsActive = Value > 0;
        if (IsActive)
        {
            _displayRank = rank;
            _lastRank = _displayRank;
        }
        else
        {
            _displayRank = _lastRank = FadeOutRankDif;
        }

        _slider.value = Value / maxValue;
    }

    private void Update()
    {
        UpdatePos(_displayRank);
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, IsActive ? 1 : 0, Time.deltaTime * FadeSpeed);
    }

    private void UpdatePos(int rank, bool useLerp = true)
    {
        var pos = _rect.anchoredPosition;
        var t = useLerp ? Time.deltaTime * MoveSpeed : 1;
        pos.y = Mathf.Lerp(pos.y, Offset - (rank - 1) * (Height + Space), t);
        _rect.anchoredPosition = pos;
    }
}
