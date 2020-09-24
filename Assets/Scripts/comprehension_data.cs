using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class comprehension_data : MonoBehaviour {

	public static int[] scores = {0, 0, 0, 0, 0, 0};

	//Functions for buttons on comprehension question panels that assign score 
	//"function_name" to question passed in as argument
	public void zero(int question){
		scores[question - 1] = 0;
	}

	public void one(int question){
		scores[question - 1] = 1;
	}

	public void two(int question){
		scores[question - 1] = 2;
	}

	public void three(int question){
		scores[question - 1] = 3;
	}
}
