using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;


public class Battlefield : MonoBehaviour 
{
    private static Battlefield _instance;
    public static Battlefield Instance
    {
        get {
            return _instance;
        }
    }

    public Vector3 WindDirection = Vector3.forward;
    public float WindForce = 1f;

    public Transform NorthBorder;
    public Transform SouthBorder;
    public Transform EastBorder;
    public Transform WestBorder;




    void Awake()
    {
        Assert.IsNull(_instance, "There must be just one instance of ShipsController");
        _instance = this;
    }


	void Start () 
    {
	
	}
	

	void Update () 
    {
	    
	}
}
