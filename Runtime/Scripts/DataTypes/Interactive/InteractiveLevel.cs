using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





[System.Serializable]
public class InteractiveLevel
{
    public string name;
    public string _id;
    public string description;
    public InteractiveLesson[] lessons;
    public InteractiveQuiz[] quizzes;


    public static InteractiveLevel current;

    public CommandEvent[] onComplete;

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

    public int NextAvailableLessonIndex()
    {
        for (int i = 0; i < lessons.Length; i++)
        {
            if (!lessons[i].isComplete() || !lessons[i].quizPassed())
                return i;
        }


        //If we got here, all the lessons have been completed and we just return the last lesson index in the array
        return lessons.Length - 1;

    }
    public int GetIndex()
    {
        return InteractiveCourse.current.levels.ToList().IndexOf(this);
    }
}
