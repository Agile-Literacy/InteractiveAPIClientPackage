using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AgileLiteracy.API
{
    partial class APIManager : MonoBehaviour
    {
        [System.Serializable]
        public class MetricsResponse
        {
            public bool success;
            public object error;
            public MetricEvent[] metrics;
        }

    
        public static void SubmitMetrics(MetricEvent[] metrics, System.Action<MetricsResponse> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"metrics", metrics }
            };


            ServerRequest.CallAPI("/metrics", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, false);
        }

        public static void GetMetrics(string teamId, string startDate, string endDate, System.Action<MetricsResponse> onComplete)
        {
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                {"teamId", teamId },
                   { "startDate", startDate },
                   { "endDate", endDate  }
                
            };


            ServerRequest.CallAPI("/metrics/list", HTTPMethod.POST, body, (r) => { ServerRequest.ResponseHandler(r, null, onComplete); }, false);
        }
    }
}