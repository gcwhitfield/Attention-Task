using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sprite_flipper : MonoBehaviour {
    [SerializeField]
    ij_event input_in;
    [SerializeField]
    Sprite redSquare;
    [SerializeField]
    Sprite blueCircle;
    [SerializeField]
    Sprite targetSprite;
    Image image;
    
	// Use this for initialization
	void Awake () {
        image = this.GetComponent<Image>();
        input_in.addListener(respond);
	}

    void respond(int i, int j)
    {
        int n = transform.GetSiblingIndex();
        int row = n / 5;
        int col = n % 5;

        if(i == row && j == col)
        {
            image.sprite = targetSprite;
        }
        else if(i == int.MinValue && j == int.MinValue){
            image.sprite = null;
        }
        else
        {
            if(Random.value > .5f)
            {
                image.sprite = blueCircle;
            }
            else
            {
                image.sprite = redSquare;
            }
        }
    }
    

}
