using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AppIcon : MonoBehaviour, IDropHandler {

    public Transform BaseAppIcon;
    public GameObject Dock;
    public int id;
    public string appname;
    public Texture2D MissingIcon;
    private Texture2D icon;
    private string path;
    private string arguments;
    private GameObject obj;

    public GameObject item {
        get {
            if (transform.childCount > 0) {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData) {
        if (item) Destroy(item); // must destroy all evidence of the object's existence before making a new one
        if (AppIconDrag.itemBeingDragged != null) {
            AppIconDrag.itemBeingDragged.transform.SetParent(transform);
            return;
        }
        appname = LauncherAppIcon.itemBeingDragged._name;
        icon = LauncherAppIcon.itemBeingDragged.icon;
        path = LauncherAppIcon.itemBeingDragged.path;
        arguments = LauncherAppIcon.itemBeingDragged.arguments;
        obj = Instantiate(BaseAppIcon, transform).gameObject;
        obj.GetComponent<Button>().onClick.AddListener(delegate {
            //Debug.Log("Dock: Starting " + path);
            if (arguments == null || arguments == "")
                System.Diagnostics.Process.Start(path);
            else
                System.Diagnostics.Process.Start(path, arguments);
        });
        obj.GetComponent<Image>().sprite = Sprite.Create((icon == null ? MissingIcon : icon), new Rect(0, 0, 48, 48), Vector2.zero);
        obj.GetComponent<AppIconDrag>().thisIcon = LauncherAppIcon.itemBeingDragged;
        LauncherAppIcon.itemBeingDragged = null;
        //Debug.Log("Moved item " + appname);
    }

}
