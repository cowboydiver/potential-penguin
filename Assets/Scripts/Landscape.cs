using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

public class Landscape : MonoBehaviour
{
    #region Variables
    const double meshDepth = 2d;
    const float meshScaleFactor = 0.01f;

    public bool EditMode;
    public TextAsset CurveDataFile;
#if UNITY_EDITOR
    public GameObject EditCurvePrefab;
#endif
    public int SurfacePointCount = 100;
    public int LineSegments = 60;

    public int PotentialLowestControlPointIndex;

    public Material LevelMeshMaterial;
    public Material SurfaceLineMaterial;

    public float PotentialLowestValue { get; private set; }

    new Camera camera;
#if UNITY_EDITOR
    EditCurve editCurve;
#endif

    LandscapeCurveData curveData;
    VectorLine line;

    Penguin penguin;
    Vector3 previous, current, next;
    bool isSimulating;

    int curveCount;
    double[] meshTopXValues;
    double[] meshTopYValues;
    double[] meshBottomXValues;
    double[] meshBottomYValues;

    Vector2[] edgeColliderPoints;

    MeshFilter meshFilter;
    EdgeCollider2D edgeCollider;

    bool isShowingSurfaceLine;
    LineRenderer surfaceLineRenderer;
    int surfaceLineStartPoint;
    int surfaceLineEndPoint;

    float lineWorldScaleFactor;
    float lineWorldYOffset;
    Vector2 boundingCenter;
    Vector2 boundingSize;
    #endregion
    #region Mono
    void Awake()
    {
        Debug.Log("Start awake");
#if !UNITY_EDITOR
        EditMode = false;
#endif

        camera = FindObjectOfType<Camera>();
        penguin = FindObjectOfType<Penguin>();
#if UNITY_EDITOR
        editCurve = FindObjectOfType<EditCurve>();
        if(editCurve == null)
        {
            editCurve = Instantiate(EditCurvePrefab).GetComponent<EditCurve>();
        }

        editCurve.gameObject.SetActive(EditMode);
        if (EditMode)
        {
            editCurve.Init(CurveDataFile, LineSegments);
        }
        else

        {
            LoadCurveDataFile();
            CreateLevelMesh();

            //HACK: Debug bounding box
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "LandscapeBoundingBox";
            go.transform.position = new Vector3(0f, boundingSize.y * 0.5f, (float)meshDepth * 0.5f);
            go.transform.localScale = new Vector3(boundingSize.x, boundingSize.y, (float)meshDepth);
            go.SetActive(false);
        }
#endif

        if (!EditMode)
        {
            LoadCurveDataFile();
            CreateLevelMesh();

            //HACK: Debug bounding box
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "LandscapeBoundingBox";
            go.transform.position = new Vector3(0f, boundingSize.y * 0.5f, (float)meshDepth * 0.5f);
            go.transform.localScale = new Vector3(boundingSize.x, boundingSize.y, (float)meshDepth);
            go.SetActive(false);
        }
        Debug.Log("End awake");
    }

