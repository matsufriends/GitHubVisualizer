using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public sealed class GitHubLoadStatusPanelMono : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    [SerializeField] private Button _stopButton;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        _text.text = "";
        GitHubNetworkCore.OnLoadingChanged.Subscribe(x => _parent.SetActive(x)).AddTo(this);
        _stopButton.OnClickAsObservable().Subscribe(_ => GitHubNetworkCore.StopLoad()).AddTo(this);
        GitHubNetworkCore.OnLoadPage.Subscribe(x => _text.text = $"Now Loading...\n(page {x})").AddTo(this);
        GitHubNetworkCore.OnLoadEnd.Subscribe(x => { _text.text = x > 0 ? $"Load End!\nFind {x} data." : "Load Failed."; }).AddTo(this);
        _parent.SetActive(false);
    }
}
