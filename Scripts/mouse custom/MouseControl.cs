/* file: MouseControl.c */

// for Unity commands
using UnityEngine;

// not sure if we even need this
using System.Collections;

// for DllImport command
using System.Runtime.InteropServices;

public class MouseControl : MonoBehaviour
{

    [DllImport("user32.dll")]
    static extern bool ClipCursor(ref RECT lpRect);
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    void Start()
    {
        RECT cursorLimits;
        cursorLimits.Left = 0;
        cursorLimits.Top = 0;
        cursorLimits.Right = Screen.width - 1;
        cursorLimits.Bottom = Screen.height - 1;
        ClipCursor(ref cursorLimits);
    }
}