using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class CannonBall : MonoBehaviour 
{
    static int MAX_SAMPLE_POINTS = 60;
    static float SAMPLE_DATA_DISTANCE = 1f;
    static float IMPACT_GIZMO_SIZE = 0.3f;

    List <Vector3> _trajectory;
    bool _hitsShip;

    [HideInInspector]
    public Cannon Cannon;

    [Range(1,50)]
    public float Damage = 5f;

    void Awake() 
    {
        _trajectory = new List<Vector3>();
    }



	void Start () 
    {
        float verticalAcceleration = Cannon.GenerateVerticalAcceleration();
        float horizontalAngle = Cannon.GenerateAngle();

        float vertDisplacement = 0;
        float x=0, y=0;

        Quaternion horRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);

        RaycastHit hit;

        _trajectory.Add(transform.position);

        bool hasHitSomething = false;
        for(int i=1;i<=MAX_SAMPLE_POINTS;i++) {
            x += SAMPLE_DATA_DISTANCE;
            y -= vertDisplacement;

            vertDisplacement += verticalAcceleration;

            _trajectory.Add(transform.position + (horRotation * transform.rotation * new Vector3(0,y,x)));

            Vector3 ini = _trajectory[i-1];
            Vector3 end = _trajectory[i];

            Ray ray = new Ray(ini, end-ini);

            int mask = (1 << LayerMask.NameToLayer("Ships")) | (1 << LayerMask.NameToLayer("Scenario"));

            if(Physics.Raycast(ray, out hit, (end-ini).magnitude, mask)) {

                _hitsShip = hit.collider.tag=="Ship";
                bool hitsWater = hit.collider.tag=="Water";

                Assert.IsTrue(_hitsShip || hitsWater, "Cannon hits something strange: " + hit.collider.name + "   tag:[" + hit.collider.tag + "]");

                _trajectory[i] = hit.point;
                hasHitSomething = true;
                break;
            } 
        }

        Assert.IsTrue(hasHitSomething, "Trajectory has hit nothing!, water or ship");

        if(_hitsShip) {
            Ship ship = hit.collider.gameObject.GetComponent<Ship>();
            Assert.IsNotNull(ship, "Object with tag ship has been hit, but it does not have Ship component");

            ship.ReceiveDamage(Damage);
        }
	}
	
	void Update () 
    {
        for(int i=1;i<_trajectory.Count;i++) {
            Debug.DrawLine(_trajectory[i], _trajectory[i-1], Color.black);
        }

        Vector3 p = _trajectory[_trajectory.Count-1];

        Debug.DrawLine(new Vector3(p.x, p.y+IMPACT_GIZMO_SIZE/2f, p.z), new Vector3(p.x, p.y-IMPACT_GIZMO_SIZE/2f, p.z), _hitsShip ? Color.red : Color.blue);
        Debug.DrawLine(new Vector3(p.x+IMPACT_GIZMO_SIZE/2f, p.y, p.z), new Vector3(p.x-IMPACT_GIZMO_SIZE/2f, p.y, p.z), _hitsShip ? Color.red : Color.blue);
        Debug.DrawLine(new Vector3(p.x, p.y, p.z+IMPACT_GIZMO_SIZE/2f), new Vector3(p.x, p.y, p.z-IMPACT_GIZMO_SIZE/2f), _hitsShip ? Color.red : Color.blue);
	}
}
