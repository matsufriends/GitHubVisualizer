using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public struct CommitDataList : IEnumerable<CommitData>
{
    public CommitData[] commits;

    public IEnumerator<CommitData> GetEnumerator()
    {
        return ((IEnumerable<CommitData>)commits).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Serializable]
public struct CommitData
{
    public string sha;
    public string name_id;
    public CommitDataDetail commit;
    public string url;
    public string html_url;
    public string comments_url;
    public UserDataDetail author;
    public UserDataDetail committer;
    public ParentData[] parents;
}

[Serializable]
public struct CommitDataDetail
{
    public UserData author;
    public UserData commiter;
    public string message;
    public TreeData tree;
    public string url;
    public int comment_count;
    public VerificationData verification;
}

[Serializable]
public struct UserData
{
    public string name;
    public string email;
    public string date;
}

[Serializable]
public struct UserDataDetail
{
    public string login;
    public string id;
    public string node_id;
    public string avatar_url;
    public string gravatar_id;
    public string url;
    public string html_url;
    public string followers_url;
    public string following_url;
    public string gits_url;
    public string starred_url;
    public string subscriptions_url;
    public string organizations_url;
    public string repos_url;
    public string events_url;
    public string received_events_url;
    public string type;
    public bool site_admin;
}

[Serializable]
public struct TreeData
{
    public string sha;
    public string url;
}

[Serializable]
public struct VerificationData
{
    public bool verified;
    public string reason;
    public string signature;
    public string payload;
}

[Serializable]
public struct ParentData
{
    public string sha;
    public string url;
    public string html_url;
}
