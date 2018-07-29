using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperChanger : MonoBehaviour {

    public InputField pathBox;

    public Texture2D LoadPNG(string filePath) {
        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(filePath)) {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            tex = LauncherAppIcon.Resize(tex, Screen.width, Screen.height);
        }
        return tex;
    }

    public void Change() {
        Texture2D newTexture = LoadPNG(pathBox.text);
        if (newTexture != null) {
            WallpaperLoader.texture = newTexture;
            WallpaperLoader.textureIsNew = true;
            Quit();
        }
    }

    public void Quit() {
        Destroy(gameObject);
    }

}
