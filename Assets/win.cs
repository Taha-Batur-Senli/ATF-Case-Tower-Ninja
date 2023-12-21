using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class win : MonoBehaviour
{
    [SerializeField] Vector3 middle = new Vector3 (6f, -0.5000001f, 90f);
    [SerializeField] GameObject keyLock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<SimpleSampleCharacterControl>() && collision.gameObject.GetComponent<playerScript>().hasKey)
        {
            Destroy(keyLock);
            collision.gameObject.GetComponent<playerScript>().hideKey();
            collision.gameObject.GetComponent<playerScript>().manager.pos = Vector3.up;
            collision.gameObject.GetComponent<playerScript>().manager.winvec = true;
        }
    }
}
