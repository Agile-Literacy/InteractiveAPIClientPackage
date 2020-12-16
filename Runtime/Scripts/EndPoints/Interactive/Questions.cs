using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace AgileLiteracy.API
{
    public partial class APIManager
    {
        public static void GetQuestion(string id, Action<InteractiveQuestion> onComplete)
        {
            //GET /api/interactive/questions/:questionId

            ServerRequest.CallAPI("/interactive/questions/" + id, HTTPMethod.GET, null, (response) => { ServerRequest.ResponseHandler(response, null, onComplete); }, true);
        }

        public static void CreateQuestion(InteractiveQuestion question, Action<InteractiveQuestion> onComplete)
        {
            CreateQuestion(question.text, question.answers.correct, question.answers.wrong, onComplete);
        }

        public static void CreateQuestion(string questionText, string[] correctAnswers, string[] wrongAnswers, Action<InteractiveQuestion> onComplete)
        {
            //POST /api/interactive/questions/create
            //Body = {team: ObjectID, text: "", answers: { correct: [], wrong: []} }

            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("team", User.current.selectedMembership.team._id);
            body.Add("text", questionText);
            body.Add("answers", new Dictionary<string, string[]>() { { "correct", correctAnswers }, { "wrong", wrongAnswers } });

            ServerRequest.CallAPI("/interactive/questions/create", HTTPMethod.POST, body, (response) => { ServerRequest.ResponseHandler(response, null, onComplete); }, true);
        }

        public static void UpdateQuestion(InteractiveQuestion question, Action<InteractiveQuestion> onComplete)
        {
            //PUT /api/interactive/questions/:questionId
            //Body =  { "text": "", "answers": { "correct": [], "wrong": []} }
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("text", question.text);
            body.Add("answers", new Dictionary<string, string[]>() { { "correct", question.answers.correct}, { "wrong", question.answers.wrong } });

            ServerRequest.CallAPI("/interactive/questions/" + question._id, HTTPMethod.PUT, body, (response) => { ServerRequest.ResponseHandler(response, null, onComplete); }, true);
        }

        public static void DeleteQuestion(string id, Action<bool> onComplete)
        {
            //DELETE /api/interactive/questions/:questionId
            ServerRequest.CallAPI("/interactive/questions/" + id, HTTPMethod.DELETE, null, (response) => { ServerRequest.ResponseHandler<Dictionary<string,object>>(response, null, (dict) => 
            {
                if(dict != null && dict.ContainsKey("ok") && System.Convert.ToInt32(dict["ok"]) == 1)
                {
                    onComplete.Invoke(true);
                }
                else
                {
                    onComplete.Invoke(false);
                }
            }); }, true);
        }

        public static void ListQuestions(string teamId, Action<InteractiveQuestion[]> onComplete)
        {
            //GET /api/interactive/questions/list/:teamId
            //Returns: {"questions": []}

            ServerRequest.CallAPI("/interactive/questions/list/" + teamId, HTTPMethod.GET, null, (response) => { ServerRequest.ResponseHandler(response, "questions", onComplete); }, true);
        }

        public static void ListQuestionsByIds(string[] questionIds, Action<InteractiveQuestion[]> onComplete)
        {
            string teamId = User.current.selectedMembership.team._id;
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"questions", questionIds }
            };
            ServerRequest.CallAPI("/interactive/questions/list/byId/" + teamId, HTTPMethod.POST, body, (response) => { ServerRequest.ResponseHandler(response, "questions", onComplete); }, true);
        }

    }
}