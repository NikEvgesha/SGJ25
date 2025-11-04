using System.Collections;
using UnityEngine;

public class ObjectLookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private void Start()
    {
        StartCoroutine(WaitForInit());
    }

    private IEnumerator WaitForInit()
    {
        while (G.Player == null)
        {
            yield return null;
        }

        Init();
    }

    private void Init()
    {
        //if (!_mainCamera)
        //    _mainCamera = FindAnyObjectByType<Camera>();

        if (!_mainCamera)
        {
            Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (var camera in cameras)
            {
                if (camera.tag == "MainCamera")
                {
                    _mainCamera = camera;
                }
            }
        }
    }
    private void Update()
    {
        if (_mainCamera)
            gameObject.transform.LookAt(_mainCamera.transform.position);
    }
}
