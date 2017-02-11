using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;


public class ShipsUserControl : MonoBehaviour 
{
    private static float DRAG_THRESHOLD = 20f;

    bool _buttonDown;
    bool _dragging;
    Vector3 _initialDragPoint; // pixelCoordinates
    Vector3 _lastCursorPoint;
    DirectionGizmo _directionGizmo;

    void Awake() 
    {
        _buttonDown = false;
        _dragging = false;
    }



	void Start () 
    {
        _directionGizmo = GameObject.FindObjectOfType<DirectionGizmo>();
        Assert.IsNotNull(_directionGizmo, "There is no DirectionGizmo in the scene");
	}
	
	
	void Update () 
    {
        if(Input.GetMouseButtonDown(0)) {
            _buttonDown = true;
            _initialDragPoint = Input.mousePosition;

            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hits = Physics.Raycast(r, out hit, 100, 1 << LayerMask.NameToLayer("Ships"));    

            ShipsController.UnselectAll();
            UIController.SetSelectedShip(false);
            Debug.Log("deselecting");

            if(hits) {
                Ship ship = hit.collider.GetComponent<Ship>();
                ship.Select();

                UIController.SetSelectedShip(true);
            }
        }

        if(Input.GetMouseButtonUp(0)) {
            _buttonDown = false;
            _dragging = false;
            _directionGizmo.HideGizmo();

            if(ShipsController.SelectedShip != null) {
                ShipsController.SelectedShip.DesiredDirection = _directionGizmo.DesiredDirection;
            }
        }

        if(_buttonDown) {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hits = Physics.Raycast(r, out hit, 100, 1 << LayerMask.NameToLayer("PointerDetector"));

            if(hits) {
                if(!_dragging) {
                    if((Input.mousePosition - _initialDragPoint).magnitude >= DRAG_THRESHOLD) {
                        _dragging = true;
                    }
                }
            }

            if(_dragging) {
                if(ShipsController.SelectedShip != null) {
                    if(ShipsController.SelectedShip.CanBeControlled){
                        _directionGizmo.TargetPosition = hit.point;
                        _directionGizmo.ShowGizmo();
                    }
                } else {
                    // Drag Camera
                    Vector3 dif = _lastCursorPoint - Input.mousePosition;
                    Camera.main.transform.Translate(dif.x * 0.1f, dif.y * 0.1f, 0);

                    // Clamp Camera to Battlefield borders
                    Vector3 camPos = Camera.main.transform.position;
                    Battlefield bf = Battlefield.Instance;
                    Camera.main.transform.position = 
                        new Vector3(Mathf.Clamp(camPos.x, bf.WestBorder.position.x, bf.EastBorder.position.x),
                                    camPos.y,
                                    Mathf.Clamp(camPos.z, bf.SouthBorder.position.z, bf.NorthBorder.position.z));





                }
            }
        }

        _lastCursorPoint = Input.mousePosition;
	}
}
