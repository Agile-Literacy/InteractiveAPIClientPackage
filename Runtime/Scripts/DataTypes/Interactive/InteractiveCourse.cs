[System.Serializable]
public class InteractiveCourse
{
    public string _id;
    public string name;
    public string description;

    public InteractiveLevel[] levels;
    public InteractiveTheme theme;
    
    public string team;


    /// <summary>
    /// Helper property for setting the currently selected course in a statically accessible way
    /// </summary>
    public static InteractiveCourse current;
}

[System.Serializable]
public class InteractiveCourseAssignment
{
    public string _id;
    public string name;
    public string description;
    public string team;
}