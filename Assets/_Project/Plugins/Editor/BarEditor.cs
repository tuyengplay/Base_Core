using GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BarEditor
{
    [MenuItem("Extension/Sprite Manager", priority = 0)]
    private static void OpenSpriteManager()
    {
        Selection.activeObject = SpriteManager.Ins;
    }
}
