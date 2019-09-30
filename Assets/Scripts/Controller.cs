using UnityEngine;

public class Controller : MonoBehaviour
{
    bool triggerPress = false;

    int jointNum = 3;
    Transform[] Pointer = new Transform[3];
    //Open hand
    //float[,] openHand = new float[,] { { 0, -51.834f, 0, 0, 0, 0, 0, 0, 0, 0, -91.219f, 0 }, { 9.070001f, -34.296f, 0, 0, 0, 0, 0, 0, 0, 0, -109.855f, 0 }, { -8.33f, -59.599f, 0, 0, 0, 0, 0, 0, 0, 0, -85.135f, 0 }, { 2.44f, -43.452f, 0, 0, 0, 0, 0, 0, 0, 0, -101.821f, 0 }, { 1.81f, -117.09f, 15.621f, -24.013f, 39.712f, 0, 0, 0, 0, 0, -64.29f, 0 } };
    //float[,] pointHand = new float[,] { { 0, -51.834f, 90f, 0, 0, 90f, 0, 0, 90f, 0, -91.219f, 0 }, { 9.070001f, -34.296f, 90, 0, 0, 90, 0, 0, 90, 0, -109.855f, 0 }, { -8.33f, -59.599f, 0, 0, 0, 0, 0, 0, 0, 0, -85.135f, 0 }, { 2.44f, -43.452f, 90, 0, 0, 90, 0, 0, 90, 0, -101.821f, 0 }, { 1.81f, -117.09f, 55.621f, -24.013f, 39.712f, 40, 0, 0, 90, 0, -64.29f, 0 } };
    //float[,] closeHand = new float[,] { { 0, -51.834f, 90f, 0, 0, 90f, 0, 0, 90f, 0, -91.219f, 0 }, { 9.070001f, -34.296f, 90, 0, 0, 90, 0, 0, 90, 0, -109.855f, 0 }, { -8.33f, -59.599f, 0, 0, 0, 0, 0, 0, 0, 0, -85.135f, 0 }, { 2.44f, -43.452f, 90, 0, 0, 90, 0, 0, 90, 0, -101.821f, 0 }, { 1.81f, -117.09f, 55.621f, -24.013f, 39.712f, 40, 0, 0, 90, 0, -64.29f, 0 } };
    float closeHandPointerz =  80.0f;

    bool pointHandFlag = true;
    //[SerializeField]
    //GameObject Hand;


    void Start()
    {
        //オブジェクト呼び出し
        Pointer[0] = transform.Find("Hand/JNT_Root/JNT_Wrist/JNT_Palm/JNT_Pointer01");
        Pointer[1] = Pointer[0].transform.Find("JNT_Pointer02");
        Pointer[2] = Pointer[1].transform.Find("JNT_Pointer03");
    }

    void Update()
    {
        SteamVR_TrackedObject trackedObject = GetComponent<SteamVR_TrackedObject>();
        var device = SteamVR_Controller.Input((int)trackedObject.index);
        /*
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            triggerPress = true;
        }

        //Hand Shapes
        //使わなかったら消す
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            changeHandShape();
        }*/
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            triggerPress = true;
        }

        //Hand Shapes
        //使わなかったら消す
        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            //Debug.Log("Pointing Hand");
            pointHandFlag = true;
            for (int i = 0; i < jointNum; i++)
            {
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, 0.0f);
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, Pointer[i].localRotation.z);
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, Pointer[i].localRotation.z);

                //Pointer[i].localRotation = Pointer[i].localRotation;
                Pointer[i].localEulerAngles = new Vector3(Pointer[i].localEulerAngles.x, Pointer[i].localEulerAngles.y, 0.0f);

            }
        }else
        {
            //Debug.Log("Close Hand");
            pointHandFlag = false;
            //Zを変える
            for (int i = 0; i < jointNum; i++)
            {
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, closeHandPointerz);
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, Pointer[i].localRotation.z);
                Pointer[i].localEulerAngles = new Vector3(Pointer[i].localEulerAngles.x, Pointer[i].localEulerAngles.y, closeHandPointerz);

                //Pointer[i].localRotation = Pointer[i].localRotation;

            }
        }
    }

    public bool triggerCheck()
    {
        if (triggerPress)
        {
            triggerPress = false;
            return true;
        }
        else { return false; }
    }

    public bool handStateCheck()
    {
        if (pointHandFlag)
        {
            return true;
        }else { return false; }
    }

    void changeHandShape()
    {
        if (pointHandFlag)
        {
            Debug.Log("Close Hand");
            pointHandFlag = false;
            //Zを変える
            for (int i = 0; i < jointNum; i++)
            {
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, closeHandPointerz);
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, Pointer[i].localRotation.z);
                Pointer[i].localEulerAngles = new Vector3(Pointer[i].localEulerAngles.x, Pointer[i].localEulerAngles.y,closeHandPointerz);

                //Pointer[i].localRotation = Pointer[i].localRotation;

            }

        }
        else
        {
            Debug.Log("Pointing Hand");
            pointHandFlag = true;
            for (int i = 0; i < jointNum; i++)
            {
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, 0.0f);
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, Pointer[i].localRotation.z);
                //Pointer[i].localRotation = Quaternion.Euler(Pointer[i].localRotation.x, Pointer[i].localRotation.y, Pointer[i].localRotation.z);

                //Pointer[i].localRotation = Pointer[i].localRotation;
                Pointer[i].localEulerAngles = new Vector3(Pointer[i].localEulerAngles.x, Pointer[i].localEulerAngles.y, 0.0f);

            }
        }
    }
}