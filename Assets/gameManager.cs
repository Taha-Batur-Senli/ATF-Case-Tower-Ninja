using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject plSpawn;
    [SerializeField] GameObject over;
    [SerializeField] GameObject joystick;
    [SerializeField] GameObject enemySpawn;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        over.SetActive(false);
        GameObject pl = Instantiate(player);
        pl.transform.SetParent(plSpawn.transform);
        pl.GetComponent<JoystickPlayerExample>().cam = cam;
        pl.GetComponent<JoystickPlayerExample>().variableJoystick = joystick.GetComponent<VariableJoystick>();
        player = pl;

        player.GetComponent<playerScript>().manager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void endGame()
    {
        player.SetActive(false);
        over.SetActive(true);
        joystick.SetActive(false);
        for(int i = 0; i < enemySpawn.transform.childCount; i++)
        {
            enemySpawn.transform.GetChild(i).GetComponent<enemyScript>().over = true;
        }
    }

    public void createEnemies()
    {
        
    }
}
