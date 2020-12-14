using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBrewStudios.Networking
{
    [System.Serializable]
    public class TeamGroup
    {
        public string _id;
        public string name;
        public InteractiveCourseAssignment[] interactiveCourses;

        public static TeamGroup current;

    }
}