using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class die : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<playerScript>())
        {
            collision.gameObject.GetComponent<playerScript>().manager.decreaseEnemyCount();
            
            if(!collision.gameObject.GetComponent<playerScript>().hasKey && gameObject.transform.parent.GetComponent<enemyScript>() && gameObject.transform.parent.GetComponent<enemyScript>().hasKey)
            {
                collision.gameObject.GetComponent<playerScript>().hasKey = true;
                collision.gameObject.GetComponent<playerScript>().showKey();
            }

            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
