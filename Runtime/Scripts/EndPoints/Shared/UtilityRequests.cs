using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgileLiteracy.API
{
    public partial class APIManager
    {
        public static void DownloadTexture(string url, System.Action<Texture> OnTextureDownloaded)
        {
            ServerAPI.DownloadTexture2D(url, OnTextureDownloaded);
        }

        public static void DownloadTexture2D(string url, System.Action<Texture2D> OnTextureDownloaded)
        {
            ServerAPI.DownloadTexture2D(url, OnTextureDownloaded);
        }
    }
}