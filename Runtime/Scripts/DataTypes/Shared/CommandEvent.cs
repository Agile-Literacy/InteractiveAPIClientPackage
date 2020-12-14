using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommandEvent
{
    public string eventName;
    public string[] args;


    public void Evaluate()
    {
        Debug.Log("Evaluating CommandEvent: " + eventName );
        switch (this.eventName.ToLower())
        {
            //case "awardbadge":
            //    for (int i = 0, imax = args.Length; i < imax; i++)
            //    {
            //        BadgeManager.Instance.AwardBadge(args[i]);
            //    }
            //    break;
            //case "awardtrophy":
            //    break;
            //case "showmessage":
            //    MessagePopup.Instance.ShowMessage(args[0]);
            //    break;
            //case "doquiz":
            default:
                break;
        }
    }

}