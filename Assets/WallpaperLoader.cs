using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Wallpaper {
    public byte[] data;
}

public class WallpaperLoader : MonoBehaviour {

    public static Texture2D texture;
    public static bool textureIsNew;
    public Image wallpaper;
    
	// Use this for initialization
	void Start () {
        string textureString = PlayerPrefs.GetString("Wallpaper");
        if (textureString != "") {
            XmlSerializer xmlDeserializer = new XmlSerializer(typeof(Wallpaper));
            using (TextReader reader = new StringReader(textureString)) {
                Wallpaper retval;
                try {
                    retval = xmlDeserializer.Deserialize(reader) as Wallpaper;
                } catch (Exception e) {
                    Debug.LogError("Could not load wallpaper from XML: " + e.Message + ".\nXML: " + textureString);
                    return;
                }
                texture = new Texture2D(48, 48);
                texture.LoadImage(retval.data);
                texture = LauncherAppIcon.Resize(texture, Screen.width, Screen.height);
                textureIsNew = true;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (textureIsNew) {
            wallpaper.sprite = Sprite.Create(texture, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Wallpaper));
            using (StringWriter textWriter = new StringWriter()) {
                xmlSerializer.Serialize(textWriter, new Wallpaper { data = texture.EncodeToPNG() });
                PlayerPrefs.SetString("Wallpaper", textWriter.ToString());
                PlayerPrefs.Save();
            }
            textureIsNew = false;
        }
	}
}
