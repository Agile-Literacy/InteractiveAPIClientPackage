using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgileLiteracy.API;
using AgileLiteracy.API;

public static class Session
{
    private static string uuid, user, team, sessionID;
    public static bool isSending; //if unable to send toggle off until the next 'Major' event as to not eat bandwidth trying to constantly send + save events unsuccesfully
    public static bool sessionInstialized; //if sessionstart gets called multiple times, or was not called, check against this bool

    private static DateTime sessionStart;

    private static int sessioncount; //# of events this session
    private static DateTime lastActionTime; //for tracking major events : quiz/activity length

    public static List<MetricEvent> sessionMetricList;
    private static List<AnswerQuizQuestion> answeredQuestions; //keep track of the questions to have a full list to append to the quiz end




    public static void StartSession()
    {

        //Check if a user is logged in
        if (User.current != null)
        {


            user = User.current._id;
            team = User.current.selectedMembership._id;
            //check for saved data that was not sent to the server

            if (SaveLoadSessionData.HasLocalSaveData())
            {
                APIManager.SubmitMetrics(SaveLoadSessionData.Load().ToArray(), (res) => HandleResponse(res));
            }


        }

        //break out if the session had already be intialized
        //covering the scenario where a user is active and logs in while the session is in progress
        if (sessionInstialized == true) { return; }

        sessionInstialized = true;

        sessionStart = DateTime.UtcNow;

        //create a unique identifier for the session for parsing multiple sessions on the same day
        GenerateSessionID();

        //systeminfo gives a unique id for the device is use
        uuid = SystemInfo.deviceUniqueIdentifier;



        //instialize the empty lists
        if (sessionMetricList == null)
        { sessionMetricList = new List<MetricEvent>(); }

        if (answeredQuestions == null)
        { answeredQuestions = new List<AnswerQuizQuestion>(); }


    }
    public static void EndSession()
    {

    }

    //this would not work with multiple users, TODO: find solution, or dont assume a user not logged in is a previous user
    //public static void SaveIDtoPrefs()
    //{ 
    //        PlayerPrefs.SetString("user", user);
    //        PlayerPrefs.SetString("team", team);

    //}

    public static string GenerateSessionID()
    {
        string sessionuuid = uuid;

        sessionuuid += RandomCharacter();
        sessionuuid += RandomCharacter();
        sessionuuid += RandomCharacter();
        sessionuuid += RandomCharacter();

        return sessionuuid;
    }

    public static string RandomCharacter()
    {
        if (UnityEngine.Random.Range(0, 10) < 5)
        {
            if (UnityEngine.Random.Range(0, 10) < 5)
            {
                return ((char)UnityEngine.Random.Range(65, 91)).ToString();
            }
            else
            {
                return ((int)UnityEngine.Random.Range(0, 10)).ToString();
            }
        }
        else
        {
            if (UnityEngine.Random.Range(0, 10) < 5)
            {
                return ((int)UnityEngine.Random.Range(0, 10)).ToString();
            }
            else
            {
                return ((char)UnityEngine.Random.Range(97, 123)).ToString();
            }
        }
        //APIManager.ListBadges
        //window_
    }


    public static void HandleResponse(APIManager.MetricsResponse res)
    {
        if (res == null || res.success == false)
        {
            isSending = false;
            if (res != null)
            { SaveLoadSessionData.Save(res.metrics); }
            else { SaveLoadSessionData.Save(sessionMetricList[sessionMetricList.Count - 1]); }

        }
        else
        {
            isSending = true;

            //if there is a ./save.dat file there is metric data that was not sent to the server
            if (SaveLoadSessionData.HasLocalSaveData())
            {
                APIManager.SubmitMetrics(SaveLoadSessionData.Load().ToArray(), (loadres) => HandleResponse(loadres));
            }
        }
    }


    public static void GenericEvent(string actionid = "", string data = "")
    {
        if (sessionMetricList == null)
        { sessionMetricList = new List<MetricEvent>(); }

        MetricEvent newEvent = new MetricEvent();
        newEvent.SetEventInfo(uuid, user, sessionID, null);
        GenericEvent metricData = new GenericEvent(actionid, data);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);


        //add it to the list of events this session
        sessionMetricList.Add(newEvent);
        //set the last action to have the duration easily accesible for the event end
        lastActionTime = newEvent.Timestamp();

