using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LayoutClass : ScriptableObject
{
    [System.Serializable]
    public struct Room
    {
        public RoomType roomType;

        //foradditionalConnections

        bool connectSpecific;
        public int connectorIndex;
    }

    public enum RoomType
    {
        StandardRoom,
        ChestRoom,
        Connector,
        BossRoom,
        StoreRoom
    }

    public List<Room> rooms = new List<Room>();


}
