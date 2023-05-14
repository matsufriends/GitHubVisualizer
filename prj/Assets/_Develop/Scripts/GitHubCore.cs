using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public static class GitHubCore
{
    private static readonly Subject<int> s_loadCommitCountSubject = new();
    public static IObservable<int> OnLoadCommitCountChanged => s_loadCommitCountSubject;

    private static string GenerateApiUrl(string ownerName, string repoName)
    {
        return $"https://api.github.com/repos/{ownerName}/{repoName}/commits";
    }

    private static async UniTask<List<CommitData>> LoadAsync(string ownerName, string repoName, bool useOAuth, string clientId, string clientSecret,
        CancellationToken token)
    {
        var origin = new List<CommitData>();
        {
            var page = 1;
            while (true)
            {
                var uri = GenerateApiUrl(ownerName, repoName);
                uri += $"?per_page=100&page={page++}";
                if (useOAuth)
                {
                    uri += $"&client_id={clientId}&client_secret={clientSecret}";
                }

                using var request = UnityWebRequest.Get(uri);
                Debug.Log(request.uri);
                await request.SendWebRequest().ToUniTask(cancellationToken: token);
                if (request.result != UnityWebRequest.Result.Success)
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
        }
        origin.Reverse();
        return origin;
    }

    public static UniTask<List<CommitData>> LoadTask(string ownerName, string repoName, string clientId, string clientSecret, CancellationToken token)
    {
        return LoadAsync(ownerName, repoName, true, clientId, clientSecret, token);
    }

    public static UniTask<List<CommitData>> LoadTask(string ownerName, string repoName, CancellationToken token)
    {
        return LoadAsync(ownerName, repoName, false, "", "", token);
    }
}
