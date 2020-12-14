using System.Linq;
using UnityEngine;


[System.Serializable]
public class InteractiveQuiz
{
    public string _id;
    public string name;
    public string description;
    public string team;
    public InteractiveQuestion[] questions;

    public CommandEvent[] onComplete;

    public static InteractiveQuiz current;



    public string badgeId()
    {

        if (onComplete == null) return "";

        CommandEvent badgeEvent = this.onComplete.FirstOrDefault(x => x.eventName.ToLower().Equals("awardbadge"));
        if (badgeEvent == null)
        {
            return "";
        }
        return badgeEvent.args[0]; //AwardBadge should only have 1 arg and it should be the ID of the badge in the database

    }

    public bool hasBadge()
    {
        return !string.IsNullOrEmpty(this.badgeId());
    }

    public void ShuffleQuestions()
    {
        for (int i = 0; i < this.questions.Length; i++)
        {
            int rand = Random.Range(0, this.questions.Length);
            InteractiveQuestion a = this.questions[i];
            InteractiveQuestion b = this.questions[rand];
            this.questions[i] = b;
            this.questions[rand] = a;
        }
    }
}
