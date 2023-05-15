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

public sealed class GitHubContentsPanelMono : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private GameObject _controllerParent;
    [SerializeField] private Slider _commitSlider;
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _playText;
    [Header("Content")]
    [SerializeField] private GitHubRowUIMono _rowPrefab;
    [SerializeField] private Transform _rowParent;
    [SerializeField] private RectTransform _rowParentRect;
    private List<CommitData> _commitDataList;
    private float _elapsedTime;
    private readonly Dictionary<string, GitHubRowUIMono> _rowDict = new();
    private readonly List<GitHubRowUIMono> _rowList = new();
    private readonly ReactiveProperty<bool> _isPlayingRP = new();
    private const float CommitApplyIntervalSeconds = 0.2f;

    private void Awake()
    {
        _playButton.OnClickAsObservable().Subscribe(_ => _isPlayingRP.Value = !_isPlayingRP.Value).AddTo(this);
        _isPlayingRP.Subscribe(x => _playText.text = x ? "Stop" : "Play").AddTo(this);
        _commitSlider.OnPointerDownAsObservable().Subscribe(_ => _isPlayingRP.Value = false).AddTo(this);
        _commitSlider.OnValueChangedAsObservable()
            .Where(_ => _commitDataList is { Count: > 0 })
            .Subscribe(x => DisplayByIndex(Mathf.FloorToInt(x)))
            .AddTo(this);
        //初期化
        _controllerParent.SetActive(false);
        _rowParent.DestroyChildren();
        _rowParentRect.sizeDelta = Vector2.zero;
    }

    public void SetData(List<CommitData> commitDataList)
    {
        _commitDataList = commitDataList;
        _controllerParent.SetActive(_commitDataList.Count > 0);
        _rowDict.Clear();
        _rowList.Clear();
        _isPlayingRP.Value = false;
        _rowParent.DetachChildren();
        _rowParentRect.sizeDelta = Vector2.zero;
        foreach (var commitData in commitDataList)
        {
            var key = GetKey(commitData);
            if (key == null || _rowDict.ContainsKey(key))
            {
                continue;
            }

            var row = Instantiate(_rowPrefab, _rowParent);
            LoadIconAsync(commitData, row, gameObject.GetCancellationTokenOnDestroy()).Forget();
            _rowDict.Add(key, row);
            _rowList.Add(row);
        }

        _commitSlider.minValue = 0;
        _commitSlider.maxValue = _commitDataList.Count - 1;
        _commitSlider.value = 0;
        if (_commitDataList.Count > 0)
        {
            DisplayByIndex(0);
        }
    }

    private static async UniTask LoadIconAsync(CommitData commitData, GitHubRowUIMono row, CancellationToken token)
    {
        var texture = await GitHubNetworkCore.LoadIconAsync(commitData.author.avatar_url, token);
        row.SetIcon(texture);
    }

    private void DisplayByIndex(int index)
    {
        Assert.IsTrue(index < _commitDataList.Count);
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

        var height = 20 + _rowList.Count * GitHubRowUIMono.Height + (_rowList.Count - 1) * GitHubRowUIMono.Space;
        _rowParentRect.sizeDelta = new Vector2(0, height);
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
}
