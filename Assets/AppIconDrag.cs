using UnityEngine;
using UnityEngine.EventSystems;

public class AppIconDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemBeingDragged;
    public LauncherAppIcon thisIcon;
    public bool isLauncher = false;
    Vector3 startPosition;
    Transform startParent;

    public void OnBeginDrag(PointerEventData eventData) {
        if (!isLauncher) {
            LauncherAppIcon.itemBeingDragged = thisIcon;
            itemBeingDragged = gameObject;
            startPosition = transform.position;
            startParent = transform.parent;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isLauncher) transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isLauncher) {
            LauncherAppIcon.itemBeingDragged = null;
            itemBeingDragged = null;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (Input.mousePosition.y > 70) {
                Destroy(gameObject);
                return;
            }
            if (transform.parent == startParent)
                transform.position = startPosition;
        }
    }
}
