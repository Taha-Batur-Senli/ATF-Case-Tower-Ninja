using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public bool hasKey = false;
    [SerializeField] public GameObject key;
    [SerializeField] public GameObject healthBar;
    [SerializeField] public GameObject healthFull;
    [SerializeField] public gameManager manager;
    public int howManyHitsCanITake = 4;
    float reduction;

    // Start is called before the first frame update
    void Start()
    {
        key.SetActive(false);
        reduction = healthBar.GetComponent<RectTransform>().sizeDelta.x / howManyHitsCanITake;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showKey()
    {
        key.SetActive(true);
    }

    public void hideKey()
    { 
        key.SetActive(false); 
    }

    public void reduceHealth()
    {
        healthFull.GetComponent<RectTransform>().sizeDelta -= new Vector2(reduction, 0);
        if (healthFull.GetComponent<RectTransform>().sizeDelta.x == 0)
        {
            manager.endGame();
        }
    }

}
