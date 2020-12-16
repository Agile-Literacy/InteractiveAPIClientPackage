using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.Networking;
using System.IO;
#if UNITY_WEBGL && DASHBOARD
using WebGLFileUploader;
#endif

namespace AgileLiteracy.API
{


    public partial class APIManager
    {

        public static void GetTeam(string id, Action<Team> onComplete)
        {
            ServerRequest.CallAPI("/teams/" + id, HTTPMethod.GET, null, (response) =>
            {
                if (!response.hasError)
                {
                    Team team = JsonConvert.DeserializeObject<Team>(response.text);
                    onComplete?.Invoke(team);
                    return;
                }
                else
                {
                    Debug.LogError("GetTeam Error: " + response.Error);
                }

                onComplete?.Invoke(null);

            }, true);
        }

        public static void UpdateTeam(string id, string name, string logo, Action<Team> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("team", id);
            body.Add("name", name);
            body.Add("logo", logo);

            ServerRequest.CallAPI("/teams/update", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, "team", onComplete); }, true);
        }

        public static void InviteMember(string email, Action<TeamInvitationResponse> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("email", email);


            ServerRequest.CallAPI("/teams/invite", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void AcceptInvitation(string inviteId, Action<TeamMember[]> onComplete)
        {
            ServerRequest.CallAPI("/teams/invitation/" + inviteId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, "memberships", onComplete); }, true);
        }

        public static void UpdateMemberRank(string memberId, int rank, Action<Dictionary<string, object>> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"id", memberId },
                {"rank", rank }
            };
            ServerRequest.CallAPI("/teams/members/rank", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }


        public static void DeleteInvitation(string inviteId, Action<Dictionary<string, object>> onComplete)
        {
            ServerRequest.CallAPI("/teams/invitation/" + inviteId, HTTPMethod.DELETE, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void ListInvitations(string teamId, Action<TeamInvitation[]> onComplete)
        {
            //GET /api/teams/invitations/:teamId

            ServerRequest.CallAPI("/teams/invitations/" + teamId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, "invitations", onComplete); }, true);
        }

        public static void ListTeamMembers(string teamId, Action<TeamMember[]> onComplete)
        {
            //GET /api/teams/members/:teamId

            ServerRequest.CallAPI("/teams/members/" + teamId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, "members", onComplete); }, true);
        }

        public static void AddGroupToMember(TeamMember member, TeamGroup group, Action<TeamMember> onComplete)
        {
            //POST /api/teams/members/addGroup

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"member", member._id },
                {"group", group._id }
            };

            ServerRequest.CallAPI("/teams/members/addGroup", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void RemoveGroupFromMember(TeamMember member, TeamGroup group, Action<TeamMember> onComplete)
        {
            //POST /api/teams/members/addGroup

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"member", member._id },
                {"group", group._id }
            };

            ServerRequest.CallAPI("/teams/members/removeGroup", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }


        public static void RemoveMember(TeamMember member, Action<Dictionary<string, object>> onComplete)
        {
            //POST /api/teams/members/remove

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"member", member._id }
            };

            ServerRequest.CallAPI("/teams/members/remove", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void CreateGroup(TeamGroup group, Action<TeamGroup> onComplete)
        {
            //POST /api/teams/groups/create

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", group.name },
                {"team", User.current.selectedMembership.team._id }
            };

            ServerRequest.CallAPI("/teams/groups/create", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void UpdateGroup(TeamGroup group, Action<TeamGroup> onComplete)
        {
            //POST /api/teams/groups/create

            List<string> courseIds = new List<string>();
            foreach (InteractiveCourseAssignment ass in group.interactiveCourses)
            {
                courseIds.Add(ass._id);
            }

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", group.name },
                {"interactiveCourses", courseIds.ToArray() }
            };

            ServerRequest.CallAPI("/teams/groups/" + group._id, HTTPMethod.PUT, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void DeleteGroup(TeamGroup group, Action<bool> onComplete)
        {
            ServerRequest.CallAPI("/teams/groups/" + group._id, HTTPMethod.DELETE, null, (response) =>
            {
                ServerRequest.ResponseHandler<Dictionary<string, object>>(response, null, (dict) =>
                {
                    if (dict != null && dict.ContainsKey("ok") && System.Convert.ToInt32(dict["ok"]) == 1)
                    {
                        onComplete.Invoke(true);
                    }
                    else
                    {
                        onComplete.Invoke(false);
                    }
                });
            }, true);
        }

        public static void ListGroups(string teamId, Action<TeamGroup[]> onComplete)
        {
            //GET /api/teams/groups/:teamId
            ServerRequest.CallAPI("/teams/groups/" + teamId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, "groups", onComplete); }, true);
        }

        public static void GroupRemoveAssignment(TeamGroup group, string courseId, System.Action<TeamGroup> onComplete)
        {
            //DELETE /api/teams/groups/:groupId/assignment

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"course", courseId }
            };

            ServerRequest.CallAPI("/teams/groups/" + group._id + "/assignment/delete", HTTPMethod.PUT, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void GroupAddAssignment(TeamGroup group, string courseId, Action<TeamGroup> onComplete)
        {
            //POST /api/teams/groups/:groupId/assignment

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"course", courseId }
            };

            ServerRequest.CallAPI("/teams/groups/" + group._id + "/assignment", HTTPMethod.PUT, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }


        public static void MemberRemoveAssignment(TeamMember member, string courseId, Action<string[]> onComplete)
        {
            //POST /api/teams/groups/:groupId/assignment/remove

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"course", courseId },
                {"member",  member._id}
            };

            ServerRequest.CallAPI("/teams/members/assignments/remove", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, "courses", onComplete); }, true);
        }

        public static void MemberAddAssignment(TeamMember member, string courseId, Action<string[]> onComplete)
        {
            //POST /api/teams/groups/:groupId/assignment

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"course", courseId },
                {"member",  member._id}
            };

            ServerRequest.CallAPI("/teams/members/assignments", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, "courses", onComplete); }, true);
        }

        public static void GetSignedUploadURL(string filename, string filetype, Action<Dictionary<string, object>> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"filename", filename },
                {"filetype", filetype },
                {"team", User.current.selectedMembership.team._id }
            };

            ServerRequest.CallAPI("/aws/getSignedUrl", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

#if UNITY_WEBGL && DASHBOARD
        public static void UploadAWSFile(string signedUrl, ImportedFileData fileData, System.Action<float> OnProgressUpdated, System.Action<bool,string> onComplete)
        {
            ServerAPI.UploadAWSFile(signedUrl, fileData, OnProgressUpdated, onComplete);
        }
        
        public static void StoreTeamFileRecord(ImportedFileData file, Action<Dictionary<string, object>> onComplete)
        {
            // POST /teams/files/:teamId/storeRecord

            /*
                {
                    "file": {
                        "originalname": "", 
                        "mimetype": "", 
                        "encoding": ""
                    }
                }
            */


            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"file", new Dictionary<string,string>(){
                    {"originalname", file.name },
                    {"mimetype", file.type },
                    {"encoding", "7bit" }
                } }
            };

            ServerRequest.CallAPI("/teams/files/" + User.current.selectedMembership.team._id + "/storeRecord", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

#endif
        public static void DeleteFile(string fileId, Action<Dictionary<string, object>> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"fileId", fileId }
            };

            ServerRequest.CallAPI("/teams/files/" + User.current.selectedMembership.team._id + "/delete", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void ListFiles(string teamId, Action<TeamFile[]> onComplete)
        {
            //GET /api/teams/files/:teamId

            ServerRequest.CallAPI("/teams/files/" + teamId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, "files", onComplete); }, true);
        }

        [System.Serializable]
        public class ListTeamEventsResponse
        {
            public bool success;
            public TeamEvent[] events;

        }
        [System.Serializable]
        public class SingleTeamEventResponse
        {
            public bool success;
            public TeamEvent teamEvent;
        }

        public static void ListTeamEvents(string teamId, Action<ListTeamEventsResponse> onComplete)
        {
            if (string.IsNullOrEmpty(teamId))
            {
                Debug.LogError("INVALID TEAM ID");
                return;
            }

            //GET /api/teams/members/:teamId

            ServerRequest.CallAPI("/teams/" + teamId + "/events/list", HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }
        public static void GetTeamEvent(string teamId, string eventId, Action<SingleTeamEventResponse> onComplete)
        {
            //GET /api/teams/:teamId/events/get/:eventId

            ServerRequest.CallAPI("/teams/" + teamId + "/events/get/" + eventId, HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }
        public static void CreateTeamEvent(TeamEvent eventData, Action<SingleTeamEventResponse> onComplete)
        {
            //POST /api/teams/members/:teamId
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"name", eventData.name },
                {"details", eventData.details },
                {"location", eventData.location },
                {"startDate", eventData.startDate },
                {"endDate", eventData.endDate },
                {"allDayEvent", eventData.allDayEvent },
                {"team", eventData.team }
            };


            ServerRequest.CallAPI("/teams/events/create", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }
        public static void UpdateTeamEvent(TeamEvent teamEvent, Action<SingleTeamEventResponse> onComplete)
        {
            //throw new System.NotImplementedException();
            //PUT /api/teams/members/:teamId

            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"eventId", teamEvent._id },
                {"team", teamEvent.team },
                {"name", teamEvent.name },
                {"location", teamEvent.location },
                {"details", teamEvent.details },
                {"startDate", teamEvent.startDate },
                {"endDate", teamEvent.endDate },
                {"allDayEvent", teamEvent.allDayEvent },
                {"newInvites", teamEvent.newInvites },
                {"removeInvites", teamEvent.removeInvites }

            };

            ServerRequest.CallAPI("/teams/events/update", HTTPMethod.PUT, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }
        public static void DeleteTeamEvent(string eventId, Action<SingleTeamEventResponse> onComplete)
        {
            //DELETE /api/teams/members/:teamId

            ServerRequest.CallAPI("/teams/events/delete/" + eventId, HTTPMethod.DELETE, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }
        /*
            { uri: '/teams/:teamId/events/list', method: 'GET', handler: teams.listTeamEvents, requireBearerToken: true },
            { uri: '/teams/:teamId/events/get/:eventId', method: 'GET', handler: teams.getTeamEvent, requireBearerToken: true },
            { uri: '/teams/events/create', method: 'POST', handler: teams.createTeamEvent, requireBearerToken: true },
            { uri: '/teams/events/update', method: 'PUT', handler: teams.updateTeamEvent, requireBearerToken: true },
            { uri: '/teams/events/delete/:eventId', method: 'DELETE', handler: teams.deleteTeamEvent, requireBearerToken: true },
        */
    }
}