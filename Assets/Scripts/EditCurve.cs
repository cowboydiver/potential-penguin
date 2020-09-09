#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Vectrosity;

public class EditCurve : MonoBehaviour
{
    #region Variables
    public Texture lineTexture;
    public Color lineColor = Color.white;
    public Texture dottedLineTexture;
    public Color dottedLineColor = Color.yellow;
    public Vector2 BoundingBoxCenter;
    public Vector2 BoundingBoxSize;

    public GameObject anchorPoint;
    public GameObject controlPoint;
    public GameObject EditorBGPrefab;

    bool initialized;
    int numberOfCurves = 1;
    int segments;

    VectorLine line;
    VectorLine controlLine;
    VectorLine boundingBoxLine;

    TextAsset curveDataFile;
    LandscapeCurveData curveData;

    List<CurvePointControl> curvePoints;
    List<Vector2> boundingBoxPoints;

    public static EditCurve use;
    public static Camera cam;

    Transform curvePointsParent;

    int pointIndex = 0;
    GameObject anchorObject;
    int oldWidth;
    bool useDottedLine = false;
    bool oldDottedLineSetting = false;
    int oldSegments;
    bool listPoints;
    #endregion
    #region Mono
    void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (Screen.width != oldWidth)
        {
            oldWidth = Screen.width;
            ChangeResolution();
        }
    }

    void OnGUI()
    {
        if(!initialized)
        {
            return;
        }

        if (GUI.Button(new Rect(20, 20, 100, 30), "Add Point"))
        {
            AddPoint();
        }
        else if (GUI.Button(new Rect(130, 20, 100, 30), "Remove Point"))
        {
            RemovePoint();
        }

        if (GUI.Button(new Rect(20, 70, 100, 30), "Save Level"))
        {
            SaveCurveDataFile();
        }
        /*
        GUI.Label(new Rect(20, 59, 200, 30), "Curve resolution: " + segments);
        segments = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(20, 80, 150, 30), segments, 3, 60));
        if (oldSegments != segments)
        {
            oldSegments = segments;
            ChangeSegments();
        }

        useDottedLine = GUI.Toggle(new Rect(20, 105, 80, 20), useDottedLine, " Dotted line");
        if (oldDottedLineSetting != useDottedLine)
        {
            oldDottedLineSetting = useDottedLine;
            SetLine();
            line.Draw();
        }

        GUILayout.BeginArea(new Rect(20, 150, 150, 800));
        if (GUILayout.Button(listPoints ? "Hide points" : "List points", GUILayout.Width(100)))
        {
            listPoints = !listPoints;
        }
        if (listPoints)
        {
            var idx = 0;
            for (var i = 0; i < controlLine.points2.Count; i += 2)
            {
                GUILayout.Label("Anchor " + idx + ": (" + (int)(controlLine.points2[i].x) + ", " + (int)(controlLine.points2[i].y) + ")");
                GUILayout.Label("Control " + idx++ + ": (" + (int)(controlLine.points2[i + 1].x) + ", " + (int)(controlLine.points2[i + 1].y) + ")");
            }
        }
        GUILayout.EndArea();
        */
    }

    void OnValidate()
    {
        if (!initialized)
        {
            return;
        }

        CheckBoundingBoxChanged();
    }
    #endregion
    #region Public Methods
    public void Init(TextAsset curveDataFile, int segments)
    {
        if(initialized)
        {
            return;
        }
        initialized = true;

        this.curveDataFile = curveDataFile;
        use = this; // Reference to this script, so FindObjectOfType etc. are not needed
        cam = Camera.main;
        this.segments = segments;
        oldSegments = segments;
        boundingBoxPoints = new List<Vector2>(new Vector2[5]);
        curvePointsParent = new GameObject("CurvePoints").transform;

        Instantiate(EditorBGPrefab, curvePointsParent);

        LoadCurveDataFile();

        /*
        // Set up initial curve points (also used for drawing the green lines that connect control points to anchor points)
        curveData.LinePoints = new List<Vector2>();
        curveData.LinePoints.Add(new Vector2(Screen.width * .25f, Screen.height * .25f));
        curveData.LinePoints.Add(new Vector2(Screen.width * .125f, Screen.height * .5f));
        curveData.LinePoints.Add(new Vector2(Screen.width - Screen.width * .25f, Screen.height - Screen.height * .25f));
        curveData.LinePoints.Add(new Vector2(Screen.width - Screen.width * .125f, Screen.height * .5f));

        // Make the control lines
        controlLine = new VectorLine("Control Line", curveData.LinePoints, 2.0f);
        controlLine.color = new Color(0.0f, .75f, .1f, .6f);
        controlLine.Draw();

        // Make the line object for the curve
        line = new VectorLine("Curve", new List<Vector2>(segments + 1), lineTexture, 5.0f, LineType.Continuous, Joins.Weld);

        // Create a curve in the VectorLine object
        line.MakeCurve(curveData.LinePoints[0], curveData.LinePoints[1], curveData.LinePoints[2], curveData.LinePoints[3], segments);
        line.Draw();

        // Make the GUITexture objects for anchor and control points (two anchor points and two control points)
        AddControlObjects();
        AddControlObjects();
        */
    }

    public void UpdateLine(int objectNumber, Vector2 pos, GameObject go)
    {
        var oldPos = curveData.LinePoints[objectNumber]; // Get previous position, so we can make the control point move with the anchor point
        curveData.LinePoints[objectNumber] = pos;
        int curveNumber = objectNumber / 4;
        int curveIndex = curveNumber * 4;

        line.MakeCurve(curveData.LinePoints[curveIndex], curveData.LinePoints[curveIndex + 1], curveData.LinePoints[curveIndex + 2], curveData.LinePoints[curveIndex + 3], segments, curveNumber * (segments + 1));

        // If it's an anchor point...
        if (objectNumber % 2 == 0)
        {
            // Move control point also
            curveData.LinePoints[objectNumber + 1] += pos - oldPos;
            go.GetComponent<CurvePointControl>().controlObject.transform.position = cam.ScreenToViewportPoint(curveData.LinePoints[objectNumber + 1]);

            // If it's not an end anchor point, move the next anchor/control points as well, and update the next curve
            if (objectNumber > 0 && objectNumber < curveData.LinePoints.Count - 2)
            {
                curveData.LinePoints[objectNumber + 2] = pos;
                curveData.LinePoints[objectNumber + 3] += pos - oldPos;
                go.GetComponent<CurvePointControl>().controlObject2.transform.position = cam.ScreenToViewportPoint(curveData.LinePoints[objectNumber + 3]);
                line.MakeCurve(curveData.LinePoints[curveIndex + 4], curveData.LinePoints[curveIndex + 5], curveData.LinePoints[curveIndex + 6], curveData.LinePoints[curveIndex + 7], segments, (curveNumber + 1) * (segments + 1));
            }
        }

        line.Draw();
        controlLine.Draw();
    }
    #endregion
    #region Private Methods
    void SetLine()
    {
        if (useDottedLine)
        {
            line.texture = dottedLineTexture;
            line.color = dottedLineColor;
            line.lineWidth = 8.0f;
            line.textureScale = 1.0f;
        }
        else
        {
            line.texture = lineTexture;
            line.color = lineColor;
            line.lineWidth = 5.0f;
            line.textureScale = 0.0f;
        }
    }

    void AddControlObjects(int curveIndex, CurvePointControl anchorObject2)
    {
        CurvePointControl curvePoint;

        if (anchorObject2 == null)
        {
            anchorObject = Instantiate(anchorPoint, cam.ScreenToViewportPoint(curveData.LinePoints[curveIndex]), Quaternion.identity, curvePointsParent);
            curvePoint = anchorObject.GetComponent<CurvePointControl>();
            curvePoint.objectNumber = curveIndex;
            curvePoints.Add(curvePoint);
        }
        
        GameObject controlObject = Instantiate(controlPoint, cam.ScreenToViewportPoint(curveData.LinePoints[curveIndex + 1]), Quaternion.identity, curvePointsParent);
        curvePoint = controlObject.GetComponent<CurvePointControl>();
        curvePoint.objectNumber = curveIndex + 1;
        curvePoint.ParentAnchor = curvePoints[curvePoints.Count - (anchorObject2 == null ? 1 : 2)];        
        curvePoints.Add(curvePoint);

        if (anchorObject2 == null)
        {
            // Make the anchor object have a reference to the control object, so they can move together
            // Having control objects be children of anchor objects would be easier, but parent/child doesn't really work with GUITextures
            anchorObject.GetComponent<CurvePointControl>().controlObject = controlObject;
        }
        else
        {
            anchorObject2.GetComponent<CurvePointControl>().controlObject2 = controlObject;
        }
    }

    void AddPoint()
    {
        // Don't do anything if adding a new point would exceed the max number of vertices per mesh
        if (line.points2.Count + controlLine.points2.Count + segments + 4 > 16383) return;

        pointIndex = numberOfCurves * 4;

        // Make the first anchor and control points of the new curve be the same as the second anchor/control points of the previous curve
        curveData.LinePoints.Add(controlLine.points2[pointIndex - 2]);
        curveData.LinePoints.Add(controlLine.points2[pointIndex - 1]);

        // Make the second anchor/control points of the new curve be offset a little ways from the first
        var offset = (curveData.LinePoints[pointIndex - 2] - curveData.LinePoints[pointIndex - 4]) * .25f;
        curveData.LinePoints.Add(curveData.LinePoints[pointIndex - 2] + offset);
        curveData.LinePoints.Add(curveData.LinePoints[pointIndex - 1] + offset);

        // If that made the new anchor point go off the screen, offset them the opposite way
        if (curveData.LinePoints[pointIndex + 2].x > Screen.width || curveData.LinePoints[pointIndex + 2].y > Screen.height ||
            curveData.LinePoints[pointIndex + 2].x < 0 || curveData.LinePoints[pointIndex + 2].y < 0)
        {
            curveData.LinePoints[pointIndex + 2] = curveData.LinePoints[pointIndex - 2] - offset;
            curveData.LinePoints[pointIndex + 3] = curveData.LinePoints[pointIndex - 1] - offset;
        }

        // For the next control point, make the initial position offset from the anchor point the opposite way as the second control point in the curve
        var controlPointPos = curveData.LinePoints[pointIndex - 1] + (curveData.LinePoints[pointIndex] - curveData.LinePoints[pointIndex - 1]) * 2;
        //pointIndex++;   // Skip the next anchor point, since we want the second anchor point of one curve and the first anchor point of the next curve
                        // to move together (this is handled in UpdateLine)
        curveData.LinePoints[pointIndex + 1] = controlPointPos;

        // Make another control point
        //GameObject controlObject = Instantiate(controlPoint, cam.ScreenToViewportPoint(controlPointPos), Quaternion.identity);
        //controlObject.GetComponent<CurvePointControl>().objectNumber = pointIndex++;

        // For the last anchor object that was made, make a reference to this control point so they can move together
        //anchorObject.GetComponent<CurvePointControl>().controlObject2 = controlObject;
        // Then make another anchor/control point group
        CurvePointControl lastControlPoint = curvePoints[curvePoints.Count - 2];

        AddControlObjects(pointIndex, lastControlPoint);
        AddControlObjects(pointIndex + 2, null);

        numberOfCurves++;

        // Update the control lines
        controlLine.Draw();

        // Update the curve with the new points
        line.Resize((segments + 1) * numberOfCurves);

        //UpdateLine(pointIndex, lastControlPoint.transform.position, lastControlPoint.gameObject);
        line.MakeCurve(curveData.LinePoints[pointIndex], curveData.LinePoints[pointIndex + 1], curveData.LinePoints[pointIndex + 2], curveData.LinePoints[pointIndex + 3], segments, (segments + 1) * (numberOfCurves - 1));
        line.Draw();
    }

    void RemovePoint()
    {
        curveData.LinePoints.RemoveRange(numberOfCurves * 4 - 4, 4);

        int curvePointCount = curvePoints.Count - 1;

        for (int i = 0; i < 3; i++)
        {
            Destroy(curvePoints[curvePointCount - i].gameObject);
            curvePoints.RemoveAt(curvePointCount - i);
        }

        numberOfCurves--;

        controlLine.Draw();

        line.Resize((segments + 1) * numberOfCurves);
        line.Draw();
    }

    void ChangeSegments()
    {
        // Don't do anything if the requested segments would make the curve exceed the max number of vertices per mesh
        if (segments * 4 * numberOfCurves > 65534) return;
        line.Resize((segments + 1) * numberOfCurves);

        RedrawLines();
    }

    void RedrawLines()
    {
        //Update all line points
        for (var i = 0; i < numberOfCurves; i++)
        {
            line.MakeCurve(controlLine.points2[i * 4], controlLine.points2[i * 4 + 1], controlLine.points2[i * 4 + 2], controlLine.points2[i * 4 + 3], segments, (segments + 1) * i);
        }

        //Update all curve point object positions
        for (int i = 0; i < curvePoints.Count; i++)
        {
            curvePoints[i].transform.position = cam.ScreenToViewportPoint(curveData.LinePoints[curvePoints[i].objectNumber]);
        }

        controlLine.Draw();
        line.Draw();
    }

    void ChangeResolution()
    {
        //float oldRatio = curveData.ScreenSize.x / curveData.ScreenSize.y;
        //float newRatio = (float)Screen.width / (float)Screen.height;

        float ratioScale = Screen.width / curveData.ScreenSize.x;

        //Reposition all line points according to new resolution
        for (int i = 0; i < curveData.LinePoints.Count; i++)
        {
            Vector2 point = curveData.LinePoints[i];
            point *= ratioScale;

            curveData.LinePoints[i] = point;
        }

        BoundingBoxCenter *= ratioScale;
        BoundingBoxSize *= ratioScale;
        CheckBoundingBoxChanged();

        RedrawLines();

        //Set new screen resolution in curve data
        curveData.ScreenSize = new Vector2(Screen.width, Screen.height);
    }

    void CheckBoundingBoxChanged()
    {
        if (curveData.BoundingCenter != BoundingBoxCenter || curveData.BoundingSize != BoundingBoxSize)
        {
            curveData.BoundingCenter = BoundingBoxCenter;
            curveData.BoundingSize = BoundingBoxSize;

            UpdateBoundingBoxLine();
        }
    }

    void UpdateBoundingBoxLine()
    {
        boundingBoxPoints[0] = boundingBoxPoints[4] = new Vector2(BoundingBoxCenter.x - BoundingBoxSize.x * 0.5f, BoundingBoxCenter.y - BoundingBoxSize.y * 0.5f);
        boundingBoxPoints[1] = new Vector2(BoundingBoxCenter.x + BoundingBoxSize.x * 0.5f, BoundingBoxCenter.y - BoundingBoxSize.y * 0.5f);
        boundingBoxPoints[2] = new Vector2(BoundingBoxCenter.x + BoundingBoxSize.x * 0.5f, BoundingBoxCenter.y + BoundingBoxSize.y * 0.5f);
        boundingBoxPoints[3] = new Vector2(BoundingBoxCenter.x - BoundingBoxSize.x * 0.5f, BoundingBoxCenter.y + BoundingBoxSize.y * 0.5f);

        boundingBoxLine.Draw();
    }

    void SaveCurveDataFile()
    {
        curveData.ScreenSize = new Vector2(Screen.width, Screen.height);

        string json = JsonUtility.ToJson(curveData);
        string path = Application.dataPath;
        path = Application.dataPath.Substring(0, path.LastIndexOf('/') + 1) + AssetDatabase.GetAssetPath(curveDataFile);

        System.IO.File.WriteAllText(path, json);

        AssetDatabase.Refresh();
    }

    void LoadCurveDataFile()
    {
        string path = Application.dataPath;
        path = Application.dataPath.Substring(0, path.LastIndexOf('/') + 1) + AssetDatabase.GetAssetPath(curveDataFile);

        string json = System.IO.File.ReadAllText(path);
        curveData = JsonUtility.FromJson<LandscapeCurveData>(json);

        if(curveData.LinePoints == null)
        {
            curveData.LinePoints = new List<Vector2>();
        }

        //Create control lines
        controlLine = new VectorLine("Control Line", curveData.LinePoints, 2.0f);
        controlLine.color = new Color(0.0f, .75f, .1f, .6f);
        controlLine.Draw();

        //Make the line object for the curve
        oldWidth = Mathf.RoundToInt(curveData.ScreenSize.x);
        numberOfCurves = curveData.LinePoints.Count / 4;
        line = new VectorLine("Curve", new List<Vector2>((segments + 1) * numberOfCurves), lineTexture, 5.0f, LineType.Continuous, Joins.Weld);
        curvePoints = new List<CurvePointControl>();

        for (int i = 0; i < numberOfCurves; i++)
        {
            // Create a curve in the VectorLine object
            line.MakeCurve(curveData.LinePoints[i * 4 + 0], curveData.LinePoints[i * 4 + 1], curveData.LinePoints[i * 4 + 2], curveData.LinePoints[i * 4 + 3], segments, (segments + 1) * i);

            // Make the GUITexture objects for anchor and control points (two anchor points and two control points)
            AddControlObjects(i * 4, (i == 0 ? null : curvePoints[curvePoints.Count - 2]));
            AddControlObjects(i * 4 + 2, null);
        }
        line.Draw();

        //Create bounding box line
        BoundingBoxCenter = curveData.BoundingCenter;
        BoundingBoxSize = curveData.BoundingSize;

        boundingBoxLine = new VectorLine("Bounding Box", boundingBoxPoints, 2.0f, LineType.Continuous, Joins.Weld);
        boundingBoxLine.color = Color.red;

        UpdateBoundingBoxLine();
    }
    #endregion
}
#endif