        //TODO: attempt to upload to database
        //if fail then save

        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;
    }


    //have the id be optional so the event can be logged even in there is unknown information
    public static void QuizStart(string quizid = "")
    {
        if (sessionMetricList == null)
        { sessionMetricList = new List<MetricEvent>(); }
        if (answeredQuestions == null)
        { answeredQuestions = new List<AnswerQuizQuestion>(); }
        else
        {
            //clear any questions from a previous quiz
            answeredQuestions.Clear();
        }


        MetricEvent newEvent = new MetricEvent();
        //newEvent.SetEventInfo("FFFFFFFFF", user, "12ss", null);
        newEvent.SetEventInfo(uuid, user, sessionID, null);
        QuizStart metricData = new QuizStart(quizid);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);


        //add it to the list of events this session
        sessionMetricList.Add(newEvent);
        //set the last action to have the duration easily accesible for the event end
        lastActionTime = newEvent.Timestamp();

        //TODO: attempt to upload to database
        //if fail then save

        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;
    }



    public static void QuizEnd(string quizid = "", string quizResult = "")
    {

        if (answeredQuestions == null)
        { answeredQuestions = new List<AnswerQuizQuestion>(); }

        MetricEvent newEvent = new MetricEvent();
        QuizEnd metricData = new QuizEnd(quizid, quizResult, answeredQuestions, DateTime.Now.Subtract(lastActionTime).TotalSeconds);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);

        sessionMetricList.Add(newEvent);

        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;


    }


    public static void AnswerQuizQuestion(string quizid = "", string answerid = "", string answerGiven = "", string answerResult = "")
    {

        if (answeredQuestions == null)
        { answeredQuestions = new List<AnswerQuizQuestion>(); }

        MetricEvent newEvent = new MetricEvent();
        AnswerQuizQuestion metricData = new AnswerQuizQuestion(answerid, answerGiven, answerResult);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);



        answeredQuestions.Add(metricData);

        if (isSending == true)
        { APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res)); }
        sessioncount++;
    }


    public static void ActivityStart(string activityid = "")
    {



        MetricEvent newEvent = new MetricEvent();
        ActivityStart metricData = new ActivityStart(activityid);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);

        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;
    }


    public static void ActivityEnd(string activityid = "", string result = "")
    {
        double duration = DateTime.Now.Subtract(lastActionTime).TotalSeconds;

        MetricEvent newEvent = new MetricEvent();
        ActivityEnd metricData = new ActivityEnd(activityid, result, Math.Abs(duration));
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);

        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;

    }



    public static void JoinCourse(string courseid = "", string assigner = "")
    {

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        MetricEvent newEvent = new MetricEvent();
        JoinCourse metricData = new JoinCourse(courseid, assigner);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);


        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;
    }


    public static void LeaveCourse(string courseid = "", string result = "", string awards = "", float progress = 0.0f)
    {


        MetricEvent newEvent = new MetricEvent();
        LeaveCourse metricData = new LeaveCourse(courseid, result, awards, progress);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);


        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;
    }


    public static void GainBadge(string badgeid = "", string newteam = "", string newawardedby = "")
    {



        MetricEvent newEvent = new MetricEvent();
        GainBadge metricData = new GainBadge(badgeid, newteam, newawardedby);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);


        APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res));
        sessioncount++;
    }


    public static void AnswerInstructorQuestion(string courseid = "", string instructorid = "", string questionid = "", string answer = "", string result = "")
    {



        MetricEvent newEvent = new MetricEvent();
        AnswerInstructorQuestion metricData = new AnswerInstructorQuestion(courseid, instructorid, questionid, answer, result);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        //Keep a tally of the events in a session in the case of time race conditions to still know which event occured first
        newEvent.SessionIndex(sessionMetricList.Count);
        if (isSending == true)
        { APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res)); }
        sessioncount++;
    }

    public static void AskInstructorQuestion(string courseid = "", string instructorid = "", string questionid = "")
    {

        MetricEvent newEvent = new MetricEvent();
        AskInstructorQuestion metricData = new AskInstructorQuestion(courseid, instructorid, questionid);
        newEvent.SetEventInfo(uuid, user, sessionID, null);

        newEvent.Timestamp(DateTime.UtcNow);
        newEvent.Data(metricData);

        if (isSending == true)
        { APIManager.SubmitMetrics((new MetricEvent[1] { newEvent }), (res) => HandleResponse(res)); }
        sessioncount++;
    }








}
