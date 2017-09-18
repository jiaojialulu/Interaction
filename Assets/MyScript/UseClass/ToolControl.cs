using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WristBand;

public class ToolControl : MonoBehaviour {

    private bool isShow = false;

    void OnDestory()
    {
        WristBandInput.callToolBoxRemoveListener(show);

    }
	// Use this for initialization
	void Start () {
        WristBandInput.callToolBoxAddListener(show);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void show()
    {
        if (!isShow)
        {
            GetComponentInChildren<Transform>().gameObject.SetActive(false);
            isShow = !isShow;
        }
        else
        {
            GetComponentInChildren<Transform>().gameObject.SetActive(true);
            isShow = !isShow;
        }
    }
}
