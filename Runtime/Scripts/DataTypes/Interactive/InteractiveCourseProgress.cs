
using System.Linq;

[System.Serializable]
public class CourseProgress
{
    public string _id;
    public string course;
    public bool completed;
    /// <summary>
    /// This value relates directly to the doors screen on the laptop and which "doors" are completed
    /// </summary>
    public InteractiveLevel[] levelsComplete;

    /// <summary>
    /// This value relates to the individual lessons inside the learning area
    /// </summary>
    public InteractiveLesson[] lessonsComplete;


    public InteractiveQuizScore[] quizScores;

    //HELPER FUNCTIONS

    public bool isLessonComplete(string lessonId)
    {
        InteractiveLesson lesson = this.lessonsComplete.ToList().FirstOrDefault((x) => x._id == lessonId);
        return !(lesson == null);
    }

    public bool isLevelComplete(string levelId)
    {
        InteractiveLevel level = this.levelsComplete.ToList().FirstOrDefault((x) => x._id == levelId);
        return !(level == null);
    }




    public bool isCourseComplete() => this.completed == true;
}