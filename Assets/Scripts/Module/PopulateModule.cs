using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateModule : MonoBehaviour
{
    // All the different obstacle prefabs
    [SerializeField] private List<GameObject> _prefabs = new();

    // Constants for different obstacles
    private static float BOUNCER_HEIGHT = 0.6f;
    private static float ROTATOR_HEIGHT = 0.6f;
    private static float REVOLVING_DOOR_HEIGHT = 0.76f;
    private static float WHIRLING_DOOR_HEIGHT = 0.8f;
    private static float FALLING_WALL_HEIGHT = 1.2f;
    private static float SPIKE_HEIGHT = 0.6f;

    private static int BOUNCER_IDX = 0;
    private static int ROTATOR_IDX = 1;
    private static int REVOLVING_DOOR_IDX = 2;
    private static int WHIRLING_DOOR_IDX = 3;
    private static int FALLING_WALL_IDX = 4;
    private static int SPIKE_IDX = 5;

    // Constants for spawing area -> not square, because area is not sqaue but like 1:0.4444
    private static int HEIGHT = 18;
    private static int WIDTH = 8;

    // Lists for saving free spaces with obstacles placed on them
    private List<(int, int)> _freeSpaces = new();
    private Dictionary<int, List<(int, int)>> _populatePlaces = new(); // key determines what kind of obstacle list of placing spaces belongs to

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Fills up module with obstacles.
     */
    public void PopulateWithPrefab(int objectAmount)
    {
        int setObjects = 0;

        // --- Init Lists ---
        _freeSpaces = Enumerable.Range(0, WIDTH).ToList().SelectMany(x => Enumerable.Range(0, HEIGHT).ToList(), (x, y) => (x, y)).ToList();
        for (int i = 0; i < 6; i++)
        {
            _populatePlaces.Add(i, new List<(int, int)>());
        }


        // Adding new obsatcles as long as maximum amount of obstacle has not been reached or until no more obstacles can be placed due to space restrictions.
        // Space restrictions might lead to less obsatcles being placed that the given object amount, but that only happens when a lot of objects should be placed (like 70+) and at that point that's okay (it's my game, I can say that).
        while ((setObjects < objectAmount) && (_freeSpaces.Count != 0))
        {
            int randomIdx = UnityEngine.Random.Range(0, _freeSpaces.Count - 1);

            var x = _freeSpaces[randomIdx].Item1;
            var y = _freeSpaces[randomIdx].Item2;

            // Can a big object be placed as well?
            // If not, only a small one will be placed. Leads to more small ones being placed than wide ones, but that's okay as well (still my game).
            if (DoubleCanBePlaced(x, y))
            {
                switch (UnityEngine.Random.Range(0, 6))
                {
                    case 0:
                        // Adding bouncer
                        _populatePlaces[BOUNCER_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing current space from free spaces
                        _freeSpaces.RemoveAt(randomIdx);
                        break;

                    case 1:
                        // Adding rotator
                        _populatePlaces[ROTATOR_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing neighbors and current space from free spaces
                        RemoveNeighborhood(x, y);
                        break;

                    case 2:
                        // Adding revolving door
                        _populatePlaces[REVOLVING_DOOR_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing neighbors from free spaces
                        RemoveNeighborhood(x, y);
                        break;

                    case 3:
                        // Adding whirling door
                        _populatePlaces[WHIRLING_DOOR_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing neighbors from free spaces
                        RemoveNeighborhood(x, y);
                        break;

                    case 4:
                        // Adding falling wall
                        _populatePlaces[FALLING_WALL_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing current space from free spaces
                        _freeSpaces.RemoveAt(randomIdx);
                        break;

                    case 5:
                        // Adding spike
                        _populatePlaces[SPIKE_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing current space from free spaces
                        _freeSpaces.RemoveAt(randomIdx);
                        break;
                }
            }
            else
            {
                switch (UnityEngine.Random.Range(0, 3))
                {
                    case 0:
                        // Adding bouncer
                        _populatePlaces[BOUNCER_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing current space from free spaces
                        _freeSpaces.RemoveAt(randomIdx);
                        break;

                    case 1:
                        // Adding falling wall
                        _populatePlaces[FALLING_WALL_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing current space from free spaces
                        _freeSpaces.RemoveAt(randomIdx);
                        break;

                    case 2:
                        // Adding spike
                        _populatePlaces[SPIKE_IDX].Add(_freeSpaces[randomIdx]);
                        // Removing current space from free spaces
                        _freeSpaces.RemoveAt(randomIdx);
                        break;
                }
            }

            setObjects++;
        }

        // --- Instantiating obstacles ---
        {
            foreach (var bouncer in _populatePlaces[BOUNCER_IDX])
            {
                InstantiateBouncer(bouncer.Item1, bouncer.Item2);
            }

            foreach (var rotator in _populatePlaces[ROTATOR_IDX])
            {
                InstantiateRotator(rotator.Item1, rotator.Item2);
            }

            foreach (var rdoor in _populatePlaces[REVOLVING_DOOR_IDX])
            {
                InstantiateRevolvingDoor(rdoor.Item1, rdoor.Item2);
            }

            foreach (var wdoor in _populatePlaces[WHIRLING_DOOR_IDX])
            {
                InstantiateWhirlingDoor(wdoor.Item1, wdoor.Item2);
            }

            foreach (var fwall in _populatePlaces[FALLING_WALL_IDX])
            {
                InstantiateFallingWall(fwall.Item1, fwall.Item2);
            }

            foreach (var spike in _populatePlaces[SPIKE_IDX])
            {
                InstantiateSpike(spike.Item1, spike.Item2);
            }
        }
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Checks if a wide object can be placed.
     * Returns true, if no small object is in neighborhood already (wide objects cannot be in neighborhood by definition).
     */
    private bool DoubleCanBePlaced(int x, int y)
    {
        List<(int, int)> neighborhood = GetNeighborhood(x, y);

        return (neighborhood.Intersect(_populatePlaces[BOUNCER_IDX]).Count() == 0)  // True if no bouncer in neighborhood
            && (neighborhood.Intersect(_populatePlaces[FALLING_WALL_IDX]).Count() == 0)  // True if no fwall in neighborhood
            && (neighborhood.Intersect(_populatePlaces[SPIKE_IDX]).Count() == 0); // True if no spike in neighborhood
    }

    /*
     * Returns list of neighborhood for specific tile (x,y).
     * Looking up neighbors based on: https://www.tech-notes.net/python-element-neighbor-in-matrix/
     */
    private List<(int, int)> GetNeighborhood(int x, int y)
    {
        List<(int, int)> neighborhood = new();

        var startX = x - 1 < 0 ? 0 : x - 1;
        var endX = x + 2 > WIDTH ? WIDTH : x + 2;
        var startY = y - 1 < 0 ? 0 : y - 1;
        var endY = y + 2 > HEIGHT ? HEIGHT : y + 2;

        for (int i = startX; i < endX; i++)
        {
            for (int j = startY; j < endY; j++)
            {
                neighborhood.Add((i, j));
            }
        }

        return neighborhood;
    }



    /*
     * Removes space at (x,y) as well as neighborhood (8 neighbors) from free spaces.
     * Used for the following obstacles, so they do not have the chance of intersecting with other obstacles: Rotator, RDoor, WDoor, FWall.
     */
    private void RemoveNeighborhood(int x, int y)
    {
        List<(int, int)> neighborhood = GetNeighborhood(x, y);

        foreach (var neighbor in neighborhood)
        {
            if (_freeSpaces.Contains(neighbor))
            {
                _freeSpaces.Remove(neighbor);
            }
        }
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Instantiates all bouncer objects on module.
     */
    private void InstantiateBouncer(int x, int y)
    {
        var go = Instantiate(_prefabs[BOUNCER_IDX]);

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
        var go = Instantiate(_prefabs[ROTATOR_IDX]);

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
        var go = Instantiate(_prefabs[REVOLVING_DOOR_IDX]);

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
        var go = Instantiate(_prefabs[WHIRLING_DOOR_IDX]);

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
        var go = Instantiate(_prefabs[FALLING_WALL_IDX]);

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
        var go = Instantiate(_prefabs[SPIKE_IDX]);

        go.transform.SetParent(this.transform, true);
        go.transform.localPosition = new Vector3(((float)x).Remap(0, WIDTH - 1, -0.5f, 0.5f),
                                                 SPIKE_HEIGHT,
                                                 ((float)y).Remap(0, HEIGHT - 1, -0.5f, 0.5f));
    }

}

