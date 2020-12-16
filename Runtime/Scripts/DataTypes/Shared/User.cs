using AgileLiteracy.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgileLiteracy.API
{



    /// <summary>
    /// This class represents an exact replica of the UserSchema on the server, with some added helper functions for ease of use
    /// </summary>


    [System.Serializable]
    public class UserSimple
    {
        /// <summary>
        /// Represents a Mongo ObjectID from the User collection in the database
        /// </summary>
        public string _id;

        /// <summary>
        /// Email address of the user, should assume validity as the server is responsible for checking it
        /// </summary>
        public string email;

        /// <summary>
        /// Username of the user, used only for display in the Chat feature we add later (so users can message eachother with @username) and to provide an alternative means of login instead of using the email address
        /// </summary>
        public string username;

        /// <summary>
        /// When displaying the name of a user anywhere in the app (besides chat) always use the Display Name property
        /// </summary>
        public string displayName;

        /// <summary>
        /// Not currently used for anything, but will be used for checking access permissions later on. Defaults to: 'player'
        /// </summary>
        public string role;
    }

    [System.Serializable]
    public class User
    {
        /// <summary>
        /// Represents a Mongo ObjectID from the User collection in the database
        /// </summary>
        public string _id;

        /// <summary>
        /// Email address of the user, should assume validity as the server is responsible for checking it
        /// </summary>
        public string email;

        /// <summary>
        /// Username of the user, used only for display in the Chat feature we add later (so users can message eachother with @username) and to provide an alternative means of login instead of using the email address
        /// </summary>
        public string username;

        /// <summary>
        /// When displaying the name of a user anywhere in the app (besides chat) always use the Display Name property
        /// </summary>
        public string displayName;

        /// <summary>
        /// Not currently used for anything, but will be used for checking access permissions later on. Defaults to: 'player'
        /// </summary>
        public string role;

        /// <summary>
        /// Contains customization settings for the players 3D avatar
        /// </summary>
        public GellingAvatar gellingAvatar;

        /*===================================================================
                    Begin Helper Functions/Getters/Setters
        =====================================================================*/

        public static event System.Action OnUserDisconnected;

        private static User _current;
        /// <summary>
        /// After successful login, you should create an Instance of the user class, and then store it into this static variable for easily accessing the local users data throughout the app code.
        /// </summary>
        public static User current
        {
            get
            {
                if (_current == null)
                {
                    OnUserDisconnected?.Invoke();
                }

                return _current;
            }
            set
            {
                _current = value;
            }
        }


        public TeamMember[] memberships;

        public TeamInvitation[] invitations;

        /// <summary>
        /// points to one of the _id's of the memberships array above
        /// </summary>
        public TeamMember selectedMembership
        {
            get
            {
                return _selectedMembership;
            }
            set
            {
                _selectedMembership = value;
                List<InteractiveCourseAssignment> courses = selectedMembership.interactiveCourses.ToList();
                for (int i = 0; i < selectedMembership.groups.Length; i++)
                {
                    for (int j = 0; j < selectedMembership.groups[i].interactiveCourses.Length; j++)
                    {
                        if (selectedMembership.interactiveCourses.Any(x => x._id == selectedMembership.groups[i].interactiveCourses[j]._id))
                            continue;

                        courses.Add(selectedMembership.groups[i].interactiveCourses[j]);
                    }
                }
                selectedMembership.interactiveCourses = courses.ToArray();
            }
        }
        private TeamMember _selectedMembership;


        public List<string> badges;

        public CourseProgress[] progress;


        public bool hasAnyAssignments()
        {
            return memberships != null && memberships.Length > 0 && memberships.Any(x => x.hasAssignments());
        }
    }


    static class UserExtensions
    {

        public static InteractiveQuizScore GetQuizScore(this User user, string quizID)
        {
            for (int x = 0; x < user.progress.Length; x++)
            {
                for (int y = 0; y < user.progress[x].quizScores.Length; y++)
                {
                    if (user.progress[x].quizScores[y]._id == quizID)
                    {
                        return user.progress[x].quizScores[y];
                    }
                }
            }

            return null;
        }

        public static bool isLevelComplete(this User user, string courseId, string levelId)
        {
            Debug.Log("checking for a match with " + courseId + "level id" + levelId);
            if (user.progress == null)
            {
                Debug.Log("user progress is empty");
                return false;
            }

            CourseProgress progress = user.progress.FirstOrDefault(x => x.course == courseId);
            if (progress != null)
            {
                Debug.Log("got course progress: " + progress.course);
                for (int i = 0, imax = progress.levelsComplete.Length; i < imax; i++)
                {
                    Debug.Log("in the level: " + progress._id);
                    if (progress.levelsComplete[i]._id == levelId)
                    {
                        Debug.Log("found a match");
                        return true;
                    }

                }
            }
            return false;
        }

        public static bool isLessonComplete(this User user, string courseId, string lessonId)
        {
            CourseProgress progress = user.progress.FirstOrDefault(x => x._id == courseId);
            if (progress == null) return false;

            return progress.isLessonComplete(lessonId);
        }

        public static bool isCourseComplete(this User user, string courseId)
        {
            CourseProgress progress = user.progress.FirstOrDefault(x => x._id == courseId);
            if (progress == null) return false;

            return progress.completed;
        }
    }

    [System.Serializable]
    public class GellingAvatar
    {
        public string _id;
        public string user;
        public string gender = "Male";
        public int skinColor = 0;
        public int hairStyle = 1;
        public int hairColor = 0;
        public int suitStyle = 0;
        public int suitColor1 = 3;
        public int suitColor2 = 1;
        public int suitColor3 = 2;
        public int suitColor4 = 1;
        public int suitColor5 = 1;

        //MALE ONLY
        public int facialHair = 1;

    }

    [System.Serializable]
    public class SupportTicket
    {
        public string _id;
        public string user;
        public string email;
        public string message;
        public string category;
        public string status; //Open, Closed, etc
    }
}