using GameBrewStudios;
using GameBrewStudios.Networking;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class InteractiveLesson

{
    public string _id;
    public string name;
    public string description;
    public string data;
    //public bool badgeOnComplete;
    //public string badgeOnCompleteId;
    //public bool badgeOnCompleteScoreRequired;
    //public string badgeOnCompleteScore;
    //public string badgeOnCompleteName;
    public Team team;
    public CommandEvent[] onComplete;

    /*====================
           HELPERS
    ====================*/


    public string quizId()
    {
        if (onComplete == null) return "";

        CommandEvent badgeEvent = this.onComplete.FirstOrDefault(x => x.eventName.ToLower().Equals("doquiz"));
        if (badgeEvent == null)
        {
            return "";
        }
        return badgeEvent.args[0];

    }

    public bool hasQuiz()
    {
        return !string.IsNullOrEmpty(this.quizId());
    }

    public string badgeId()
    {

        CommandEvent id = this.onComplete.FirstOrDefault(x => x.eventName.ToLower().Equals("awardbadge"));
        if (id == null)
        {
            return "";
        }
        return id.args[0]; //AwardBadge should only have 1 arg and it should be the ID of the badge in the database

    }

    public bool hasBadge()
    {
        return !string.IsNullOrEmpty(this.badgeId());
    }


    public void EvaluateCommands()
    {
        onComplete.ToList().ForEach((x) => x.Evaluate());
    }
    public bool isComplete()
    {
        for (int i = 0; i < User.current.progress.Length; i++)
        {
            for (int j = 0; j < User.current.progress[i].lessonsComplete.Length; j++)
            {
                if (User.current.progress[i].lessonsComplete[j]._id == this._id)
                    return true;
            }
        }

        return false;
    }

    public bool isPlayable()
    {
        int index = GetIndex();
        return index == 0 || (InteractiveLevel.current.lessons[index - 1].isComplete() && InteractiveLevel.current.lessons[index - 1].quizPassed());
    }


    public int GetIndex()
    {
        return InteractiveLevel.current.lessons.ToList().IndexOf(this);
    }

    public static InteractiveLesson current;

    internal bool quizPassed()
    {
        if (!this.hasQuiz() || string.IsNullOrEmpty(this.quizId()))
        {
            Debug.Log("Lesson doesnt have a quiz: " + this.name);
            return true;
        }

        for (int i = 0; i < User.current.progress.Length; i++)
        {
            for (int j = 0; j < User.current.progress[i].quizScores.Length; j++)
            {
                if (User.current.progress[i].quizScores[j].quiz == this.quizId())
                {
                    Debug.Log("Quiz ID " + this.quizId() + " didPass = " + User.current.progress[i].quizScores[j].didPass);
                    return User.current.progress[i].quizScores[j].didPass;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// This method calls PUT /api/progress on the server, which will automatically mark parent Levels and Courses complete as necessary
    /// </summary>
    /// <param name="onComplete"></param>
    
    public void CompleteLesson(Action<CourseProgress[],bool,bool> onComplete)
    {
        


        bool levelCompleted = false;
        bool courseCompleted = false;
        APIManager.CompleteLesson(InteractiveCourse.current, InteractiveLesson.current, true, (courseprogress) =>
        {
            if(hasQuiz())
            {
                onComplete?.Invoke(courseprogress, levelCompleted, courseCompleted);
                return;
            }
            APIManager.UpdateProgress(courseprogress, onComplete);

        });
    }
}