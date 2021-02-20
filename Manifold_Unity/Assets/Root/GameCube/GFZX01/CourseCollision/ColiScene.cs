﻿using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameCube.GFZX01.CourseCollision
{
    [Serializable]
    public class ColiScene : IBinarySerializable, IFile
    {
        [SerializeField] string name;

        // Generate some metadata to be used by some processes
        [SerializeField]
        public CourseID courseID;
        [SerializeField]
        public CourseVenueID courseVenueID;
        [SerializeField]
        public int id;


        public Header header;
        public TrackNode[] trackNodes;
        public TrackLength trackInformation;
        // some other stuff goes here...
        public GameObject[] gameObjects;

        public List<TrackTransform> trackTransforms = new List<TrackTransform>();

        public string FileName
        {
            get => name;
            set => name = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianess(false);

            Debug.Log(FileName);

            // Generate some metadata
            id = int.Parse(System.Text.RegularExpressions.Regex.Match(FileName, @"\d+").Value);
            courseID = CourseUtility.GetCourseID(id);
            courseVenueID = CourseUtility.GetVenueID(id);


            reader.ReadX(ref header, true);

            // 0x08 - Track Nodes
            reader.BaseStream.Seek(header.trackNodeAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref trackNodes, header.trackNodeCount, true);

            // 0x48 - Game Objects
            Assert.IsTrue(header.gameObjectAbsPtr > 0); // NOT TRUE FOR AX
            reader.BaseStream.Seek(header.gameObjectAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref gameObjects, header.gameObjectCount, true);

            // 0x90 - Track Transforms
            reader.BaseStream.Seek(header.trackInfoAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref trackInformation, true);

            // Let's build the transform after-the-fact
            // We want to remove duplicate references to the same instance.
            var usedKeys = new List<int>();
            foreach (var node in trackNodes)
            {
                var transformAbsPtr = node.trackTransformAbsPtr;
                if (!usedKeys.Contains(transformAbsPtr))
                {
                    usedKeys.Add(transformAbsPtr);

                    reader.BaseStream.Seek(transformAbsPtr, SeekOrigin.Begin);
                    var value = new TrackTransform();
                    value.Deserialize(reader);
                    trackTransforms.Add(value);
                }
            }

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
