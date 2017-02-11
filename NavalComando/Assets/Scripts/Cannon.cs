using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class Cannon : MonoBehaviour 
{
    public enum CannonState { ReadyToShoot=1, Aiming=2, Reloading=3, ShotDone=4 };

    Renderer _rend;
    Color _originalColor;
    Ship _ship;

    [Range(0,5)]
    public float MaxTimeToShoot = 3; 

    [Range(0,5)]
    public float MinTimeToShoot = 0.3f; 

    [Range(0,5)]
    public float ReloadTime = 2f;

    CannonState _currentState;
    public CannonState CurrentState {
        get{return _currentState;}
        set{
            _currentState = value;

            if(value==CannonState.ReadyToShoot) {
                _rend.material.color = _originalColor;
            }

            if(value==CannonState.Aiming) {
                _rend.material.color = Color.yellow;
            }

            if(value==CannonState.Reloading) {
                _rend.material.color = Color.red;
            }

            if(value==CannonState.ShotDone) {
                _rend.material.color = Color.green;
            }
        }
    }
     
    public GameObject CannonBallPrototype;


    void Awake()
    {
        _rend = GetComponentInChildren<Renderer>(); 
        _originalColor = _rend.material.color;
        CurrentState = CannonState.ReadyToShoot;

        _ship = GetComponentInParent<Ship>();
    }




	void Start () 
    {
        
	}


	
	void Update () 
    {
        
	}


    public void Shoot()
    {
        CurrentState = CannonState.Aiming;

        StartCoroutine(_ShootCoroutine());
    }


    public float GenerateVerticalAcceleration()
    {
        return Random.Range(_ship.MinCannonballVerticalAcceleration, _ship.MaxCannonballVerticalAcceleration);
    }

    public float GenerateAngle()
    {
        return Random.Range(-_ship.HorizontalCannonAngleRange, _ship.HorizontalCannonAngleRange);
    }




    IEnumerator _ShootCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(MinTimeToShoot, MaxTimeToShoot));

        CurrentState = CannonState.Reloading;

        GameObject obj = (GameObject)Instantiate(CannonBallPrototype, transform.position, transform.rotation);
        obj.GetComponent<CannonBall>().Cannon = this;

        Destroy(obj, 3);

        yield return new WaitForSeconds(ReloadTime);

        CurrentState = CannonState.ShotDone;
    }

}
