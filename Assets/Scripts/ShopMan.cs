using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMan : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private Button shopButton;

    void Awake()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }


        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OpenShopUI);
        }
        else
        {
            Debug.LogWarning("ShopMan: Shop button is not assigned!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShopUI();
        }
    }

    public void ToggleShopUI()
    {
        if (shopUI != null)
        {
            bool isActive = shopUI.activeSelf;
            shopUI.SetActive(!isActive);
            Debug.Log($"ShopMan: Shop UI toggled to {!isActive}");
        }
        else
        {
            Debug.LogWarning("ShopMan: Shop UI is not assigned!");
        }
    }

    public void OpenShopUI()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(true);
            Debug.Log("ShopMan: Shop UI opened");
        }
    }

    public void CloseShopUI()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
            Debug.Log("ShopMan: Shop UI closed");
        }
    }
}
