using System.Collections;
using UnityEngine;

public class BillboardWorldUI : MonoBehaviour
{
    CameraController cameraController;
    // Use this for initialization
    void Start()
    {
        if(GameManager.Instance.CameraController != null)
        {
            cameraController = GameManager.Instance.CameraController;
            transform.rotation = cameraController.transform.rotation;
        }
        else
        {
            GameManager.Instance.OnGameStart.AddListener(() =>
            {
                cameraController = GameManager.Instance.CameraController;
                transform.rotation = cameraController.transform.rotation;
            });
        } 
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cameraController.transform.rotation;
    }
}
