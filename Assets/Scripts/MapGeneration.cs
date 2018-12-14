using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

    public List<GameObject> bgList;
    public List<Transform> currentManagedBGList;
    public GameObject lastSpawnedBG;

    public PlayerControler playerControler;

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
			GameObject parent = other.transform.parent.gameObject;

			currentManagedBGList.Remove(parent.transform);
			Destroy(parent);

            Vector3 spawnPos = lastSpawnedBG.transform.position;
			float height = GetHeightBackground (lastSpawnedBG.gameObject.transform) / 2.0f;
            lastSpawnedBG = Instantiate(bgList[Random.Range(0, bgList.Count)], spawnPos,Quaternion.identity);

			height += GetHeightBackground (lastSpawnedBG.gameObject.transform) / 2.0f;
            spawnPos.y += height;

            lastSpawnedBG.transform.position = spawnPos;

            currentManagedBGList.Add(lastSpawnedBG.transform);

            SetObstacleMaterialFollowingPlayer(lastSpawnedBG.gameObject.transform);
        }
    }

    float GetHeightBackground(Transform parent)
    {
        float height = 0;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag("BG"))
            {
                height = child.gameObject.GetComponent<Collider>().bounds.size.y;

                break;
            }
        }

        return height;
    }

    void SetObstacleMaterialFollowingPlayer(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag("Obstacles"))
            {
                int randomMaterial = Random.Range(0, playerControler.players.Count);
                Material mat = playerControler.players[randomMaterial].GetComponent<MeshRenderer>().material;

                child.gameObject.GetComponent<MeshRenderer>().material = mat;
            }
        }
    }
}
