using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MornLib.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public sealed class GitHubManagerMono : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_InputField _ownerNameInputField;
    [SerializeField] private TMP_InputField _repoNameInputField;
    [SerializeField] private Button _loadButton;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TMP_InputField _clientIdInputField;
    [SerializeField] private TMP_InputField _clientSecretInputField;
    [SerializeField] private Toggle _useOAuthToggle;
    [Header("Controller")]
    [SerializeField] private GameObject _controllerActivator;
    [SerializeField] private Slider _commitSlider;
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _playText;
    [Header("Content")]
    [SerializeField] private GitHubRowUIMono _rowPrefab;
    [SerializeField] private Transform _contentsParent;
    private List<CommitData> _commitDataList;
    private float _elapsedTime;
    private readonly ReactiveProperty<bool> _isPlayingRP = new();
    private readonly Dictionary<string, GitHubRowUIMono> _rowDict = new();
    private readonly List<GitHubRowUIMono> _rowList = new();
    private const float CommitApplyIntervalSeconds = 0.2f;

    private void Awake()
    {
        _loadButton.OnClickAsObservable().Subscribe(_ => OnLoadButtonAsync(gameObject.GetCancellationTokenOnDestroy()).Forget()).AddTo(this);
        _playButton.OnClickAsObservable().Subscribe(_ => _isPlayingRP.Value = !_isPlayingRP.Value).AddTo(this);
        _isPlayingRP.Subscribe(x => _playText.text = x ? "Stop" : "Play").AddTo(this);
        _commitSlider.OnPointerDownAsObservable().Subscribe(_ => _isPlayingRP.Value = false).AddTo(this);
        _commitSlider.OnValueChangedAsObservable()
            .Where(_ => _commitDataList is { Count: > 0 })
            .Subscribe(x => DisplayByIndex(Mathf.FloorToInt(x)))
            .AddTo(this);
        GitHubCore.OnLoadCommitCountChanged.Subscribe(x => { _resultText.text = $"Now Loading...\n({x} commits.)"; }).AddTo(this);
        Init();
    }

    private void DisplayByIndex(int index)
    {
        Assert.IsTrue(index < _commitDataList.Count);
        var commitData = _commitDataList[index];
        var key = GetKey(commitData);
        if (key != null && _rowDict.ContainsKey(key) == false)
        {
            var row = Instantiate(_rowPrefab, _contentsParent);
            _rowDict.Add(key, row);
            _rowList.Add(row);
        }

        foreach (var pair in _rowDict)
        {
            var value = GetValue(pair.Key, _commitDataList, index);
            pair.Value.SetData(pair.Key, value);
        }

        _rowList.Sort((a, b) =>
            Math.Abs(a.Value - b.Value) < 0.00001f ? string.Compare(a.Key, b.Key, StringComparison.Ordinal) : b.Value.CompareTo(a.Value));
        var max = _rowList[0].Value;
        for (var i = 0; i < _rowList.Count; i++)
        {
            _rowList[i].SetRank(i + 1, max);
        }
    }

    private void Init()
    {
        _resultText.text = "";
        _commitSlider.minValue = 0;
        _commitSlider.maxValue = 0;
        _commitSlider.value = 0;
        _rowDict.Clear();
        _rowList.Clear();
        _contentsParent.DestroyChildren();
        _controllerActivator.SetActive(false);
    }

    private static string GetKey(CommitData commitData)
    {
        return commitData.author.login;
    }

    private static float GetValue(string key, IReadOnlyList<CommitData> commitDataList, int range)
    {
        var count = 0;
        for (var i = 0; i <= range; i++)
        {
            var commitData = commitDataList[i];
            if (GetKey(commitData) == key)
            {
                count++;
            }
        }

        return count;
    }

    private void Update()
    {
        if (!_isPlayingRP.Value)
        {
            return;
        }

        _elapsedTime += Time.deltaTime * _speedSlider.value;
        while (Mathf.Abs(_elapsedTime) >= CommitApplyIntervalSeconds)
        {
            var min = _commitSlider.minValue;
            var max = _commitSlider.maxValue;
            if (_elapsedTime > 0)
            {
                _elapsedTime -= CommitApplyIntervalSeconds;
                var value = _commitSlider.value + 1;
                _commitSlider.value = Mathf.Clamp(value, min, max);
            }
            else
            {
                _elapsedTime += CommitApplyIntervalSeconds;
                var value = _commitSlider.value - 1;
                _commitSlider.value = Mathf.Clamp(value, min, max);
            }
        }
    }

    private async UniTask OnLoadButtonAsync(CancellationToken token)
    {
        Init();
        _resultText.text = "Now Loading...";
        var owner = _ownerNameInputField.text;
        var repo = _repoNameInputField.text;
        var clientId = _clientIdInputField.text;
        var clientSecret = _clientSecretInputField.text;
        if (_useOAuthToggle.isOn)
        {
            _commitDataList = await GitHubCore.LoadTask(owner, repo, clientId, clientSecret, token);
        }
        else
        {
            _commitDataList = await GitHubCore.LoadTask(owner, repo, token);
        }

        token.ThrowIfCancellationRequested();
        _resultText.text = $"Loaded {_commitDataList.Count} commits";
        _commitSlider.minValue = 0;
        _commitSlider.maxValue = _commitDataList.Count - 1;
        _controllerActivator.SetActive(_commitDataList.Count > 0);
        _isPlayingRP.Value = false;
        if (_commitDataList.Count > 0)
        {
            DisplayByIndex(0);
        }
    }
}
