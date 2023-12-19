using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class die : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<playerScript>())
        {
            collision.gameObject.GetComponent<playerScript>().decreaseEnemyCount();
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
