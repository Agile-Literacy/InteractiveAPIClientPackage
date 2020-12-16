using AgileLiteracy.API;
using UnityEngine;

public class EndpointTester : MonoBehaviour
{
    public string authCode;
    public string teamId;
    private void Start()
    {
        UserSignin();
    }

    [ContextMenu("Authenticate")]
    public void UserSignin()
    {
        APIManager.Authenticate("donima4u2004@yahoo.com","password",(success) =>
        {
            Debug.Log("Authentication success = "  + success);
        });
    }
    [ContextMenu("ListCourse")]
    public void ListCourse()
    {
        APIManager.ListCourses(teamId,(courses) =>
        {
            Debug.Log(courses.Length);
        });
    }
    [ContextMenu("ListLessons")]
    public void ListLessons()
    {
        APIManager.ListLessons(teamId, (response) =>
        {
            
        });
    }
    [ContextMenu("ListQuizes")]
    public void ListQuizes()
    {
        APIManager.ListQuizs(teamId, (quizzes) =>
        {
            Debug.Log("Got " + quizzes.Length + " quizzes back from the server");
        });
    }
    [ContextMenu("ListLevels")]
    public void ListLevels()
    {
        APIManager.ListLevels(teamId, (levels) =>
        {
            Debug.Log(levels.Length);
        });
    }
    [ContextMenu("ListQuestions")]
    public void ListQuestions()
    {
        APIManager.ListQuestions(teamId, (questions) =>
        {
            Debug.Log("Found Questions: " + questions.Length);
        });
    }

}
