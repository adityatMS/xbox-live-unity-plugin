// -----------------------------------------------------------------------
//  <copyright file="Leaderboard.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Social : MonoBehaviour
{
    [HideInInspector]
    public Dropdown presenceFilter;

    [HideInInspector]
    public Dropdown relationshipFilter;

    [HideInInspector]
    public Transform contentPanel;

    [HideInInspector]
    public ScrollRect scrollRect;

    private ObjectPool entryObjectPool;
    private bool isLocalUserAdded;

    private void Awake()
    {
        this.EnsureEventSystem();

        this.entryObjectPool = this.GetComponent<ObjectPool>();

        StatsManagerComponent.Instance.LocalUserAdded += this.LocalUserAdded;
        StatsManagerComponent.Instance.GetLeaderboardCompleted += this.GetLeaderboardCompleted;
        this.isLocalUserAdded = false;
    }

    public void Refresh()
    {
        
    }

    private void UpdateData(uint newPage)
    {
        if (!this.isLocalUserAdded) return;

        //LeaderboardQuery query;
        //if (newPage == this.currentPage + 1 && this.leaderboardData != null && this.leaderboardData.HasNext)
        //{
        //    query = this.leaderboardData.NextQuery;
        //}
        //else
        //{
        //    query = new LeaderboardQuery
        //    {
        //        StatName = this.stat.Name,
        //        SocialGroup = this.socialGroup,
        //        SkipResultsToRank = newPage == 0 ? 0 : (this.currentPage * this.entryCount) - 1,
        //        MaxItems = this.entryCount,
        //    };

        //    // Handle last page
        //    if (this.totalPages > 0 && newPage == this.totalPages)
        //    {
        //        query.SkipResultsToRank = (newPage * this.entryCount) - 1;
        //        newPage -= 1;
        //    }
        //}

        //this.currentPage = newPage;
        //XboxLive.Instance.StatsManager.GetLeaderboard(XboxLiveComponent.Instance.User, query);
    }

    private void LocalUserAdded(object sender, XboxLiveUserEventArgs e)
    {
        this.isLocalUserAdded = true;
        Refresh();
    }

    //private void GetLeaderboardCompleted(object sender, XboxLivePrefab.StatEventArgs e)
    //{
    //    if (e.EventData.ErrorInfo != null) return;

    //    LeaderboardResultEventArgs leaderboardArgs = (LeaderboardResultEventArgs)e.EventData.EventArgs;
    //    this.LoadResult(leaderboardArgs.Result);
    //}

    /// <summary>
    /// Load the leaderboard result data from the service into the view.
    /// </summary>
    /// <param name="result"></param>
    private void LoadResult()
    {
        // if (this.stat == null || this.stat.Name != result.NextQuery.StatName || this.socialGroup != result.NextQuery.SocialGroup) return;

        // this.leaderboardData = result;

        while (this.contentPanel.childCount > 0)
        {
            var entry = this.contentPanel.GetChild(0).gameObject;
            this.entryObjectPool.ReturnObject(entry);
        }

        //foreach (LeaderboardRow row in this.leaderboardData.Rows)
        //{
        //    GameObject entryObject = this.entryObjectPool.GetObject();
        //    LeaderboardEntry entry = entryObject.GetComponent<LeaderboardEntry>();

        //    entry.Data = row;

        //    entryObject.transform.SetParent(this.contentPanel);
        //}

        // Reset the scroll view to the top.
        this.scrollRect.verticalNormalizedPosition = 1;
        // this.UpdateButtons();
    }
}