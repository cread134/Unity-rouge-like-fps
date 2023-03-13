using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public int startMoney = 50;
    public int curMoney = 150;
    public GameObject moneyText;
    TextMeshProUGUI m_text_UGUI;
    public GameObject moneyNotPos;
    ObjectPooler objectPooler;

    private void Start()
    {
        m_text_UGUI = moneyText.GetComponent<TextMeshProUGUI>();
        m_text_UGUI.text = "$" + curMoney.ToString();
        objectPooler = ObjectPooler.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            AddMoney(Random.Range(20, 100));
        }
    }


    public void AddMoney(int amount)
    {
        curMoney += amount;
        m_text_UGUI.text = "$" + curMoney.ToString();
        GameObject obj = objectPooler.SpawnFromPool("MoneyNotification", moneyNotPos.transform.position, Quaternion.identity, moneyNotPos.transform);
        obj.GetComponent<TextMeshProUGUI>().text = "$" + amount.ToString();
    }
}
