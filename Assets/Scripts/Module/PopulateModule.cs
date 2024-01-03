using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateModule : MonoBehaviour
{

    [SerializeField] private List<GameObject> _prefabs = new();
    [SerializeField] private Texture2D _sampleTexture;

    private static float BOUNCER_HEIGHT = 0.6f;
    private static float ROTATOR_HEIGHT = 0.6f;
    private static float REVOLVING_DOOR_HEIGHT = 0.76f;
    private static float WHIRLING_DOOR_HEIGHT = 0.8f;
    private static float FALLING_WALL_HEIGHT = 1.2f;
    private static float SPIKE_HEIGHT = 0.6f;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        PopulateWithPrefab();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void PopulateWithPrefab()
    {
        //int height = _sampleTexture.height;
        //int width = _sampleTexture.width;

        int height = 18;
        int width = 8;



        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //if (_sampleTexture.GetPixel(x, y).r == 0)
                //{
                if (x%2==0)
                {
                    var go = Instantiate(_prefabs[5]);

                    go.transform.SetParent(this.transform, true);
                    go.transform.localPosition = new Vector3(((float)x).Remap(0, width - 1, -0.5f, 0.5f),
                                                             SPIKE_HEIGHT,
                                                             ((float)y).Remap(0, height - 1, -0.5f, 0.5f));
                 //   IsFallingWall(go);

                }
                else
                {
                    var go = Instantiate(_prefabs[4]);
                    go.transform.SetParent(this.transform, true);
                    go.transform.localPosition = new Vector3(((float)x).Remap(0, width - 1, -0.5f, 0.5f),
                                                             FALLING_WALL_HEIGHT,
                                                             ((float)y).Remap(0, height - 1, -0.5f, 0.5f));
                    IsFallingWall(go);

                }

            }
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Sets up falling wall obstacle.
     */
    private void IsFallingWall(GameObject go)
    {
        // maybe turn sideways
        if (UnityEngine.Random.value > 0.5f)
        {
            go.transform.Rotate(new Vector3(0, 90f, 0));
        }

        // randomize speed
        FallingWall wall = go.GetComponentInChildren<FallingWall>();
        wall.SetTimes(UnityEngine.Random.Range(0.1f, 0.7f), UnityEngine.Random.Range(0.5f, 3f));
        wall.SetFalling(UnityEngine.Random.value > 0.5f);
    }

    /*
     * Sets up door objects.
     */
    private void IsDoor(GameObject go)
    {
        // Reconfiguring the Anchor, so it works inside parent object as well
        var joint = go.GetComponentInChildren<HingeJoint>();
        joint.autoConfigureConnectedAnchor = true;
        // Randomizing how fast door moves
        var motor = joint.motor;
        motor.force = UnityEngine.Random.Range(-20f, 20f);
        motor.targetVelocity = UnityEngine.Random.Range(10f, 250f);
        joint.motor = motor;
    }
}

