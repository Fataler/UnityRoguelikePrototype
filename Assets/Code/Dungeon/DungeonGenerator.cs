using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Map Size")]
        public int mapWidth;

        public int mapHeight;

        [Header("Room Size")]
        public int widthMinRoom;

        public int widthMaxRoom;
        public int heightMinRoom;
        public int heightMaxRoom;

        [Header("Corridor Size")]
        public int minCorridorLength;

        public int maxCorridorLength;
        public int corridorWidth;

        [Header("Features")]
        public int maxFeatures;

        [Space]
        private int _countFeatures;

        public List<Feature> allFeatures;

        [Space]
        public bool isASCII;

        public void InitializeDungeon()
        {
            MapManager.map = new Tile[mapWidth, mapHeight];
        }

        public void GenerateDungeon()
        {
            GenerateFeatire(FeatureType.Room, new Wall(), true);
            for (int i = 0; i < 500; i++)
            {
                Feature originFeature;
                if (allFeatures.Count == 1)
                {
                    originFeature = allFeatures[0];
                }
                else
                {
                    originFeature = allFeatures[Random.Range(0, allFeatures.Count - 1)];
                }
                Wall wall = ChoseWall(originFeature);
                if (wall == null) continue;

                FeatureType type;
                if (originFeature.type == FeatureType.Room)
                {
                    type = FeatureType.Coridor;
                }
                else
                {
                    if (Random.Range(0, 100) < 90)
                    {
                        type = FeatureType.Room;
                    }
                    else
                    {
                        type = FeatureType.Coridor;
                    }
                }
                GenerateFeatire(type, wall);
                if (_countFeatures >= maxFeatures) break;
            }
            DrawMap(isASCII);
        }

        private void GenerateFeatire(FeatureType type, Wall wall, bool isFirst = false)
        {
            Feature room = new Feature
            {
                positions = new List<Vector2Int>()
            };
            int roomWidth = 0;
            int roomHeight = 0;

            if (type == FeatureType.Room)
            {
                //it's room
                roomWidth = Random.Range(widthMinRoom, widthMaxRoom);
                roomHeight = Random.Range(heightMinRoom, heightMaxRoom);
            }
            else
            {
                //or it's a corridor
                switch (wall.direction)
                {
                    case DirectionTypes.South:
                    case DirectionTypes.North:
                        roomWidth = corridorWidth;
                        roomHeight = Random.Range(minCorridorLength, maxCorridorLength);
                        break;

                    case DirectionTypes.East:
                    case DirectionTypes.West:
                        roomWidth = Random.Range(minCorridorLength, maxCorridorLength);
                        roomHeight = corridorWidth;
                        break;
                }
            }
            int xStartingPoint;
            int yStartingPoint;

            if (isFirst)
            {
                xStartingPoint = mapWidth / 2;
                yStartingPoint = mapHeight / 2;
            }
            else
            {
                int id;
                if (wall.positions.Count == 3)
                {
                    id = 1;
                }
                else
                {
                    id = Random.Range(1, wall.positions.Count - 2);
                }
                xStartingPoint = wall.positions[id].x;
                yStartingPoint = wall.positions[id].y;
            }
            Vector2Int lastWallPosition = new Vector2Int(xStartingPoint, yStartingPoint);
            if (isFirst)
            {
                xStartingPoint -= Random.Range(1, roomWidth);
                yStartingPoint -= Random.Range(1, roomHeight);
            }
            else
            {
                switch (wall.direction)
                {
                    case DirectionTypes.South:
                        if (type == FeatureType.Room)
                            xStartingPoint -= Random.Range(1, roomWidth - 2);
                        else xStartingPoint--;
                        yStartingPoint -= Random.Range(1, roomHeight - 2);
                        break;

                    case DirectionTypes.North:
                        if (type == FeatureType.Room)
                            xStartingPoint -= Random.Range(1, roomWidth - 2);
                        else xStartingPoint--;
                        yStartingPoint++;
                        break;

                    case DirectionTypes.West:
                        xStartingPoint -= roomWidth;
                        if (type == FeatureType.Room)
                            yStartingPoint -= Random.Range(1, roomHeight - 2);
                        else yStartingPoint--;
                        break;

                    case DirectionTypes.East:
                        xStartingPoint++;
                        if (type == FeatureType.Room)
                            yStartingPoint -= Random.Range(1, roomHeight - 2);
                        else yStartingPoint--;
                        break;
                }
            }
            if (!CheckIfHasSpace(
                new Vector2Int(xStartingPoint, yStartingPoint),
                new Vector2Int(xStartingPoint + roomWidth - 1, yStartingPoint + roomHeight - 1)))
            {
                return;
            }

            room.walls = new Wall[4];

            for (int i = 0; i < room.walls.Length; i++)
            {
                room.walls[i] = new Wall
                {
                    positions = new List<Vector2Int>(),
                    length = 0
                };

                switch (i)
                {
                    case 0:
                        room.walls[i].direction = DirectionTypes.South;
                        break;

                    case 1:
                        room.walls[i].direction = DirectionTypes.North;
                        break;

                    case 2:
                        room.walls[i].direction = DirectionTypes.West;
                        break;

                    case 3:
                        room.walls[i].direction = DirectionTypes.East;
                        break;
                }
            }

            for (int y = 0; y < roomHeight; y++)
            {
                for (int x = 0; x < roomWidth; x++)
                {
                    Vector2Int position = new Vector2Int
                    {
                        x = xStartingPoint + x,
                        y = yStartingPoint + y
                    };

                    room.positions.Add(position);

                    MapManager.map[position.x, position.y] = new Tile
                    {
                        xPosition = position.x,
                        yPosition = position.y,
                        type = TileType.Floor
                    };

                    if (y == 0)
                    {
                        room.walls[0].positions.Add(position);
                        room.walls[0].length++;
                        MapManager.map[position.x, position.y].type = TileType.Wall;
                    }
                    if (y == (roomHeight - 1))
                    {
                        room.walls[1].positions.Add(position);
                        room.walls[1].length++;
                        MapManager.map[position.x, position.y].type = TileType.Wall;
                    }
                    if (x == 0)
                    {
                        room.walls[2].positions.Add(position);
                        room.walls[2].length++;
                        MapManager.map[position.x, position.y].type = TileType.Wall;
                    }
                    if (x == (roomWidth - 1))
                    {
                        room.walls[3].positions.Add(position);
                        room.walls[3].length++;
                        MapManager.map[position.x, position.y].type = TileType.Wall;
                    }

                    if (MapManager.map[position.x, position.y].type != TileType.Wall)
                    {
                        MapManager.map[position.x, position.y].type = TileType.Floor;
                    }
                }
            }
            if (!isFirst)
            {
                MapManager.map[lastWallPosition.x, lastWallPosition.y].type = TileType.Floor;
                switch (wall.direction)
                {
                    case DirectionTypes.South:
                        MapManager.map[lastWallPosition.x, lastWallPosition.y - 1].type = TileType.Floor;
                        break;

                    case DirectionTypes.North:
                        MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].type = TileType.Floor;
                        break;

                    case DirectionTypes.West:
                        MapManager.map[lastWallPosition.x - 1, lastWallPosition.y].type = TileType.Floor;
                        break;

                    case DirectionTypes.East:
                        MapManager.map[lastWallPosition.x + 1, lastWallPosition.y].type = TileType.Floor;
                        break;
                }
            }
            room.width = roomWidth;
            room.height = roomHeight;
            room.type = type;

            allFeatures.Add(room);
            _countFeatures++;
        }

        private bool CheckIfHasSpace(Vector2Int start, Vector2 end)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                for (int x = start.x; x <= end.x; x++)
                {
                    if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight) return false;
                    if (MapManager.map[x, y] != null) return false;
                }
            }
            return true;
        }

        private Wall ChoseWall(Feature feature)
        {
            for (int i = 0; i < 10; i++)
            {
                int id = Random.Range(0, 100) / 25;
                if (!feature.walls[id].hasFeature)
                {
                    return feature.walls[id];
                }
            }
            return null;
        }

        /// <summary>
        /// Funcrion for rendering
        /// </summary>
        /// <param name="isASCII"></param>
        private void DrawMap(bool isASCII)
        {
            if (isASCII)
            {
                Text screen = GameObject.Find("ASCIITest").GetComponent<Text>();

                string asciiMap = "";

                for (int y = (mapHeight - 1); y >= 0; y--)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        if (MapManager.map[x, y] != null)
                        {
                            switch (MapManager.map[x, y].type)
                            {
                                case TileType.Wall:
                                    asciiMap += "#";
                                    break;

                                case TileType.Floor:
                                    asciiMap += ".";
                                    break;
                            }
                        }
                        else
                        {
                            asciiMap += " ";
                        }

                        if (x == (mapWidth - 1))
                        {
                            asciiMap += "\n";
                        }
                    }
                }

                screen.text = asciiMap;
            }
        }
    }
}