using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

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
			GameObject parent = other.transform.parent.gameObject;

			currentManagedBGList.Remove(parent.transform);
			Destroy(parent);

            Vector3 spawnPos = lastSpawnedBG.transform.position;

			print ("Last Spawned:" + lastSpawnedBG.gameObject.name + " - height = " + GetHeightBackground(lastSpawnedBG.gameObject.transform));
			float height = GetHeightBackground (lastSpawnedBG.gameObject.transform) / 2.0f;
			//spawnPos.y += height;
            lastSpawnedBG = Instantiate(bgList[Random.Range(0, bgList.Count)], spawnPos,Quaternion.identity);

			height += GetHeightBackground (lastSpawnedBG.gameObject.transform) / 2.0f;

			print ("Spawning:" + lastSpawnedBG.gameObject.name + " - height = " + GetHeightBackground(lastSpawnedBG.gameObject.transform) + "->Total = " + height);

			spawnPos.y += height;//GetHeightBackground(lastSpawnedBG.gameObject.transform) / 2.0f;
            lastSpawnedBG.transform.position = spawnPos;

            currentManagedBGList.Add(lastSpawnedBG.transform);
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

		//print ("GetHeightBackground:" + parent.name + " - height = " + height);

        return height;
    }
}
