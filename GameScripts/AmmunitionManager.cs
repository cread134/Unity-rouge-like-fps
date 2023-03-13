using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
; using TMPro;

public class AmmunitionManager : MonoBehaviour
{
    public List<int> quantity = new List<int>();

    public List<string> ammoNames = new List<string>();
    public Sprite[] iconSprites;

    public Transform notificationPos;
    ObjectPooler objPooler;

    private void Start()
    {
        objPooler = ObjectPooler.Instance;
    }

    public void PickupAmmo(int ammount, int index)
    {
        quantity[index] += ammount;
        GameObject notif = objPooler.SpawnFromPool("AmmoNotifier", notificationPos.position, Quaternion.identity, notificationPos);
        notif.GetComponent<TextMeshProUGUI>().text = "+" + ammount.ToString();
        notif.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = iconSprites[index];
        notif.GetComponent<Animator>().SetTrigger("Spawn");
    }
}
