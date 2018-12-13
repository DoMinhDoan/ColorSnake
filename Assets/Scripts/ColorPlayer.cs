using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlayer : MonoBehaviour {
    	
    //If you use only2D object use OnTriggerEnter2D(Collider2D other)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Colors"))
        {
            print("OnTriggerEnter Colors\n");

            Material mat = other.gameObject.GetComponent<MeshRenderer>().material;
            gameObject.transform.parent.GetComponent<PlayerControler>().AddPlayerNode(mat);

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Obstacles"))
        {
            print("OnTriggerEnter Obstacles\n");

            Material mat = other.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            if(gameObject.transform.parent.GetComponent<PlayerControler>().HitObstacle(mat, other.gameObject))
            {
                //Destroy(other.gameObject);
            }
            
        }
    }
}
