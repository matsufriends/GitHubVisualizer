using UniRx;
using UnityEngine;

public sealed class GitHubPanelManagerMono : MonoBehaviour
{
    [SerializeField] private GitHubLoadPanelMono _loadPanel;
    [SerializeField] private GitHubContentsPanelMono _contentsPanel;

    private void Awake()
    {
        _loadPanel.OnLoadCommit.Subscribe(_contentsPanel.SetData).AddTo(this);
    }
}
