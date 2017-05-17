﻿// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using global::System.Collections;
using global::System.Collections.Generic;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine;
using UnityEngine.UI;

public class Social : MonoBehaviour
{
    public Dropdown presenceFilterDropdown;
    public Transform contentPanel;
    public ScrollRect scrollRect;

    private Dictionary<int, XboxSocialUserGroup> socialUserGroups = new Dictionary<int, XboxSocialUserGroup>();
    private ObjectPool entryObjectPool;

    private void Awake()
    {
        this.EnsureEventSystem();
        this.entryObjectPool = this.GetComponent<ObjectPool>();
        SocialManagerComponent.Instance.EventProcessed += this.OnEventProcessed;

        presenceFilterDropdown.options.Clear();
        presenceFilterDropdown.options.Add(new Dropdown.OptionData() { text = PresenceFilter.All.ToString() });
        presenceFilterDropdown.options.Add(new Dropdown.OptionData() { text = PresenceFilter.AllOnline.ToString() });
        presenceFilterDropdown.value = 0;
        presenceFilterDropdown.RefreshShownValue();

        presenceFilterDropdown.onValueChanged.AddListener(delegate
        {
            PresenceFilterValueChangedHandler(presenceFilterDropdown);
        });
    }

    private void OnEventProcessed(object sender, SocialEvent socialEvent)
    {
        switch (socialEvent.EventType)
        {
            case SocialEventType.LocalUserAdded:
                if (socialEvent.Exception == null)
                {
                    CreateDefaulSocialGraphs();
                }
                break;
            case SocialEventType.SocialUserGroupLoaded:
                break;
        }

        RefreshSocialGroups();
    }

    private void PresenceFilterValueChangedHandler(Dropdown target)
    {
        RefreshSocialGroups();
    }

    private void CreateDefaulSocialGraphs()
    {
        XboxSocialUserGroup allSocialUserGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromFilters(XboxLiveComponent.Instance.User, PresenceFilter.All, RelationshipFilter.Friends);
        this.socialUserGroups.Add(0, allSocialUserGroup);

        XboxSocialUserGroup allOnlineSocialUserGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromFilters(XboxLiveComponent.Instance.User, PresenceFilter.AllOnline, RelationshipFilter.Friends);
        this.socialUserGroups.Add(1, allOnlineSocialUserGroup);
    }

    private void RefreshSocialGroups()
    {
        XboxSocialUserGroup socialUserGroup;
        if (!this.socialUserGroups.TryGetValue(presenceFilterDropdown.value, out socialUserGroup))
        {
            throw new Exception("Invalid PresenceFilter selected");
        }

        while (this.contentPanel.childCount > 0)
        {
            var entry = this.contentPanel.GetChild(0).gameObject;
            this.entryObjectPool.ReturnObject(entry);
        }

        foreach (XboxSocialUser user in socialUserGroup.Users)
        {
            GameObject entryObject = this.entryObjectPool.GetObject();
            XboxSocialUserEntry entry = entryObject.GetComponent<XboxSocialUserEntry>();

            entry.Data = user;
            entryObject.transform.SetParent(this.contentPanel);
        }

        // Reset the scroll view to the top.
        this.scrollRect.verticalNormalizedPosition = 1;
    }
}
