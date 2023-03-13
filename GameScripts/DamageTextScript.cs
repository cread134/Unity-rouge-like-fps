using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextScript : MonoBehaviour, IPooledObject
{
    public float pos_range;
    public float lifeTime;
    GameObject player;

    // Start is called before the first frame update

    void Start()
    {
        player = GameObject.FindWithTag("Player");

    }

    public void OnObjectSpawn()
    {
        StartCoroutine(LifeTime());


        float x_pos = Random.Range(-pos_range, pos_range);
        float y_pos = Random.Range(-pos_range, pos_range);
        float z_pos = Random.Range(-pos_range, pos_range);

        RectTransform thisTransform = this.GetComponent<RectTransform>();

        thisTransform.position = new Vector3(thisTransform.position.x + x_pos, thisTransform.position.y + y_pos, thisTransform.position.z + z_pos);
    }


    // Update is called once per frame
    void Update()
    {
        var newRotation = Quaternion.LookRotation(transform.position - player.transform.position, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.z = 0.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
    }


    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);
    }
}
