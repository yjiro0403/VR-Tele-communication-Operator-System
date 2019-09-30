using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HelperLogger : MonoBehaviour {
    [SerializeField]
    Transform Hmd, ControllerL, ControllerR;

    enum Type
    {
        Avatar, HandTip
    }

    [SerializeField]
    Type type;

    StreamWriter helperTransform;

    // Use this for initialization
    void Start() {
        if (type == Type.Avatar) {
            helperTransform = new StreamWriter("HelperTransformLog_Avatar.txt", false);
        } else {
            helperTransform = new StreamWriter("HelperTransformLog_HandTip.txt", false);
        }
    }


    // Update is called once per frame
    void Update () {
        helperTransform.WriteLine(System.DateTime.Now.Minute.ToString() + "分" + System.DateTime.Now.Second.ToString() + "秒" + System.DateTime.Now.Millisecond.ToString());
        helperTransform.WriteLine(Hmd.position.x.ToString("F5") + "," + Hmd.position.y.ToString("F5") + "," + Hmd.position.z.ToString("F5") + "," + Hmd.rotation.x.ToString("F5") + "," + Hmd.rotation.y.ToString("F5") + "," + Hmd.rotation.z.ToString("F5")
            + "," + ControllerL.position.x.ToString("F5") + "," + ControllerL.position.y.ToString("F5") + "," + ControllerL.position.z.ToString("F5") + "," + ControllerL.rotation.x.ToString("F5") + "," + ControllerL.rotation.y.ToString("F5") + "," + ControllerL.rotation.z.ToString("F5")
            + "," + ControllerR.position.x.ToString("F5") + "," + ControllerR.position.y.ToString("F5") + "," + ControllerR.position.z.ToString("F5") + "," + ControllerR.rotation.x.ToString("F5") + "," + ControllerR.rotation.y.ToString("F5") + "," + ControllerR.rotation.z.ToString("F5")
            );
        helperTransform.Flush();
    }

    private void OnDestroy()
    {
        helperTransform.Close();
    }
}
