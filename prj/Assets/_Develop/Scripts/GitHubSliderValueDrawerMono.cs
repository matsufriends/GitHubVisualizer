using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public sealed class GitHubSliderValueDrawerMono : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _format;

    private void Awake()
    {
        _slider.OnValueChangedAsObservable().Subscribe(x => { _text.text = string.Format(_format, x); }).AddTo(this);
    }
}
