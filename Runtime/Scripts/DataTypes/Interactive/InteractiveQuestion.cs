using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class InteractiveQuestion
{
    public static InteractiveQuestion current;

    public string _id;
    public string team;
    public string text;
    public InteractiveQuestionAnswers answers;

    public int pickCount()
    {
     
        return answers.correct.Length;
        
    }

    public List<string> allAnswers()
    {
        List<string> _allAnswers = answers.correct.ToList();
        _allAnswers.AddRange(answers.wrong.ToList());
        return _allAnswers;
    }

    public bool IsAnswerCorrect(string answer)
    {
        return answers.correct.Contains(answer);
    }
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class InteractiveQuestionAnswers
{
    public string[] correct;
    public string[] wrong;
}