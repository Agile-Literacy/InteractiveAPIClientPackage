
using GameBrewStudios;
using GameBrewStudios.Networking;
using UnityEngine;

[System.Serializable]
public class TeamMember
{
    public string _id;
    public UserSimple user; //we use the UserSimple class so it wont throw errors about recursive serialization
    public bool hidden;
    public string role;
    public bool isOwner;//defines whether or not this teammember is the owner of the team
    public Team team;

    public bool active;
    public InteractiveCourseAssignment[] interactiveCourses;
    //public string[] groups;
    public TeamGroup[] groups;
    public bool hasAssignments()
    {
        return this.interactiveCourses != null && this.interactiveCourses.Length > 0;
    }

    public Sprite GetRoleSprite()
    {
        try
        {
            Sprite icon = Resources.Load<Sprite>("icons/role/" + this.role);
            Debug.Log("LOOKING FOR ICON: /icons/role/" + this.role);
            return icon;
        }
        catch {
            return null;
        }

    }
}

[System.Serializable]
public class TeamInvitation
{
    public string _id;
    public string team;
    public string email;
}

[System.Serializable]
public class TeamInvitationResponse
{
    public bool success;
    public TeamInvitation invitation;
    public bool isNew;
}