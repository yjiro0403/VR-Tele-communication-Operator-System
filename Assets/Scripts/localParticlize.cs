﻿using UnityEngine;
using Windows.Kinect;

//fileIO
using System.IO;

public class localParticlize : MonoBehaviour {
    public float size = 0.2f;
    public float scale = 10f;

    private CoordinateMapper _Mapper;
    private FrameDescription colorFrameDesc;
    private FrameDescription depthFrameDesc;
    private MultiSourceFrameReader multiFrameSourceReader;

    ushort[] depthFrameData;
    byte[] colorFrameData;
    CameraSpacePoint[] cameraSpacePoints;
    ColorSpacePoint[] colorSpacePoints;
    private int bytesPerPixel = 4;

    ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] particles;

    private KinectSensor _Sensor;

    StreamWriter sw;

    UDPCom udpCom;

    //particle Range
    public int x_min = 0;
    public int x_max = 0;
    public int y_min = 0;
    public int y_max = 0;
    public float z_min = 0;
    public float z_max = 0;


    // Use this for initialization
    void Start() {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            multiFrameSourceReader = _Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color);

            multiFrameSourceReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            _Mapper = _Sensor.CoordinateMapper;

            depthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;
            int depthWidth = depthFrameDesc.Width;
            int depthHeight = depthFrameDesc.Height;
            depthFrameData = new ushort[depthWidth * depthHeight];
            colorSpacePoints = new ColorSpacePoint[depthWidth * depthHeight];
            cameraSpacePoints = new CameraSpacePoint[depthWidth * depthHeight];

            colorFrameDesc = _Sensor.ColorFrameSource.FrameDescription;

            int colorWidth = colorFrameDesc.Width;
            int colorHeight = colorFrameDesc.Height;

            // allocate space to put the pixels being received
            colorFrameData = new byte[colorWidth * colorHeight * bytesPerPixel];

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }

            particles = new ParticleSystem.Particle[depthWidth * depthHeight];
        }

        //sw = new StreamWriter("Log.txt", false);

    }

// Update is called once per frame
    void Update () {
	
	}

    void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
    {
        int depthWidth = 0;
        int depthHeight = 0;
        int colorWidth = 0;
        int colorHeight = 0;


        bool multiSourceFrameProcessed = false;
        bool colorFrameProcessed = false;
        bool depthFrameProcessed = false;

        MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

        if (_Sensor == null) return;

        if(multiSourceFrame != null)
        {
            using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
            {
                using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
                {
                    if (depthFrame != null)
                    {
                        FrameDescription depthFrameDescription = depthFrame.FrameDescription;
                        depthWidth = depthFrameDescription.Width;
                        depthHeight = depthFrameDescription.Height;
                        depthFrame.CopyFrameDataToArray(depthFrameData);
                        depthFrameProcessed = true;
                    }

                    if (colorFrame != null)
                    {
                        FrameDescription colorFrameDescription = colorFrame.FrameDescription;
                        colorWidth = colorFrameDescription.Width;
                        colorHeight = colorFrameDescription.Height;
                        colorFrame.CopyConvertedFrameDataToArray(colorFrameData, ColorImageFormat.Bgra);
                        //string text = System.Text.Encoding.Unicode.GetString(colorFrameData);
                        //sw.WriteLine(text[1]);
                        //sw.Flush();
                     
                        colorFrameProcessed = true;
                    }

                    multiSourceFrameProcessed = true;
                }

                if (multiSourceFrameProcessed && depthFrameProcessed && colorFrameProcessed)
                {
                    _Mapper.MapDepthFrameToColorSpace(depthFrameData, colorSpacePoints);
                    _Mapper.MapDepthFrameToCameraSpace(depthFrameData, cameraSpacePoints);

                    int particleCount = 0;

                    for (int y = 0; y < depthHeight; y += 2)
                    {
                        for (int x = 0; x < depthWidth; x += 2)
                        {
                            int depthIndex = (y * depthWidth) + x;
                            CameraSpacePoint p = cameraSpacePoints[depthIndex];
                            ColorSpacePoint colorPoint = colorSpacePoints[depthIndex];

                            byte r = 0;
                            byte g = 0;
                            byte b = 0;
                            byte a = 0;

                            int colorX = (int)System.Math.Floor(colorPoint.X + 0.5);
                            int colorY = (int)System.Math.Floor(colorPoint.Y + 0.5);

                            if ((colorX >= 0 + x_min) && (colorX < colorWidth - x_max) && (colorY >= 0 + y_min) && (colorY < colorHeight - y_max))
                            {
                                int colorIndex = ((colorY * colorWidth) + colorX) * bytesPerPixel;
                                //int displayIndex = depthIndex * bytesPerPixel;
                                b = colorFrameData[colorIndex++];
                                g = colorFrameData[colorIndex++];
                                r = colorFrameData[colorIndex++];
                                a = colorFrameData[colorIndex++];
                            }

                            if (!(double.IsInfinity(p.X)) && !(double.IsInfinity(p.Y)) && !(double.IsInfinity(p.Z)))
                            {
                                //注意:Zだけx,yと仕様が違う
                                if ((colorX >= 0 + x_min) && (colorX < colorWidth - x_max) && (colorY >= 0 + y_min) && (colorY < colorHeight - y_max) && (p.Z > z_min) && (p.Z < z_max)) { 
                                    //particles[particleCount].position = new Vector3(x/5, y/-5, x/4);
                                    particles[particleCount].position = new Vector3(p.X * scale, (-1) * p.Y * scale, p.Z * scale);
                                particles[particleCount].startColor = new Color(r/255F,g/255F,b/255F);
                                //particles[particleCount].startColor = color;
                                particles[particleCount].startSize = size;
                                particleCount++;
                                }
                            }
                        }
                    }
                    //Debug.Log(particles[100].startColor.g);

                    _particleSystem = gameObject.GetComponent<ParticleSystem>();
                    _particleSystem.SetParticles(particles, particles.Length);
                    //udpCom.SendMessage();
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        multiFrameSourceReader.Dispose();
        multiFrameSourceReader = null;

        if (_Mapper != null)
        {
            _Mapper = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
