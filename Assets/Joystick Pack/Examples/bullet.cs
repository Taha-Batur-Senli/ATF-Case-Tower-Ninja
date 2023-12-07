using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bullet : MonoBehaviour
{
    bool dead = false;
    public float moveSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!dead)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void Shoot(GameObject obj, GameObject enemy)
    {
        Vector3 looker = obj.transform.position;
        looker.y = enemy.transform.position.y;

        gameObject.transform.LookAt(looker);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<playerScript>())
        {
            other.GetComponent<playerScript>().reduceHealth();
        }

        Destroy(gameObject);
    }
}
