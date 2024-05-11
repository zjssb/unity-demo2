using System.IO;
using UnityEngine;
using XLua;

public class GM : MonoBehaviour
{
    private static LuaEnv luaEnv;

    public static LuaEnv GetLuaEnv()
    {
        return luaEnv;
    }

    public GameObject Map;

    public GameObject Player;

    private void Awake()
    {
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(MyLoader);
    }

    private void Start()
    {
        Res.res.upDateOver += GameInit;
        Res.res.StartLoad();
    }

    public void GameInit()
    {
        LoadAsset();
        LuaLoad();
        Init();
    }

    private void LoadAsset()
    {
        Map = LoadAB.Load("map*.ab", "map") as GameObject;

        Player = LoadAB.Load("player*.ab", "player") as GameObject;

        Player.GetComponent<Player>().Ammo = LoadAB.Load("ammo*.ab", "ammo") as GameObject;
    }

    /// <summary>
    /// 在开始时，加载lua代码AB包文件，并加载
    /// </summary>
    private void LuaLoad()
    {
        // 从lua更新包的ab包中获取所有名称 AssetBundle.GetAllAssetNames
        string[] abName = LoadAB.GetABAllFileName("update");

        // 遍历并写入 lua 资源
        foreach (string name in abName)
        {
            GetLuaEnv().DoString("require '" + name + "'");
        }
    }

    private void Init()
    {
        Instantiate(Map);

        var player = Instantiate(Player);
        Camera.main.transform.SetParent(player.transform);
    }

    private byte[] MyLoader(ref string fileName)
    {
        // string absPath = "Assets/jiaoben/" + filePath + ".lua.txt";

        //AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/update");
        string ABPath = Application.streamingAssetsPath;

        string[] AB = Directory.GetFiles(Application.streamingAssetsPath, "update*.ab");

        if (AB == null)
        {
            return System.Text.Encoding.UTF8.GetBytes("CS.UnityEngine.Debug.Log('没有LuaAB包文件')");
        }

        string path = Path.Combine(ABPath, Path.GetFileName(AB[0]));
        AssetBundle ab = AssetBundle.LoadFromFile(path);

        ////加载主包
        //AssetBundle abMain = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "StandaloneWindows");
        ////加载主包中的固定文件
        //AssetBundleManifest abManifest = abMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        ////从固定文件中得到依赖信息
        //string[] strs = abManifest.GetAllDependencies("update");
        ////得到依赖包的名字并加载
        //foreach (var s in strs)
        //{
        //    AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + s);
        //}

        TextAsset text = ab.LoadAsset(fileName) as TextAsset;

        ab.Unload(false);
        //abMain.Unload(false);
        return System.Text.Encoding.UTF8.GetBytes(text.text);
        //return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(absPath));
    }
}
