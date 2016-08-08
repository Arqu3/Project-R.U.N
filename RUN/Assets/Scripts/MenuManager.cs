using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour {

    public Sprite logoImage;

	void Start () {
        float range = Random.Range(0, 100);

        if (range < 10)
        {
            GetComponent<Image>().sprite = logoImage;
        }


	}
}
