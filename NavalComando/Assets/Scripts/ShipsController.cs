using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class ShipsController : MonoBehaviour 
{
    private static ShipsController _instance;
    public static ShipsController Instance
    {
        get {
            return _instance;
        }
    }


    private List<Ship> _ships;
    public Ship _selectedShip;


    void Awake()
    {
        Assert.IsNull(_instance, "There must be just one instance of ShipsController");

        _instance = this;

        _ships = new List<Ship>();
    }

               

	void Start () 
    {
	
	}


	
	void Update () 
    {
	
	}



    public static void RegisterShip(Ship ship)
    {
        if(_instance._ships.IndexOf(ship) == -1) {
            _instance._ships.Add(ship);
        }
    }



    public static void UnregisterShip(Ship ship)
    {
        _instance._ships.Remove(ship);
    }



    public static void UnselectAll()
    {
        for(int i=0;i<_instance._ships.Count;i++) {
            _instance._ships[i].Unselect();
        }

        _instance._selectedShip = null;
    }


    public static Ship SelectedShip {
        get{return _instance._selectedShip;}
        set{_instance._selectedShip = value;}
    }


    #region COMMAND METHODS

    public static void ShootRightCannons() 
    {
        if(SelectedShip == null) return;

        Debug.Log("shooting right: " + SelectedShip.name);

        SelectedShip.RightCannons.Shoot();
    }



    public static void ShootLeftCannons() 
    {
        if(SelectedShip == null) return;

        Debug.Log("shooting left: " + SelectedShip.name);

        SelectedShip.LeftCannons.Shoot();
    }

    #endregion
}
