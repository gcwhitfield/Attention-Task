using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum practice_screen {None, Target, Catch};

public class input_manager : MonoBehaviour {

    //In-game tracking variables
    public static bool practice;
    public static bool endGame;
    practice_screen wrong_practice;
    public bool listening;
    int max_time = 8;
    List<Image> child_sprites;

    //Drag-in panels/images
    [SerializeField] int comprehension_max = 16;
    [SerializeField] KeyCode Present;
    [SerializeField] KeyCode NotPresent;
    [SerializeField] event_obj input_out;
    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject redX;
    [SerializeField] GameObject practice_wrong_target;
    [SerializeField] GameObject practice_wrong_catch;
    [SerializeField] GameObject outro;
    [SerializeField] GameObject outro_practice;

    //Data tracking variables
    public static List<float> reaction_times_search;
    public static List<float> reaction_times_catch;
    bool comprehension_enabled;
    string book_enjoyment;    string recommend;    string reading_enjoyment;
    float time;               float end_time;      float start_time;
    int timeout_target;                int timeout_catch;
    int corr_target;                   int corr_catch;
    int wrong_target;                  int wrong_catch;

    //starting routines
    void Awake(){
        listening = false;
        practice = true;
        endGame = false;
        wrong_practice = practice_screen.None;
        reaction_times_catch = new List<float>();
        reaction_times_search = new List<float>();
        start_time = 0;
        end_time = 0;

        //initialize list of child sprites
        child_sprites = new List<Image>();
        foreach (Transform child in transform)
            child_sprites.Add(child.GetComponent<Image>());
    }

	void OnEnable () {
        time = 0;
        if (!practice) start_time = Time.time;
        if (listening) StartCoroutine(listen());
	}

    void OnDisable()
    {
        StopCoroutine(listen());
    }

    //If we got something wrong in practice, update the screens
    private bool set_wrong_practice_screen(practice_screen p) {
        if (p != practice_screen.None) {
            if (p == practice_screen.Target) {
                practice_wrong_target.SetActive(true);
            }
            else {
                practice_wrong_catch.SetActive(true);
            }
            return true;
        }
        return false;
    }


    //Main coroutine that listens for button presses during the game
    IEnumerator listen() {
        while (true)
        {
            //Update time
            time += Time.deltaTime;

            //If game should be over
            if (endGame){
                //First set wrong practice screen
                if (set_wrong_practice_screen(wrong_practice)) {
                    this.gameObject.SetActive(false);
                    break;
                }

                //Next check if we should turn off practice
                if(practice){
                    endGame = false;
                    practice = false;
                    visited.clear();
                    outro_practice.SetActive(true);
                    this.GetComponent<target_picker>().ClearGens();
                    //remove all listeners
                }

                //Else, game is totally finished, go to outro
                else{
                    outro.SetActive(true);
                    end_time = Time.time;
                }

                //Don't need me anymore :(
                this.gameObject.SetActive(false);
                break;
            }

            //Check if we have a timeout
            if(time >= max_time){
                if (practice) redX.SetActive(true);
                else crosshair.SetActive(true);

                //hide sprites from view
                foreach (Image i in child_sprites) i.sprite = null;

                //display image for one second
                yield return new WaitForSeconds(1f);

                //update data fields (if in-game)
                if (!practice) {
                    if (random_gen.isTarget) {
                        timeout_target++;
                        reaction_times_search.Add(max_time);
                    }
                    else {
                        timeout_catch++;
                        reaction_times_catch.Add(max_time);
                    }
                }

                //call listener function using default value
                input_out.Invoke(hand.NotPresent);

                //reset time
                time = 0;
            }

            //If player indicates there is no target
            else if (Input.GetKeyDown(NotPresent))
            {
                //If practice is wrong
                if(practice && random_gen.isTarget){
                    wrong_practice = practice_screen.Target;
                    redX.SetActive(true);
                }
                else{ //we are in-game or practice is correct
                    crosshair.SetActive(true);
                }

                //hide sprites from view
                foreach (Image i in child_sprites) i.sprite = null;

                //display image for one second
                yield return new WaitForSeconds(1f);

                //update data fields (if in-game)
                if (!practice) {
                    if (random_gen.isTarget) {
                        wrong_target++;
                        reaction_times_search.Add(time);
                    }
                    else {
                        corr_catch++;
                        reaction_times_catch.Add(time);
                    }
                }

                //call listener function using default value
                input_out.Invoke(hand.NotPresent);

                //reset time
                time = 0;
            }

            //If players indicates there is a target
            else if (Input.GetKeyDown(Present))
            {
                //If practice is wrong
                if(practice && !(random_gen.isTarget)){
                    wrong_practice = practice_screen.Catch;
                    redX.SetActive(true);
                }
                else{ //we are in-game or practice was correct
                    crosshair.SetActive(true);
                }

                //hide sprites from view
                foreach (Image i in child_sprites) i.sprite = null;

                //display image for one second
                yield return new WaitForSeconds(1f);

                //update data fields (if in-game)
                if (!practice) {
                    if (random_gen.isTarget) {
                        corr_target++;
                        reaction_times_search.Add(time);
                    }
                    else {
                        wrong_catch++;
                        reaction_times_catch.Add(time);
                    }
                }

                //call listener function using default value
                input_out.Invoke(hand.Present);

                //reset time
                time = 0;
            }

            //Disable all on-screen items
            redX.SetActive(false);
            crosshair.SetActive(false);

            //Check if we are in practice mode and got a wrong answer
            if (set_wrong_practice_screen(wrong_practice)) {
                wrong_practice = practice_screen.None;
                this.gameObject.SetActive(false);
                break;
            }

            yield return null;
        } //end while loop
	} //end coroutine


