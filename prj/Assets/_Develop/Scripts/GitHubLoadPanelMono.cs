using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public sealed class GitHubLoadPanelMono : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_InputField _ownerNameInputField;
    [SerializeField] private Button _loadReposButton;
    [SerializeField] private TMP_Dropdown _reposDropdown;
    [SerializeField] private Button _loadCommitButton;
    [SerializeField] private TMP_InputField _clientIdInputField;
    [SerializeField] private TMP_InputField _clientSecretInputField;
    private readonly Subject<List<CommitData>> _loadCommitDataListSubject = new();
    public IObservable<List<CommitData>> OnLoadCommit => _loadCommitDataListSubject;

    private void Awake()
    {
        _loadReposButton.OnClickAsObservable()
            .Subscribe(_ => OnLoadRepositoryButtonAsync(gameObject.GetCancellationTokenOnDestroy()).Forget())
            .AddTo(this);
        _loadCommitButton.OnClickAsObservable()
            .Subscribe(_ => OnLoadCommitButtonAsync(gameObject.GetCancellationTokenOnDestroy()).Forget())
            .AddTo(this);
    }

    private async UniTask OnLoadRepositoryButtonAsync(CancellationToken token)
    {
        var owner = _ownerNameInputField.text;
        var clientId = _clientIdInputField.text;
        var clientSecret = _clientSecretInputField.text;
        var repoList = await GitHubNetworkCore.LoadRepos(owner, clientId, clientSecret, token);
        var optionList = repoList.Select(repo => new TMP_Dropdown.OptionData(repo.name)).ToList();
        _reposDropdown.options = optionList;
        _reposDropdown.value = optionList.Count > 0 ? 0 : -1;
    }

    private async UniTask OnLoadCommitButtonAsync(CancellationToken token)
    {
        if (_reposDropdown.value >= _reposDropdown.options.Count)
        {
            return;
        }

        var owner = _ownerNameInputField.text;
        var repo = _reposDropdown.options[_reposDropdown.value].text;
        var clientId = _clientIdInputField.text;
        var clientSecret = _clientSecretInputField.text;
        _loadCommitDataListSubject.OnNext(await GitHubNetworkCore.LoadCommits(owner, repo, clientId, clientSecret, token));
    }
}
