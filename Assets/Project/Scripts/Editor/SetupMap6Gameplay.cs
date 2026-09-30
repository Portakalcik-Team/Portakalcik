using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Portakalcik;

namespace Portakalcik.Editor
{
    [InitializeOnLoad]
    public static class SetupMap6Gameplay
    {
        static SetupMap6Gameplay()
        {
            EditorApplication.delayCall += Execute;
        }

        [MenuItem("Portakalcik/Setup Map 6 Complete Gameplay")]
        public static void Execute()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.name != "Map_6")
            {
                var scenePath = "Assets/Scenes/Map_6.unity";
                if (System.IO.File.Exists(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }

            // 1. Oyuncuyu Kur
            GameObject player = GameObject.Find("Player");
            if (player == null) player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(4f, 0.1f, -4f);
            player.transform.rotation = Quaternion.Euler(0, 0, 0);

            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc == null) cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.4f;
            cc.center = new Vector3(0, 0.9f, 0);
            cc.stepOffset = 0.3f;
            cc.slopeLimit = 45f;

            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc == null) pc = player.AddComponent<PlayerController>();
            pc.walkSpeed = 6.0f;
            pc.sprintSpeed = 9.0f;
            pc.maxHealth = 100f;
            pc.currentHealth = 100f;
            pc.mandalinaCount = 6;
            pc.portakalCount = 0;
            pc.limonCount = 0;

            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }
            mainCam.transform.parent = player.transform;
            mainCam.transform.localPosition = new Vector3(0, 1.6f, 0);
            mainCam.transform.localRotation = Quaternion.identity;
            pc.cameraHolder = mainCam.transform;

            // 2. Kapıları Kur
            Door[] allDoors = Object.FindObjectsByType<Door>(FindObjectsSortMode.None);
            GameObject doorsParent = GameObject.Find("Kapilar");
            if (doorsParent != null)
            {
                foreach (Transform child in doorsParent.transform)
                {
                    Door d = child.GetComponent<Door>();
                    if (d == null) d = child.gameObject.AddComponent<Door>();
                }
            }
            allDoors = Object.FindObjectsByType<Door>(FindObjectsSortMode.None);

            // 3. Ağaçları Kur (Denge Dokümanı Seviye 6 Verimleri)
            GameObject treesParent = GameObject.Find("Agaclar_YerTutucu");
            if (treesParent != null)
            {
                foreach (Transform child in treesParent.transform)
                {
                    FruitTree ft = child.GetComponent<FruitTree>();
                    if (ft == null) ft = child.gameObject.AddComponent<FruitTree>();

                    string nameLower = child.name.ToLower();
                    if (nameLower.Contains("mandalina"))
                    {
                        ft.treeType = TreeType.Mandalina;
                        ft.fruitYield = 11; // 5 + 6 = 11
                    }
                    else if (nameLower.Contains("portakal"))
                    {
                        ft.treeType = TreeType.Portakal;
                        ft.fruitYield = 3;  // Seviye 6 = 3
                    }
                    else if (nameLower.Contains("limon"))
                    {
                        ft.treeType = TreeType.Limon;
                        ft.fruitYield = 2;  // Seviye 6 = 2
                    }
                }
            }

            // 4. Canavarları Kur ve Kapılarla Eşleştir
            GameObject enemiesParent = GameObject.Find("Canavarlar_YerTutucu");
            if (enemiesParent != null)
            {
                foreach (Transform child in enemiesParent.transform)
                {
                    Enemy e = child.GetComponent<Enemy>();
                    if (e == null) e = child.gameObject.AddComponent<Enemy>();

                    string nameLower = child.name.ToLower();
                    if (nameLower.Contains("diken"))
                    {
                        e.enemyType = EnemyType.DikenBocegi;
                        e.monsterName = "Diken Böceği";
                        e.maxHealth = 11f;     // 6 + 5 = 11
                        e.currentHealth = 11f;
                        e.attackDamage = 9f;    // 4 + 5 = 9
                        e.moveSpeed = 2.4f;     // 0.4 * 6
                        e.attackCooldown = 2.0f;
                    }
                    else if (nameLower.Contains("yarasa"))
                    {
                        e.enemyType = EnemyType.GolgeYarasa;
                        e.monsterName = "Gölge Yarasa";
                        e.maxHealth = 14f;     // 9 + 5 = 14
                        e.currentHealth = 14f;
                        e.attackDamage = 11f;   // 6 + 5 = 11
                        e.moveSpeed = 7.2f;     // 1.2 * 6 (Hızlı!)
                        e.attackCooldown = 1.0f;
                    }

                    // En yakın kapıyı bul ve koruma kapısı olarak bağla
                    Door nearestDoor = null;
                    float minDoorDist = 12.0f;
                    foreach (var door in allDoors)
                    {
                        if (door != null)
                        {
                            float d = Vector3.Distance(child.position, door.transform.position);
                            if (d < minDoorDist)
                            {
                                minDoorDist = d;
                                nearestDoor = door;
                            }
                        }
                    }
                    e.guardedDoor = nearestDoor;
                }
            }

            // 5. Çıkış Kapısını Kur
            GameObject exitObj = GameObject.Find("Cikis_Bitis");
            if (exitObj != null)
            {
                ExitGate eg = exitObj.GetComponent<ExitGate>();
                if (eg == null) eg = exitObj.AddComponent<ExitGate>();
                eg.requiredOranges = 1;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("[Portakalcik] Harita 6 (Seviye 6) tam oynanış mekaniği başarıyla kuruldu!");
        }
    }
}
