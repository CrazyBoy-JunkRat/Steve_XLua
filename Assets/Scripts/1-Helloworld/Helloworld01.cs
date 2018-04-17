using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class Helloworld01 : MonoBehaviour {

    LuaEnv lunenv = new LuaEnv();
	// Use this for initialization
	void Start () {
        lunenv.DoString("print('Hello World,This is Print method')");
        lunenv.DoString("CS.UnityEngine.Debug.Log('This is Debug method')");
        lunenv.Dispose();
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
