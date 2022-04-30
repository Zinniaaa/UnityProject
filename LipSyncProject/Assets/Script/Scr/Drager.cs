using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drager : MonoBehaviour
{
    public Transform targetRoot;

    private Camera camera;

    private void Awake()
    {
        if (Camera.main)
        {
            camera = Camera.main;
        }
        else
        {
            var canvas = GameObject.Find("Canvas");
            var uiCamera = canvas.transform.Find("UICamera").GetComponent<Camera>() ;
            camera = uiCamera;
        }
    }

    void OnMouseEnter()
    {

    }

    void OnMouseExit()
    {

    }

    IEnumerator OnMouseDown()
    {
        Vector3 screenSpace = camera.WorldToScreenPoint(targetRoot.position);//三维物体坐标转屏幕坐标  
        //将鼠标屏幕坐标转为三维坐标，再计算物体位置与鼠标之间的距离  
        var offset = targetRoot.position - camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
        while (Input.GetMouseButton(0))
        {
            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            var curPosition = camera.ScreenToWorldPoint(curScreenSpace) + offset;
            targetRoot.position = curPosition;
            yield return new WaitForFixedUpdate();
        }
    }
}
