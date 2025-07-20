using UnityEngine;

public class CameraNearClip : MonoBehaviour
{
    private GameObject _camera;
    [SerializeField] private float clipDistance;
    [SerializeField] private GameObject clipObject;
    
    void Start()
    {
        _camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    }
    
    void FixedUpdate()
    {
        clipObject.SetActive(_camera.transform.position.y > clipDistance);
    }
}
