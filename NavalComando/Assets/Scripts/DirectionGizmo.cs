using UnityEngine;
using System.Collections;

public class DirectionGizmo : MonoBehaviour 
{
    private static string GIZMO_CHILD_NAME = "Gizmo";

    GameObject _gizmo;


    public Vector3 DesiredDirection
    {
        get {
            Vector3 v = transform.rotation * Vector3.forward;
            return new Vector3(v.x, 0, v.z);
        }
    }


    public Vector3 TargetPosition 
    {
        set{
            if(ShipsController.SelectedShip == null)
                return;

            Vector3 dif = value - ShipsController.SelectedShip.transform.position;

            transform.rotation = Quaternion.LookRotation(new Vector3(dif.x, 0, dif.z));
            transform.position = ShipsController.SelectedShip.transform.position;
        }
    }



	void Start () 
    {
        _gizmo = transform.FindChild(GIZMO_CHILD_NAME).gameObject;
	}
	

	void Update () 
    {
	}


    public void ShowGizmo()
    {
        _gizmo.SetActive(true);
    }



    public void HideGizmo()
    {
        _gizmo.SetActive(false);
    }
}
