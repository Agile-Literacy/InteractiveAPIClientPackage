using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameBrewStudios.Networking
{
    public partial class APIManager
    {
        [System.Serializable]
        public class InstructorDeckResponse
        {
            public InstructorDeckResponse()
            {

            }

            public bool success;
            public string error;
            public InteractiveInstructorDeck deck;
        }

        [System.Serializable]
        public class InstructorDecksResponse
        {
            public InstructorDecksResponse()
            {

            }

            public bool success;
            public string error;
            public InteractiveInstructorDeck[] decks;
        }

        public static void CreateInstructorDeck(Action<InstructorDeckResponse> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>() {
                {"team", User.current.selectedMembership.team._id },
                {"deck", new InteractiveInstructorDeck(){name = "NEW INSTRUCTOR DECK"} }
            };
            ServerRequest.CallAPI("/interactive/instructor/decks/create", HTTPMethod.POST, body, (response) => ServerRequest.ResponseHandler(response, null, onComplete), true);
        }
        public static void ListInstructorDecks(Action<InstructorDecksResponse> onComplete)
        {
            ServerRequest.CallAPI("/interactive/instructor/decks/list", HTTPMethod.GET, null, (response) => ServerRequest.ResponseHandler(response, null, onComplete), true);
        }

        public static void UpdateInstructorDeck(InteractiveInstructorDeck deck, Action<InstructorDeckResponse> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>() {
                {"team", User.current.selectedMembership.team._id },
                {"deck", deck}
            };
            ServerRequest.CallAPI("/interactive/instructor/decks/update", HTTPMethod.POST, body, (response) => ServerRequest.ResponseHandler(response, null, onComplete), true);
        }

        public static void DeleteInstructorDeck(string id, Action<InstructorDeckResponse> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>() {
                {"team", User.current.selectedMembership.team._id }
            };
            ServerRequest.CallAPI("/interactive/instructor/decks/delete/" + id, HTTPMethod.POST, body, (response) => ServerRequest.ResponseHandler(response, null, onComplete), true);
        }

        public static void GenerateMinigameLink(InteractiveInstructorDeck deck, Action<MinigameDataResponse> onComplete)
        {
            ServerRequest.CallAPI("/interactive/instructor/generatelink/" + deck._id, HTTPMethod.GET, null, (response) => ServerRequest.ResponseHandler(response, null, onComplete), false);
        }

        public static void GetMinigameData(string gameCode, Action<MinigameDataResponse> onComplete)
        {
            ServerRequest.CallAPI("/interactive/instructor/minigame/" + gameCode, HTTPMethod.GET, null, (response) => ServerRequest.ResponseHandler(response, null, onComplete), false);
        }
    }
}
