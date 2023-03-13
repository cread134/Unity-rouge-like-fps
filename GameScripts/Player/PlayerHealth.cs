using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public int maxHealth;

    [SerializeField]
    public int curhealth;


    public GameObject deathScreenUi;
    GameObject deathScreenInstance;
    GameObject UiHolder;
    MouseLook m_look;

    PlayerMovement playerMovement;
    WeaponWhellManager w_manager;
    public GameObject head;

    [Space]
    public GameObject createIconParent;
    public GameObject iconPrefab;
    public Queue<GameObject> healthIcons = new Queue<GameObject>();
    public HorizontalLayoutGroup h_layout;

    [Space]

    public float invisibleFrames;
    // Start is called before the first frame update
    void Start()
    {
        curhealth = maxHealth;

        UiHolder = GameObject.FindGameObjectWithTag("UIHolder");

        playerMovement = this.GetComponent<PlayerMovement>();
        deathScreenInstance = Instantiate(deathScreenUi, this.transform);
        deathScreenInstance.SetActive(false);

        
        m_look = head.GetComponent<MouseLook>();
        w_manager = this.GetComponent<WeaponWhellManager>();

        CreateHealthIcons(maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Heal(1);
        }
    }

    public void CreateHealthIcons(int amount)
    {

        foreach (GameObject g in healthIcons)
        {          
            Destroy(g);
        }

        healthIcons.Clear();
        List<GameObject> goList = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            GameObject iconInstance = Instantiate(iconPrefab, createIconParent.transform);
            goList.Insert(0, iconInstance);
        }

        foreach (GameObject g in goList)
        {
            healthIcons.Enqueue(g);
        }

        
    }

    public void TakeDamage(int amount)
    {


        Debug.Log("aadaaaaa" + amount);

        curhealth -= amount;
       
        if (curhealth <= 0)
        {
            Die();
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject toDeactivate = healthIcons.Peek();
            toDeactivate.transform.GetChild(0).gameObject.SetActive(false);
            healthIcons.Dequeue();
            healthIcons.Enqueue(toDeactivate);
        }

     
    }

    public void Heal(int amount)
    {
        curhealth += amount;
        curhealth = Mathf.Clamp(curhealth, 0, maxHealth);
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        CreateHealthIcons(maxHealth);
        //  h_layout.enabled = false;
        for (int i = 0; i < maxHealth - curhealth; i++)
        {
            GameObject toDeactivate = healthIcons.Peek();
            toDeactivate.transform.GetChild(0).gameObject.SetActive(false);
            healthIcons.Dequeue();
            healthIcons.Enqueue(toDeactivate);
        }
    }

    public void Die()
    {
        w_manager.enabled = false;
        UiHolder.SetActive(false);
        playerMovement.canMove = false;
        deathScreenInstance.SetActive(true);
        m_look.lockCursor = false;
        Cursor.visible = true;
        m_look.canLook = false;
        Debug.Log("dead!");
    }
}
