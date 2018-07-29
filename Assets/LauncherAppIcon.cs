using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface AppCompatible {
    string title {
        get;
        set;
    }
    string _name {
        get;
        set;
    }
}

public class LauncherAppIcon: MonoBehaviour, System.IComparable, IBeginDragHandler, IDragHandler, IEndDragHandler, AppCompatible {
    public string _name {
        get {
            return __name;
        }

        set {
            __name = value;
        }
    }

    public string title {
        get {
            return _title;
        }

        set {
            _title = value;
        }
    }

    private string _title;
    private string __name;
    public string hoverName; // soon
    public Texture2D icon;
    public string path;
    public string arguments;
    public Transform AddIconDialog;
    public GameObject Canvas;
    public Launcher LauncherObject;
    public Texture2D MissingIcon;

    public class SerializedApplication {
        public string title;
        public string name;
        public string hoverName; // soon
        public byte[] icon;
        public bool hasIcon = false;
        public string path;
        public string arguments;
    }

    public SerializedApplication SerializeApplication() {
        return new SerializedApplication {
            title = title,
            name = _name,
            hoverName = hoverName,
            path = path,
            arguments = arguments,
            icon = (icon == MissingIcon ? null : icon.EncodeToPNG()),
            hasIcon = icon != null && icon != MissingIcon
        };
    }

    public void Execute() {
        if (!dragging) {
            if (_name == "add") {
                int width = (int)Math.Floor(Canvas.GetComponent<RectTransform>().rect.width);
                int height = (int)Math.Floor(Canvas.GetComponent<RectTransform>().rect.height);
                Transform dialog = Instantiate(AddIconDialog, new Vector3(width / 2, height / 2, 0), Quaternion.identity, Canvas.transform);
                NewIcon scr = dialog.GetComponent<NewIcon>();
                if (scr != null) {
                    scr.LauncherObject = LauncherObject;
                    scr.Self = dialog.gameObject;
                }
                return;
            }
            //Debug.Log("Starting " + path);
            if (arguments == null || arguments == "")
                System.Diagnostics.Process.Start(path);
            else
                System.Diagnostics.Process.Start(path, arguments);
            LauncherObject.OpenLauncher(); // this actually closes the launcher
        } //else Debug.Log("Not starting");
        dragging = false;
    }

    public int CompareTo(object obj) {
        AppCompatible objapp = obj as AppCompatible;
        if (objapp == null) {
            Debug.LogError(obj);
            throw new ArgumentException("Tried to compare Application with non-AppCompatible");
        }
        if (_name == "add")
            return "ZZZZZZZZZZ".CompareTo(objapp.title);
        if (objapp._name == "add")
            return title.CompareTo("ZZZZZZZZZZ");
        return title.CompareTo(objapp.title);
    }

    public string Serialize() {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SerializedApplication));
        using (StringWriter textWriter = new StringWriter()) {
            xmlSerializer.Serialize(textWriter, SerializeApplication());
            return textWriter.ToString();
        }
    }

    public LauncherAppIcon(string xmlstr) {
        XmlSerializer xmlDeserializer = new XmlSerializer(typeof(SerializedApplication));
        using (TextReader reader = new StringReader(xmlstr)) {
            SerializedApplication retval;
            try {
                retval = xmlDeserializer.Deserialize(reader) as SerializedApplication;
            } catch (Exception e) {
                Debug.LogError("Could not load application from XML: " + e.Message + ".\nXML: " + xmlstr);
                return;
            }
            //Debug.Log("Created app " + retval.title);
            title = retval.title;
            _name = retval.name;
            hoverName = retval.hoverName;
            if (retval.hasIcon) {
                icon = new Texture2D(48, 48);
                icon.LoadImage(retval.icon);
                icon = Resize(icon, 48, 48);
            }
            path = retval.path;
            arguments = retval.arguments;
        }
    }

    public LauncherAppIcon() {
        //Debug.Log("Created blank app");
        title = "";
        _name = "";
        hoverName = "";
        icon = null;
        path = "";
        arguments = "";
    }

    public override string ToString() {
        return title;
    }

    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight) {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        return nTex;

    }

    public void UpdateIcon() {
        GetComponentInChildren<Image>().sprite = Sprite.Create((icon == null ? MissingIcon : icon), new Rect(0, 0, 48, 48), Vector2.zero);
        GetComponentInChildren<Image>().preserveAspect = true;
        //GetComponentInChildren<Image>().SetNativeSize();
        GetComponentInChildren<Text>().text = title;
    }

    // dragging code
    public static LauncherAppIcon itemBeingDragged;
    Vector3 startPosition;
    static bool dragging = false;

    public void OnBeginDrag(PointerEventData eventData) {
        if (_name != "add") {
            itemBeingDragged = this;
            startPosition = transform.position;
            dragging = true;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            LauncherObject.iconIsTrashCan = true;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (_name != "add") {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (_name != "add") {
            LauncherObject.iconIsTrashCan = false;
            dragging = false;
            if (Input.mousePosition.x < 70 && Input.mousePosition.y < 70) {
                LauncherObject.DeregisterApp(this);
                Destroy(gameObject);
                return;
            }
            //itemBeingDragged = null;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            /*if (transform.parent == startParent)*/
            transform.position = startPosition;
        }
    }
}
