using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace AgileLiteracy.API
{
    public partial class APIManager
    {
        public static void GetLesson(string id, Action<ServerResponse> onComplete)
        {
            ServerRequest.CallAPI("/interactive/lessons/" + id, HTTPMethod.GET, null, onComplete, true);
        }
        public static void CreateLesson(InteractiveLesson lesson, Action<InteractiveLesson> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>() {
                {"name", lesson.name },
                {"description", lesson.description },
                {"type", "Video" },
                {"data", lesson.data },
                {"teamId", User.current.selectedMembership.team._id },
                {"onComplete", lesson.onComplete }
            };

            ServerRequest.CallAPI("/interactive/lessons/create", HTTPMethod.POST, body, (response) => ServerRequest.ResponseHandler(response, null, onComplete), true);
        }
        public static void UpdateLesson(InteractiveLesson lesson, Action<InteractiveLesson> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>() {
                {"name", lesson.name },
                {"description", lesson.description },
                {"type", "Video" },
                {"data", lesson.data },
                {"team", User.current.selectedMembership.team._id },
                {"onComplete", lesson.onComplete }
            };

            ServerRequest.CallAPI("/interactive/lessons/" + lesson._id, HTTPMethod.PUT, body, (response) => ServerRequest.ResponseHandler(response, null, onComplete), true);
            
        }
        public static void DeleteLesson(InteractiveLesson lesson, Action<bool> onComplete)
        {
            ServerRequest.CallAPI("/interactive/lessons/" + lesson._id, HTTPMethod.DELETE, null, (response) =>
            {
                ServerRequest.ResponseHandler<Dictionary<string, object>>(response, null,
                (dict) =>
                {
                    if (dict != null && dict.ContainsKey("ok") && System.Convert.ToInt32(dict["ok"]) == 1)
                        onComplete?.Invoke(true); //success == true
                    else
                        onComplete?.Invoke(false);
                });
            }, true);
        }
        public static void ListLessons(string teamId, Action<InteractiveLesson[]> onComplete)
        {
            ServerRequest.CallAPI("/interactive/lessons/list/" + teamId, HTTPMethod.GET, null, (response) => { ServerRequest.ResponseHandler(response, "lessons", onComplete); }, true);
        }

    }
}