using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeamFile
{
    public static TeamFile current;

    const string AMAZON_S3_BUCKET_URL = "https://gelling-authoring-content.s3.us-east-2.amazonaws.com";
    public const string AMAZON_S3_HOST = "gelling-authoring-content.s3.us-east-2.amazonaws.com";

    public string _id;

    public string filename;
    public string encoding;
    public string mimetype;
    public string team;

    //Helpers
    public string FileURL()
    {
        return AMAZON_S3_BUCKET_URL + "/" + team + "/" + filename;
    }

    public static string StringToFileURL(string team, string filename)
    {
        return AMAZON_S3_BUCKET_URL + "/" + team + "/" + filename;
    }

    public bool isPNG
    {
        get 
        {
            return mimetype == "image/png";
        }
    }

    public bool isJPEG
    {
        get
        {
            return mimetype == "image/jpg" || mimetype == "image/jpeg";
        }
    }

    public bool isImage
    {
        get
        {
            return this.isJPEG || this.isPNG;
        }
    }

    public bool isMPEG4
    {
        get
        {
            return mimetype == "video/mp4";
        }
    }
}
