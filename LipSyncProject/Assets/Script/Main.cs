using System.Collections;
using System.Collections.Generic;
using libx;
using UnityEngine;

public class Main : MonoBehaviour
{
    private ManifestRequest manifestRequest;

    // Start is called before the first frame update
    void Awake()
    {
        manifestRequest = Assets.Initialize();

    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var list = Assets.GetAllAssetPaths();
            foreach (var item in list)
            {
                Debug.LogError(item);
            }
        }
    }
}
