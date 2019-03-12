using StarkTools.IO;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGMA : MonoBehaviour
{
    public string file = "stage\\st43,lz.gma";
    public GMA gma = new GMA();

    const string temp = "D:\\Temp\\";
    //const string filePath = temp + "vehicle_parts\\parts_bdy01.gma";
    string filePath => temp + file;

    void Start()
    {
        using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var reader = new BinaryReader(file))
            {
                reader.ReadX(ref gma, false);
            }
        }
    }
}
