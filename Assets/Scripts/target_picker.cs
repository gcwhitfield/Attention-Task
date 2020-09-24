using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_picker : MonoBehaviour {

[SerializeField]
event_obj e; //hand event

[SerializeField]
ij_event ij;

[SerializeField]
random_gen rg;
Tuple<int,int> t;
	// Use this for initialization
	void Awake(){
		e.addListener(pick);
	}

	//runs first trial
	void OnEnable(){
		if (!(this.GetComponent<input_manager>().listening)) return;
		if(!(input_manager.practice) || rg == null){
			rg = new random_gen();
    	t = rg.gen_pos();
		}
    ij.Invoke(t.L, t.R);
	}

	void pick(hand h){
		t = rg.gen_pos();
		ij.Invoke(t.L,t.R);
  }

	public void ClearGens(){
		rg = null;
	}
}
