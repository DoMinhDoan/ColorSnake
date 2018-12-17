using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour {

	public MapGeneration mapGeneration;

    public List<Material> playersMaterialInitialize;

    public List<GameObject> players;
    public GameObject playerSample;
    public int maxPlayerCount = 5;

    public GameObject restartGUI;

    public int maxMovingStep = 1;
    public float distanceMoving = 1.75f;

    float stepIntervalY = 1.0f;
    float stepIntervalX = 0.02f;
    int movingOffset = 0;

    //Change me to change the touch phase used.
    TouchPhase touchPhase = TouchPhase.Ended;

    // TOUCH    
    // The angle range for detecting swipe
    private const float mAngleRange = 30;

    // To recognize as swipe user should at lease swipe for this many pixels
    private const float mMinSwipeDist = 50.0f;
    public float maxTouchDist = 10.0f; 

    // To recognize as a swipe the velocity of the swipe
    // should be at least mMinVelocity
    // Reduce or increase to control the swipe speed
    private const float mMinVelocity = 2000.0f;

    private Vector2 mStartPosition;
    private float mSwipeStartTime;
    private readonly Vector2 mXAxis = new Vector2(1, 0);
    private readonly Vector2 mYAxis = new Vector2(0, 1);

    void Start()
    {
        if(playersMaterialInitialize.Count == 0)
        {
            AddPlayerNode(null);
        }
        else
        {
            for(int i = 0; i < playersMaterialInitialize.Count; i++)
            {                
                AddPlayerNode(playersMaterialInitialize[i]);
            }

            UpdatePlayerTransformLocationY(0);
        }
    }

    void Update()
    {   
        int swipeDirection = Swipe();

        if(swipeDirection == -1)
        {
            //We check if we have more than one touch happening.
            //We also check if the first touches phase is Ended (that the finger was lifted)

            if (/*(Input.touchCount > 0 && Input.GetTouch(0).phase == touchPhase) ||*/ Input.GetMouseButtonUp(0))
            {                
                GameObject first = players[0];
                players.RemoveAt(0);
                players.Add(first);

                UpdatePlayerTransformLocationY(0);
            }
        }
        
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow) || swipeDirection == 3)
            {
                if (movingOffset > -maxMovingStep)
                {
                    movingOffset--;
                    transform.Translate(Vector3.left * distanceMoving/* * Time.deltaTime*/);
                }

            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || swipeDirection == 4)
            {
                if (movingOffset < maxMovingStep)
                {
                    movingOffset++;
                    transform.Translate(Vector3.right * distanceMoving/* * Time.deltaTime*/);
                }
            }
        }        

        // update snake transform
		if (!mapGeneration.isGameOver && stepIntervalY > 0 && players[0].transform.localPosition.y > 0)
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
        if (players.Count > 0)
        {
            if (mat.name.Contains(players[0].GetComponent<MeshRenderer>().sharedMaterial.name) || players[0].GetComponent<MeshRenderer>().sharedMaterial.name.Contains(mat.name))
            {                
                var toRemove = new List<GameObject>();
                for(int i = 0; i < players.Count; i++)
                {
                    print("Obstacle Hit OK");
                    if (mat.name.Contains(players[i].GetComponent<MeshRenderer>().sharedMaterial.name) || players[i].GetComponent<MeshRenderer>().sharedMaterial.name.Contains(mat.name))
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
                    print("HitObstacle Player Count = 0");

                    GameOver();
                    return false;
                }

                Destroy(obstacle);
                UpdatePlayerTransformLocationY(0);

                return true;
            }
            else
            {
                print("HitObstacle Wrong Material");

                GameOver();
                return false;
            }
        }
        else if (players.Count == 1 && players[0].GetComponent<MeshRenderer>().sharedMaterial.name.Contains(mat.name))
        {
            Destroy(obstacle);
            return true;
        }

        return false;
    }

    void GameOver()
    {
		mapGeneration.isGameOver = true;
        restartGUI.SetActive(true);
    }

    int Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Record start time and position
            mStartPosition = new Vector2(Input.mousePosition.x,
                                         Input.mousePosition.y);
            mSwipeStartTime = Time.time;
        }

        // Mouse button up, possible chance for a swipe
        if (Input.GetMouseButtonUp(0))
        {
            float deltaTime = Time.time - mSwipeStartTime;

            Vector2 endPosition = new Vector2(Input.mousePosition.x,
                                               Input.mousePosition.y);
            Vector2 swipeVector = endPosition - mStartPosition;

            float velocity = swipeVector.magnitude / deltaTime;

            if (velocity > mMinVelocity &&
                swipeVector.magnitude > mMinSwipeDist)
            {
                // if the swipe has enough velocity and enough distance

                swipeVector.Normalize();

                float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
                angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;

                // Detect left and right swipe
                if (angleOfSwipe < mAngleRange)
                {
                    //OnSwipeRight();
                    return 4;
                }
                else if ((180.0f - angleOfSwipe) < mAngleRange)
                {
                    //OnSwipeLeft();
                    return 3;
                }
                else
                {
                    // Detect top and bottom swipe
                    angleOfSwipe = Vector2.Dot(swipeVector, mYAxis);
                    angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                    if (angleOfSwipe < mAngleRange)
                    {
                        //OnSwipeTop();

                        return 1;

                    }
                    else if ((180.0f - angleOfSwipe) < mAngleRange)
                    {
                        //OnSwipeBottom();
                        return 2;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else if (swipeVector.magnitude < maxTouchDist) //need to change color
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        return -1;
    }


}