    void Start()
    {
        Debug.Log("Does this show??");
        //HACK: To work when starting in level scene (load game scene and then load level scene)
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (FindObjectOfType<GameManager>() == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(Constants.MainMenu, UnityEngine.SceneManagement.LoadSceneMode.Single);
        
            UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode> action = null;
        
            action = (UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1) =>
            {
                UnityEngine.SceneManagement.SceneManager.sceneLoaded -= action;

                int level = int.Parse(currentSceneName.Substring(currentSceneName.Length - 3, 3));
                GameManager.Inst.LoadLevel(level);
                //UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        
                //CustomSceneManager.Inst.LoadSceneAsync(currentSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
                //{
                //    UIManager.Inst.ShowMenu(4, data);
                //});
            };
        
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += action;
        }
        else if (!EditMode)
        {
            StartSimulation();
        }
        else
        {
            var gui = FindObjectOfType<InGameGUI>();
            if(gui != null)
            {
                gui.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            ShowSurfaceLine(!isShowingSurfaceLine, 10, 90);

        if(!isSimulating)
        {
            return;
        }

        GetPoints(penguin.nextXValue, out previous, out current, out next);

        penguin.UpdatePenguinPosition(previous, current, next);
    }
    #endregion
    #region Public Methods
    public void LoadCurveDataFile()
    {
        string json = CurveDataFile.text;
        curveData = JsonUtility.FromJson<LandscapeCurveData>(json);
        
        if (curveData.LinePoints == null)
        {
            curveData.LinePoints = new List<Vector2>();
        }

        //Make the line object for the curve
        curveCount = curveData.LinePoints.Count / 4;

        line = new VectorLine("Curve", new List<Vector2>((LineSegments + 1) * curveCount), null, 5.0f, LineType.Continuous, Joins.Weld);

        UpdateLine();

        line.rectTransform.gameObject.SetActive(false);

        //Create bounding box line
        //BoundingBoxCenter = curveData.BoundingCenter;
        //BoundingBoxSize = curveData.BoundingSize;
        //
        //boundingBoxLine = new VectorLine("Bounding Box", boundingBoxPoints, 2.0f, LineType.Continuous, Joins.Weld);
        //boundingBoxLine.color = Color.red;
        //
        //UpdateBoundingBoxLine();
    }

    public void RebuildMesh()
    {
        LoadCurveDataFile();
        CreateLevelMesh();

        DestroyImmediate(GameObject.Find("VectorCanvas"));
    }

    public void StartSimulation()
    {
        PotentialLowestValue = GetLineControlWorldPoint(PotentialLowestControlPointIndex).y;

        penguin.nextXValue = penguin.transform.position.x;
        GetPoints(penguin.nextXValue, out previous, out current, out next);
        //penguin.CalculatePotE(current.y);
        penguin.SetInitialEnergy(current.y, PotentialLowestValue);
        isSimulating = true;
    }

    public bool GetPoints(float x, out Vector3 previous, out Vector3 current, out Vector3 next)
    {
        int lineCount = line.points2.Count;

        float percentage;
        float percentageMaxCount = lineCount - 1;
        Vector3 point0, point1, dir;
        int linePointIndex;

        for (int i = 0; i < lineCount; i++)
        {
            percentage = i / percentageMaxCount;
            point1 = GetLineWorldPoint(percentage, out linePointIndex);

            if (point1.x > x)
            {
                if(linePointIndex == 0)
                {
                    break;
                }

                point0 = GetLineWorldPoint(linePointIndex - 1);

                dir = (point1 - point0);
                percentage = (x - point0.x) / (point1.x - point0.x);

                previous = point0;
                current = (point0 + dir * percentage);
                next = point1;

                float yValue = (x - previous.x) * ((next.y - previous.y)
                                            / (next.x - previous.x))
                                            + previous.y;
                current = new Vector3(x, yValue);
                return true;
            }
        }

        previous = Vector3.zero;
        current = Vector3.zero;
        next = Vector3.zero;
        return false;
    }

    public Vector3[] GetLineControlPoints(params int[] indexes)
    {
        Vector3[] points = new Vector3[indexes.Length];
        int controlPointIndex;

        for (int i = 0; i < points.Length; i++)
        {
            controlPointIndex = Mathf.Max(0, indexes[i] * 4 - 2);

            if(controlPointIndex >= 0 && controlPointIndex < curveData.LinePoints.Count)
            {
                points[i] = LinePointToWorldPoint(curveData.LinePoints[controlPointIndex]);
            }
            else
            {
                Debug.LogError("Line control point doesn't exist, change dragger indexes!");
            }
        }
        return points;
    }

    public void MoveLineControlPoints(float[] yValues, params int[] indexes)
    {
        Vector3[] points = new Vector3[indexes.Length];
        int controlPointIndex;
        Vector2 point, oldPoint, moveDiff;

        for (int i = 0; i < indexes.Length; i++)
        {
            controlPointIndex = Mathf.Max(0, indexes[i] * 4 - 2);

            point = oldPoint = curveData.LinePoints[controlPointIndex];
            point.y = WorldPointToLinePoint(new Vector3(0f, yValues[i])).y;
            moveDiff = point - oldPoint;

            curveData.LinePoints[controlPointIndex] = point;
            curveData.LinePoints[controlPointIndex + 1] += moveDiff;

            if(controlPointIndex > 0 && controlPointIndex < curveData.LinePoints.Count - 2)
            {
                curveData.LinePoints[controlPointIndex + 2] = point;
                curveData.LinePoints[controlPointIndex + 3] += moveDiff;
            }
        }

        UpdateLineAndMesh();
    }

    public void UpdateEnergyInSystem(Vector2 oldPengionPos) {

        GetPoints(oldPengionPos.x, out previous, out current, out next);

       // penguin.CalculatePotE(current.y, oldPengionPos.y, true);
        penguin.UpdateTotalEnergyWithPotential(current.y, oldPengionPos.y);
    }

    public void ShowSurfaceLine(bool show, int startPointIndex, int endPointIndex)
    {
        isShowingSurfaceLine = show;

        if(show)
        {
            surfaceLineStartPoint = startPointIndex;
            surfaceLineEndPoint = endPointIndex;

            if (surfaceLineRenderer == null)
            {
                GameObject go = new GameObject("SurfaceLine");
                surfaceLineRenderer = go.AddComponent<LineRenderer>();
                surfaceLineRenderer.sharedMaterial = SurfaceLineMaterial;
                surfaceLineRenderer.startWidth = surfaceLineRenderer.endWidth = 0.1f;
            }
            else
            {
                surfaceLineRenderer.gameObject.SetActive(true);
            }

            surfaceLineRenderer.positionCount = (endPointIndex + 1) - startPointIndex;

            UpdateSurfaceLine();
        }
        else if(surfaceLineRenderer != null)
        {
            surfaceLineRenderer.gameObject.SetActive(false);
        }
    }
    #endregion
    #region Private Methods
    void CreateLevelMesh()
    {
        DestroyImmediate(GameObject.Find("LevelMesh"));

        GameObject levelMeshObject = new GameObject("LevelMesh");
        Transform levelMeshTransform = levelMeshObject.transform;
        levelMeshTransform.position = Vector3.zero;

        camera = FindObjectOfType<Camera>();

        meshFilter = levelMeshObject.AddComponent<MeshFilter>();
        var meshRenderer = levelMeshObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = LevelMeshMaterial;
        edgeCollider = levelMeshObject.AddComponent<EdgeCollider2D>();

        const float fileScaleXAxisSize = 1920f;
        const float minY = -10f;

        edgeColliderPoints = new Vector2[SurfacePointCount];

        meshTopXValues = new double[SurfacePointCount];
        meshTopYValues = new double[SurfacePointCount];
        meshBottomXValues = new double[SurfacePointCount];
        meshBottomYValues = new double[SurfacePointCount];

        float screenWorldWidth = (camera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f - camera.transform.position.z)) - camera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f - camera.transform.position.z))).magnitude;

        lineWorldScaleFactor = (fileScaleXAxisSize / curveData.ScreenSize.x) * meshScaleFactor;
        boundingCenter = curveData.BoundingCenter * lineWorldScaleFactor;
        boundingSize = curveData.BoundingSize * lineWorldScaleFactor;
        lineWorldYOffset = boundingCenter.y - boundingSize.y * 0.5f;

        //float scale = screenWorldWidth / boundingSize.x;
        //levelMeshTransform.localScale = new Vector3(scale, scale, 1f);

        for (int i = 0; i < SurfacePointCount; i++)
        {
            float percentage = i / (float)(SurfacePointCount - 1);
            Vector2 point = GetLineWorldPoint(percentage);

            meshTopXValues[i] = meshBottomXValues[i] = point.x;
            meshTopYValues[i] = point.y;
            meshBottomYValues[i] = minY - lineWorldYOffset * 0.5f;
        }

        UpdateMesh();
    }

    void UpdateLineAndMesh()
    {
        UpdateLine();
        UpdateMesh();
        UpdateSurfaceLine();
    }

    void UpdateLine()
    {
        for (int i = 0; i < curveCount; i++)
        {
            line.MakeCurve(curveData.LinePoints[i * 4 + 0], curveData.LinePoints[i * 4 + 1], curveData.LinePoints[i * 4 + 2], curveData.LinePoints[i * 4 + 3], LineSegments, (LineSegments + 1) * i);
        }
        line.Draw();
    }

    void UpdateMesh()
    {
        for (int i = 0; i < SurfacePointCount; i++)
        {
            float percentage = i / (float)(SurfacePointCount - 1);
            Vector2 point = GetLineWorldPoint(percentage);
            meshTopYValues[i] = point.y;

            edgeColliderPoints[i] = point;
        }

        edgeCollider.points = edgeColliderPoints;
        meshFilter.sharedMesh = MeshBuilder.fillLineMesh(meshTopXValues, meshTopYValues, meshBottomXValues, meshBottomYValues, meshDepth);
    }

    void UpdateSurfaceLine()
    {
        if (isShowingSurfaceLine)
        {
            int count = (surfaceLineEndPoint + 1) - surfaceLineStartPoint;
            Vector3 point;

            for (int i = 0; i < count; i++)
            {
                point = edgeColliderPoints[surfaceLineStartPoint + i];
                point.z = -0.01f;
                surfaceLineRenderer.SetPosition(i, point);
            }
        }
    }

    Vector3 GetLineWorldPoint(float percentage)
    {
        int index;
        return GetLineWorldPoint(percentage, out index);
    }

    Vector3 GetLineWorldPoint(float percentage, out int index)
    {
        Vector2 point = line.GetPoint01(percentage, out index);
        return LinePointToWorldPoint(point);
    }

    Vector3 GetLineWorldPoint(int linePointIndex)
    {
        Vector2 point = line.points2[linePointIndex];
        return LinePointToWorldPoint(point);
    }

    Vector3 GetLineControlWorldPoint(int lineControlPointIndex)
    {
        int linePointIndex = Mathf.Max(0, lineControlPointIndex * 4 - 2);
        Vector2 point = curveData.LinePoints[linePointIndex];
        return LinePointToWorldPoint(point);
    }

    Vector3 LinePointToWorldPoint(Vector3 linePoint)
    {
        linePoint *= lineWorldScaleFactor;
        linePoint.x -= boundingCenter.x;
        linePoint.y -= lineWorldYOffset;
        return linePoint;
    }

    Vector3 WorldPointToLinePoint(Vector3 worldPoint)
    {
        worldPoint.x += boundingCenter.x;
        worldPoint.y += lineWorldYOffset;
        worldPoint /= lineWorldScaleFactor;
        return worldPoint;
    }
    #endregion
}