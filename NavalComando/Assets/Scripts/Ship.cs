using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class Ship : MonoBehaviour 
{
    private static string SELECTION_GIZMO_CHILD_NAME = "SelectionGizmo";
    private static float DESIRED_DIRECTION_ANGLE_THRESHOLD = 0.5f;


    Rigidbody _rb;
    Quaternion _desiredDirection;
    GameObject _selectionGizmo;
    float _currentImpulse;
    float _health = 100;
    bool _destroyed;
    bool _sinking;
    Quaternion _destroyRotation;
    CollisionSensor [] _collisionSensors;


    public Transform DummyHitPoint;

    public float Impulse = 1f;
    public float RotationSmoothness = 0.1f;
    public CannonGroup LeftCannons;
    public CannonGroup RightCannons;
    public Transform BodyTransform;
    /* Cannon trajectories */
    [Range(0.001f, 0.5f)]
    public float MinCannonballVerticalAcceleration = 0.3f;
    [Range(0.001f, 0.5f)]
    public float MaxCannonballVerticalAcceleration = 0.9f;
    [Range(0,70)]
    public float HorizontalCannonAngleRange = 20f;
    public bool ShowCannonTrajectories = false;

    [Range(10,100)]
    public float MinDamagePerImpact = 50f;
    [Range(10,100)]
    public float MaxDamagePerImpact = 50f;

    /* health UI */
    public float MaxHealth = 100f;
    public RectTransform BackgroundHealthUI;
    public RectTransform HealthUI;
    public Image HealthImageUI; 

    /* Non configurable public properties */
    [HideInInspector]
    public bool CanBeControlled;

    public Vector3 DesiredDirection 
    {
        set {
            Assert.IsTrue(value.x!=0 && value.z!=0, "Desired Direction must not be Vector Zero");

            if(CanBeControlled) {
                _desiredDirection = Quaternion.LookRotation(new Vector3(value.x, 0, value.z));
                _currentImpulse = Impulse;    
            }
        }
    }

    public float Health {
        get{return _health;}
        set{
            _health = Mathf.Max(value, 0f);

            HealthUI.sizeDelta = new Vector2((_health/MaxHealth) * BackgroundHealthUI.rect.width, HealthUI.rect.height);
            HealthImageUI.color = Color.Lerp(Color.red, Color.green, _health/MaxHealth);
        }
    }




    #region UNITY METHODS

    void Awake()
    {
        Assert.IsTrue(MinCannonballVerticalAcceleration <= MaxCannonballVerticalAcceleration, "Max And Min CannonBall Accelerations are bad configured");
        Assert.IsTrue(MinDamagePerImpact <= MaxDamagePerImpact, "MinDamagePerImpact is Greater than MaxDamagePerImpact");

        _rb = GetComponent<Rigidbody>();
        _currentImpulse = 0f;
        _destroyed = false;
        _sinking = false;
        CanBeControlled = true;

        _collisionSensors = GetComponentsInChildren<CollisionSensor>();

        Health = MaxHealth;

        ShipsController.RegisterShip(this);
    }



	void Start () 
    {
        Transform gizmo = transform.FindChild(SELECTION_GIZMO_CHILD_NAME);
        Assert.IsNotNull(gizmo, "Ship does not have child named " + SELECTION_GIZMO_CHILD_NAME);   
        _selectionGizmo = gizmo.gameObject;

        Assert.IsNotNull(BodyTransform, "Body Transform must have been configured in Ship's property");
	}

	

	void Update () 
    {
        if(false == _destroyed) {
            if(_desiredDirection != Quaternion.identity){
                transform.rotation = Quaternion.Lerp(transform.rotation, _desiredDirection, RotationSmoothness);

                if(Quaternion.Angle(transform.rotation, _desiredDirection) <= DESIRED_DIRECTION_ANGLE_THRESHOLD){
                    _desiredDirection = Quaternion.identity;
                }
            }
        } else { // destroy routine
            if(_sinking){
                transform.Translate(Vector3.down * Time.deltaTime * 0.08f);
            } else {
                transform.rotation = Quaternion.Lerp(transform.rotation, _destroyRotation, 0.01f);
            }
        }
	}



    void FixedUpdate()
    {
        if(false == _destroyed) {
            _rb.AddForce(transform.forward * _currentImpulse, ForceMode.Impulse);
        }
    }



    void OnCollisionEnter(Collision coll) 
    {
        Assert.AreEqual<string>(coll.collider.tag, "Ship",  "Ship Collided with something that is not a ship: " + coll.collider.name + "["+coll.collider.tag+"]");

        Ship ship = coll.collider.GetComponent<Ship>();
        Assert.IsNotNull(ship, "Other collision object does not have Ship Component");

        Collision(coll.contacts[0].point, ship.transform.forward);
    }



    void OnDestroy() 
    {
        ShipsController.UnregisterShip(this);
    }

    #endregion




    public void Select() 
    {
        _selectionGizmo.SetActive(true);
        ShipsController.SelectedShip = this;
    }


    public void Unselect()
    {
        _selectionGizmo.SetActive(false);
    }


    #region Movement Commands

    public void FullTurn()
    {
        _desiredDirection = Quaternion.LookRotation(transform.forward * -1);
    }

    #endregion



    public void ReceiveDamage(float quantity)
    {
        Health -= quantity;

        if(Health <= 0f){
            Sink();
        }
    }


    public void DummyCollision()
    {
        Collision(DummyHitPoint.transform.position, DummyHitPoint.transform.forward);
    }





    public void Sink()
    {
        StartCoroutine(SinkCoroutine());
    }



    void Collision(Vector3 collisionPoint, Vector3 otherShipForward)
    {
        float averageInfluence =0;
        int total = 0;

        for(int i=0;i<_collisionSensors.Length;i++) {

            if(Vector3.Distance(collisionPoint, _collisionSensors[i].transform.position) <= CollisionSensor.INFLUENCE_THRESHOLD) {
                averageInfluence += _collisionSensors[i].DamageSensitiveness;
                total ++;
            }  
        }

        averageInfluence /= total;

        float damageFactor = averageInfluence * (1-Mathf.Abs(Vector3.Dot(otherShipForward, transform.forward)));

        ReceiveDamage(Mathf.Clamp((damageFactor * MaxDamagePerImpact), MinDamagePerImpact, MaxDamagePerImpact));
    }




    IEnumerator SinkCoroutine()
    {
        _destroyed = true;
        _sinking = false;

        _destroyRotation = Quaternion.AngleAxis(Random.Range(-40f,40f), Vector3.right) * 
                           Quaternion.AngleAxis(Random.Range(-10f,10f), Vector3.forward) * 
                           transform.rotation;

        Destroy(_rb);
        Destroy(GetComponent<Collider>());

        yield return new WaitForSeconds(10f);

        _sinking = true;

        yield return new WaitForSeconds(35f);

        Destroy(gameObject);
    }
}
