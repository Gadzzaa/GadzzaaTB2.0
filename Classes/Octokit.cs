﻿using Octokit;
using System;
using System.Threading.Tasks;

namespace GadzzaaTB.Classes;

public class Octokit
{
    public static async Task Main(string title, string description)
    {
        // client initialization and authentication 
        var client = new GitHubClient(new ProductHeaderValue("Gadzzaa"));
        await client.User.Get("Gadzzaa");
        var tokenAuth = new Credentials(Passwords.githubOAuth);
        client.Credentials = tokenAuth;


        // user input
        var userIssueTitle = title?.Trim();

        var userIssue = description?.Trim();

        // input validation
        while (string.IsNullOrEmpty(userIssue) || string.IsNullOrEmpty(userIssueTitle)) break;

        var newIssue = new NewIssue(userIssueTitle) { Body = userIssue };
        var issue = await client.Issue.Create("Gadzzaa", "GadzzaaTB2.0", newIssue);

        var issueId = issue.Id;
        Console.WriteLine(@"Issue created succesfully with the ID of : " + issueId);
    }
}