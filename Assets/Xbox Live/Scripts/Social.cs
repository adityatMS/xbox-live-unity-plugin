// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using global::System.Collections;
using global::System.Collections.Generic;
using System.Globalization;

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
        //SocialManagerComponent.Instance.EventProcessed += this.OnEventProcessed;

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

    public void FixedUpdate()
    {
        try
        {
            var socialEvents = XboxLive.Instance.SocialManager.DoWork();

            foreach (SocialEvent socialEvent in socialEvents)
            {
                Debug.LogFormat("[SocialManager] Processed {0} event.", socialEvent.EventType);
                this.OnEventProcessed(socialEvent);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void OnEventProcessed(SocialEvent socialEvent)
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
            case SocialEventType.SocialUserGroupUpdated:
            case SocialEventType.PresenceChanged:
                this.RefreshSocialGroups();
                break;
        }
    }

    private void PresenceFilterValueChangedHandler(Dropdown target)
    {
        this.RefreshSocialGroups();
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
            //  WWW www = new WWW(user.DisplayPicRaw + "&w=128");
            // yield return www;

            //Texture2D t = www.texture;
            //Rect r = new Rect(0, 0, t.width, t.height);

            GameObject entryObject = this.entryObjectPool.GetObject();
            XboxSocialUserEntry entry = entryObject.GetComponent<XboxSocialUserEntry>();
            entry.Data = user;
            //entry.gamerpicImage.sprite = Sprite.Create(t, r, Vector2.zero);
            //entry.gamerpicMask.color = Color.white;
            entryObject.transform.SetParent(this.contentPanel);
        }

        // Reset the scroll view to the top.
        this.scrollRect.verticalNormalizedPosition = 1;
    }

    public static Color ColorFromHexString(string color)
    {
        float r = (float)byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber) / 255;
        float g = (float)byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber) / 255;
        float b = (float)byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber) / 255;

        return new Color(r, g, b);
    }
}
