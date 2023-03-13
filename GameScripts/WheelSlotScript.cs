using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WheelSlotScript : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private GameObject player;
    public GameObject iconRenderObj;
    public GunScript slotGun;

    [HideInInspector]
    public int thisGunAmmo;

    public int gunID;

    [HideInInspector]
    public bool mouseOver = false;

    public Vector2 scale;
    public Vector2 targetScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        this.GetComponent<RectTransform>().localScale = scale;

        UpdateGunData();

    }


    public void Update()
    {
        if(slotGun != null)
        {
            thisGunAmmo = slotGun.curAmmo - 1;
        }
        else
        {
            thisGunAmmo = 0;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        this.GetComponent<RectTransform>().localScale = targetScale;
        mouseOver = true;
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (slotGun != null)
        {
            player.GetComponent<WeaponWhellManager>().NewWeaponSelected(this.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;

        this.GetComponent<RectTransform>().localScale = scale;
        this.GetComponent<Image>().SetNativeSize();
    }

    public void UpdateGunData()
    {
        if (slotGun != null)
        {
            iconRenderObj.SetActive(true);
            slotGun.curAmmo = slotGun.maxAmmo;
            iconRenderObj.GetComponent<Image>().sprite = slotGun.w_Icon;
            gunID = slotGun.weaponIndex;
            
        }
        else
        {
            iconRenderObj.SetActive(false);
            gunID = 0;
        }
    }

    

}
