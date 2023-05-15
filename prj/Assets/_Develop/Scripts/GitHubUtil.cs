using System;
using System.Collections.Generic;
using UnityEngine;

public static class GitHubUtil
{
    public static bool TryConvertRepos(string json, List<RepositoryData> origin)
    {
        var list = RepositoryDataList.ConvertFromJson(json);
        if (list.repos.Length == 0)
        {
            return false;
        }

        origin.AddRange(list.repos);
        return true;
    }

    public static bool TryConvertCommits(string json, List<CommitData> origin)
    {
        var list = CommitDataList.ConvertFromJson(json);
        if (list.commits.Length == 0)
        {
            return false;
        }

        origin.AddRange(list.commits);
        return true;
    }
}

[Serializable]
public struct RepositoryDataList
{
    public RepositoryData[] repos;

    public static RepositoryDataList ConvertFromJson(string json)
    {
        return JsonUtility.FromJson<RepositoryDataList>($"{{\"{nameof(repos)}\":{json}}}");
    }
}

[Serializable]
public struct CommitDataList
{
    public CommitData[] commits;

    public static CommitDataList ConvertFromJson(string json)
    {
        return JsonUtility.FromJson<CommitDataList>($"{{\"{nameof(commits)}\":{json}}}");
    }
}

[Serializable]
public struct RepositoryData
{
    public long id;
    public string node_id;
    public string name;
    public string full_name;
    public bool @private;
    public UserDataDetail owner;
    public string html_url;
    public string description;
    public bool fork;
    public string url;
    public string forks_url;
    public string keys_url;
    public string collaborators_url;
    public string teams_url;
    public string hooks_url;
    public string issue_events_url;
    public string events_url;
    public string assignees_url;
    public string branches_url;
    public string tags_url;
    public string blobs_url;
    public string git_tags_url;
    public string git_refs_url;
    public string trees_url;
    public string statuses_url;
    public string languages_url;
    public string stargazers_url;
    public string contributors_url;
    public string subscribers_url;
    public string subscription_url;
    public string commits_url;
    public string git_commits_url;
    public string comments_url;
    public string issue_comment_url;
    public string contents_url;
    public string compare_url;
    public string merges_url;
    public string archive_url;
    public string downloads_url;
    public string issues_url;
    public string pulls_url;
    public string milestones_url;
    public string notifications_url;
    public string labels_url;
    public string releases_url;
    public string deployments_url;
    public string created_at;
    public string updated_at;
    public string pushed_at;
    public string git_url;
    public string ssh_url;
    public string clone_url;
    public string svn_url;
    public string homepage;
    public int size;
    public int stargazers_count;
    public int watchers_count;
    public string language;
    public bool has_issues;
    public bool has_projects;
    public bool has_downloads;
    public bool has_wiki;
    public bool has_pages;
    public bool has_discussions;
    public int forks_count;
    public string mirror_url;
    public bool archived;
    public bool disabled;
    public int open_issues_count;
    public LicenseData license;
    public bool allow_forking;
    public bool is_template;
    public bool web_commit_signoff_required;
    public string topics;
    public string visibility;
    public int forks;
    public int open_issues;
    public int watchers;
    public string default_branch;
}

[Serializable]
public struct LicenseData
{
    public string key;
    public string name;
    public string spdx_id;
    public string url;
    public string node_id;
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
    public long id;
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
