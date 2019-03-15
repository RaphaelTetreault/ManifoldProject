using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextAnalyzerSobj<T> : AnalyzerSobjs<T>
    where T : IBinarySerializable, INamedFile, new()
{
    [Header("Text Writer Settings")]
    [SerializeField]
    protected FileStreamSettings write = FileStreamSettings.Write;

    public virtual void CreateAnalysis(T value, string writeFile, Action<T, StreamWriter> writeAnalysis)
    {
        // Write content
        using (var fileStream = File.Open(writeFile, write.mode, write.access, write.share))
        {
            using (var writer = new StreamWriter(fileStream))
            {
                writeAnalysis(value, writer);
            }
        }
    }

}
