using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateModule : MonoBehaviour
{

    [SerializeField] private List<GameObject> _prefabs = new();

    private static float BOUNCER_HEIGHT = 0.6f;
    private static float ROTATOR_HEIGHT = 0.6f;
    private static float REVOLVING_DOOR_HEIGHT = 0.76f;
    private static float WHIRLING_DOOR_HEIGHT = 0.8f;
    private static float FALLING_WALL_HEIGHT = 1.2f;
    private static float SPIKE_HEIGHT = 0.6f;


    private static int HEIGHT = 18;
    private static int WIDTH = 8;

    private int _objectAmount = 40;
    private int _setObjects = 0;
    private List<(int, int)> _freeSpaces = new();
    private Dictionary<int, List<(int, int)>> _populatePlaces = new();
    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        PopulateWithPrefab();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void PopulateWithPrefab()
    {
        _freeSpaces = Enumerable.Range(0, WIDTH).ToList().SelectMany(x => Enumerable.Range(0, HEIGHT).ToList(), (x, y) => (x, y)).ToList();
        for(int i = 0; i < 6; i++)
        {
            _populatePlaces.Add(i, new List<(int, int)>());
        }
       


        while ((_setObjects < _objectAmount) || (_freeSpaces.Count == 0))
        {
            int randomIdx = UnityEngine.Random.Range(0,_freeSpaces.Count-1);

            switch (UnityEngine.Random.Range(0, 6))
            {
                case 0:
                    // Adding bouncer
                    var currentList = _populatePlaces[0];
                    currentList.Add(_freeSpaces[randomIdx]);
                    _populatePlaces[0] = currentList;
                    // Removing current space from free spaces
                    _freeSpaces.RemoveAt(randomIdx);
                    break;

                case 1:
                    // Adding rotator
                    var currentList2 = _populatePlaces[1];
                    currentList2.Add(_freeSpaces[randomIdx]);
                    _populatePlaces[1] = currentList2;
                    // Removing current space from free spaces
                    _freeSpaces.RemoveAt(randomIdx);
                    // Removing neighbors from free spaces
                    //TODO
                    break;

                case 2:
                    // Adding revolving door
                    var currentList3 = _populatePlaces[2];
                    currentList3.Add(_freeSpaces[randomIdx]);
                    _populatePlaces[2] = currentList3;
                    // Removing current space from free spaces
                    _freeSpaces.RemoveAt(randomIdx);
                    // Removing neighbors from free spaces
                    //TODO
                    break;

                case 3:
                    // Adding whirling door
                    var currentList4 = _populatePlaces[3];
                    currentList4.Add(_freeSpaces[randomIdx]);
                    _populatePlaces[3] = currentList4;
                    // Removing current space from free spaces
                    _freeSpaces.RemoveAt(randomIdx);
                    // Removing neighbors from free spaces
                    //TODO
                    break;

                case 4:
                    // Adding falling wall
                    var currentList5 = _populatePlaces[4];
                    currentList5.Add(_freeSpaces[randomIdx]);
                    _populatePlaces[4] = currentList5;
                    // Removing current space from free spaces
                    _freeSpaces.RemoveAt(randomIdx);
                    break;

                case 5:
                    // Adding spike
                    var currentList6 = _populatePlaces[5];
                    currentList6.Add(_freeSpaces[randomIdx]);
                    _populatePlaces[5] = currentList6;
                    // Removing current space from free spaces
                    _freeSpaces.RemoveAt(randomIdx);
                    break;
            }


            _setObjects++;
        }

        {
            foreach (var bouncer in _populatePlaces[0])
            {
                InstantiateBouncer(bouncer.Item1, bouncer.Item2);
            }

            foreach (var rotator in _populatePlaces[1])
            {
                InstantiateRotator(rotator.Item1, rotator.Item2);
            }

            foreach (var rdoor in _populatePlaces[2])
            {
                InstantiateRevolvingDoor(rdoor.Item1, rdoor.Item2);
            }

            foreach (var wdoor in _populatePlaces[3])
            {
                InstantiateWhirlingDoor(wdoor.Item1, wdoor.Item2);
            }

            foreach (var fwall in _populatePlaces[4])
            {
                InstantiateFallingWall(fwall.Item1, fwall.Item2);
            }

            foreach (var spike in _populatePlaces[5])
            {
                InstantiateSpike(spike.Item1, spike.Item2);
            }
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/


    /*
     * Instantiates all bouncer objects on module.
     */
    private void InstantiateBouncer(int x, int y)
    {
        var go = Instantiate(_prefabs[0]);

        go.transform.SetParent(this.transform, true);
        go.transform.localPosition = new Vector3(((float)x).Remap(0, WIDTH - 1, -0.5f, 0.5f),
                                                 BOUNCER_HEIGHT,
                                                 ((float)y).Remap(0, HEIGHT - 1, -0.5f, 0.5f));
    }

    /*
     * Instantiates all rotator objects on module.
     */
    private void InstantiateRotator(int x, int y)
    {
        var go = Instantiate(_prefabs[1]);

        go.transform.SetParent(this.transform, true);
        go.transform.localPosition = new Vector3(((float)x).Remap(0, WIDTH - 1, -0.5f, 0.5f),
                                                 ROTATOR_HEIGHT,
                                                 ((float)y).Remap(0, HEIGHT - 1, -0.5f, 0.5f));
    }

    /*
     * Instantiates all revolving door objects on module.
     */
    private void InstantiateRevolvingDoor(int x, int y)
    {
        var go = Instantiate(_prefabs[2]);

        go.transform.SetParent(this.transform, true);
        go.transform.localPosition = new Vector3(((float)x).Remap(0, WIDTH - 1, -0.5f, 0.5f),
                                                 REVOLVING_DOOR_HEIGHT,
                                                 ((float)y).Remap(0, HEIGHT - 1, -0.5f, 0.5f));

        // Reconfiguring the Anchor, so it works inside parent object as well
        var joint = go.GetComponentInChildren<HingeJoint>();
        joint.autoConfigureConnectedAnchor = true;
        // Randomizing how fast door moves
        var motor = joint.motor;
        motor.force = UnityEngine.Random.Range(-20f, 20f);
        motor.targetVelocity = UnityEngine.Random.Range(10f, 250f);
        joint.motor = motor;
    }

    /*
    * Instantiates all whirling door objects on module.
    */
    private void InstantiateWhirlingDoor(int x, int y)
    {
        var go = Instantiate(_prefabs[3]);

        go.transform.SetParent(this.transform, true);
        go.transform.localPosition = new Vector3(((float)x).Remap(0, WIDTH - 1, -0.5f, 0.5f),
                                                 WHIRLING_DOOR_HEIGHT,
                                                 ((float)y).Remap(0, HEIGHT - 1, -0.5f, 0.5f));

        // Reconfiguring the Anchor, so it works inside parent object as well
        var joint = go.GetComponentInChildren<HingeJoint>();
        joint.autoConfigureConnectedAnchor = true;
        // Randomizing how fast door moves
        var motor = joint.motor;
        motor.force = UnityEngine.Random.Range(-20f, 20f);
        motor.targetVelocity = UnityEngine.Random.Range(10f, 250f);
        joint.motor = motor;
    }


    /*
     * Instantiates all falling wall objects on module.
     */
    private void InstantiateFallingWall(int x, int y)
    {
        var go = Instantiate(_prefabs[4]);

        go.transform.SetParent(this.transform, true);
        go.transform.localPosition = new Vector3(((float)x).Remap(0, WIDTH - 1, -0.5f, 0.5f),
                                                 FALLING_WALL_HEIGHT,
                                                 ((float)y).Remap(0, HEIGHT - 1, -0.5f, 0.5f));

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
    * Instantiates all spike objects on module.
    */
    private void InstantiateSpike(int x, int y)
    {
        var go = Instantiate(_prefabs[5]);

        go.transform.SetParent(this.transform, true);
        go.transform.localPosition = new Vector3(((float)x).Remap(0, WIDTH - 1, -0.5f, 0.5f),
                                                 SPIKE_HEIGHT,
                                                 ((float)y).Remap(0, HEIGHT - 1, -0.5f, 0.5f));
    }

}

