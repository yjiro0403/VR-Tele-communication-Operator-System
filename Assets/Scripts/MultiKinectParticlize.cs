using UnityEngine;

//open .exe
using System.Diagnostics;
using System;

class Position
{
    float x, y, z;
}

public class MultiKinectParticlize : MonoBehaviour
{
    Process exProcess;

    public WinApi SMem;

    //not match framedesc width height
    const int cameraWidth = 512;
    const int cameraHeight = 424;
    const int m_dwFileSize = sizeof(float) * cameraHeight * cameraWidth * 6;
    const int ElementsNumber = cameraWidth * cameraHeight;
    //const int ElementsNumber = 12;

    public float size = 0.2f;

    ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] particles;




    // Use this for initialization
    void Start()
    {
        //ProcessStart();

        SMem = new WinApi("test");
        //SetTestData();
    }

    // Update is called once per frame
    
    void Update()
    {

        float[] data = SMem.GetData();

        //Particle Draw
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystem.Emit(ElementsNumber);
        particles = new ParticleSystem.Particle[ElementsNumber];
        _particleSystem.GetParticles(particles);
        int index = 0;


        for (int particlecount = 0; particlecount < ElementsNumber; particlecount += 1){ // x, y, z, r, g, b
           index = particlecount * 6;

           particles[particlecount].position = poscheck(data[index]*10, data[index + 1]*10, data[index + 2]*10);
           particles[particlecount].startColor = new Color(data[index+3] / 255F, data[index+4] / 255F, data[index+5] / 255F);
           particles[particlecount].startSize = size;
        }

        _particleSystem.SetParticles(particles, particles.Length);

        //UnityEngine.Debug.Log("data = " + particles[14000].startColor.r);        //}
    }



    //Avoid Nan Error
    Vector3 poscheck(float x, float y, float z)
    {
        float xnew, ynew, znew;

        if (float.IsNaN(x)) { xnew = 0; }
        else { xnew = x; }

        if (float.IsNaN(x)) { ynew = 0; }
        else { ynew = y; }

        if (float.IsNaN(z)) { znew = 0; }
        else {znew = z; }

        return new Vector3(xnew, ynew, znew);
    }

    /*
    void ProcessStart() {
        exProcess = new Process();
        exProcess.StartInfo.FileName = Application.dataPath + "\\getPointCloud.exe";

        // exit event
        exProcess.EnableRaisingEvents = true;
        exProcess.Exited += ProcessExited;

        exProcess.Start();
        exProcess.WaitForExit();
    }

    private void ProcessExited(object sender, System.EventArgs e)
    {
        exProcess.Dispose();
        exProcess = null;
    }*/


}
