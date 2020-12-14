using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBrewStudios.Networking
{
    public partial class APIManager
    {
        public static void CreateBadge(InteractiveBadge badge, Action<InteractiveBadge> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"team", User.current.selectedMembership.team._id },
                {"name", badge.name },
                {"description", badge.description },
                {"icon", badge.icon },
                {"iconType", badge.iconType }
            };

            ServerRequest.CallAPI("/interactive/badges/create", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, "badge", onComplete); }, true);
        }

        public static void GetBadge(string id)
        {
            throw new NotImplementedException();
        }

        public static void AwardBadge(string badgeId, System.Action<string[]> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"user", User.current._id },
                {"badgeId", badgeId }
            };

            ServerRequest.CallAPI("/interactive/badges/user", HTTPMethod.PUT, body, (r) => ServerRequest.ResponseHandler(r, "badges", onComplete) , true);
        }

        public static void UpdateBadge(InteractiveBadge badge, Action<InteractiveBadge> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                
                {"name", badge.name },
                {"description", badge.description },
                {"iconType", badge.iconType },
                {"icon", badge.icon }
            };

            ServerRequest.CallAPI("/interactive/badges/" + badge._id, HTTPMethod.PUT, body, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }

        public static void DeleteBadge(string badgeId, Action<bool> onComplete)
        {
            
            ServerRequest.CallAPI("/interactive/badges/" + badgeId, HTTPMethod.DELETE, null, (response) => {
                ServerRequest.ResponseHandler<Dictionary<string, object>>(response, null, (dict) =>
                {
                    if (dict != null && dict.ContainsKey("ok") && System.Convert.ToInt32(dict["ok"]) == 1)
                    {
                        onComplete.Invoke(true);
                    }
                    else
                    {
                        onComplete.Invoke(false);
                    }
                });
            }, true);
        }

        public static void ListBadges(string teamId, Action<InteractiveBadge[]> onComplete)
        {

            ServerRequest.CallAPI("/interactive/badges/list/" + User.current.selectedMembership.team._id, HTTPMethod.GET, null, (r) => ServerRequest.ResponseHandler(r, "badges", onComplete), true);
        }
    }
}