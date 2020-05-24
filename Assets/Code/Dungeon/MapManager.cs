using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Dungeon
{
    public enum DirectionTypes
    {
        West,
        East,
        North,
        South
    }

    public enum TileType
    {
        Wall,
        Floor
    }

    public enum FeatureType
    {
        Room,
        Coridor
    }

    public static class MapManager
    {
        public static Tile[,] map;
    }

    [Serializable]
    public class Tile
    { // Holds all the information for each tile on the map
        public int xPosition;
        public int yPosition;

        [NonSerialized]
        public GameObject baseObject; // the map game object attached to that position: a floor, a wall, etc.

        public TileType type; // The type of the tile, if it is wall, floor, etc
    }

    [Serializable]
    public class Wall
    { // A class for saving the wall information, for the dungeon generation algorithm
        public List<Vector2Int> positions;
        public DirectionTypes direction;
        public int length;
        public bool hasFeature = false;
    }

    [Serializable]
    public class Feature
    { // A class for saving the feature (corridor or room) information, for the dungeon generation algorithm
        public List<Vector2Int> positions;
        public Wall[] walls;
        public FeatureType type;
        public int width;
        public int height;
    }
}