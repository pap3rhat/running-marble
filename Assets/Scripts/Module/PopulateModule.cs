using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateModule : MonoBehaviour
{

    [SerializeField] private GameObject _prefab;
    [SerializeField] private Texture2D _sampleTexture;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        PopulateWithPrefab();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void PopulateWithPrefab()
    {
        for (int y = 0; y < _sampleTexture.height; y++)
        {
            for (int x = 0; x < _sampleTexture.width; x++)
            {
                if (_sampleTexture.GetPixel(x, y).r == 0)
                {
                    // TODO: Figure out how to set y correctly
                    var go = Instantiate(_prefab);
                    go.transform.SetParent(this.transform, true);
                    go.transform.localPosition = new Vector3(((float)x).Remap(0, 9, -0.5f, 0.5f), 0.6f, ((float)y).Remap(0, 9, -0.5f, 0.5f));
                }
            }
        }
    }
}
