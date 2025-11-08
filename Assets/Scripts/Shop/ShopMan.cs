using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopMan : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private Animator animator;
    public bool isShopOpen;
    [SerializeField] private GameObject nextUI;

    [SerializeField] private StageData stageData;

    void Start()
    {
        isShopOpen = false;
        if (shopUI != null)
        {
            shopUI.SetActive(false);
            nextUI.SetActive(false);
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isShopOpen == false)
        {
            if (animator != null)
            {
                animator.SetBool("isHover", true);
                Debug.Log("ShopMan: isHover = true");
            }
        }

        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isShopOpen == false)
        {
            if (animator != null)
            {
                animator.SetBool("isHover", false);
                Debug.Log("ShopMan: isHover = false");
            }
        } 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isShopOpen == false)
        { 
            if (animator != null)
            {
                animator.SetBool("isHover", false);
                animator.SetBool("isPressed", true);
                Debug.Log("ShopMan: isHover = false, isPressed = true");
            }

            if (shopUI != null)
            {
                shopUI.SetActive(true);
                Debug.Log("ShopMan: Shop UI opened");
            }
        }
    }

    public void CloseShopUI()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
            nextUI.SetActive(true);
            Debug.Log("ShopMan: Shop UI deactivated");
        }

        if (animator != null)
        {
            animator.SetBool("isPressed", false);
        }
        isShopOpen = false;
        Debug.Log("ShopMan: Shop UI closed, isPressed = false");
    }
}