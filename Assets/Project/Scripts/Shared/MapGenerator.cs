using UnityEngine;

/// <summary>
/// Harita text'ini parse edip tüm objeleri (duvar, yol, kapı, düşman, ağaç, oyuncu, çıkış)
/// otomatik olarak sahneye yerleştiren generator scripti.
/// 
/// Sembol Rehberi:
///   █ = Duvar (Wall)
///   . = Yol (Path)
///   S = Giriş / Oyuncu başlangıç noktası
///   E = Çıkış
///   D = Kapı (Door)
///   1 = Diken Böceği
///   2 = Gölge Yarasa
///   3 = Orman Tilkisi
///   4 = Kök Golemi
///   5 = Labirent Muhafızı
///   m = Mandalina Ağacı
///   p = Portakal Ağacı
///   l = Limon Ağacı
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("Harita Verisi")]
    [TextArea(15, 25)]
    public string mapText;

    [Header("Hücre Boyutu")]
    public float cellSize = 3f;

    [Header("Prefab Referansları")]
    public GameObject wallPrefab;
    public GameObject pathPrefab;
    public GameObject playerPrefab;
    public GameObject exitPrefab;
    public GameObject doorPrefab;

    [Header("Ağaç Prefab'ları")]
    public GameObject limonPrefab;
    public GameObject mandalinaPrefab;
    public GameObject portakalPrefab;

    [Header("Düşman Prefab'ları")]
    public GameObject dikenBocegiPrefab;
    public GameObject golgeYarasaPrefab;
    public GameObject ormanTilkisiPrefab;
    public GameObject kokGolemiPrefab;
    public GameObject labirentMuhafizPrefab;

    [Header("Ayarlar")]
    [Tooltip("Oluşturulan objelerin parent'ı. Boş bırakılırsa bu objenin altına eklenir.")]
    public Transform spawnParent;

    [Tooltip("Duvar yüksekliği (Y ekseni)")]
    public float wallHeight = 1.5f;

    [Tooltip("Oyun başladığında otomatik oluştur")]
    public bool generateOnStart = true;

    // Oluşturulan objeleri tutmak için container
    private Transform _generatedContainer;

    void Start()
    {
        if (generateOnStart)
        {
            GenerateMap();
        }
    }

    /// <summary>
    /// Haritayı parse edip objeleri oluşturur.
    /// Inspector'dan veya başka scriptlerden çağrılabilir.
    /// </summary>
    public void GenerateMap()
    {
        if (string.IsNullOrEmpty(mapText))
        {
            Debug.LogError("[MapGenerator] mapText boş! Harita text'ini Inspector'da doldurun.");
            return;
        }

        // Önceki oluşturulmuş objeleri temizle
        ClearGenerated();

        // Container oluştur
        Transform parent = spawnParent != null ? spawnParent : transform;
        GameObject container = new GameObject("GeneratedMap");
        container.transform.SetParent(parent);
        container.transform.localPosition = Vector3.zero;
        _generatedContainer = container.transform;

        // Alt container'lar
        Transform wallsContainer = CreateSubContainer("Walls");
        Transform pathsContainer = CreateSubContainer("Paths");
        Transform doorsContainer = CreateSubContainer("Doors");
        Transform treesContainer = CreateSubContainer("Trees");
        Transform enemiesContainer = CreateSubContainer("Enemies");

        // Haritayı satırlara böl
        string[] lines = mapText.Split('\n');
        int rowCount = lines.Length;

        // İstatistikler
        int wallCount = 0, pathCount = 0, doorCount = 0;
        int enemyCount = 0, treeCount = 0;

        for (int row = 0; row < rowCount; row++)
        {
            string line = lines[row].TrimEnd('\r'); // Windows satır sonu temizle

            // Satırdaki her karakteri oku (boşluk ayrıcı)
            // Harita formatı: "█ . █ . l . █" şeklinde boşlukla ayrılmış
            string[] cells = line.Split(' ');

            for (int col = 0; col < cells.Length; col++)
            {
                string cell = cells[col].Trim();
                if (string.IsNullOrEmpty(cell)) continue;

                // Pozisyon hesapla (X = sağa, Z = yukarı - Unity'de Z ileri)
                // Haritada üstten aşağı okunuyor, o yüzden Z'yi ters çeviriyoruz
                Vector3 position = new Vector3(col * cellSize, 0f, -row * cellSize);

                switch (cell)
                {
                    case "█":
                        SpawnObject(wallPrefab, position, wallsContainer, $"Wall_{row}_{col}",
                            new Vector3(0, wallHeight, 0));
                        wallCount++;
                        break;

                    case ".":
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_{row}_{col}");
                        pathCount++;
                        break;

                    case "S":
                        // Giriş noktası: hem yol hem oyuncu
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_Start_{row}_{col}");
                        SpawnObject(playerPrefab, position, _generatedContainer, "Player",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        break;

                    case "E":
                        // Çıkış noktası
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_Exit_{row}_{col}");
                        SpawnObject(exitPrefab, position, _generatedContainer, "Exit",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        break;

                    case "D":
                        // Kapı
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_Door_{row}_{col}");
                        SpawnObject(doorPrefab, position, doorsContainer, $"Door_{row}_{col}",
                            new Vector3(0, wallHeight, 0));
                        pathCount++;
                        doorCount++;
                        break;

                    case "l":
                        // Limon ağacı: yolun üzerine koy
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_Limon_{row}_{col}");
                        SpawnObject(limonPrefab, position, treesContainer, $"Limon_{row}_{col}",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        treeCount++;
                        break;

                    case "m":
                        // Mandalina ağacı
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_Mandalina_{row}_{col}");
                        SpawnObject(mandalinaPrefab, position, treesContainer, $"Mandalina_{row}_{col}",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        treeCount++;
                        break;

                    case "p":
                        // Portakal ağacı
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_Portakal_{row}_{col}");
                        SpawnObject(portakalPrefab, position, treesContainer, $"Portakal_{row}_{col}",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        treeCount++;
                        break;

                    case "1":
                        // Diken Böceği
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_DikenBocegi_{row}_{col}");
                        SpawnObject(dikenBocegiPrefab, position, enemiesContainer, $"DikenBocegi_{row}_{col}",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        enemyCount++;
                        break;

                    case "2":
                        // Gölge Yarasa
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_GolgeYarasa_{row}_{col}");
                        SpawnObject(golgeYarasaPrefab, position, enemiesContainer, $"GolgeYarasa_{row}_{col}",
                            new Vector3(0, 1f, 0));
                        pathCount++;
                        enemyCount++;
                        break;

                    case "3":
                        // Orman Tilkisi
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_OrmanTilkisi_{row}_{col}");
                        SpawnObject(ormanTilkisiPrefab, position, enemiesContainer, $"OrmanTilkisi_{row}_{col}",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        enemyCount++;
                        break;

                    case "4":
                        // Kök Golemi
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_KokGolemi_{row}_{col}");
                        SpawnObject(kokGolemiPrefab, position, enemiesContainer, $"KokGolemi_{row}_{col}",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        enemyCount++;
                        break;

                    case "5":
                        // Labirent Muhafızı
                        SpawnObject(pathPrefab, position, pathsContainer, $"Path_LabirentMuhafiz_{row}_{col}");
                        SpawnObject(labirentMuhafizPrefab, position, enemiesContainer, $"LabirentMuhafiz_{row}_{col}",
                            new Vector3(0, 0.5f, 0));
                        pathCount++;
                        enemyCount++;
                        break;

                    default:
                        // Bilinmeyen karakter — uyarı ver ama bozmayalım
                        if (cell != "" && cell != " ")
                        {
                            Debug.LogWarning($"[MapGenerator] Bilinmeyen sembol: '{cell}' ({row},{col})");
                        }
                        break;
                }
            }
        }

        Debug.Log($"[MapGenerator] Harita oluşturuldu! " +
                  $"Duvar: {wallCount}, Yol: {pathCount}, Kapı: {doorCount}, " +
                  $"Düşman: {enemyCount}, Ağaç: {treeCount}");
    }

    /// <summary>
    /// Bir objeyi sahneye yerleştirir. Prefab null ise uyarı verir.
    /// </summary>
    private void SpawnObject(GameObject prefab, Vector3 basePosition, Transform parent, string name,
        Vector3 offset = default)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[MapGenerator] '{name}' için prefab atanmamış! Atlanıyor...");
            return;
        }

        GameObject obj = Instantiate(prefab, basePosition + offset, Quaternion.identity, parent);
        obj.name = name;
    }

    /// <summary>
    /// Alt container oluşturur (Walls, Paths, vb.)
    /// </summary>
    private Transform CreateSubContainer(string name)
    {
        GameObject container = new GameObject(name);
        container.transform.SetParent(_generatedContainer);
        container.transform.localPosition = Vector3.zero;
        return container.transform;
    }

    /// <summary>
    /// Daha önce oluşturulmuş objeleri temizler.
    /// </summary>
    public void ClearGenerated()
    {
        if (_generatedContainer != null)
        {
            if (Application.isPlaying)
                Destroy(_generatedContainer.gameObject);
            else
                DestroyImmediate(_generatedContainer.gameObject);
            _generatedContainer = null;
        }

        // İsimle de ara (Editor'den tekrar çalıştırma durumu için)
        Transform parent = spawnParent != null ? spawnParent : transform;
        Transform existing = parent.Find("GeneratedMap");
        if (existing != null)
        {
            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
        }
    }
}
