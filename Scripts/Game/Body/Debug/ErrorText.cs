using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ErrorText : MonoBehaviour {

    public Text error;

    void SetText(string words) {
        error.text += words + Environment.NewLine;
    }

    void Remove() {
        error.text = "No Error";
    }
}
