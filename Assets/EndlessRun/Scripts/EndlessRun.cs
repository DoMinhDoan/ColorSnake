using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRun : MonoBehaviour {

    public List<GameObject> bgList;
    public List<Transform> currentManagedBGList;
    public GameObject lastSpawnedBG;
    public bool isGameOver = false;

    public float speedMoving = 5.0f;

	void Start () {
		
	}
	
	void Update () {
        if(!isGameOver)
		    foreach(Transform t in currentManagedBGList)
            {
                t.transform.Translate(Vector3.down * speedMoving * Time.deltaTime);
            }
	}

    //If you use only2D object use OnTriggerEnter2D(Collider2D other)
    private void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals("BG"))
        {
            currentManagedBGList.Remove(other.transform);
            Destroy(other.gameObject);

            Vector3 spawnPos = lastSpawnedBG.transform.position;
            spawnPos.y += lastSpawnedBG.GetComponent<Collider>().bounds.size.y/2;


            lastSpawnedBG = Instantiate(bgList[Random.Range(0, bgList.Count)], spawnPos,Quaternion.identity);
            spawnPos.y += lastSpawnedBG.GetComponent<Collider>().bounds.size.y / 2;
            lastSpawnedBG.transform.position = spawnPos;

            currentManagedBGList.Add(lastSpawnedBG.transform);
        }
    }
}
