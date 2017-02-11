using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour
{
    private static UIController _instance;
    public static UIController Instance
    {
        get {
            return _instance;
        }
    }


    public RectTransform RightCannonsButton;
    public RectTransform LeftCannonsButton;

    void Awake()
    {
        _instance = this;
    }


	void Start () 
    {
        SetSelectedShip(false);
	}
	
	void Update () 
    {
        if(ShipsController.SelectedShip != null) {
            RightCannonsButton.position = ShipsController.SelectedShip.RightCannons.GizmoScreenPos;
            LeftCannonsButton.position = ShipsController.SelectedShip.LeftCannons.GizmoScreenPos;
        }
	
	}


    public void UI_ShootRightCannons() 
    {
        ShipsController.ShootRightCannons();
    }



    public void UI_ShootLeftCannons() 
    {
        ShipsController.ShootLeftCannons();
    }



    public static void SetSelectedShip(bool isSelected)
    {
        _instance.RightCannonsButton.gameObject.SetActive(isSelected);
        _instance.LeftCannonsButton.gameObject.SetActive(isSelected);
    }
}
