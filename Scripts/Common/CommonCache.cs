using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CommonCache{

    private Hashtable m_Hash = new Hashtable();

    /*******************************************************/
    /* !@brief      : set
     *  @param[in]  : key   ->  キャッシュキー
     *  @param[in]  : value ->  キャッシュデータ
     *  @retval     : なし
     *  @date       : 2014/04/19
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public void set(string key, object value) {
        if (!m_Hash.ContainsKey(key)) {
            m_Hash.Add(key, value);
        }
    }

    /*******************************************************/
    /* !@brief      : get
     *  @param[in]  : key    ->  キャッシュキー
     *  @retval     : object ->  キャッシュデータ
     *  @date       : 2014/04/19
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public object get(string key) {
        if (m_Hash.Contains(key)) {
            return m_Hash[key];
        }
        return null;
    }

    /*******************************************************/
    /* !@brief      : allGet
     *  @param[in]  : なし
     *  @retval     : object[] ->  キャッシュデータ
     *  @date       : 2014/04/19
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public object[] allGet() {
        if (m_Hash.Count > 0) {
            List<object> data = new List<object>();
            foreach (string key in m_Hash.Keys) {
                data.Add(m_Hash[key]);
            }
            return data.ToArray();
       }
        return null;
    }

    /*******************************************************/
    /* !@brief      : clear
     *  @param[in]  : key   ->  キャッシュキー
     *  @retval     : なし
     *  @date       : 2014/04/19
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public void clear(string key = null) {
        if (key == null) { m_Hash.Clear(); }
        m_Hash.Remove(key);
    }

    /*******************************************************/
    /* !@brief      : setArr
     *  @param[in]  : key   ->  キャッシュキー
     *  @param[in]  : value ->  キャッシュデータ配列
     *  @retval     : なし
     *  @date       : 2014/05/05
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public void setArr(string key, object[] valueArr) {
        foreach (var data in valueArr.Select((valData, i) => new { valData, i })) {
            if (!m_Hash.ContainsKey(key + "_" + data.i)) {
                m_Hash.Add(key + "_" + data.i, data.valData);
            }
        }
    }

    /*******************************************************/
    /* !@brief      : getArr
     *  @param[in]  : key   ->  キャッシュキー
     *  @retval     : なし
     *  @date       : 2014/05/05
     *  @author     : コロソブス(korombus)
     *******************************************************/
    public object[] getArr(string key) {
        List<object> data = new List<object>();
        for (int ii = 0; ii < m_Hash.Count; ii++) {
            if (m_Hash.ContainsKey(key + "_" + ii)) {
                data.Add(m_Hash[key + "_" + ii]);
            }
            else { break; }
        }
        return data.ToArray();
    }
}
