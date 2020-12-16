/*
    Author: Keith Brewer (kbgamedev.com)
    For: Agile Literacy, LLC
    
    This script is intended for use only by the party that it was written for. 
    If you wish to use this script you must request written permission from 
    the author.

    This script will send JSON metric data to any RESTful API endpoint that is setup to accept it. In this case, a custom NodeJS/ExpressJS server router is used.
    The server is responsible for receiving the message and doing something with that data. Ideally it would store the metric events individually into a Mongo database
    to be used/parsed for showing graphs and charts later on.
 */


//Uncomment to get helpful debugging messages in the console for the metrics server calls.
#define GELLING_METRICS_LOGGING_ENABLED

using AgileLiteracy.API;
using Newtonsoft.Json;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace KBGameDev.Metrics
{
    [System.Serializable]
    public class MetricData
    {
        public Metric[] events;

        public MetricData(List<Metric> events)
        {
            this.events = events.ToArray();
        }
    }

    [System.Serializable]
    public class Metric
    {
        

        public string userId;
        public string licenseToken;
        public string team;
        public long localtimeUtcInMs;
        
        public string gameId = "interactive";
        public string eventType;
        public object data;

        public Metric(string eventType, object data = null)
        {
            this.userId = User.current != null ? User.current._id : null;
            this.team = User.current?.selectedMembership?.team._id;
            //this.licenseToken = 
            //    GameSparksManager.localUser != null && 
            //    GameSparksManager.localUser.licenses != null && 
            //    GameSparksManager.localUser.licenses.Length > 0 ? GameSparksManager.localUser.licenses[0]?.GetString("token") : null;
            this.localtimeUtcInMs = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.eventType = eventType;
            this.data = data;
        }
    }

    public class GellingMetrics : MonoBehaviour
    {

        private static GellingMetrics _instance;

        private static string metricsKey = "Gelling Metrics JSON";

        public static GellingMetrics Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("Gelling Metrics");
                    _instance = obj.AddComponent<GellingMetrics>();
                }

                return _instance;
            }
        }
        
        void SendDataIfNeeded()
        {
            string jsonData = PlayerPrefs.GetString(metricsKey, "");
            if (!string.IsNullOrEmpty(jsonData))
            {

                Debug.Log("GELLINGMETRICS.SEND Retrying..");

                StartCoroutine(Upload(true));
            }
            else
            {

                Debug.Log("GELLINGMETRICS.SEND Retry Cancelled: No data");

            }
        }

        bool sending = false;

        List<Metric> leftovers = new List<Metric>();

        public void Send(Metric eventObj)
        {
            if (sending)
            {
                leftovers.Add(eventObj);
                return;
            }
            sending = true;
            List<Metric> events = new List<Metric>();
            events.Add(eventObj);
            Debug.Log(events);
            Debug.Log(events.Count);
            Send(events);
        }

        float timeSinceLastAttempt = 0f;
        float lastAttemptTime = 0f;
        public void Send(List<Metric> events)
        {
            timeSinceLastAttempt = Time.time - lastAttemptTime;
            Debug.Log("<color=Red>Last attempt to send metrics was: </color> " + timeSinceLastAttempt + " seconds ago..");
            lastAttemptTime = Time.time;
            //Check for already stored events
            string jsonData = PlayerPrefs.GetString(metricsKey, "");
            if (!string.IsNullOrEmpty(jsonData))
            {
                //Add our events to the end of the already stored events
                MetricData stored = JsonConvert.DeserializeObject<MetricData>(jsonData);
                List<Metric> eventList = new List<Metric>(stored.events);

                if (leftovers != null && leftovers.Count > 0)
                    eventList.AddRange(leftovers);

                eventList.AddRange(events);
                stored.events = eventList.ToArray();


                Debug.LogWarning("<color=Magenta>New events added to existing stored events.</color>");

                string json = JsonConvert.SerializeObject(stored);
                PlayerPrefs.SetString(metricsKey, json);
            }
            else
            {
                List<Metric> evnts = new List<Metric>();

                if (leftovers != null && leftovers.Count > 0)
                    evnts.AddRange(leftovers);

                evnts.AddRange(events);

                MetricData data = new MetricData(evnts);

                string json = JsonConvert.SerializeObject(data);

                PlayerPrefs.SetString(metricsKey, json);


                Debug.LogWarning("New metric data stored.");
                Debug.LogWarning(json);


            }

            //Just incase there is a scheduled retry, lets cancel it since we are technically retrying right now.
            CancelInvoke("SendDataIfNeeded");

            //string json = JsonUtility.ToJson(data);

            try {
                StartCoroutine(Upload());
            }
            catch{
                sending = false;
            }
        }


        IEnumerator Upload(bool isRetry = false)
        {
            yield break;
            Debug.Log("Sending metrics from " + metricsKey);

            if (Time.unscaledTime < 5f)
            {
                yield return new WaitForSeconds(5f);
            }
            string jsonData = PlayerPrefs.GetString(metricsKey, "");

            Debug.Log("METRICS: Uploading metric data: " + jsonData);


            //to send JSON data we need to use PUT method
            //throw new System.NotImplementedException();
            string url = "";// GellingAPISettings.Instance.developmentMode ? "http://127.0.0.1:3001/api/metrics" : "https://dashboard.agileliteracy.com/api/metrics";
            UnityWebRequest www = UnityWebRequest.Put(url, jsonData);
            www.SetRequestHeader("Content-Type", "application/json");

            DownloadHandler b = new DownloadHandlerBuffer();
            www.downloadHandler = b;


            //Send to server
            //yield return www.SendWebRequest();
            www.chunkedTransfer = false;
            //Try to do this without blocking main thread
            UnityWebRequestAsyncOperation op = www.SendWebRequest();
            yield return new WaitUntil(() => op.isDone);


            if (www.isNetworkError || www.isHttpError)
            {

                Debug.LogError("GELLINGMETRICS.SEND FAILED: " + www.error);

                //Wait 30 seconds and try to send again.
                Debug.Log("Waiting 30 seconds and will try to send again");

                Invoke("SendDataIfNeeded", 30f);
            }
            else
            {

                Debug.LogWarning("GELLINGMETRICS.SEND SUCCESS");
                Debug.Log("Metric Response: " + b.text);


                //Clear metric storage if there were no server errors.
                PlayerPrefs.DeleteKey(metricsKey);
            }

            sending = false;
        }
    }
}