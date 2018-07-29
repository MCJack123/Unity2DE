using System;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour {

    public GameObject LauncherObject;
    public GameObject BaseLauncherObject;
    public GameObject Canvas;
    public GameObject BaseSlot;
    public GameObject Dock;
    public GameObject LauncherButton;
    public Transform LauncherAppIcon;
    public Transform AddIconDialog;
    public Texture2D AddIcon;
    public Sprite LauncherButtonDefaultIcon;
    public Sprite LauncherButtonTrashIcon;

    public bool iconIsTrashCan {
        get {
            return trash;
        } set {
            if (value) LauncherButton.GetComponent<UnityEngine.UI.Image>().sprite = LauncherButtonTrashIcon;
            else LauncherButton.GetComponent<UnityEngine.UI.Image>().sprite = LauncherButtonDefaultIcon;
            trash = value;
        }
    }

    private struct AppCount {
        public int rows;
        public int columns;
        public int pages;
        public float rowPadding;
        public float columnPadding;
    }

    private List<LauncherAppIcon> apps = new List<LauncherAppIcon>();
    private List<GameObject> appIcons = new List<GameObject>();
    private List<GameObject> slots = new List<GameObject>();
    private int currentPage = 0;
    private bool isOpen = false;
    private bool trash = false;

    public void RegisterApp(LauncherAppIcon app) {
        apps.Add(app);
        UpdateIcons();
    }

    public void DeregisterApp(LauncherAppIcon app) {
        //Debug.Log("Removing " + app._name);
        for (int i = 0; i < apps.Count; i++) if (apps[i]._name == app._name) { apps.RemoveAt(i); break; }
        UpdateIcons();
    }

    // Use this for initialization
    void Start() {
        string prefs = PlayerPrefs.GetString("LauncherApps");
        if (prefs != "") {
            string[] preflist = prefs.Split(new char[] {'|'});
            foreach (string pref in preflist) apps.Add(new LauncherAppIcon(pref));
        }
        apps.Add(new LauncherAppIcon {
            title = "Add Icon",
            _name = "add",
            icon = AddIcon,
        });
        if (slots.Count == 0) {
            slots.Add(BaseSlot);
            int slotsn = (int)Math.Floor((double)Screen.width / 70) - 2;
            string ent = PlayerPrefs.GetString("DockApps");
            string[] entries = ent.Split(',');
            for (int i = 0; i < slotsn; i++) {
                GameObject slot = Instantiate(BaseSlot, transform);
                slot.GetComponent<AppIcon>().id = i + 1;
                if (ent != "") {
                    foreach (string entry in entries) {
                        if (int.Parse(entry.Split('=')[0]) == i + 1) {
                            string appname = entry.Split('=')[1];
                            foreach (LauncherAppIcon app in apps) if (app._name == appname) {
                                //Debug.Log("adding");
                                global::LauncherAppIcon.itemBeingDragged = app;
                                slot.GetComponent<AppIcon>().OnDrop(null);
                            }
                        }
                    }
                }
                slots.Add(slot);
            }
            if (ent != "") foreach (string entry in entries) if (int.Parse(entry.Split('=')[0]) == 0) {
                string appname = entry.Split('=')[1];
                //Debug.Log("Adding to 0 " + appname);
                foreach (LauncherAppIcon app in apps) if (app._name == appname) {
                    //Debug.Log("adding");
                    global::LauncherAppIcon.itemBeingDragged = app;
                    slots[0].GetComponent<AppIcon>().OnDrop(null);
                }
            }
        }
    }

    public void Save() {
        string serialized = "";
        string dock = "";
        foreach (LauncherAppIcon app in apps) if (app._name != "add") serialized += (serialized == "" ? "" : "|") + app.Serialize();
        foreach (AppIcon slot in Dock.GetComponentsInChildren<AppIcon>()) if (slot.item) dock += (dock == "" ? "" : ",") + slot.id + "=" + slot.appname;
        PlayerPrefs.SetString("LauncherApps", serialized);
        PlayerPrefs.SetString("DockApps", dock);
    }

    public void OpenLauncher() {
        if (!isOpen) {
            isOpen = true;
            LauncherObject = Instantiate(BaseLauncherObject, new Vector3(Canvas.GetComponent<RectTransform>().rect.width / 2, Canvas.GetComponent<RectTransform>().rect.height / 2 + 20, 0), Quaternion.identity, Canvas.transform);
            UpdateIcons();
        } else {
            isOpen = false;
            Save();
            Destroy(LauncherObject);
        }
    }

    AppCount CalculateAppCount(int numberOfApps = 0) {
        AppCount count = new AppCount();
        int width = (int)Math.Floor(LauncherObject.GetComponent<RectTransform>().rect.width);
        int height = (int)Math.Floor(LauncherObject.GetComponent<RectTransform>().rect.height);
        for (count.columns = 1; (width - (100 * (count.columns + 1))) / (count.columns + 2) >= 40; count.columns++) ;
        for (count.rows = 1; (height - (100 * (count.rows + 1))) / (count.rows + 2) >= 40; count.rows++) ;
        if (numberOfApps > 0) count.pages = (int)Math.Ceiling((double)(numberOfApps / (count.rows * count.columns)));
        count.columnPadding = (width - (count.columns * 100)) / (count.columns + 1);
        count.rowPadding = (height - (count.rows * 100)) / (count.rows + 1);
        //Debug.Log("cols: " + count.columns);
        //Debug.Log("rows: " + count.rows);
        //Debug.Log("colPad: " + count.columnPadding);
        //Debug.Log("rowPad: " + count.rowPadding);
        return count;
    }

    void UpdateIcons() {
        foreach (GameObject app in appIcons) Destroy(app);
        appIcons = new List<GameObject>();
        apps.Sort();
        AppCount count = CalculateAppCount(apps.Count);
        float y = LauncherObject.GetComponent<RectTransform>().rect.height;
        //Debug.Log("base height = " + y);
        int i = 0;
        foreach (LauncherAppIcon app in apps) {
            if (Math.Floor((double)i / (count.columns * count.rows)) == currentPage) {
                GameObject appIcon = Instantiate(LauncherAppIcon, LauncherObject.transform).gameObject;
                appIcon.GetComponent<LauncherAppIcon>()._name = app._name;
                appIcon.GetComponent<LauncherAppIcon>().title = app.title;
                appIcon.GetComponent<LauncherAppIcon>().hoverName = app.hoverName;
                appIcon.GetComponent<LauncherAppIcon>().icon = app.icon;
                appIcon.GetComponent<LauncherAppIcon>().path = app.path;
                appIcon.GetComponent<LauncherAppIcon>().arguments = app.arguments;
                appIcon.GetComponent<LauncherAppIcon>().UpdateIcon();
                appIcon.GetComponent<LauncherAppIcon>().LauncherObject = this;
                appIcon.GetComponent<LauncherAppIcon>().AddIconDialog = AddIconDialog;
                appIcon.GetComponent<LauncherAppIcon>().Canvas = Canvas;
                //appIcon.GetComponent<AppIcon>().Dock = Dock;
                appIcons.Add(appIcon);
            }
            i++;
        }
    }
    
}
