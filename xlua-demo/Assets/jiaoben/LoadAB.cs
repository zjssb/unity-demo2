using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadAB : MonoBehaviour
{

    public static string ABPath = Application.streamingAssetsPath;

    /// <summary>
    /// 从ab包中获取对象，并返回
    /// </summary>
    /// <param name="abName">ab包名称</param>
    /// <param name="name">实例名称</param>
    /// <returns></returns>
    public static Object Load(string abName, string name)
    {
        string[] AB = Directory.GetFiles(ABPath, abName);
        if (AB.Length != 1)
        {
            return null;
        }
        string path = Path.Combine(ABPath, Path.GetFileName(AB[0]));
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        Object ject = null;
        if (ab != null)
        {
             ject = ab.LoadAsset(name);
        }

        ab.Unload(false);
        return ject;
    }

    public static string[] GetABAllFileName(string abName)
    {

        string[] filename = Directory.GetFiles(Application.streamingAssetsPath,abName+"*.ab");

        string name = Path.GetFileName(filename[0]);

        AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath,name));
        string[] ABFileName = ab.GetAllAssetNames();
        ab.Unload(false);
        int index = ABFileName.Length;
        for(int i = 0;i<index;i++)
        {
            ABFileName[i] = Path.GetFileName(ABFileName[i]);
        }
        print(ABFileName[0]);
        return ABFileName;


    }

}
