using GameBrewStudios;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Team
{
    public string _id;
    public string name;
    public string owner;
    public string[] members;

    public object[] products;
    public string logo; //url to texture online
    
    /// <summary>
    /// Populate this field manually using APIManager.ListTeamEvents
    /// </summary>
    public TeamEvent[] events;
}

[System.Serializable]
public class TeamEvent
{
    public string _id;
    public string team;
    public string creator;
    public string name;
    public string location;
    public string details;
    public bool allDayEvent;

    /// <summary>
    /// ISO String Format: 2020-08-09T19:22:54.638+00:00 ("yyyy-MM-ddTHH\:mm\:ss.fffzzz")
    /// </summary>
    public string startDate;
    public string endDate;


    public TeamEventInvitation[] attendees;
    
    
    internal List<string> newInvites;
    internal List<string> removeInvites;

    public DateTime startDateTime
    {
        get
        {
            return DateTime.Parse(this.startDate, null, System.Globalization.DateTimeStyles.AssumeUniversal);
        }
    }

    public DateTime endDateTime
    {
        get
        {
            return DateTime.Parse(this.endDate, null, System.Globalization.DateTimeStyles.AssumeUniversal);
        }
    }

}

[System.Serializable]
public enum TeamEventAttendanceStatus
{
    WaitingForResponse = 0,
    Attending = 1,
    Tentative = 2,
    NotAttending = 3
}

[System.Serializable]
public class TeamEventInvitation
{
    public string _id;
    public string email;
    public string member;
    public TeamEventAttendanceStatus status;
}