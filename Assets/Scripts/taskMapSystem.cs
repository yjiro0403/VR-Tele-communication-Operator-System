using UnityEngine;
using System.Collections;
using System.IO;

public class taskMapSystem : MonoBehaviour {
    public GameObject parentObj;
    private GameObject nowTask;
    public GameObject[] taskMap;
    public Vector3 mapPosition;
    public Vector3 mapRotation;
    public Vector3 size = new Vector3(4, 4, 1);

    int i = 0;
    private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    int errorNum = 0;
    int captureNum = 0;

    StreamWriter streamWriter;

    public enum taskType{
        voiceCriteria,
        training,
        boneTask,
        tipTask,
    }

    public taskType task;

    // Use this for initialization
    void Start () {
        nowTask = (GameObject)Instantiate(taskMap[0], mapPosition, Quaternion.Euler(mapRotation.x, mapRotation.y, mapRotation.z));
        //nowTask.transform.parent = parentObj.transform;
        nowTask.transform.position = mapPosition;
        nowTask.transform.localScale = new Vector3(size.x, size.y, size.z);
        Debug.Log("Map Create");
        streamWriter = new StreamWriter(task + "_timeElapse.txt", false);
    }
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0))
        {
            Application.CaptureScreenshot("capture" + captureNum + ".png");
            captureNum++;
            /*
            if (nowTask == null)
            {
                nowTask = (GameObject)Instantiate(taskMap[i], mapPosition, Quaternion.Euler(mapRotation.x, mapRotation.y, mapRotation.z));
                //nowTask.transform.parent = parentObj.transform;
                nowTask.transform.position = mapPosition;
                nowTask.transform.localScale = new Vector3(size.x, size.y, size.z);
                Debug.Log("Map Create");
            }*/
        }
        /*else if (Input.GetMouseButtonDown(1))
        {
            Destroy(nowTask);
            Debug.Log("Map Destroy");
        }*/

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //Debug.Log("NEXT TASK");
            if (!sw.IsRunning) {
                sw.Start();
            }
            Destroy(nowTask);
            if (i < taskMap.Length-1) { i++; }
            if (i == taskMap.Length-1) {
                sw.Stop();
            }
            Debug.Log("Task Number = " + i);
            //Debug.Log("End Time = " + sw.Elapsed);
            streamWriter.WriteLine("Task Number = " + i + " : Time = " + sw.Elapsed);
            //streamWriter.WriteLine("Task Number = ");
            streamWriter.Flush();

            nowTask = (GameObject)Instantiate(taskMap[i], mapPosition, Quaternion.Euler(mapRotation.x, mapRotation.y, mapRotation.z));
            //nowTask.transform.parent = parentObj.transform;
            nowTask.transform.position = mapPosition;
            nowTask.transform.localScale = new Vector3(size.x, size.y, size.z);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("PREV TASK");
            Destroy(nowTask);
            if (i > 0) { i--; }
            nowTask = (GameObject)Instantiate(taskMap[i], mapPosition, Quaternion.Euler(mapRotation.x, mapRotation.y, mapRotation.z));
            //nowTask.transform.parent = parentObj.transform;
            nowTask.transform.position = mapPosition;
            nowTask.transform.localScale = new Vector3(size.x, size.y, size.z);
        } else if (Input.GetKeyDown("e"))
        {
            streamWriter.WriteLine("Error Time : " + sw.Elapsed);
            streamWriter.Flush();
            errorNum++;
        } else if (Input.GetKeyDown("r"))
        {
            streamWriter.WriteLine("Last inputed Error is Not Error");
            streamWriter.Flush();

            errorNum--;
        }
    }

    void OnApplicationQuit() {
        streamWriter.WriteLine("Error Number = " + errorNum);
        streamWriter.Flush();
        streamWriter.Close();
    }
}
