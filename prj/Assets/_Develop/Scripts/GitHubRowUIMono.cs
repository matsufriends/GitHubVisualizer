using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GitHubRowUIMono : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private TextMeshProUGUI _labelText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Slider _slider;
    private const float MoveSpeed = 2000;
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
        var maxDif = MoveSpeed * Time.deltaTime;
        var pos = _rect.anchoredPosition;
        var clampedDif = Mathf.Clamp(CalcAimY() - pos.y, -maxDif, maxDif);
        pos.y += clampedDif;
        _rect.anchoredPosition = pos;
    }

    private float CalcAimY()
    {
        return Offset - (_rank - 1) * (Height + Space);
    }
}
