using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TextureDebug : MonoBehaviour {

    private enum STAGE_BG_POS
    {
        FRONT,
        MIDDLE,
        SEMIMIDDLE,
        BACK,
    }

    const string BG_PATH = @"\BG";
    const string ENEMY_PATH = @"\ENEMY";
    const string SPEED_DATA_FILE_NAME = @"\speed_data.txt";

    const string TEXTURE = "Textures";

    public string dirRoot;
    public string bgRoot;
    public string enemyRoot;
    public string nowSelectStageName = "stage1";
    public string speedFileDataStr = "";

    public bool bgListApper = false;
    public bool enemyListApper = false;

    public float speedLimit = 3f;

    Dictionary<string, Dictionary<STAGE_BG_POS, string>> bgTexDic = new Dictionary<string, Dictionary<STAGE_BG_POS, string>>();
    List<string> backgroundList = new List<string>();
    List<string> enemyTexList = new List<string>();

    public List<Renderer> bgMaterial = new List<Renderer>();
    public List<BackScroll> bgscrollVal = new List<BackScroll>();

    public RawImage enemyImage;

    void Start() {
        dirRoot = Directory.GetCurrentDirectory() + @"\" + TEXTURE;
        
        bgRoot = dirRoot + BG_PATH;
        enemyRoot = dirRoot + ENEMY_PATH;
        
        if (!Directory.Exists(dirRoot)) {
            Directory.CreateDirectory(dirRoot);

            if (!Directory.Exists(bgRoot)) {
                Directory.CreateDirectory(bgRoot);
            }

            if (!Directory.Exists(enemyRoot)) {
                Directory.CreateDirectory(enemyRoot);
            }
        }

        FileInfo file = new FileInfo(dirRoot + SPEED_DATA_FILE_NAME);
        using (FileStream fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite))
        using (StreamReader sr = new StreamReader(fs)) {
            speedFileDataStr = sr.ReadToEnd();
        }
        

        setBGTextureList();
        setEnemyTextureList();
    }
    Vector2 vecBg = Vector2.zero;
    void OnGUI() {
        if (GUI.Button(new Rect(30, 50, 120, 100), "背景再読み込み")) {
            setBGTextureList();
        }

        if (GUI.Button(new Rect(30, 400, 120, 120), "背景速度吐き出し")) {
            OutputSpeedData();
        }

        if (GUI.Button(new Rect(30, 200, 120, 100), "敵再読み込み")) {
            setEnemyTextureList();
        }

        if (!bgListApper && !enemyListApper) {
            if (GUI.Button(new Rect(1110, 50, 120, 100), "背景生成")) {
                bgListApper = true;
            }

            if (GUI.Button(new Rect(1110, 200, 120, 100), "敵生成")) {
                enemyListApper = true;
            }
        }


        if (bgListApper) {
            vecBg = GUI.BeginScrollView(new Rect(300, 10, 500, Screen.height - 10), vecBg, new Rect(300, 10, 250, backgroundList.Count * 100));
            foreach (var bg in backgroundList.Select((value, index) => new {index, value})) {
                if (GUI.Button(new Rect(300, 40 * (bg.index + 1), bg.value.Length * 20, 30), bg.value)) {
                    nowSelectStageName = bg.value;
                    foreach (KeyValuePair<STAGE_BG_POS, string> bgImg in bgTexDic[bg.value]) {
                        Texture2D tex = new Texture2D(0, 0);
                        tex.LoadImage(loadBytes(bgImg.Value));
                        tex.filterMode = FilterMode.Point;
                        tex.wrapMode = TextureWrapMode.Repeat;
                        //tex.alphaIsTransparency = true;
                        tex.texelSize.Set(512, 512);
                        tex.Apply();
                        switch (bgImg.Key) {
                            case STAGE_BG_POS.FRONT:
                                bgscrollVal[0].SetTexture(tex);
                                break;
                            case STAGE_BG_POS.MIDDLE:
                                bgscrollVal[1].SetTexture(tex);
                                break;
                            case STAGE_BG_POS.SEMIMIDDLE:
                                bgscrollVal[2].SetTexture(tex);
                                break;
                            case STAGE_BG_POS.BACK:
                                bgscrollVal[3].SetTexture(tex);
                                break;
                        }
                    }
                    bgListApper = false;
                }
            }
            GUI.EndScrollView();
        }

        if (enemyListApper) {
            vecBg = GUI.BeginScrollView(new Rect(300, 10, 500, Screen.height - 10), vecBg, new Rect(300, 10, 250, enemyTexList.Count * 100));
            foreach (var bg in enemyTexList.Select((value, index) => new { index, value })) {
                if (GUI.Button(new Rect(300, 40 * (bg.index + 1), bg.value.Substring(bg.value.LastIndexOf("\\") + 1).Length * 20, 30), bg.value.Substring(bg.value.LastIndexOf("\\") + 1))) {
                    Texture2D tex = new Texture2D(0, 0);
                    tex.LoadImage(loadBytes(bg.value));
                    enemyImage.texture = tex;
                    enemyImage.transform.localScale = new Vector3((float)tex.width / 100, (float)tex.width / 100);
                    enemyListApper = false;
                }
            }
            GUI.EndScrollView();
        }

        GUI.TextField(new Rect(1110, 320, 140, 20), "front");
        GUI.TextArea(new Rect(1130, 320, 80, 20), bgscrollVal[0].speed.ToString());
        bgscrollVal[0].SetSpeed(GUI.HorizontalSlider(new Rect(1110, 350, 120, 50), bgscrollVal[0].speed, 0f, speedLimit));

        GUI.TextField(new Rect(1110, 370, 140, 20), "middle");
        GUI.TextArea(new Rect(1130, 370, 80, 20), bgscrollVal[1].speed.ToString());
        bgscrollVal[1].SetSpeed(GUI.HorizontalSlider(new Rect(1110, 400, 120, 50), bgscrollVal[1].speed, 0f, speedLimit));

        GUI.TextField(new Rect(1110, 420, 140, 20), "semiddle");
        GUI.TextArea(new Rect(1130, 420, 80, 20), bgscrollVal[2].speed.ToString());
        bgscrollVal[2].SetSpeed(GUI.HorizontalSlider(new Rect(1110, 450, 120, 50), bgscrollVal[2].speed, 0f, speedLimit));

        GUI.TextField(new Rect(1110, 470, 140, 20), "back");
        GUI.TextArea(new Rect(1130, 470, 80, 20), bgscrollVal[3].speed.ToString());
        bgscrollVal[3].SetSpeed(GUI.HorizontalSlider(new Rect(1110, 500, 120, 50), bgscrollVal[3].speed, 0f, speedLimit));

        GUI.TextField(new Rect(1110, 520, 200, 20), "enemy_scale");
        float scale = GUI.HorizontalSlider(new Rect(1110, 550, 120, 50), enemyImage.transform.localScale.x, 0.1f, 6f);
        GUI.TextArea(new Rect(1200, 520, 80, 20), scale.ToString());

        GUI.TextField(new Rect(1110, 570, 200, 20), "speed_limit");
        speedLimit = GUI.HorizontalSlider(new Rect(1110, 600, 120, 50), speedLimit, 0f, 50f);
        GUI.TextArea(new Rect(1200, 570, 80, 20), speedLimit.ToString());

        enemyImage.transform.localScale = new Vector3(scale, scale, 0);
    }

    void setBGTextureList() {
        bgTexDic.Clear();
        backgroundList.Clear();

        DirectoryInfo dir = new DirectoryInfo(bgRoot);
        foreach (FileInfo file in dir.GetFiles()) {
            string[] name = file.Name.Split(new char[] { '_' });
            if (bgTexDic.ContainsKey(name[1])) {
                bgTexDic[name[1]].Add((STAGE_BG_POS)Enum.Parse(typeof(STAGE_BG_POS), name[2].Replace(".png", "").ToUpper(), true), file.FullName);
            }
            else {
                bgTexDic.Add(name[1], new Dictionary<STAGE_BG_POS, string>() { { (STAGE_BG_POS)Enum.Parse(typeof(STAGE_BG_POS), name[2].Replace(".png", "").ToUpper(), true), file.FullName } });
                backgroundList.Add(name[1]);
            }
        }
    }

    void setEnemyTextureList() {
        setList(enemyTexList, enemyRoot);
    }

    void setList(List<string> texList, string root) {
        texList.Clear();

        DirectoryInfo dir = new DirectoryInfo(root);
        foreach (FileInfo file in dir.GetFiles()) {
            texList.Add(file.FullName);
        }
    }

    byte[] loadBytes(string path) {
        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryReader bin = new BinaryReader(fs);
        byte[] result = bin.ReadBytes((int)bin.BaseStream.Length);
        bin.Close();
        return result;
    }

    void OutputSpeedData() {
        FileInfo file = new FileInfo(dirRoot + SPEED_DATA_FILE_NAME);
        speedFileDataStr += nowSelectStageName + "," + bgscrollVal[0].speed.ToString() + "," + bgscrollVal[1].speed.ToString() + "," + bgscrollVal[2].speed.ToString() + "," + bgscrollVal[3].speed.ToString() + Environment.NewLine;
        using (FileStream fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
            using (StreamWriter sw = new StreamWriter(fs)) {
                sw.Write(speedFileDataStr);
            }
        }
    }
}
