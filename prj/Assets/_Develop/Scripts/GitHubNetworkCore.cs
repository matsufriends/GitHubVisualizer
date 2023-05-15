using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public static class GitHubNetworkCore
{
    private static readonly ReactiveProperty<bool> s_isLoading = new();
    private static readonly Subject<int> s_loadPageSubject = new();
    private static readonly Subject<int> s_loadEndSubject = new();
    public static IObservable<bool> OnLoadingChanged => s_isLoading;
    public static IObservable<int> OnLoadPage => s_loadPageSubject;
    public static IObservable<int> OnLoadEnd => s_loadEndSubject;

    public static void StopLoad()
    {
        s_isLoading.Value = false;
    }

    private static async UniTask<List<T>> LoadDataAsync<T>(Func<string, List<T>, bool> tryConvertAndAddElementFunc, string uri, string clientId,
        string clientSecret, CancellationToken token)
    {
        var origin = new List<T>();
        var page = 1;
        s_isLoading.Value = true;
        while (s_isLoading.Value)
        {
            var curUri = uri + $"?per_page=100&page={page}";
            if (clientId.Length > 0 && clientSecret.Length > 0)
            {
                curUri += $"&client_id={clientId}&client_secret={clientSecret}";
            }

            s_loadPageSubject.OnNext(page);
            using var request = UnityWebRequest.Get(curUri);
            try
            {
                await request.SendWebRequest().ToUniTask(cancellationToken: token);
            }
            catch
            {
                Debug.Log("GET Request Failure");
                Debug.Log(request.error);
                break;
            }

            if (tryConvertAndAddElementFunc(request.downloadHandler.text, origin) == false)
            {
                break;
            }

            page++;
            await UniTask.NextFrame(token);
        }

        s_isLoading.Value = false;
        s_loadEndSubject.OnNext(origin.Count);
        return origin;
    }

    public static UniTask<List<RepositoryData>> LoadRepos(string ownerName, string clientId, string clientSecret, CancellationToken token)
    {
        var uri = $"https://api.github.com/users/{ownerName}/repos";
        return LoadDataAsync<RepositoryData>(GitHubUtil.TryConvertRepos, uri, clientId, clientSecret, token);
    }

    public static UniTask<List<CommitData>> LoadCommits(string ownerName, string repoName, string clientId, string clientSecret,
        CancellationToken token)
    {
        var uri = $"https://api.github.com/repos/{ownerName}/{repoName}/commits";
        return LoadDataAsync<CommitData>(GitHubUtil.TryConvertCommits, uri, clientId, clientSecret, token);
    }

    public static async UniTask<Texture2D> LoadIconAsync(string url, CancellationToken token)
    {
        using var request = new UnityWebRequest(url);
        request.downloadHandler = new DownloadHandlerTexture();
        try
        {
            await request.SendWebRequest().ToUniTask(cancellationToken: token);
            return DownloadHandlerTexture.GetContent(request);
        }
        catch
        {
            Debug.LogError($"Error downloading image: {request.error}");
            return null;
        }
    }
}
