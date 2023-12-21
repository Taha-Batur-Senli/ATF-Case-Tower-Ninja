using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerKeyRotate : MonoBehaviour
{
    [SerializeField] GameObject body;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(rotate());
    }

    IEnumerator rotate()
    {
        body.transform.rotation *= Quaternion.Euler(0, 0, 1);
        yield return new WaitForSeconds(0.5f);
    }
}
