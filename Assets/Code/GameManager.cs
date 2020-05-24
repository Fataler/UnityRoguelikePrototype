using Assets.Code.Dungeon;
using UnityEngine;

namespace Assets.Code
{
    public class GameManager : MonoBehaviour
    {
        private DungeonGenerator dungeonGenerator;

        private void Start()
        {
            dungeonGenerator = GetComponent<DungeonGenerator>();
            dungeonGenerator.InitializeDungeon();
            dungeonGenerator.GenerateDungeon();
        }
    }
}