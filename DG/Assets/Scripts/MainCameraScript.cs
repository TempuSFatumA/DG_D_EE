using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour {

    public GameObject topBoundary;
    public GameObject bottomBoundary;
    public GameObject rightBoundary;
    public GameObject leftBoundary;
    //public GUIText text1;
    //public GUIText text2;

    Quaternion old_rotation;
	float old_fov;
	Quaternion target_rotation;
	float target_fov;
	bool zoomed;
	float zoom_amount;
	const float ZOOM_INERTIA = 0.0001f;
	const float ZOOM_SPEED = 10.0f;
	const float ZOOM_FOV = 0.25f;
	const float PAN_SPEED = 10.0f;
    
	// Use this for initialization
	void Start () {
		old_rotation = transform.rotation;
        old_fov = camera.fov;
		zoomed = false;
		zoom_amount = 0.0f;
		int width = Screen.width;
		int height = Screen.height;
		if(Screen.width < Screen.height * 16 / 10){
			height = Screen.width * 10 / 16;
		} else {
			width = Screen.height * 16 / 10;
		}
		camera.pixelRect = new Rect((Screen.width - width)*0.5f,(Screen.height - height)*0.5f,width,height);
	}
	
	void StartZoom() {
		var cursor = ObjectManagerScript.Instance().GetMyCursorObject();
		if(cursor){
			target_rotation.SetLookRotation(cursor.transform.position - transform.position, old_rotation * new Vector3(0,1,0));
		}
		zoomed = true;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("z")){
			if(!zoomed){
				StartZoom ();
			} else {
				zoomed = false;
			}
		}
		if(zoomed){
            Ray ray;
            RaycastHit[] hits;
            bool isOnBottom = false;
            bool isOnTop = false;
            bool isOnLeft = false;
            bool isOnRight = false;

            ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            hits = Physics.RaycastAll(ray);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject == rightBoundary)
                    isOnRight = true;
                if (hits[i].transform.gameObject == leftBoundary)
                    isOnLeft = true;
                if (hits[i].transform.gameObject == topBoundary)
                    isOnTop = true;
                if (hits[i].transform.gameObject == bottomBoundary)
                    isOnBottom = true;
            }
            
            if (Input.GetKey("d") && !isOnRight) {
				Quaternion rotate = Quaternion.AngleAxis(Time.deltaTime * PAN_SPEED, old_rotation * new Vector3(0,1,0));
				target_rotation = rotate * target_rotation;
            }
            if (Input.GetKey("a") && !isOnLeft) {
				Quaternion rotate = Quaternion.AngleAxis(-Time.deltaTime * PAN_SPEED, old_rotation * new Vector3(0,1,0));
				target_rotation = rotate * target_rotation;
            }
            if (Input.GetKey("s") && !isOnBottom) {
				Quaternion rotate = Quaternion.AngleAxis(Time.deltaTime * PAN_SPEED, old_rotation * new Vector3(1,0,0));
				target_rotation = rotate * target_rotation;
            }
            if (Input.GetKey("w") &&  !isOnTop) {
				Quaternion rotate = Quaternion.AngleAxis(-Time.deltaTime * PAN_SPEED, old_rotation * new Vector3(1,0,0));
				target_rotation = rotate * target_rotation;
            }
       	}
		if(zoomed){	
			zoom_amount = Mathf.Min(1.0f, zoom_amount + Time.deltaTime * ZOOM_SPEED);
		} else {
			zoom_amount = Mathf.Max(0.0f, zoom_amount - Time.deltaTime * ZOOM_SPEED);
		}
		transform.rotation = Quaternion.Lerp(old_rotation, target_rotation, zoom_amount);		
		camera.fov = Mathf.Lerp(old_fov, old_fov * ZOOM_FOV, zoom_amount);		
	}
}
