using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDialog : MonoBehaviour {

    public Transform menu;
    public GameObject main;
    private GameObject parent;
    private RectTransform bounds;
    public Transform menub;
    public Transform ChangeWallpaperDialog;
    public GameObject Dock;
    public bool showing = false;
    public static bool doneFading = false;

    IEnumerator FadeCanvas() {
        // loop over 1 second
        parent.GetComponent<CanvasGroup>().alpha = 0;
        for (float i = 0; i <= 1; i += Time.deltaTime) {
            // set color with i as alpha
            parent.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
    }

    // Use this for initialization
    private void Start() {
        parent = GameObject.FindGameObjectWithTag("Canvas");
        if (!doneFading) StartCoroutine(FadeCanvas());
        doneFading = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
        //Debug.Log("hi");
    }

    public void Show() {
        bounds = parent.GetComponent<RectTransform>();
        if (bounds == null)
            throw new System.Exception();
        //throw new System.Exception("width: " + bounds.rect.xMin + ", height: " + bounds.rect.yMin);
        //Debug.Log("clicked");
        if (!main.GetComponent<MenuDialog>().showing) {
            //Debug.Log("showing");
            menub = Instantiate(menu, new Vector3((170 / 2), (bounds.rect.height * bounds.localScale.y) - 90, -20), Quaternion.identity, parent.transform);
            menub.GetComponent<MenuDialog>().main = main;
            menub.GetComponent<MenuDialog>().Dock = Dock;
            main.GetComponent<MenuDialog>().showing = true;
        } else {
            //Debug.Log("going away");
            Destroy(main.GetComponent<MenuDialog>().menub.gameObject);
            main.GetComponent<MenuDialog>().showing = false;
        }
    }

    public void ChangeWallpaper() {
        if (main.GetComponent<MenuDialog>().showing) {
            Destroy(main.GetComponent<MenuDialog>().menub.gameObject);
            main.GetComponent<MenuDialog>().showing = false;
        }
        Instantiate(ChangeWallpaperDialog, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity, parent.transform);
    }

    public void Shutdown() {
        Dock.GetComponent<Launcher>().Save();
        if (main.GetComponent<MenuDialog>().showing) {
            Destroy(main.GetComponent<MenuDialog>().menub.gameObject);
            main.GetComponent<MenuDialog>().showing = false;
        }
        //System.Diagnostics.Process.Start("/bin/systemctl", "poweroff"); // needs password
    }

    public void Restart() {
        Dock.GetComponent<Launcher>().Save();
        if (main.GetComponent<MenuDialog>().showing) {
            Destroy(main.GetComponent<MenuDialog>().menub.gameObject);
            main.GetComponent<MenuDialog>().showing = false;
        }
        //System.Diagnostics.Process.Start("/bin/systemctl", "reboot"); // needs password
    }

    public void LogOut() {
        Dock.GetComponent<Launcher>().Save();
        UnityEngine.Application.Quit();
    }
	
	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButton(0) && main.GetComponent<MenuDialog>().showing && (Input.mousePosition.x > 120 || Screen.height - Input.mousePosition.y > 120 || (Screen.height - Input.mousePosition.y < 30 && Input.mousePosition.x > 52))) {
            //Debug.Log(Input.mousePosition.ToString());
            //Debug.Log("Closing");
            Destroy(main.GetComponent<MenuDialog>().menub.gameObject);
            main.GetComponent<MenuDialog>().showing = !main.GetComponent<MenuDialog>().showing;
            //clicked = true;
        }
	}
}
