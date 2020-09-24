using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tuple<T,T2>
{
    T piL;
    T2 piR;

    public T L
    {
        get { return piL; }
    }
    public T2 R
    {
        get { return piR; }
    }

    public Tuple(T piL, T2 piR)
    {
        this.piL = piL;
        this.piR = piR;
    }
}

public class random_gen {
	System.Random rand;
	int numTrials;
	static bool is_target;

	public static bool isTarget
	{
		get { return is_target; }
	}
    public int trial
    {
        get { return numTrials; }
    }
	public random_gen () {
		//use unity random + make sure > .5
		rand = new System.Random();
		input_manager.endGame = false;
		numTrials = 0;
	}
	
	public Tuple<int,int> gen_pos () {
		int maxTrials = input_manager.practice ? 6 : 24;
		int numTargs = input_manager.practice ? 4 : 16;
		int gen = 2;
		int gen2 = 2;
		int numLeft = numTargs - visited.size();
		is_target = false;
		
		//input_manager ends game
		//stop recording data here
		if (numTrials >= maxTrials){
			input_manager.endGame = true;
			numTrials = 0;
			return new Tuple<int,int>(int.MinValue,int.MinValue);
		}
		//no more targets
		if (numLeft == 0){
			numTrials++;
			return new Tuple<int,int>(-1,-1);
		}

		//check if we have any non-targets left
		else if (maxTrials - numTrials > numLeft){
			float max = 2/3;
			float r = Random.Range(0f, 1f);
			if (Mathf.Round(r - max) > 0){
				numTrials++;
				return new Tuple<int,int>(-1,-1);
			}
		}

		//generate a target
		while(gen == 2 || gen2 == 2 || !(visited.visit(gen, gen2))){
			is_target = true;
			gen = rand.Next(5);
			gen2 = rand.Next(5);
		}
		numTrials++;
		return new Tuple<int, int>(gen, gen2);
	}
}
