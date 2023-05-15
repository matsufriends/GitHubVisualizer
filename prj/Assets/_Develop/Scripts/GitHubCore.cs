using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public static class GitHubCore
{
    private static bool s_loadNext;
    private static readonly Subject<int> s_loadCommitCountSubject = new();
    public static IObservable<int> OnLoadCommitCountChanged => s_loadCommitCountSubject;

    private static string GenerateApiUrl(string ownerName, string repoName)
    {
        return $"https://api.github.com/repos/{ownerName}/{repoName}/commits";
    }

    public static void StopLoad()
    {
        s_loadNext = false;
    }

    private static async UniTask<List<CommitData>> LoadAsync(string ownerName, string repoName, bool useOAuth, string clientId, string clientSecret,
        CancellationToken token)
    {
        var origin = new List<CommitData>();
        var page = 1;
        s_loadNext = true;
        while (s_loadNext)
        {
            var uri = GenerateApiUrl(ownerName, repoName);
            uri += $"?per_page=100&page={page++}";
            if (useOAuth)
            {
                uri += $"&client_id={clientId}&client_secret={clientSecret}";
            }

            using var request = UnityWebRequest.Get(uri);
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

            var json = request.downloadHandler.text;
            json = $"{{\"commits\":{json}}}";
            var commitDataList = JsonUtility.FromJson<CommitDataList>(json);
            if (commitDataList.commits.Length == 0)
            {
                break;
            }

            origin.AddRange(commitDataList.commits);
            s_loadCommitCountSubject.OnNext(origin.Count);
            await UniTask.NextFrame(token);
        }

        origin.Reverse();
        return origin;
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

    public static UniTask<List<CommitData>> LoadRepository(string ownerName, string repoName, string clientId, string clientSecret,
        CancellationToken token)
    {
        return LoadAsync(ownerName, repoName, true, clientId, clientSecret, token);
    }

    public static UniTask<List<CommitData>> LoadRepository(string ownerName, string repoName, CancellationToken token)
    {
        return LoadAsync(ownerName, repoName, false, "", "", token);
    }
}
