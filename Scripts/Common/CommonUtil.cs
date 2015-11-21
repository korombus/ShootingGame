using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class CommonUtil {

    /// <summary>
    /// プレハブインスタンス
    /// </summary>
    /// <param name="objName">オブジェクト名</param>
    /// <param name="objPath">オブジェクトパス</param>
    /// <param name="parent">親オブジェクト</param>
    /// <returns>GameObject</returns>
    public static GameObject PrefabInstance(string objName, string objPath, Transform parent)
    {
        GameObject obj = PrefabInstance(objPath, parent);
        obj.name = objName;
        return obj;
    }

    /// <summary>
    /// プレハブインスタンス
    /// </summary>
    /// <param name="objPath">オブジェクトパス</param>
    /// <param name="parent">親Transform</param>
    /// <returns>GameObject</returns>
    public static GameObject PrefabInstance(string objPath, Transform parent = null)
    {
        if (objPath == null) {
            return null;
        }
        GameObject obj = GameObject.Instantiate(Resources.Load(objPath)) as GameObject;
        if (obj != null && parent != null) {
            obj.transform.parent = parent;
        }
        return obj;
    }

    /// <summary>
    /// プレハブインスタンス
    /// </summary>
    /// <param name="prefab">プレハブ</param>
    /// <param name="parent">親Transform</param>
    /// <returns>GameObject</returns>
    public static GameObject PrefabInstance(GameObject prefab, Transform parent = null) {
        if (prefab == null) { return null; }
        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
        if (obj != null) {
            obj.name = prefab.name;
            if (parent != null) {
                obj.transform.parent = parent;
            }
        }
        return obj;
    }

    /// <summary>
    /// オブジェクトを検索する
    /// </summary>
    /// <param name="objectName">探したいオブジェクト名</param>
    /// <param name="trans">探したいオブジェクトが存在するTransform</param>
    /// <param name="count">探索個数</param>
    /// <param name="countNum">何番目を引っ掛けるか</param>
    /// <returns></returns>
    public static GameObject SearchObject(string objectName, Transform trans, int count, int countNum){
        if (trans.gameObject.name.Equals(objectName)) {
            if (count >= countNum) {
                return trans.gameObject;
            }
            count++;
        }
        foreach (Transform child in trans) {
            GameObject obj = SearchObject(objectName, child, count, countNum);
            if (obj != null) {
                return obj;
            }
        }
        return null;
    }


    /// <summary>
    /// 子オブジェクトを探索する
    /// </summary>
    /// <param name="objectName">探したいオブジェクト名</param>
    /// <param name="parent">探索したいオブジェクトが存在するTransform</param>
    /// <param name="countNum">何番目を引っ掛けるか</param>
	public static GameObject SearchObjectChild(string objectName, Transform parent = null, int countNum = 0){
        int count = 0;
        if(parent == null){
            return GameObject.Find(objectName);
        }
        foreach (Transform child in parent) {
            GameObject obj = SearchObject(objectName, child, count, countNum);
            if (obj != null) {
                return obj;
            }
        }
        return null;
    }

    /// <summary>
    /// GameObjectにTextureをセット
    /// </summary>
    /// <param name="texture">Texture2D</param>
    /// <param name="objName">オブジェクト名</param>
    /// <returns></returns>
    public static GameObject SetTexture(Texture2D texture, string objName) {
        GameObject obj = SearchObjectChild(objName);
        if (obj == null) {
            obj = PrefabInstance(objName, "Prefabs/Common/" + objName, null);
            if (obj == null) {
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(0, 0, 0);
                obj.transform.localScale = new Vector3(1024, 768, 0);
            }
        }
        return obj;
    }

    /// <summary>
    /// GameObjectにTextureをセット
    /// </summary>
    /// <param name="texture">Texture2D</param>
    /// <param name="obj">オブジェクト名</param>
    public static void SetTexture(Texture2D texture, GameObject obj) {
        if (obj == null) { return; }
        obj.GetComponent<Renderer>().material.mainTexture = texture;
    }

    /// <summary>
    /// ランダムで抽選した値をひとつ返す
    /// </summary>
    /// <typeparam name="T">使用する型</typeparam>
    /// <param name="value">使用する配列</param>
    /// <returns>配列の中の値の一つ</returns>
    public static T RandPickOne<T>(T[] value) {
        return value[CreateRandomNumber(value.Length)];
    }

    /// <summary>
    /// ランダムで抽選した値の配列を返す
    /// </summary>
    /// <typeparam name="T">使用する型</typeparam>
    /// <param name="value">使用する配列</param>
    /// <param name="len">戻す長さ</param>
    /// <returns>指定長の配列</returns>
    public static T[] RandPickUp<T>(T[] value, int len) {
        // 型の配列を指定長で生成
        T[] array = new T[len];
        for (int ii = 0; ii < len; ii++) {
            array[ii] = RandPickOne<T>(value);
        }
        return array;
    }

    /// <summary>
    /// ランダム数値を作成
    /// </summary>
    /// <param name="max">ランダムの最大値</param>
    /// <returns>ランダム値</returns>
    public static int CreateRandomNumber(int i_max) {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        int max = i_max;
        // BitConverterの仕様で４バイトの長さが必要なので4より最大が4より小さい場合は強制4にしてしまう
        if (max < 4) { max = 4; }
        byte[] rand = new byte[max];
        rng.GetBytes(rand);
        // 配列の最大長で乱数を生成
        int value = 1 + (int)((max - 1) * (BitConverter.ToUInt32(rand, 0) / ((double)uint.MaxValue + 1.0)));
        
        // 最大を強制4にしている影響で最大を超える場合が存在するので、そういう場合はUnity既存のRandomに頼る
        if (value > i_max) { value = UnityEngine.Random.Range(0, i_max); }
        return value;
    }

    /// <summary>
    /// 最低値と最大値の間からランダム数値を作成
    /// </summary>
    /// <param name="min">ランダムの最低値</param>
    /// <param name="max">ランダムの最大値</param>
    /// <returns>ランダム値</returns>
    public static int CreateRandomNumber(int min, int max) {
        int value = 0;
        // 同値だったら最低値を返す
        if (min == max) { return min; }
        if (min > max)  { return max; }

        // BitConverterの仕様で４バイトの長さが必要なので4より最大が4より小さい場合は強制4にしてしまう
        if (max < 4) { max = 4; }
        do {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] rand = new byte[max];
            rng.GetBytes(rand);
            // 配列の最大長で乱数を生成
            value = 1 + (int)((max - 1) * (BitConverter.ToUInt32(rand, 0) / ((double)uint.MaxValue + 1.0)));
        } while (value < min);

        // 最大を強制4にしている影響で最大を超える場合が存在するので、そういう場合はUnity既存のRandomに頼る
        if (value > max) { value = UnityEngine.Random.Range(min, max); }
        return value;
    }

    /// <summary>
    /// ランダムでTrueかFalseを返す
    /// </summary>
    /// <returns>bool</returns>
    public static bool GetRandomTorF() {
        return CreateRandomNumber(0, CreateRandomNumber(5)) == CreateRandomNumber(5);
    }

    /// <summary>
    /// 文字列をcharバイトコードに変換
    /// </summary>
    /// <param name="str">文字列</param>
    /// <returns>バイトコード配列</returns>
    public static byte[] GetStrToByte(string str) {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    /// <summary>
    /// バイトコードを文字列に変換
    /// </summary>
    /// <param name="bytes">バイトコード配列</param>
    /// <returns>文字列</returns>
    public static string GetByteToStr(byte[] bytes) {
        char[] chars = new char[bytes.Length / sizeof(char)];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    /// <summary>
    /// shift-JIS文字列をUTF-8文字列に変換
    /// </summary>
    /// <param name="shiftStrings"></param>
    /// <returns></returns>
    private string ConvertShiftJIStoUTF8(string shiftStrings) {
        System.Text.Encoding shiftJIS = System.Text.Encoding.GetEncoding("shift_jis");
        byte[] shiftBytes = shiftJIS.GetBytes(shiftStrings);

        System.Text.Encoding utf = System.Text.Encoding.UTF8;
        byte[] convStringData = System.Text.Encoding.Convert(shiftJIS, utf, shiftBytes);
        char[] convCharData = new char[utf.GetCharCount(convStringData, 0, convStringData.Length)];

        utf.GetChars(convStringData, 0, convStringData.Length, convCharData, 0);

        return new string(convCharData);
    }

    /// <summary>
    /// 文字列配列から空文字を削除
    /// </summary>
    /// <param name="arr"></param>
    public static void Unset(ref string[] arr) {
        List<string> list = new List<string>();
        foreach (string data in arr) {
            list.Add(data);
        }
        Unset(ref list);
        arr = list.ToArray();
    }

    /// <summary>
    /// 文字列から空文字を削除
    /// </summary>
    /// <param name="list">文字列リスト</param>
    public static void Unset(ref List<string> list) {
        list.RemoveAll(UnSet);
    }

    /// <summary>
    /// 文字列から空文字を削除するためのデータ
    /// </summary>
    /// <param name="s">文字列</param>
    /// <returns>bool</returns>
    private static bool UnSet(string s) {
        return s == "" || s == "\r" || s == "\n";
    }

    /// <summary>
    /// ジェネリック型リストを配列に変換
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <param name="list">ジェネリック型List</param>
    /// <returns>配列</returns>
    public static T[] ListToArray<T>(List<T> list) {
        T[] arr = new T[list.Count];
        foreach (var data in list.Select((value, index) => new { index, value})) {
            arr[data.index] = data.value;
        }
        return arr;
    }

    /// <summary>
    /// 配列をジェネリック型リストに変換
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <param name="arr">配列</param>
    /// <returns>ジェネリック型List</returns>
    public static List<T> ArrayToList<T>(T[] arr) {
        List<T> list = new List<T>();
        foreach (T data in arr) {
            list.Add(data);
        }
        return list;
    }

    /// <summary>
    /// 一行データの状態を正規化する
    /// </summary>
    /// <param name="data">全体を格納したデータ</param>
    /// <returns></returns>
    public static List<string> InitLineData(List<string> data) {
        CommonUtil.Unset(ref data);
        data.RemoveAll(pre => pre.Substring(0, 2) == "//");
        return data;
    }

    /// <summary>
    /// 配列をジェネリック型リストに変換
    /// </summary>
    /// <returns>List</returns>
    public static List<int> ArrayToListIntParse(string[] arr){
        List<int> list = new List<int>();
        foreach (string data in arr) {
            list.Add(int.Parse(data));
        }
        return list;
    }

    /// <summary>
    /// 配列をジェネリック型リストに変換
    /// </summary>
    /// <returns>List</returns>
    public static List<float> ArrayToListFloatParse(string[] arr) {
        List<float> list = new List<float>();
        foreach (string data in arr) {
            list.Add(float.Parse(data));
        }
        return list;
    }

    /// <summary>
    /// 配列をジェネリック型リストに変換
    /// </summary>
    /// <returns>List</returns>
    public static List<double> ArrayToListDoubleParse(string[] arr) {
        List<double> list = new List<double>();
        foreach (string data in arr) {
            list.Add(double.Parse(data));
        }
        return list;
    }

    /// <summary>
    /// 配列をジェネリック型リストに変換
    /// </summary>
    /// <returns>List</returns>
    public static List<long> ArrayToListLongParse(string[] arr) {
        List<long> list = new List<long>();
        foreach (string data in arr) {
            list.Add(long.Parse(data));
        }
        return list;
    }

    /// <summary>
    /// CSVから取得したTextAssetをListに変換
    /// </summary>
    /// <param name="data">CSVから取得したTextAsset</param>
    /// <returns>List</returns>
    public static List<string> GetCsvTextToList(TextAsset data) {
        List<string> list = CommonUtil.ArrayToList<string>(data.text.Split(new char[] { '\n', '\r' }));
        CommonUtil.Unset(ref list);
        list.RemoveAll(sl => sl.Substring(0, 2) == "//");
        return list;
    }
}
