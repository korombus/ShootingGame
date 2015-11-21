using UnityEngine;
using System.Collections;

public class CommonResource {

    // 内部変数
    private Object data         = null; //!< データ

    private IEnumerator create(UnityEngine.MonoBehaviour MBeh, string path, System.Type type, Transform parent = null) {
        IEnumerator load = resourceLoad(path, type, parent);
        while (load.MoveNext()) { yield return load.Current; }
    }

    private IEnumerator resourceLoad(string path, System.Type type, Transform parent) {
        if (parent == null) {
            data = Resources.Load(path, type);
        }
        else {
            data = CommonUtil.PrefabInstance(path, parent);
        }
        while (data == null) { yield return null; }
    }

    /*******************************************************/
    /* !@brief      : リソースロード
     *  @param[in]  : path ->  リソースパス
     *  @param[in]  : type ->  リソースタイプ
     *  @retval     : Object
     *  @date       : 2014/05/02
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public object Load(UnityEngine.MonoBehaviour MBeh, string path, System.Type type) {
        data = null;
        MBeh.StartCoroutine(create(MBeh, path, type));
        return data;
    }

    /*******************************************************/
    /* !@brief      : プレハブからオブジェクトをインスタンス
     *  @param[in]  : path      ->  リソースパス
     *  @param[in]  : type      ->  リソースタイプ
     *  @param[in]  : parent    ->  親オブジェクトのTransform
     *  @retval     : Object
     *  @date       : 2014/05/02
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public GameObject PrefabInstanceObj(UnityEngine.MonoBehaviour MBeh, string path, Transform parent, string objName) {
        MBeh.StartCoroutine(create(MBeh, path, typeof(GameObject), parent));
        GameObject obj = data as GameObject;
        obj.name = objName;
        return obj;
    }
}
