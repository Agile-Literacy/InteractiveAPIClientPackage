using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AgileLiteracy.API;
using AgileLiteracy.API;

[System.Serializable]
public enum BadgeIconType
{
    Standard,
    Web
}

[System.Serializable]
public class InteractiveBadge
{
    public static InteractiveBadge current;

    public string name;
    public string _id;
    public string iconType;
    public string icon;
    public string description;
    public Team team;





    //Helpers 

    private Texture2D _texture;


    public bool isEarnedByUser(User user)
    {
        return user != null && user.badges != null && user.badges.Count > 0 && user.badges.Contains(this._id);
    }

    public void LoadOrDownloadTexture(System.Action<Texture2D> OnTextureLoaded)
    {
        if (this._texture != null && this._texture.name == this.icon)
        {
            //Texture has already been downloaded or loaded from resources and assigned, and still matches the name of the image in the icon field
            OnTextureLoaded.Invoke(this._texture);
            return;
        }

        //Texture either has not been loaded/downloaded or the icon field no longer matches the name of the _texture
        if (this.GetIconType() == BadgeIconType.Standard)
        {
            _texture = Resources.Load<Texture2D>("badges/" + this.icon);
            this._texture.name = this.icon;

            //Just announce in the console if the built in badge we're looking for failed to get loaded
            if (_texture == null)
            {
                Debug.LogError("Failed to load built-in badge " + this._id + " from resources: " + this.icon);
            }

            OnTextureLoaded?.Invoke(_texture);
        }
        else if (this.GetIconType() == BadgeIconType.Web)
        {
            //this is a custom icon, "icon" field should be a file url
            string url = this.icon;
            APIManager.DownloadTexture2D(url, (texture) =>
            {
                if (texture != null)
                {
                    this._texture = texture;
                    this._texture.name = this.icon;
                    OnTextureLoaded?.Invoke(_texture);
                }
                else
                {
                    Debug.LogError("Failed to download texture from: " + url);
                    this._texture = null;
                    OnTextureLoaded?.Invoke(null);
                }
            });
        }
        else
        {
            OnTextureLoaded?.Invoke(null);
        }
    }

    public BadgeIconType GetIconType()
    {
        return this.iconType.ToLower() == "standard" ? BadgeIconType.Standard : BadgeIconType.Web;
    }

    public void GetSprite(System.Action<Sprite> OnSpriteReady)
    {
        this.LoadOrDownloadTexture((texture) =>
        {

            if (texture == null)
            {
                OnSpriteReady.Invoke(null);
            }
            else
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                OnSpriteReady.Invoke(sprite);
            }
        });

    }



}