using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string ABPath = Application.streamingAssetsPath;

        string[] AB = Directory.GetFiles(Application.streamingAssetsPath, "update*.ab");
        string path = Path.Combine(ABPath, Path.GetFileName(AB[0]));
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        var text = ab.LoadAsset("lua1.lua") as TextAsset;
        //var bytes= System.Text.Encoding.UTF8.GetBytes(text.text);
        //print(System.Text.Encoding.UTF8.GetString(bytes));
        print(text.text);
        ab.Unload(false);


    }

}
