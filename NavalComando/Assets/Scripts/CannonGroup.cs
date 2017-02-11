using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class CannonGroup : MonoBehaviour 
{
    private static string GIZMO_CHILD_NAME = "Gizmo";

    List<Cannon> _cannons;
    bool _shooting;
    Transform _gizmo;

    public float MaxDelayTime = 2f;

    #region Properties

    public Vector3 GizmoScreenPos {
        get {
            return Camera.main.WorldToScreenPoint(_gizmo.position);
        }
    }

    #endregion





    void Awake()
    {
        _cannons = new List<Cannon>(GetComponentsInChildren<Cannon>());
        _shooting = false;
    }



    void Start()
    {
        _gizmo = transform.FindChild(GIZMO_CHILD_NAME);
        Assert.IsNotNull(_gizmo, "Cannon Group has no Gizmo");
    }



	void Update () 
    {
        if(_shooting) { // verify state of all cannons before shoot again (all must be waiting for cannons before ready)
            bool completed = true;
            foreach(Cannon c in _cannons) {
                if(c.CurrentState==Cannon.CannonState.Aiming || c.CurrentState==Cannon.CannonState.Reloading) {
                    completed = false;
                    break;
                }
            }

            if(completed) {
                foreach(Cannon c in _cannons) {
                    c.CurrentState = Cannon.CannonState.ReadyToShoot;
                }

                _shooting = false;
            }
        }
	}


    public void Shoot()
    {
        _shooting = true;
        foreach(Cannon c in _cannons) {
            c.Shoot();
        }
    }
}
