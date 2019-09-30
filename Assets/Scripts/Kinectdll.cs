using UnityEngine;
using System.Runtime.InteropServices;

class Freenect2Device
{
struct ColorCameraParams
{
    float fx; ///< Focal length x (pixel)
    float fy; ///< Focal length y (pixel)
    float cx; ///< Principal point x (pixel)
    float cy; ///< Principal point y (pixel)

    /** @name Extrinsic parameters
     * These parameters are used in [a formula](https://github.com/OpenKinect/libfreenect2/issues/41#issuecomment-72022111) to map coordinates in the
     * depth camera to the color camera.
     *
     * They cannot be used for matrix transformation.
     */
    ///@{
    float shift_d, shift_m;

    float mx_x3y0; // xxx
    float mx_x0y3; // yyy
    float mx_x2y1; // xxy
    float mx_x1y2; // yyx
    float mx_x2y0; // xx
    float mx_x0y2; // yy
    float mx_x1y1; // xy
    float mx_x1y0; // x
    float mx_x0y1; // y
    float mx_x0y0; // 1

    float my_x3y0; // xxx
    float my_x0y3; // yyy
    float my_x2y1; // xxy
    float my_x1y2; // yyx
    float my_x2y0; // xx
    float my_x0y2; // yy
    float my_x1y1; // xy
    float my_x1y0; // x
    float my_x0y1; // y
    float my_x0y0; // 1
                   ///@}
};

/** IR camera intrinsic calibration parameters.
 * Kinect v2 includes factory preset values for these parameters. They are used in depth image decoding, and Registration.
 */
struct IrCameraParams
{
    float fx; ///< Focal length x (pixel)
    float fy; ///< Focal length y (pixel)
    float cx; ///< Principal point x (pixel)
    float cy; ///< Principal point y (pixel)
    float k1; ///< Radial distortion coefficient, 1st-order
    float k2; ///< Radial distortion coefficient, 2nd-order
    float k3; ///< Radial distortion coefficient, 3rd-order
    float p1; ///< Tangential distortion coefficient
    float p2; ///< Tangential distortion coefficient
};

    /** Configuration of depth processing. */
    struct Config
    {
        float MinDepth;             ///< Clip at this minimum distance (meter).
        float MaxDepth;             ///< Clip at this maximum distance (meter).

        bool EnableBilateralFilter; ///< Remove some "flying pixels".
        bool EnableEdgeAwareFilter; ///< Remove pixels on edges because ToF cameras produce noisy edges.

    };

}

public class Kinectdll : MonoBehaviour {

    // 関数定義
    [DllImport("sample")]
    public static extern int fnsample();

    [DllImport("Kinectdll")]
    public static extern int fnKinectdll();

    [DllImport("freenect2sharp")]
    public static extern int fnfreenect2sharp();

    [DllImport("freenect2sharp")]
    public static extern int EnumerateDevices();

    [DllImport("freenect2sharp")]
    public static extern string GetDefaultSerial();

    [DllImport("freenect2sharp")]
    public static extern void OpenDevice(int idx);
}
