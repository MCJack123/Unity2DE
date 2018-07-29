using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NewIcon : MonoBehaviour {

    public Launcher LauncherObject;
    public InputField title;
    public InputField _name;
    public InputField iconPath;
    public InputField path;
    public InputField arguments;
    public GameObject Self;
    public Texture2D MissingIcon;

    public Texture2D LoadPNG(string filePath) {
        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(filePath)) {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            tex = LauncherAppIcon.Resize(tex, 48, 48);
        }
        return tex;
    }

    public void CreateApplication() {
        LauncherAppIcon app = new LauncherAppIcon {
            title = title.text,
            _name = _name.text,
            path = path.text,
            arguments = arguments.text,
            icon = LoadPNG(iconPath.text)
        };
        LauncherObject.RegisterApp(app);
        Quit();
    }

    public void Quit() {
        Destroy(Self);
    }

}
