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
                    // TODO: Map positionbetween -0.5 and 0.5
                    // Figure out how to set y correctly
                    Instantiate(_prefab, new Vector3(x, 7f, y), _prefab.transform.rotation, this.transform);
                }
            }
        }
    }
}
