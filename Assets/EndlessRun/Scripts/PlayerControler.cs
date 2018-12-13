using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour {

    public EndlessRun er;

    public List<GameObject> players;
    public GameObject playerSample;
    public int maxPlayerCount = 5;

    public GameObject restartGUI;

    public int maxMovingStep = 1;
    public int distanceMoving = 100;

    float stepIntervalY = 1.0f;
    float stepIntervalX = 0.02f;
    int movingOffset = 0;

    Vector3 touchPosWorld;

    //Change me to change the touch phase used.
    TouchPhase touchPhase = TouchPhase.Ended;


    void Start()
    {
        AddPlayerNode(null);        
    }

    void Update()
    {
        //We check if we have more than one touch happening.
        //We also check if the first touches phase is Ended (that the finger was lifted)
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == touchPhase) || Input.GetMouseButtonDown(0))
        {
            Ray ray;
            if (Input.touchCount > 0)   // ??? phone
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {                
                GameObject touchedObject = hit.transform.gameObject;
                if(touchedObject.transform.CompareTag("Player"))
                {
                    GameObject first = players[0];
                    players.RemoveAt(0);
                    players.Add(first);

                    UpdatePlayerTransformLocationY(0);
                }                
            }
        }


        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {           
            if(movingOffset > -maxMovingStep)
            {
                movingOffset--;
                transform.Translate(Vector3.left * distanceMoving * Time.deltaTime);
            }
            
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (movingOffset < maxMovingStep)
            {
                movingOffset++;
                transform.Translate(Vector3.right * distanceMoving * Time.deltaTime);
            }                
        }

        // update snake transform
        if (!er.isGameOver && stepIntervalY > 0 && players[0].transform.localPosition.y > 0)
        {
            UpdatePlayerTransformLocationY(stepIntervalY);

            stepIntervalY -= 0.02f;
        }
        
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdatePlayerTransformLocationX(float interval)
    {
        for (int i = 0; i < players.Count; i++)
        {
            Vector3 localPos = new Vector3((-i / maxPlayerCount) + interval, players[i].transform.localPosition.y, 0);

            players[i].transform.localPosition = localPos;
        }
    }

    void UpdatePlayerTransformLocationY(float interval)
    {
        for (int i = 0; i < players.Count; i++)
        {
            Vector3 localPos = new Vector3(0, -i + interval, 0);

            players[i].transform.localPosition = localPos;
        }
    }

    public void AddPlayerNode(Material mat)
    {
        GameObject go = Instantiate(playerSample, new Vector3(0, players.Count, 0), Quaternion.identity, this.transform) as GameObject;

        if(players.Count == 0)
        {
            go.transform.localPosition = new Vector3(0, 0, 0);
        }

        if(mat != null)
        {
            go.GetComponent<MeshRenderer>().material = mat;
        }

        if(players.Count == maxPlayerCount)
        {
            GameObject lastGO = players[maxPlayerCount - 1];
            players.RemoveAt(maxPlayerCount - 1);
            Destroy(lastGO);
        }
        players.Insert(0, go);       
        stepIntervalY = 1.0f;
    }

    public bool HitObstacle(Material mat, GameObject obstacle)
    {
        if (players.Count > 1)
        {
            if( players[0].GetComponent<MeshRenderer>().sharedMaterial.name.Contains(mat.name))
            {                
                var toRemove = new List<GameObject>();
                for(int i = 0; i < players.Count; i++)
                {
                    print("Obstacle Hit OK");
                    if (players[i].GetComponent<MeshRenderer>().sharedMaterial.name.Contains(mat.name))
                    {
                        toRemove.Add(players[i]);
                        Destroy(players[i].gameObject);
                    }
                    else
                    {
                        break;
                    }                    
                }

                players.RemoveAll(toRemove.Contains);
                if(players.Count == 0)
                {
                    GameOver();
                    return false;
                }

                Destroy(obstacle);
                UpdatePlayerTransformLocationY(0);

                return true;
            }
            else
            {
                GameOver();
                return false;
            }
        }
        else if (players.Count == 1 && players[0].GetComponent<MeshRenderer>().sharedMaterial.name.Contains(mat.name))
        {
            Destroy(obstacle);
            return true;
        }

        GameOver();
        return false;
    }

    void GameOver()
    {
        er.isGameOver = true;
        restartGUI.SetActive(true);
    }
   
}