    /******************************************
        FUNCTIONS FOR BUTTONS TO SET VARIABLES
     ******************************************/

    public void SetComprehension(bool b){
        comprehension_enabled = b;
    }

    public void SetPractice(bool t){
        practice = t;
    }

    public void SetListening(bool t){
        listening = t;
    }

    public void SetBookEnjoyment(string feedback){
        book_enjoyment = feedback;
    }

    public void SetReadingEnjoyment(string feedback){
        reading_enjoyment = feedback;
    }

    public void SetReadAgain(string feedback){
        recommend = feedback;
    }


    /*******************************************
        FUNCTIONS TO CONVERT DATA TO STRINGS
     *******************************************/
    string s(float v){
		return v.ToString();
	}
	string s(int v){
		return v.ToString();
	}

    string sDiv (int num, int denom) {
		float answer = (((float)num) / ((float)denom)) * 100;
		return answer.ToString ();
	}

    float div(int num, int denom)
    {
        return (((float)num) / ((float)denom)) * 100;
    }


    /* Gathers data tracking variables, converts to string and sends to
      data_saver, which saves and then quits the game */
    public void SaveAndQuit(){
        //If we quit in the middle of the game
        if (end_time == 0 & start_time != 0) {
            end_time = Time.time;
        }
        float average_search_rt = 0;
        float average_catch_rt = 0;
        List<string> info = new List<string>();

        if (comprehension_enabled){
            int[] scores = comprehension_data.scores;
            int Q13 = scores[0] + scores[1] + scores[2];
            int Q46 = scores[3] + scores[4] + scores[5];
            int Q16 = Q13 + Q46;
            info.AddRange(new string[]{s(scores[0]), s(scores[1]), s(scores[2]), s(scores[3]), s(scores[4]), s(scores[5]), s(Q16), sDiv(Q16, comprehension_max), book_enjoyment, recommend, reading_enjoyment});
        }
    
        reaction_times_search.ForEach(x => average_search_rt += x);
        reaction_times_catch.ForEach(x => average_catch_rt += x);
        float searchCount = reaction_times_search.Count;
        float catchCount = reaction_times_catch.Count;
        if(searchCount != 0) average_search_rt /= searchCount;
        else average_search_rt = 0;
        if (catchCount != 0) average_catch_rt /= catchCount;
        else average_catch_rt = 0;

        float total_time = (end_time == 0f) ? 0f : (end_time - start_time);
    
        info.AddRange(new string[] {s(total_time), s(corr_target), s(wrong_target + timeout_target), sDiv(corr_target, 16), s(average_search_rt), s(corr_catch), s(wrong_catch + timeout_catch), sDiv(corr_catch, 8), s(average_catch_rt), s(corr_catch + corr_target), s(wrong_catch + wrong_target + timeout_catch + timeout_target)});
    
        // Uncomment this to enable data saving to local file
        // this.GetComponent<data_saver>().Save(info, comprehension_enabled);

        this.GetComponent<data_saver>().CreateDatabaseObject(
            total_time,
            corr_target,
            wrong_target + timeout_target,
            div(corr_target, 16),
            average_search_rt,
            corr_catch,
            wrong_catch + timeout_catch,
            div(corr_catch, 8),
            average_catch_rt,
            corr_catch + corr_target,
            wrong_catch + wrong_target + timeout_catch + timeout_target
        );
    
    }
}
