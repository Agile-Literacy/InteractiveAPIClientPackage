using GameBrewStudios.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameBrewStudios.Networking
{
    public partial class APIManager
    {

        
        public static void CompleteLesson(InteractiveCourse course, InteractiveLesson lesson, bool complete, Action<CourseProgress[]> onComplete)
        {
            //PUT /api/interactive/progress
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"type", "lesson" },
                {"id", lesson._id },
                {"course", course._id },
                {"complete", complete },
                {"member", User.current.selectedMembership._id}
            };

            ServerRequest.CallAPI("/interactive/progress", HTTPMethod.PUT, body, (r) => ServerRequest.ResponseHandler(r, "userProgress", onComplete), true);
        }

        public static void CompleteLevel(InteractiveCourse course, InteractiveLevel level, bool complete, Action<CourseProgress[]> onComplete)
        {
            //PUT /api/interactive/progress
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"type", "level" },
                {"id", level._id },
                {"course", course._id },
                {"complete", complete },
                {"member", User.current.selectedMembership._id}
            };

            ServerRequest.CallAPI("/interactive/progress", HTTPMethod.PUT, body, (r) => ServerRequest.ResponseHandler(r, "userProgress", onComplete), true);
        }



        public static void CompleteCourse(InteractiveCourse course, bool complete, Action<CourseProgress[]> onComplete)
        {
            //PUT /api/interactive/progress
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"type", "course" },
                {"course", course._id },
                {"complete", complete },
                {"member", User.current.selectedMembership._id}
            };

            ServerRequest.CallAPI("/interactive/progress", HTTPMethod.PUT, body, (r) => ServerRequest.ResponseHandler(r, "userProgress", onComplete), true);
        }

        public static void GetCourseProgress(Action<CourseProgress[]> onComplete)
        {
            ServerRequest.CallAPI("/interactive/progress/" + User.current._id, HTTPMethod.GET, null, (r) => ServerRequest.ResponseHandler(r, "userProgress", onComplete), true);
        }
        public static void SubmitQuizScore(InteractiveQuizScore quizScore, Action<CourseProgress[]> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("score", quizScore.score);
            body.Add("didPass", quizScore.didPass);
            body.Add("quiz", quizScore.quiz);
            body.Add("member", User.current.selectedMembership._id);
            body.Add("course", InteractiveCourse.current._id);

            ServerRequest.CallAPI("/interactive/progress/quiz", HTTPMethod.PUT, body, (response) => ServerRequest.ResponseHandler(response, "userProgress", onComplete), true);
        }

        public static void UpdateProgress(CourseProgress[] cp,Action<CourseProgress[], bool, bool> finished)
        {
            bool levelCompleted = false;
            bool courseCompleted = false;
            if (InteractiveLesson.current.GetIndex() == InteractiveLevel.current.lessons.Length - 1)
            {
                APIManager.CompleteLevel(InteractiveCourse.current, InteractiveLevel.current, true, (cl) =>
                {
                    levelCompleted = true;
                    if (InteractiveLevel.current.GetIndex() == InteractiveCourse.current.levels.Length - 1)
                    {
                        APIManager.CompleteCourse(InteractiveCourse.current, true, (cc) =>
                        {
                            courseCompleted = true;
                            finished?.Invoke(cc, levelCompleted, courseCompleted);
                            return;

                        });
                    }
                    else
                    {
                        finished?.Invoke(cl, levelCompleted, courseCompleted);
                        return;
                    }

                });
            }
            else
            {
                finished?.Invoke(cp, levelCompleted, courseCompleted);
                return;
            }
        }
    }
}
