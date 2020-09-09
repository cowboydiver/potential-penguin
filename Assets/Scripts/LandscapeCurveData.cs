using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct LandscapeCurveData
{
    public Vector2 ScreenSize;
    public Vector2 BoundingCenter;
    public Vector2 BoundingSize;
    public List<Vector2> LinePoints;
}