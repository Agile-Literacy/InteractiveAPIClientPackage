using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InteractiveTheme
{
    public string _id;
    public string name;
    public string bgURL;
    public string levelSelectStyle;
    public string lessonAreaStyle;

    public static InteractiveTheme current;
}