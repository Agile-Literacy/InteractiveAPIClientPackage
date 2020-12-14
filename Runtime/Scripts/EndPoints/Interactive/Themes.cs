using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBrewStudios.Networking;
using System;
using Newtonsoft.Json;

namespace GameBrewStudios.Networking
{
    public partial class APIManager
    {
        public static void CreateTheme(InteractiveTheme theme, Action<InteractiveTheme> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", theme.name },
                {"bgURL", theme.bgURL },
                {"levelSelectStyle", theme.levelSelectStyle },
                {"lessonAreaStyle", theme.lessonAreaStyle },
                {"team", User.current.selectedMembership.team._id }
            };
            
            ServerRequest.CallAPI("/interactive/themes/create", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void DeleteTheme(InteractiveTheme theme, Action<bool> onComplete)
        {
            ServerRequest.CallAPI("/interactive/themes/" + theme._id, HTTPMethod.DELETE, null, (response) => {
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


        public static void GetTheme(string themeId, Action<InteractiveTheme> onComplete)
        {
            ServerRequest.CallAPI("/interactive/themes/" + themeId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void UpdateTheme(InteractiveTheme theme, Action<InteractiveTheme> onComplete)
        {
            Debug.Log("Updating theme: " + theme.name);
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", theme.name },
                {"bgURL", theme.bgURL },
                {"levelSelectStyle", theme.levelSelectStyle },
                {"lessonAreaStyle", theme.lessonAreaStyle }
            };

            ServerRequest.CallAPI("/interactive/themes/" + theme._id, HTTPMethod.PUT, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void ListThemes(string teamId, Action<InteractiveTheme[]> onComplete)
        {
            ServerRequest.CallAPI("/interactive/themes/list/" + teamId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, "themes", onComplete); }, true);
        }
    }
}