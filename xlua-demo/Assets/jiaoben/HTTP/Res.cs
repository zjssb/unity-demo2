using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using Unity.SharpZipLib.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

class UpDateRes
{
    public string[] Download { get; set; }

    public string[] Delete { get; set; }

    public int Size { get; set; }
}

public class Res : MonoBehaviour
{
    public static Res res { get; set; }

    private void Awake()
    {
        res = this;
    }

    public static string isUpDateURL = "http://127.0.0.1:5000/";

    public static string UpDateURL = "http://127.0.0.1:5000/update";

    string dataPath = Application.streamingAssetsPath;

    public delegate void UpDateOver();

    public event UpDateOver upDateOver;

    public void StartLoad()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        // 获取目录下所有文件名称   Directory.GetFiles 获取的名称目录分级符为 反斜杠
        string[] files_path = Directory.GetFiles(dataPath, "*", SearchOption.TopDirectoryOnly);
        string[] file_name = new string[files_path.Length];
        for (int i = 0; i < files_path.Length; i++)
        {
            // 从路径中提取 文件名
            file_name[i] = Path.GetFileName(files_path[i]);
        }

        List<IMultipartFormSection> toData = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("names", string.Join(",", file_name))
        };

        UnityWebRequest www = UnityWebRequest.Post(isUpDateURL, toData);

        yield return www.SendWebRequest();

        if (!www.isDone)
        {
            Debug.Log("信息发送失败");
            www.Dispose();
            yield break;
        }

        DownloadHandler dh = www.downloadHandler;

        string data = dh.text;
        var json = JsonConvert.DeserializeObject<UpDateRes>(data);

        foreach (var item in json.Delete)
        {
            File.Delete(Path.Combine(dataPath, item));
        }

        if (json.Size == 0)
        {
            Debug.Log("不需要更新");
            upDateOver();
        }
        else
        {
            StartCoroutine(DowmloadFile(json));
        }

        //FileStream zip = File.Create(path + "/update.zip");
        //zip.Write(dh.data, 0, dh.data.Length);
        //zip.Flush();
        //zip.Close();

        // 对下载的压缩包进行解压，完成后删除


        www.Dispose();
    }

    IEnumerator DowmloadFile(UpDateRes data)
    {
        GameObject Dialog = Instantiate(Resources.Load("Dialog/Dialog")) as GameObject;
        int flag = 0;
        Dialog.transform.SetParent(GameObject.Find("Canvas").transform);
        Dialog.GetComponent<RectTransform>().localPosition = Vector3.zero;
        Dialog.transform.Find("text").GetComponent<TMP_Text>().text += data.Size + "B";
        Dialog
            .transform.Find("yes")
            .GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                flag = 1;
                Destroy(Dialog);
            });
        Dialog
            .transform.Find("no")
            .GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                flag = -1;
                Destroy(Dialog);
            });
        yield return new WaitUntil(() => flag != 0);
        // 点击退出按钮
        if (flag == -1)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            yield break;
        }

        List<IMultipartFormSection> toData = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("name", SystemInfo.deviceUniqueIdentifier),
            new MultipartFormDataSection("files", string.Join(",", data.Download))
        };

        UnityWebRequest www = UnityWebRequest.Post(UpDateURL, toData);

        www.downloadHandler = new DownloadHandlerFile(Path.Combine(dataPath, "download.ab"));

        yield return www.SendWebRequest();

        if (!www.isDone)
        {
            Debug.Log("资源下载失败");
            www.Dispose();
            yield break;
        }

        yield return new WaitUntil(() => www.downloadProgress == 1);

        yield return StartCoroutine(Decompression(Path.Combine(dataPath, "download.ab"), dataPath));
        File.Delete(Path.Combine(dataPath, "download.ab"));
        www.Dispose();
        upDateOver();
    }

    /// <summary>
    /// 解压协程
    /// </summary>
    /// <param name="path">压缩包路径</param>
    /// <param name="inPath">内容输出路径</param>
    /// <returns></returns>
    IEnumerator Decompression(string inPath, string outPath)
    {
        string jieya_path = Path.Combine(inPath, "../", "downloadData");
        Directory.CreateDirectory(jieya_path);
        // 将压缩文件 解压到 指定目录（目录会被清空）
        ZipUtility.UncompressFromZip(inPath, "", jieya_path);

        string[] files = Directory.GetFiles(jieya_path);
        foreach (string file in files)
        {
            string name = Path.GetFileName(file);
            string newPath = Path.Combine(outPath, name);
            File.Move(file, newPath);
        }
        Directory.Delete(jieya_path, true);
        yield return null;
    }
}
