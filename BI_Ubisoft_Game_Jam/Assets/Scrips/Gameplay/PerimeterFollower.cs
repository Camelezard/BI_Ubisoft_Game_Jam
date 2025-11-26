using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Transform))]
public class PerimeterFollower : MonoBehaviour
{
    public enum SourceMode { RectangleFromPlane, Waypoints}

    [Header("Source")]
    [SerializeField]
    private SourceMode _sourceMode = SourceMode.RectangleFromPlane;
    [Tooltip("Plane / Mesh object used for RectangleFromPlane or MeshBoundary modes")]
    [SerializeField]
    private GameObject _sourceObject;
    [Tooltip("Manual waypoints (ordered) if using Waypoints mode")]
    [SerializeField]
    private Transform[] _waypoints;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _speedIncreasePerSecond = 5f;
    [SerializeField] private float _speedDecreasePerSecond = 5f;
    
    private List<Vector3> _pathPoints = new List<Vector3>();
    private float[] _segmentLengths;
    private float _totalLength;
    private float _currentDistance = 0f;

    private float _velocity = 0f;

    private InputAction _move;
    private const string MOVE = "Move";
    
    #region singleton
    
    private static PerimeterFollower _Instance;
    public static PerimeterFollower instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.Log("no PerimeterFollower instance found");
                return null;
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
            Debug.Log("PerimeterFollower already exists");
        }
    }
    
    #endregion
    
    private void Start()
    {
        _move = InputManager.instance.GetInputAction(MOVE);
        RebuildPath();
    }

    private void Update()
    {
        if (_pathPoints == null || _pathPoints.Count < 2) return;

        float lInput = -_move.ReadValue<float>();
        
        if(lInput != 0f)
        {
            _velocity += lInput * _speedIncreasePerSecond;
            _velocity = Mathf.Clamp(_velocity, -_maxSpeed, _maxSpeed);   
        }
        else
        {
            _velocity = _velocity > 0f ? Mathf.Max(_velocity - _speedDecreasePerSecond * Time.deltaTime, 0f) :
                Mathf.Min(_velocity + _speedDecreasePerSecond * Time.deltaTime, 0f);
        }
        
        _currentDistance += _velocity * Time.deltaTime;
        
        if (_totalLength > 0f)
        {
            _currentDistance = Mathf.Repeat(_currentDistance, _totalLength);
        }
    
        transform.SetPositionAndRotation(GetPointAtDistance(_currentDistance), GetRotationToFieldCenter());
    }
    
    private void RebuildPath()
    {
        _pathPoints.Clear();

        if (_sourceMode == SourceMode.RectangleFromPlane && _sourceObject != null)
        {
            BuildRectangleFromPlane(_sourceObject);
        }
        else if (_sourceMode == SourceMode.Waypoints && _waypoints != null && _waypoints.Length >= 2)
        {
            for (int i = 0; i < _waypoints.Length; i++)
                _pathPoints.Add(_waypoints[i].position);
        }

        PrepareLengths();
    }

    #region Path Math
    private void PrepareLengths()
    {
        int lCount = _pathPoints.Count;
        if (lCount < 2)
        {
            _totalLength = 0f;
            _segmentLengths = null;
            return;
        }
        
        _segmentLengths = new float[lCount];
        _totalLength = 0f;
        for (int i = 0; i < lCount; i++)
        {
            Vector3 lSegmentStart = _pathPoints[i];
            Vector3 lSegmentEnd = _pathPoints[(i + 1) % lCount];
            float lLength = Vector3.Distance(lSegmentStart, lSegmentEnd);
            _segmentLengths[i] = lLength;
            _totalLength += lLength;
        }
    }
    
    private Vector3 GetPointAtDistance(float pDistance)
    {
        if (_pathPoints.Count == 0) return transform.position;
        if (_pathPoints.Count == 1) return _pathPoints[0];
        
        pDistance = Mathf.Repeat(pDistance, _totalLength);
        
        float lDistanceCount = 0f;
        float lSegmentPercent;
        Vector3 lSegmentStart, lSegmentEnd;
        for (int i = 0; i < _segmentLengths.Length; i++)
        {
            if (lDistanceCount + _segmentLengths[i] >= pDistance)
            {
                lSegmentPercent = (pDistance - lDistanceCount) / _segmentLengths[i];
                lSegmentStart = _pathPoints[i];
                lSegmentEnd = _pathPoints[(i + 1) % _pathPoints.Count];
                return Vector3.Lerp(lSegmentStart, lSegmentEnd, lSegmentPercent);
            }
            lDistanceCount += _segmentLengths[i];
        }
        // fallback end
        return _pathPoints[_pathPoints.Count - 1];
    }

    private Quaternion GetRotation(float pDistance)
    {
        if (_pathPoints.Count < 2) return Quaternion.identity;
        
        pDistance = Mathf.Repeat(pDistance, _totalLength);
        float lDistanceCount = 0f;
        Vector3 lSegmentStart, lSegmentEnd;
        for (int i = 0; i < _segmentLengths.Length; i++)
        {
            if (lDistanceCount + _segmentLengths[i] >= pDistance)
            {
                lSegmentStart = _pathPoints[i];
                lSegmentEnd = _pathPoints[(i + 1) % _pathPoints.Count];
                return Quaternion.LookRotation(-Vector3.Cross(lSegmentEnd - lSegmentStart, Vector3.up), Vector3.up);
            }
            lDistanceCount += _segmentLengths[i];
        }
        return Quaternion.LookRotation(-Vector3.Cross(_pathPoints[1] - _pathPoints[0], Vector3.up), Vector3.up);
    }
    #endregion
    
    private Quaternion GetRotationToFieldCenter()
    {
        return Quaternion.LookRotation(Vector3.ProjectOnPlane(_sourceObject.transform.position - transform.position, Vector3.up), Vector3.up);
    }

    #region Builders

    private void BuildRectangleFromPlane(GameObject planeObj)
    {
        MeshFilter mf = planeObj.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            // Try to recover corners from mesh bounds
            Bounds b = mf.sharedMesh.bounds;
            Vector3[] cornersLocal = new Vector3[4]
            {
                new Vector3(b.min.x, 0, b.min.z),
                new Vector3(b.min.x, 0, b.max.z),
                new Vector3(b.max.x, 0, b.max.z),
                new Vector3(b.max.x, 0, b.min.z)
            };
            for (int i = 0; i < 4; i++)
            {
                Vector3 world = planeObj.transform.TransformPoint(cornersLocal[i]);
                _pathPoints.Add(world);
            }
        }
        else
        {
            // Fallback: use transform scale assuming center at transform.position
            Vector3 s = planeObj.transform.lossyScale;
            Vector3 pos = planeObj.transform.position;
            float halfX = 0.5f * s.x;
            float halfZ = 0.5f * s.z;
            _pathPoints.Add(pos + planeObj.transform.right * -halfX + planeObj.transform.forward * -halfZ);
            _pathPoints.Add(pos + planeObj.transform.right * -halfX + planeObj.transform.forward * halfZ);
            _pathPoints.Add(pos + planeObj.transform.right * halfX + planeObj.transform.forward * halfZ);
            _pathPoints.Add(pos + planeObj.transform.right * halfX + planeObj.transform.forward * -halfZ);
        }
    }

    #endregion
    
    public void SetSpeed(float lCoeff)
    {
        _maxSpeed *= lCoeff;
    }
    
    #region Editor Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_pathPoints == null || _pathPoints.Count < 2)
        {
            if (_sourceMode == SourceMode.RectangleFromPlane && _sourceObject != null)
            {
                MeshFilter lMeshFilter = _sourceObject.GetComponent<MeshFilter>();
                if (lMeshFilter != null && lMeshFilter.sharedMesh != null)
                {
                    Bounds lBounds = lMeshFilter.sharedMesh.bounds;
                    Vector3[] lCorners = new Vector3[4]
                    {
                        new(lBounds.min.x, 0, lBounds.min.z),
                        new(lBounds.min.x, 0, lBounds.max.z),
                        new(lBounds.max.x, 0, lBounds.max.z),
                        new(lBounds.max.x, 0, lBounds.min.z)
                    };
                    Gizmos.color = Color.yellow;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 lStart = _sourceObject.transform.TransformPoint(lCorners[i]);
                        Vector3 lEnd = _sourceObject.transform.TransformPoint(lCorners[(i + 1) % 4]);
                        Gizmos.DrawLine(lStart, lEnd);
                    }
                }
            }
            return;
        }

        Gizmos.color = Color.cyan;
        for (int i = 0; i < _pathPoints.Count; i++)
        {
            Vector3 lStart = _pathPoints[i];
            Vector3 lEnd = _pathPoints[(i + 1) % _pathPoints.Count];
            Gizmos.DrawLine(lStart, lEnd);
            
            // arrow
            Vector3 lMiddle = (lStart + lEnd) * 0.5f;
            Vector3 lDirection = (lEnd - lStart).normalized;
            Gizmos.DrawRay(lMiddle, lDirection * 0.5f);
        }
    }
#endif
    #endregion
}
