#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

[Serializable]
public class CollectionData
{
    public string fileGuid;
    public string fileName;
    public List<string> referenceGids = new List<string>();
    public DateTime timeStamp;
}

[Serializable]
public class TypeDate
{
    public string assemblly;
    public string fileName;
    public string guid;
    public DateTime timeStamp;
    public List<string> typeFullName = new List<string>();

    public Type[] types
    {
        get { return typeFullName.Select(c => Type.GetType(c)).ToArray(); }
    }

    public void Add(Type addtype)
    {
        assemblly = addtype.Assembly.FullName;
        var typeName = addtype.FullName;
        if (typeFullName.Contains(typeName) == false) typeFullName.Add(typeName);
    }
}

public interface IReferenceCollection
{
    void CollectionFiles();
    void Init(List<CollectionData> refs);
}