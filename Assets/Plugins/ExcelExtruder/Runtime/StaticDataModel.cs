using System.Collections.Generic;
using UnityEngine;
using MemoryPack;

public class StaticDataModel
{
    // @Dont delete - for Gen property@

    public void Init()
    {
        // @Dont delete - for Gen Init Func@
    }
    private T MemoryPackDeserialize<T>(string filename)
    {
        var bin = Resources.Load<TextAsset>("StaticData/" + filename).bytes;
        return MemoryPackSerializer.Deserialize<T>(bin);
    }
}
