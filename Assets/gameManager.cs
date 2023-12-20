using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.IO;
public class PlayerData
{
    public int gold;
}

public class gameManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject plSpawn;
    [SerializeField] GameObject over;
    [SerializeField] GameObject win;
    [SerializeField] GameObject joystick;
    [SerializeField] GameObject gate;
    [SerializeField] public GameObject enemySpawn;
    [SerializeField] public TextMeshProUGUI enemyText;
    [SerializeField] public TextMeshProUGUI goldText;
    string saveFilePath;
    PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        playerData = new PlayerData();
        saveFilePath = Application.persistentDataPath + "/PlayerData.json";
        LoadGame();
        gate.SetActive(false);
        enemyText.text = "x" + enemySpawn.transform.childCount;
        win.SetActive(false);
        gameObject.SetActive(true);
        over.SetActive(false);
        GameObject pl = Instantiate(player);
        pl.transform.SetParent(plSpawn.transform);
        pl.GetComponent<JoystickPlayerExample>().cam = cam;
        pl.GetComponent<JoystickPlayerExample>().variableJoystick = joystick.GetComponent<VariableJoystick>();
        player = pl;

        player.GetComponent<playerScript>().manager = this;
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string loadPlayerData = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(loadPlayerData);
            goldText.text = playerData.gold.ToString();
        }
        else
        {
            playerData.gold = 0;
            string savePlayerData = JsonUtility.ToJson(playerData);
            File.WriteAllText(saveFilePath, savePlayerData);
        }
    }

    public void saveGame()
    {
        string savePlayerData = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, savePlayerData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void decreaseEnemyCount()
    {
        enemyText.text = "x" + (enemySpawn.transform.childCount - 1);

        if (enemySpawn.transform.childCount - 1 == 0)
        {
            gate.SetActive(true);
        }
    }

    public void winGame()
    {
        win.SetActive(true);
        joystick.SetActive(false);
        saveGame();
        Destroy(player);
    }

    public void incrementGold()
    {
        goldText.text = (int.Parse(goldText.text) + 1).ToString();
        playerData.gold = int.Parse(goldText.text);
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

    public void createEnemies(Vector3 loc)
    {
        
    }
}
