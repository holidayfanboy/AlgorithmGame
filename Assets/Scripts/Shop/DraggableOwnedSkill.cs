// using UnityEngine;
// using UnityEngine.EventSystems;

// [RequireComponent(typeof(RectTransform))]
// public class DraggableOwnedSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
// {
//     private SkillLayoutShop layout;
//     private Canvas canvas;
//     private RectTransform rect;
//     private Vector2 startLocalPos;

//     public void Initialize(SkillLayoutShop l)
//     {
//         layout = l;
//         rect = GetComponent<RectTransform>();
//         canvas = GetComponentInParent<Canvas>();
//     }

//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         if (layout == null) return;
//         startLocalPos = rect.localPosition;
//         layout.BeginDrag(this);
//     }

//     public void OnDrag(PointerEventData eventData)
//     {
//         if (canvas == null) return;

//         Vector2 localPoint;
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             (RectTransform)rect.parent,
//             eventData.position,
//             eventData.pressEventCamera,
//             out localPoint);

//         rect.localPosition = new Vector3(localPoint.x, startLocalPos.y, 0);
//         layout.DragUpdate(this);
//     }

//     public void OnEndDrag(PointerEventData eventData)
//     {
//         layout?.EndDrag(this);
//         // Snap back vertically
//         rect.localPosition = new Vector3(rect.localPosition.x, startLocalPos.y, 0);
//     }

//     public int ParentIndex()
//     {
//         return transform.GetSiblingIndex();
//     }
// }