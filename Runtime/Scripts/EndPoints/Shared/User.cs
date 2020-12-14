using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace GameBrewStudios.Networking
{
    public partial class APIManager
    {
        public static void CanConnect(Action<ServerResponse> onComplete)
        {
            ServerRequest.CallAPI("/connect", HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, false);
        }

        // Start is called before the first frame update
        public static void Authenticate(string username, string password, Action<bool> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            if (username.Contains("@"))
                body.Add("email", username);
            else
                body.Add("username", username);

            body.Add("password", password);


            ServerRequest.CallAPI("/authenticate", HTTPMethod.POST, body, (response) =>
            {
                if (response.hasError)
                {
                    Debug.LogError(response.Error);
                    onComplete?.Invoke(false);
                    return;
                }

                Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.text);

                if (responseData != null)
                {
                    if (responseData.ContainsKey("token"))
                    {
                        AuthToken.current = new AuthToken((string)responseData["token"], responseData.ContainsKey("refreshToken") ? (string)responseData["refreshToken"] : null);
                    }

                    if (responseData.ContainsKey("user"))
                    {
                        User.current = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(responseData["user"]));
                    }

                    if (User.current != null && AuthToken.current != null)
                    {
                        
                        if (responseData.ContainsKey("memberships"))
                        {
                            User.current.memberships = JsonConvert.DeserializeObject<TeamMember[]>(JsonConvert.SerializeObject(responseData["memberships"]));
                            //Debug.Log("Found " + User.current.memberships.Length + " memberships for user");
                        }
                        else
                        {
                            User.current.memberships = new TeamMember[] { };
                        }

                        if (responseData.ContainsKey("invitations"))
                        {
                            User.current.invitations = JsonConvert.DeserializeObject<TeamInvitation[]>(JsonConvert.SerializeObject(responseData["invitations"]));
                            //Debug.Log("Found " + User.current.memberships.Length + " memberships for user");
                        }
                        else
                        {
                            User.current.invitations = new TeamInvitation[] { };
                        }

                        //if (responseData.ContainsKey("groups"))
                        //{
                        //    User.current.groups = JsonConvert.DeserializeObject<TeamGroup[]>(JsonConvert.SerializeObject(responseData["groups"]));
                        //    //Debug.Log("Found " + User.current.groups.Length + " groups for user");
                        //}
                        //else
                        //{
                        //    User.current.groups = new TeamGroup[] { };
                        //}

                        onComplete?.Invoke(true);
                        return;
                    }
                }

                onComplete?.Invoke(false);
            }, false);
        }
        public static void Register(string username, string password, string displayName, string teamName, Action<bool> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();

            body.Add("email", username);
            body.Add("password", password);
            body.Add("displayName", displayName);
            body.Add("teamName", teamName);

            ServerRequest.CallAPI("/register", HTTPMethod.POST, body, (response) =>
             {
                 if (response.hasError)
                 {
                     Debug.LogError(response.Error);
                     onComplete?.Invoke(false);
                     return;
                 }

                 Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.text);

                 if (responseData != null)
                 {
                     if (responseData.ContainsKey("token"))
                     {
                         AuthToken.current = new AuthToken((string)responseData["token"], responseData.ContainsKey("refreshToken") ? (string)responseData["refreshToken"] : null);
                     }

                     if (responseData.ContainsKey("user"))
                     {
                         User.current = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(responseData["user"]));
                     }

                     if (User.current != null && AuthToken.current != null)
                     {
                         if (responseData.ContainsKey("memberships"))
                         {
                             User.current.memberships = JsonConvert.DeserializeObject<TeamMember[]>(JsonConvert.SerializeObject(responseData["memberships"]));
                             //Debug.Log("Found " + User.current.memberships.Length + " memberships for user");
                             if (User.current.memberships.Length == 1)
                                 User.current.selectedMembership = User.current.memberships[0];

                         }
                         else
                         {
                             User.current.memberships = new TeamMember[] { };
                         }

                         //if (responseData.ContainsKey("groups"))
                         //{
                         //    User.current.groups = JsonConvert.DeserializeObject<TeamGroup[]>(JsonConvert.SerializeObject(responseData["groups"]));
                         //    //Debug.Log("Found " + User.current.groups.Length + " groups for user");

                         
                         //}
                         //else
                         //{
                         //    User.current.groups = new TeamGroup[] { };
                         //}

                         onComplete?.Invoke(true);
                         return;
                     }
                 }

                 onComplete?.Invoke(false);
             }, false);
        }
        public static void RefreshToken(Action<ServerResponse> onComplete)
        {
            ServerRequest.CallAPI("/refreshToken", HTTPMethod.GET, null, onComplete, true);
        }

        public static void ForgotPassword(string email, Action<Dictionary<string, object>> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>() 
            {
                {"email", email }
            };
            ServerRequest.CallAPI("/forgot", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, false);
        }

        public static void ResetPassword(string email, string code, string password, Action<Dictionary<string, object>> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"code", code },
                {"email", email },
                {"password", password }
            };
            ServerRequest.CallAPI("/reset", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, false);
        }

        public static void GetUserDetails(Action<Dictionary<string,object>> onComplete)
        {
            ServerRequest.CallAPI("/me", HTTPMethod.GET, null, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, true);
        }

        public static void CreateSupportTicket(User user, string application, string message, string category, Action<SupportTicket> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"email", user.email },
                {"user", user._id },
                {"message", message },
                {"category", category},
                {"application", application },
                {"team", user.selectedMembership != null ? user.selectedMembership._id : null}
            };
            ServerRequest.CallAPI("/support/tickets", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, "ticket", onComplete); }, true);
        }

        

        public static void UpdateAvatar(GellingAvatar avatar, Action<ServerResponse> onComplete)
        {
            /*
            {
                "_id": ObjectId
                "user": ObjectId
                "gender": "MALE"
                skinColor: 0
                hairStyle: 0
                hairColor: 0
                suitStyle: 0
                suitColor1: 0
                suitColor2: 0
                suitColor3: 0
                suitColor4: 0
            }
            */
            avatar.gender = avatar.gender.Equals("MALE") ? "Male" : "Female";
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"_id", avatar._id },
                {"user", avatar.user },
                {"gender", avatar.gender },
                {"skinColor", avatar.skinColor },
                {"hairStyle", avatar.hairStyle },
                {"hairColor", avatar.hairColor },
                {"suitStyle", avatar.suitStyle },
                {"suitColor1", avatar.suitColor1 },
                {"suitColor2", avatar.suitColor2 },
                {"suitColor3", avatar.suitColor3 },
                {"suitColor4", avatar.suitColor4 },
                 {"facialHair", avatar.facialHair }

            };
            ServerRequest.CallAPI("/avatar", HTTPMethod.PUT, body, onComplete, true);
        }

    }
}