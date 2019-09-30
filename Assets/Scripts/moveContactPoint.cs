using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveContactPoint : MonoBehaviour {
    public Transform _camera, _workerLooking, _helperFace, _helperLooking;
    [SerializeField]
    Renderer _workerContactRenderer, _helperContactRenderer;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "HelperGazeLine")
        {
            _helperContactRenderer.enabled = true;
        }
        if (other.gameObject.name == "WorkerGazeLine")
        {
            _workerContactRenderer.enabled = true;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        RaycastHit hit;
        //Debug.Log(collider.gameObject.name);
        //Debug.Log(_helperFace.transform.position);
        //helpergazeline->もとから90度ずれてる
        //helpergazeline.x:100->helperFace.rot.x = 100-90 = 10
        if (collider.gameObject.name == "HelperGazeLine")
        {
            if (Physics.Raycast(_helperFace.transform.position, _helperFace.transform.forward, out hit))
            {
                //Debug.Log("Contact Point : " + hit.point);
                _helperLooking.position = hit.point;
            }
        }
        else if (collider.gameObject.name == "WorkerGazeLine")
        {
            //Debug.Log("Camera Position : " + _camera.position);
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit))
            {
                //Debug.Log("Contact Point : " + hit.point);
                _workerLooking.position = hit.point;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "HelperGazeLine")
        {
            _helperContactRenderer.enabled = false;
        }
        if (other.gameObject.name == "WorkerGazeLine")
        {
            _workerContactRenderer.enabled = false;
        }

    }
}
