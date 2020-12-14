using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;


[System.Serializable]

public class MetricEvent 
{

    //The eventdata class is structured to mimic the way it will appear in the database
    //This was when performing a local save the data will remain in the format needed in the database

    public string uuid = null;
    public string user = null;
    public string team = null;
    public DateTime localTime;
    public int sessionEventIndex;
    public MetricData data;

    public MetricEvent()
    { }

    public MetricData Data()
    { return this.data; }

    public void Data(MetricData newData)
    { this.data = newData; }

    public virtual string GetInfo()
    {
        //{ 
        //    "user": this.User(),
        //    "team": String,
        //    "localTime": String,
        //    "sessionEventIndex": Int,
        //    "uuid": String,
        //    "data": Dictionary<string, object> 
        // }
       
        string tempstring = " Event Information: UUID - ";
        tempstring += this.Uuid();
        tempstring += " : User -";
        tempstring += this.User();
        tempstring += " : Team -";
        tempstring += this.Team();


        return tempstring;
    }

    public void SetEventInfo(string newuuid, string newuser, string newsessionID, string newteam)
    {
        this.uuid = newuuid;
        this.user = newuser;
        this.team = newteam;
    }

    



   

    public void Uuid(string newuuid)
    { this.uuid = newuuid; }

    public string Uuid()
    { return this.uuid; }




    public void User(string newuser)
    { this.user = newuser; }

    public string User()
    { return this.user; }


    public void Team(string newteam)
    { this.team = newteam; }

    public string Team()
    { return this.team; }


    public void Timestamp(DateTime newtime)
    { this.localTime = newtime; }

    public DateTime Timestamp()
    { return this.localTime; }

    public void SessionIndex(int newIndex)
    { this.sessionEventIndex = newIndex; }

    public int SessionIndex()
    { return this.sessionEventIndex; }

 

    public virtual string ActionID()
    {
        return ""; 
    }

    public string PrintInfo()
    {
        return ""; 
    }
}


[System.Serializable]
public class MetricData {

    public string eventType;
    public string actionID;
    public void Eventtype(string newEventType)
    { this.eventType = newEventType; }

    public string Eventtype()
    { return this.eventType; }

    public void ActionID(string newActionID)
    {
        this.actionID = newActionID;
    }
    public virtual string ActionID()
    {
        return this.actionID;
    }

    public virtual string GetInfo()
    {
        //{ 
        //    "user": this.User(),
        //    "team": String,
        //    "localTime": String,
        //    "sessionEventIndex": Int,
        //    "uuid": String,
        //    "data": Dictionary<string, object> 
        // }

        string tempstring = " Event Information: UUID - ";



        return tempstring;
    }

}

[System.Serializable]
public class QuizStart : MetricData
{
    public string quizID;
    
    public QuizStart(string quizid)
    {
     
        this.quizID = quizid;
        this.actionID = quizid;
        this.Eventtype("QuizStart");
    }
    public string QuizID() { return this.quizID; }



    public override string GetInfo()
    {

        string tempstring = " quiz start";



        return tempstring;
    }

    public override string ActionID()
    {
        return this.quizID;
    }

}


[System.Serializable]
public class QuizEnd : MetricData
{

    public string quizID;
    public string result;
    public List<AnswerQuizQuestion> questions;
    public double duration;

 

    public QuizEnd(string quizid, string result, List<AnswerQuizQuestion> questions, double newduration)
    {
        this.quizID = quizid;
        this.result = result;
        this.questions = questions;
        this.duration = newduration;

        this.Eventtype("QuizEnd");
    }

    public QuizEnd(string quizid)
    {
        this.quizID = quizid;

    }
  

    public override string GetInfo()
    {

        string tempstring = " quiz end";
 

        return tempstring;
    }

    public override string ActionID()
    {
        return this.quizID;
    }

    public string QuizID() { return this.quizID; }
    public string Result() { return this.result; }
    public double Duration() { return this.duration; }

}

[System.Serializable]
public class AnswerQuizQuestion : MetricData
{
    public string questionID;
    public string answer;
    public string result;

    public AnswerQuizQuestion(string questionID, string answer, string result)
    {
        this.questionID = questionID;
        this.answer = answer;
        this.result = result;
        this.Eventtype("AnswerQuizQuestion");
    }



    public string QuestionID() { return this.questionID; }
    public string Answer() { return this.answer; }
    public string Result() { return this.result; }

}


[System.Serializable]
public class ActivityStart : MetricData
{
    public string activityID;
    public string assigner;

    public ActivityStart(string activityid)
    {
        this.activityID = activityid;

    }

    public ActivityStart(string activityid, string assigner)
    {
        this.activityID = activityid;
        this.assigner = assigner;
        this.Eventtype("ActivityStart");
    }

