// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ClickScript : MonoBehaviour
// {
//     Vector3 mousePos;
//     RaycastHit2D raycastHit2D;
//     Transform clickObject;
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         mousePos = Input.mousePosition;
//         Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

//         if (Input.GetMouseButtonDown(0))
//         {
//             raycastHit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
//             clickObject = raycastHit2D ? raycastHit2D.collider.transform : null;
//             if (raycastHit2D)
//             {
//                 clickObject = raycastHit2D.transform;
//                 if (clickObject.CompareTag("ShopMan"))
//                 {
//                     ShopMan shopMan = clickObject.GetComponent<ShopMan>();
//                     if (shopMan != null)
//                     {
//                         shopMan.ToggleShopUI();
//                     }
//                 }
//             }
//         }
//     }
// }
