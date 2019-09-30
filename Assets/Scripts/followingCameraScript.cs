using UnityEngine;
using System.Collections;

public class followingCameraScript : MonoBehaviour {
    [SerializeField]
    private GameObject target;
    [SerializeField]
    public Vector3 distance = new Vector3(0, 200, 4);
    [SerializeField]
    public Vector3 lookPoint = new Vector3(0, 1.35f, 0);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = target.transform.position + distance;
        Vector3 lookVector = target.transform.position + lookPoint - transform.position;
        this.transform.rotation = Quaternion.LookRotation(lookVector);
    }
}
