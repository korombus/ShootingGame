using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DirectionData {

    public class BezierBase
    {
        public Vector3 endPoint { private set; get; }
        public Vector3 CtlPoint1 { private set; get; }
        public Vector3 CtlPoint2 { private set; get; }
        public float bezTime { private set; get; }
        public float bezWarmTime { private set; get; }

        public BezierBase(Vector3 i_end, Vector3 i_ctlPos1, Vector3 i_ctlPos2, float i_beztime, float i_uptime) {
            this.endPoint = i_end;
            this.CtlPoint1 = i_ctlPos1;
            this.CtlPoint2 = i_ctlPos2;
            this.bezTime = i_beztime;
            this.bezWarmTime = i_uptime;
        }
    }

    public static Dictionary<int, BezierBase> bezierDirectionData = new Dictionary<int, BezierBase>();

    public static void CreateBezierDirectionData(string bezierData = null) {
        string bezRoot = "DirectionData/BezierData";
        if (bezierData == null) {
            if (bezierDirectionData.Count > 0) return;
            TextAsset data = Resources.Load<TextAsset>(bezRoot);
            if (data != null) {
                List<string> lineData = CommonUtil.InitLineData(data.text.Split(new char[] { '\n', '\r' }).ToList<string>());
                foreach (string line in lineData) {
                    string[] bez = line.Split(new char[] { ',' });
                    CommonUtil.Unset(ref bez);
                    bezierDirectionData.Add(int.Parse(bez[0]), new BezierBase(new Vector2(float.Parse(bez[1]), float.Parse(bez[2])), new Vector2(float.Parse(bez[3]), float.Parse(bez[4])), new Vector2(float.Parse(bez[5]), float.Parse(bez[6])), float.Parse(bez[7]), float.Parse(bez[8])));
                }
            }
        }
        else {
            bezierDirectionData.Clear();
            List<string> lineData = CommonUtil.InitLineData(bezierData.Split(new char[] { '\n', '\r' }).ToList<string>());
            foreach (string line in lineData) {
                string[] bez = line.Split(new char[] { ',' });
                CommonUtil.Unset(ref bez);
                bezierDirectionData.Add(int.Parse(bez[0]), new BezierBase(new Vector2(float.Parse(bez[1]), float.Parse(bez[2])), new Vector2(float.Parse(bez[3]), float.Parse(bez[4])), new Vector2(float.Parse(bez[5]), float.Parse(bez[6])), float.Parse(bez[7]), float.Parse(bez[8])));
            }
        }
    }
}
