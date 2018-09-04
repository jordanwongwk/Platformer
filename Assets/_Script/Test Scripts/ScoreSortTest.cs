using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PostSomething(string something);

public class ScoreSortTest : MonoBehaviour {

    public bool testCheck = false;
    public bool testCheck2 = false;
    public bool testCheck3 = false;
    PostSomething testingOut;

    const string TESTING = "testing_out";

	// Use this for initialization
	void Start () {
        testingOut = this.HelloWorld;
	}

    void HelloWorld(string post)
    {
        Debug.Log(post);
    }

    void HelloWorldVer2(string post)
    {
        Debug.Log("Aye I overriden it " + post);
    }
	
	// Update is called once per frame
	void Update () {
        if (testCheck)
        {
            testingOut("Hey there");
            testCheck = false;
        }
        else if (testCheck2)
        {
            PostSomething ayeMate = HelloWorldVer2;
            ayeMate("Hey there");
            testCheck2 = false;
        }
        else if (testCheck3)
        {
            TestingOutString(2);
            testCheck3 = false;
        }
	}

    void TestingOutString(int value)
    {
        Debug.Log(TESTING + value.ToString());
    }
}
