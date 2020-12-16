using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgileLiteracy.API;
using System;
using Newtonsoft.Json;
namespace AgileLiteracy.API
{
    public partial class APIManager
    {
        public static void GetLevel(string id, Action<InteractiveLevel> onComplete)
        {
            //GET /api/interactive/levels/:levelId
            ServerRequest.CallAPI("/interactive/levels/" + id, HTTPMethod.GET, null, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }

        public static void CreateLevel(string name, string description, string[] quizIds, string[] lessonIds, Action<InteractiveLevel> onComplete)
        {
            //POST /api/interactive/levels/create
            // Body = {"name": "", "description": "", "lessons": [lessonIDs], "quizzes": [quiz ids], "team": "teamId"}
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", name },
                {"description", description },
                {"quizzes", quizIds },
                {"lessons", lessonIds},
                {"team", User.current.selectedMembership.team._id }
            };

            ServerRequest.CallAPI("/interactive/levels/create", HTTPMethod.POST, body, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }

        public static void UpdateLevel(InteractiveLevel level, Action<InteractiveLevel> onComplete)
        {
            List<string> quizIds = new List<string>();
            for (int i = 0; i < level.quizzes.Length; i++)
            {
                if (level.quizzes[i] != null && !string.IsNullOrEmpty(level.quizzes[i]._id))
                    quizIds.Add(level.quizzes[i]._id);
            }
            
            List<string> lessonIds = new List<string>();
            for (int i = 0; i < level.lessons.Length; i++)
            {
                if (level.lessons[i] != null && !string.IsNullOrEmpty(level.lessons[i]._id))
                    lessonIds.Add(level.lessons[i]._id);
            }

            UpdateLevel(level._id, level.name, level.description, quizIds.ToArray(), lessonIds.ToArray(), onComplete);
        }

        public static void UpdateLevel(string id, string name, string description, string[] quizIds, string[] lessonIds, Action<InteractiveLevel> onComplete)
        {
            //PUT /api/interactive/levels/:levelId
            

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", name },
                {"description", description },
                {"quizzes", quizIds },
                {"lessons", lessonIds},
                {"team", User.current.selectedMembership.team._id }
            };

            ServerRequest.CallAPI("/interactive/levels/" + id, HTTPMethod.PUT, body, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }
        public static void DeleteLevel(string id, Action<Dictionary<string,object>> onComplete)
        {
            //DELETE /api/interactive/levels/:levelId

            ServerRequest.CallAPI("/interactive/levels/" + id, HTTPMethod.DELETE, null, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }
        public static void ListLevels(string teamId, Action<InteractiveLevel[]> onComplete)
        {
            //GET /api/interactive/levels/list/:teamId

            ServerRequest.CallAPI("/interactive/levels/list/" + teamId, HTTPMethod.GET, null, (r) => ServerRequest.ResponseHandler(r, "levels", onComplete), true);
        }

    }
}