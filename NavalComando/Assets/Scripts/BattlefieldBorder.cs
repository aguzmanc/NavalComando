using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class BattlefieldBorder : MonoBehaviour 
{
	void Start () 
    {
	
	}
	
	void Update () 
    {
	   
	}

    void OnTriggerEnter(Collider coll)
    {
        Ship ship = coll.GetComponent<Ship>();

        Assert.IsNotNull(ship, "BattlefieldBorder is starting collide with strange non ship object: [" + coll.name + "]");

        ship.CanBeControlled = false;
        ship.FullTurn();

    }


    void OnTriggerExit(Collider coll)
    {
        Ship ship = coll.GetComponent<Ship>();

        Assert.IsNotNull(ship, "BattlefieldBorder is ending collide with strange non ship object: [" + coll.name + "]");

        ship.CanBeControlled = true;
    }
}
