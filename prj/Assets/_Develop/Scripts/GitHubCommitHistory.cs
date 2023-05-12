using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public sealed class GitHubCommitHistory : MonoBehaviour
{
    private string repoUrl = "https://api.github.com/repos/owner/repoName/commits";
    private string accessToken = "YOUR_ACCESS_TOKEN";

    IEnumerator Start()
    {
        string apiUrl = repoUrl + "?access_token=" + accessToken;

        // GitHub APIにGETリクエストを送信
        using (WebClient webClient = new WebClient())
        {
            webClient.Headers.Add("User-Agent", "Unity web player");
            webClient.Headers.Add("Accept", "application/json");
            try
            {
                string json = webClient.DownloadString(apiUrl);
                // 取得したJSONを解析してコミット履歴を取得
                List<Commit> commits = JsonUtility.FromJson<CommitList>(json).commits;

                // コミット履歴をDebug.Logに出力
                foreach (Commit commit in commits)
                {
                    Debug.Log("Commit ID: " + commit.commitId);
                    Debug.Log("Author: " + commit.author.name);
                    Debug.Log("Message: " + commit.message);
                    Debug.Log("------------------------------");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }

        yield return null;
    }

    [Serializable]
    public class CommitList
    {
        public List<Commit> commits;
    }

    [Serializable]
    public class Commit
    {
        public string commitId;
        public Author author;
        public string message;
    }

    [Serializable]
    public class Author
    {
        public string name;
    }
}
