using MugenForever.SFF;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MugenForever { 
    public class StartGame : MonoBehaviour {

	    // Use this for initialization
	    void Start () {
            SFFInfo sffInfo = SFFFactory.read("mugen\\chars\\kfm\\kfm.sff");
           // Debug.Log(sffInfo.totalGroup);
        }
	
	    // Update is called once per frame
	    void Update () {
		
	    }
    }
}
