using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exteriorHit : MonoBehaviour
{
    [SerializeField] public int speed = 4;
    bool chase = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(chase)
        {
            StartCoroutine(getTo());
        }
    }

    IEnumerator getTo()
    {
        transform.parent.transform.position += transform.forward * speed * Time.deltaTime;
        yield return new WaitForEndOfFrame();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<SimpleSampleCharacterControl>())
        {
            Debug.Log("s");
            transform.parent.transform.LookAt(collision.gameObject.transform.position);
            chase = true;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.GetComponent<SimpleSampleCharacterControl>())
        {
            transform.parent.transform.LookAt(collision.gameObject.transform.position);
            chase = true;
        }
    }
}
