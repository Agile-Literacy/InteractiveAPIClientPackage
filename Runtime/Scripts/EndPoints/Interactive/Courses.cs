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
        public static void GetCourse(string id, Action<InteractiveCourse> onComplete)
        {
            ServerRequest.CallAPI("/interactive/courses/" + id, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }
        public static void CreateCourse(string name, string description, string themeId, string[] levelIds, Action<InteractiveCourse> onComplete)
        {
            //POST /api/interactive/courses/create
            //Body = { "name": "", "description": "", "theme": "theme id", "team": "team id", "levels": [levelIds]}

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"team", User.current.selectedMembership.team._id },
                {"name", name },
                {"description", description },
                {"theme", themeId },
                {"levels", levelIds }
            };

            ServerRequest.CallAPI("/interactive/courses/create", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void UpdateCourse(string id, string name, string description, string themeId, string[] levelIds, Action<InteractiveCourse> onComplete)
        {
            // PUT /api/interactive/courses/:courseId
            //Body = { "name": "", "description": "", "theme": "theme id", "levels": [levelIds]}

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", name },
                {"description", description },
                {"theme", themeId },
                {"levels", levelIds }
            };

            ServerRequest.CallAPI("/interactive/courses/" + id, HTTPMethod.PUT, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void UpdateCourse(InteractiveCourse course, Action<InteractiveCourse> onComplete)
        {
            // PUT /api/interactive/courses/:courseId
            //Body = { "name": "", "description": "", "theme": "theme id", "levels": [levelIds]}
            List<string> levelIds = new List<string>();

            for (int i = 0; i < course.levels.Length; i++)
            {
                levelIds.Add(course.levels[i]._id);
            }

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", course.name },
                {"description", course.description },
                {"levels", levelIds }
            };

            if (course.theme != null)
            {
                body.Add("theme", course.theme._id);
            }
            else
            {
                body.Add("theme", null);
            }

            ServerRequest.CallAPI("/interactive/courses/" + course._id, HTTPMethod.PUT, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void DeleteCourse(string id, Action<bool> onComplete)
        {
            //DELETE /api/interactive/courses/delete/:courseId

            ServerRequest.CallAPI("/interactive/courses/" + id, HTTPMethod.DELETE, null, (response) => {
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
        public static void ListCourses(string teamId, Action<InteractiveCourse[]> onComplete)
        {
            ServerRequest.CallAPI("/interactive/courses/list/" + teamId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, "courses", onComplete); }, true);
        }

        public static void GetUserAssignments(string memberId, Action<InteractiveCourseAssignment[]> onComplete)
        {
            //GET /api/interactive/courses/assignments/:memberId

            ServerRequest.CallAPI("/interactive/courses/assignments/" + memberId, HTTPMethod.GET, null, (response) => { ServerRequest.ResponseHandler(response, "assignments", onComplete); }, true);
        }

    }
}
