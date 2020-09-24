using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAME_STOP_NOW : MonoBehaviour
{

    [SerializeField] KeyCode EXIT;
    [SerializeField] GameObject manager;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(EXIT))
        {
            manager.GetComponent<input_manager>().SaveAndQuit();
        }
    }
}
