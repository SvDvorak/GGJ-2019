﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    //list of objects that need to be spawened


    //while spawned objects is under some count:
    //  select an object to spawn
    //  select a location
    //  check if location is valid
    //  if so, spawn it


    //public ObjectCountDictionary SpawnableObjects;
    public Bounds SpawnBounds;

    public List<PlaceableObject> SpawnLibrary;

    public int SpawnCount;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(SpawnBounds.center, SpawnBounds.size);
    }

    private void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        var spawnCandidates = new List<PlaceableObject>(SpawnLibrary);

        spawnCandidates.Shuffle();

        //foreach(var kvp in SpawnableObjects)
        //{
        //    for (int i = 0; i < kvp.Value; i++)
        //    {
        //        spawnCandidates.Add(kvp.Key);
        //    }
        //}

        var infiniteLoopGuard = 9999;
        var spawned = 0;
        while(spawned < SpawnCount)
        {
            start:

            if (infiniteLoopGuard-- < 0) return;

            var spawnIndex = Random.Range(0, spawnCandidates.Count);
            var selectedObject = spawnCandidates[spawnIndex];

            var pos = Randx.Position(SpawnBounds);
            var rot = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);

            var trs = Matrix4x4.TRS(pos, rot, Vector3.one);
            foreach(var b in selectedObject.BoundingBoxes)
            {


                var rr = rot * Quaternion.AngleAxis(b.Rotation, Vector3.up);
                Debug.DrawRay(trs.MultiplyPoint(b.Position), rr * Vector3.forward, Color.magenta);

                var res = Physics.OverlapBox(trs.MultiplyPoint(b.Position), b.Extents * 0.5f, rot * Quaternion.AngleAxis(b.Rotation, Vector3.up));
                if(res.Length > 0)
                {
                    Debug.DrawRay(pos, Vector3.up);


                    goto start;
                }


            }

            

            Instantiate(selectedObject, pos, rot, null);
            spawnCandidates.RemoveAt(spawnIndex);

            spawned++;

        }
    }
}