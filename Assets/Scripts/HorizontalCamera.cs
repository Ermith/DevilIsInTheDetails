using UnityEngine;

[ExecuteInEditMode]
public class HorizontalCamera : MonoBehaviour
{
    Camera _camera;
    float _lastAspect;

    [SerializeField] float _fieldOfView = 60f;
    public float FieldOfView
    {
        get => _fieldOfView;
        set
        {
            if (_fieldOfView == value) return;
            _fieldOfView = value;
            RefreshCamera();
        }
    }

    [SerializeField] float _orthographicSize = 5f;
    public float OrthographicSize
    {
        get => _orthographicSize;
        set
        {
            if (_orthographicSize == value) return;
            _orthographicSize = value;
            RefreshCamera();
        }
    }

    protected void OnEnable()
    {
        RefreshCamera();
    }

    protected void Update()
    {
        float aspect = _camera.aspect;
        if (aspect != _lastAspect)
            AdjustCamera(aspect);
    }

    public void RefreshCamera()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();

        AdjustCamera(_camera.aspect);
    }

    void AdjustCamera(float aspect)
    {
        _lastAspect = aspect;

        // Credit: https://forum.unity.com/threads/how-to-calculate-horizontal-field-of-view.16114/#post-2961964
        float _1OverAspect = 1f / aspect;
        _camera.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(_fieldOfView * Mathf.Deg2Rad * 0.5f) * _1OverAspect) * Mathf.Rad2Deg;
        _camera.orthographicSize = _orthographicSize * _1OverAspect;
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        RefreshCamera();
    }
#endif
}
