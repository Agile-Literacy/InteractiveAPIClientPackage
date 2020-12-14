using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameBrewStudios.Networking
{
    public partial class APIManager
    {
        public static void GetQuiz(string id, Action<InteractiveQuiz> onComplete)
        {
            //GET /api/interactive/quiz/:quizId
            ServerRequest.CallAPI("/interactive/quiz/" + id, HTTPMethod.GET, null, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }
        public static void CreateQuiz(string name, string description, string[] questionIds, Action<InteractiveQuiz> onComplete)
        {
            //POST /api/interactive/quiz/create
            //Body = {team: ObjectID, name: "Quiz Name", description: "", "questions":["5e10e120212b0b7af47e4e11"]}
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"team", User.current.selectedMembership.team._id },
                {"name", name },
                {"description", description },
                {"questions", questionIds },
                {"onComplete", new CommandEvent[]{ } }
            };


            ServerRequest.CallAPI("/interactive/quiz/create", HTTPMethod.POST, body, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }

        public static void UpdateQuiz(InteractiveQuiz quiz, Action<InteractiveQuiz> onComplete)
        {
            List<string> questionIds = new List<string>();
            
            foreach (InteractiveQuestion question in quiz.questions)
                questionIds.Add(question._id);


            UpdateQuiz(quiz._id, quiz.name, quiz.description, questionIds.ToArray(), quiz.onComplete, onComplete);
        }

        public static void UpdateQuiz(string id, string name, string description, string[] questionIds, CommandEvent[] commandEvents, Action<InteractiveQuiz> onComplete)
        {
            //PUT /api/interactive/quiz/:quizId
            //Body = {name: "", description: "", questions: []}
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"team", User.current.selectedMembership.team._id },
                {"name", name },
                {"description", description },
                {"questions", questionIds },
                {"onComplete", commandEvents }
            };

            ServerRequest.CallAPI("/interactive/quiz/" + id, HTTPMethod.PUT, body, (r) => ServerRequest.ResponseHandler(r, null, onComplete), true);
        }

        public static void DeleteQuiz(string id, Action<bool> onComplete)
        {
            //DELETE /api/interactive/quiz/:quizId

            ServerRequest.CallAPI("/interactive/quiz/" + id, HTTPMethod.DELETE, null, (r) => ServerRequest.ResponseHandler<Dictionary<string,object>>(r, null, (dict) => 
            {
                if(dict != null && dict.ContainsKey("ok") && System.Convert.ToInt32(dict["ok"]) == 1)
                {
                    onComplete.Invoke(true);
                }
                else
                {
                    onComplete.Invoke(false);
                }
            }), true);
        }
        public static void ListQuizs(string teamId, Action<InteractiveQuiz[]> onComplete)
        {
            //GET /api/interactive/quiz/list/:teamId

            ServerRequest.CallAPI("/interactive/quiz/list/" + teamId, HTTPMethod.GET, null, (response) => ServerRequest.ResponseHandler(response, "quizzes", onComplete), true);
        }



    }
}