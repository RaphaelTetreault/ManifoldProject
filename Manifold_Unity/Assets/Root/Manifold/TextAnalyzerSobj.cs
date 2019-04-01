//using StarkTools.IO;
//using System;
//using System.IO;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public abstract class TextAnalyzerSobj<T> : AnalyzerSobjs<T>
//    where T : IBinarySerializable, IFile, new()
//{
//    public virtual void CreateAnalysis(T value, string writeFile, Action<T, StreamWriter> writeAnalysis)
//    {
//        // Write content
//        using (var fileStream = File.Open(writeFile, write.mode, write.access, write.share))
//        {
//            using (var writer = new StreamWriter(fileStream))
//            {
//                writeAnalysis(value, writer);
//            }
//        }
//    }
//}

//// only def is different
//public abstract class NewTextAnalyzerSobj<T> : NewAnalyzerSobjs<T>
//    where T : ScriptableObject, IBinarySerializable, IFile, new()
//{
//    public virtual void CreateAnalysis(T value, string writeFile, Action<T, StreamWriter> writeAnalysis)
//    {
//        // Write content
//        using (var fileStream = File.Open(writeFile, write.mode, write.access, write.share))
//        {
//            using (var writer = new StreamWriter(fileStream))
//            {
//                writeAnalysis(value, writer);
//            }
//        }
//    }

//}
