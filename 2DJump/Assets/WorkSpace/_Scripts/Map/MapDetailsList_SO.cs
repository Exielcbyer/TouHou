using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDetailsList_SO", menuName = "Map/MapDetailsList")]
public class MapDetailsList_SO : ScriptableObject
{
    public List<MapDetails> mapDeatilList;

    /// <summary>
    /// 根据名字返回声音数据
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MapDetails GetSoundDeatils(MapName name)
    {
        return mapDeatilList.Find(m => m.mapName == name);
    }
}

[System.Serializable]
public class MapDetails
{
    public MapName mapName;
    public int leftBorder;
    public int rightBorder;
}
