using UnityEngine;
using System.Collections;

public class CollisionSensor : MonoBehaviour 
{
    public static float INFLUENCE_THRESHOLD = 3f;

    [Range(0, 1f)]
    public float DamageSensitiveness = 0.5f;

    Renderer _rend;

    void Awake()
    {
        _rend = GetComponent<Renderer>();
    }
    

	void Start () 
    {
	    
	}
	

	void Update () 
    {
        _rend.material.color = Color.Lerp(Color.black, Color.red, DamageSensitiveness);
	}
}