    public override string ActionID()
    {
        return this.activityID;
       
    }

    public string ActivityID() { return this.activityID; }
    public string Assigner() { return this.assigner; }
}


[System.Serializable]
public class ActivityEnd : MetricData
{
    public string activityID;
    public string result;
    public double duration;

    public ActivityEnd(string activityid, string result, double duration)
    {
        this.activityID = activityid;
        this.result = result;
        this.duration = duration;
        this.Eventtype("ActivityEnd");
    }

    public override string ActionID()
    {
        return this.activityID;
    }

    public string ActivityID() { return this.activityID; }
    public string Result() { return this.result; }
    public double Duration() { return this.duration; }

}


[System.Serializable]
public class JoinCourse : MetricData
{
    public string courseID;
    public string assigner;

    public JoinCourse(string courseid)
    {
        this.courseID = courseid;
        this.Eventtype("JoinCourse");
    }

    public JoinCourse(string courseid, string assigner)
    {
        this.courseID = courseid;
        this.assigner = assigner;
        this.Eventtype("JoinCourse");
    }

}


[System.Serializable]
public class LeaveCourse : MetricData
{
    public string courseID;
    public string result;
    public string awards;
    public float progress;
    public LeaveCourse(string courseid)
    {
        this.courseID = courseid;
        this.Eventtype("LeaveCourse");
    }

    public LeaveCourse(string courseid, string newresult, string newawards, float newprogress)
    {
        this.courseID = courseid;
        this.result = newresult;
        this.awards = newawards;
        this.progress = newprogress;
        this.Eventtype("LeaveCourse");
    }

    public void CourseID(string newcourseid)
    { this.courseID = newcourseid; }

    public string CourseID()
    { return this.courseID; }

}


[System.Serializable]
public class GainBadge : MetricData
{
    public string badgeID;
    public string team;
    public string awardedBy;

    public GainBadge(string badgeid, string newteam, string newawardedby)
    {
        this.badgeID = badgeid;
        this.team = newteam;
        this.awardedBy = newawardedby;
        this.Eventtype("GainBadge");
    }

    public void BadgeID(string newbadgeid)
    { this.badgeID = newbadgeid; }

    public string BadgeID()
    { return this.badgeID; }

    public void Team(string newteam)
    { this.team = newteam; }

    public string Team()
    { return this.team; }

    public void AwardedBy(string newawardedby)
    { this.awardedBy = newawardedby; }

    public string AwardedBy()
    { return this.awardedBy; }

}


[System.Serializable]
public class AnswerInstructorQuestion : MetricData
{
    public string courseID;
    public string instructorID;
    public string questionID;
    public string answer;
    public string result;

    public AnswerInstructorQuestion(string newcourseID, string newinstructorID, string newQuestionID, string newanswer,string newresult)
    {
        this.courseID = newcourseID;
        this.instructorID = newinstructorID;
        this.questionID = newQuestionID;
        this.answer = newanswer;
        this.result = newresult;
        this.Eventtype("AnswerInstructorQuestion");
    }

    public void QuestionID(string newquestionID)
    { this.questionID = newquestionID; }

    public string QuestionID()
    { return this.questionID; }

    public void InstructorID(string newinstructorID)
    { this.instructorID = newinstructorID; }

    public string InstructorID()
    { return this.instructorID; }

    public void CourseID(string newcourseID)
    { this.courseID = newcourseID; }

    public string CourseID()
    { return courseID; }

    public string Answer()
    { return this.answer; }

    public string Result()
    { return this.result; }

}


[System.Serializable]
public class AskInstructorQuestion : MetricData
{
    public string courseID;
    public string instructorID;
    public string questionID;

    public AskInstructorQuestion(string newcourseID, string newinstructorid, string questionid)
    {
        this.courseID = newcourseID;
        this.instructorID = newinstructorid;
        this.questionID = questionid;
        this.Eventtype("AskInstructorQuestion");
    }

    public void QuestionID(string newquestionID)
    { this.questionID = newquestionID; }

    public string QuestionID()
    { return this.questionID; }

    public void InstructorID(string newinstructorID)
    { this.instructorID = newinstructorID; }

    public string InstructorID()
    { return this.instructorID; }

    public void CourseID(string newcourseID)
    { this.courseID = newcourseID; }

    public string CourseID()
    { return courseID; }

}

[System.Serializable]
public class GenericEvent : MetricData
{
    public string actioniD;
    public string info;

    public GenericEvent(string newactionID, string newinfo)
    {
        this.actionID = newactionID;
        this.info = newinfo;
        this.Eventtype("GenericEvent");
    }


    public override string ActionID()
    {
        return this.actioniD;
    }
    public void Info(string newinfo)
    { this.info = newinfo; }

    public string Info()
    { return this.info; }
}