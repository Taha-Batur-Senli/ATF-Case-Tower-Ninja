using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject healthFull;
    [SerializeField] public gameManager manager;
    public int howManyHitsCanITake = 4;
    float reduction;

    // Start is called before the first frame update
    void Start()
    {
        reduction = healthBar.GetComponent<RectTransform>().sizeDelta.x / howManyHitsCanITake;
    }

    // Update is called once per frame
    void Update()
    {

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
