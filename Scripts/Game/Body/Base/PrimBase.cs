using UnityEngine;
using System.Collections;

public class PrimBase : MonoBehaviour {

    protected TopWindow parent;

	public virtual void Start(){
        parent = CommonSys.GetSystem<TopWindow>();
    }
}
