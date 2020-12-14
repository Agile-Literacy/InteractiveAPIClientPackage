
[System.Serializable]
public class InteractiveInstructorDeck
{
    public string _id;

    public string name;

    public string team;

    public string createdBy;

    //URLs to .MP4 files that were uploaded to the dashboard
    public string[] videos;
    
    //ObjectID's of quizzes in the database
    public string[] questions;
    
    //URLs to .OBJ files that were uploaded to the dashboard
    public string[] models;

    //ObjectID's of slideShow objects in the database
    public string[] slideShows;

    //HELPERS

    public static InteractiveInstructorDeck current;

}
