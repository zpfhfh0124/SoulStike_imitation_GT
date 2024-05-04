using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

namespace GT
{
#if UNITY_EDITOR
    [CustomEditor(typeof(JsonDataManager))]
    public class JsonDataManagerButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            JsonDataManager jsonDataManager = (JsonDataManager)target;
            
            if(GUILayout.Button("SpawnData 설정"))
            {
                jsonDataManager.SetSpawnData();
            }

            if (GUILayout.Button("PlayerData 설정"))
            {
                jsonDataManager.SetPlayerData();
            }

            if (GUILayout.Button("EnemyData 설정"))
            {
                jsonDataManager.SetEnemyData();
            }

            if (GUILayout.Button("WeaponData 설정"))
            {
                jsonDataManager.SetWeaponData();
            }
        }
    }
#endif

    /// <summary>
    /// SpawnManager에서 쓰이는 데이터
    /// </summary>
    public class JsonDataManager : SingletonMB<JsonDataManager>
    {
        public readonly string FILEPATH_SPAWNDATA = "Assets/JSON/JSON_SpawnData.json";
        public readonly string FILEPATH_PLAYERDATA = "Assets/JSON/JSON_PlayerData.json";
        public readonly string FILEPATH_ENEMYDATA = "Assets/JSON/JSON_EnemyData.json";
        public readonly string FILEPATH_WEAPONDATA = "Assets/JSON/JSON_WeaponData.json";
        public SpawnData spawnData;
        public PlayerData playerData;
        public EnemyData[] enemyData;
        public WeaponData weaponData;

        public void SetSpawnData()
        {
            var json_spawnData = JsonConvert.SerializeObject(spawnData);
            Debug.Log(json_spawnData);
            File.WriteAllText(FILEPATH_SPAWNDATA, json_spawnData);
        }

        public void SetPlayerData()
        {
            var json_playerData = JsonConvert.SerializeObject(playerData);
            Debug.Log(json_playerData);
            File.WriteAllText(FILEPATH_PLAYERDATA, json_playerData);
        }

        public void SetEnemyData()
        {
            var json_enemyData = JsonConvert.SerializeObject(enemyData);
            Debug.Log(json_enemyData);
            File.WriteAllText(FILEPATH_ENEMYDATA, json_enemyData);
        }

        public void SetWeaponData()
        {
            var json_weaponData = JsonConvert.SerializeObject(weaponData);
            Debug.Log(json_weaponData);
            File.WriteAllText(FILEPATH_WEAPONDATA, json_weaponData);
        }
    }
}