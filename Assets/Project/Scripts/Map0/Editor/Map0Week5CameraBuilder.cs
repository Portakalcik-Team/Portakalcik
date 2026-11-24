using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 5. hafta: slayttaki 17 adımı Map_0'a uygular.
// Takip kamerası + çerçeve ayarları + sarsıntı + buzlu rampa için ikinci kamera.
// Değerler slaytlardaki başlangıç değerleri; neden bu sayılar: Map0_README.md "Feel notları".
public static class Map0Week5CameraBuilder
{
    const string PlayerName = "Map0_Week1_Oyuncu";
    const string FollowName = "Map0_Week5_CM_Follow";
    const string RampName = "Map0_Week5_CM_Ramp";
    const string ZoneName = "Map0_Week5_RampaBolgesi";

    [MenuItem("Map0/Hafta 5 Kamerayı Kur")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Map_0")
        {
            EditorUtility.DisplayDialog("Map0", "Önce Assets/Scenes/Map_0.unity sahnesini aç.", "Tamam");
            return;
        }

        GameObject player = GameObject.Find(PlayerName);
        Camera main = Camera.main;
        if (player == null || main == null)
        {
            EditorUtility.DisplayDialog("Map0", "Oyuncu ya da Main Camera bulunamadı.", "Tamam");
            return;
        }

        // tekrar çalıştırılabilsin: eskileri sil
        foreach (string old in new[] { FollowName, RampName, ZoneName })
        {
            GameObject found = FindIncludingInactive(old);
            if (found != null) Undo.DestroyObjectImmediate(found);
        }

        // Adım 3 + 16: Brain gerçek kamerayı yönetir, geçiş 1 sn Ease In Out
        CinemachineBrain brain = main.GetComponent<CinemachineBrain>();
        if (brain == null) brain = Undo.AddComponent<CinemachineBrain>(main.gameObject);
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 1f);

        // Adım 3–9: takip kamerası
        CinemachineCamera follow = CreateCamera(FollowName, player.transform, 40f, 14f);

        // Adım 13: rampa kamerası — daha yüksek ve dik, başta kapalı
        CinemachineCamera ramp = CreateCamera(RampName, player.transform, 60f, 16f);
        ramp.gameObject.SetActive(false);

        // Adım 15: rampa + platformu saran tetik kutusu
        GameObject zone = new GameObject(ZoneName);
        Undo.RegisterCreatedObjectUndo(zone, "Map0 Hafta 5");
        zone.transform.position = new Vector3(-3f, 2f, -15f);
        BoxCollider box = zone.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(12f, 4f, 3f); // alt koridor: x -9..3, rampa + platform
        Map0CameraZone camZone = zone.AddComponent<Map0CameraZone>();
        SerializedObject so = new SerializedObject(camZone);
        so.FindProperty("zoneCamera").objectReferenceValue = ramp.gameObject;
        so.ApplyModifiedPropertiesWithoutUndo();

        // Adım 10: kargoyu düşüren her tehlike sarsıntı gönderir
        int count = 0;
        foreach (Map0Hazard hazard in Object.FindObjectsByType<Map0Hazard>(FindObjectsInactive.Include))
        {
            AddShake(hazard.gameObject);
            count++;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = follow.gameObject;
        Debug.Log("Map0: Hafta 5 kamerası kuruldu (" + count + " tehlikeye sarsıntı eklendi). Kaydetmeyi unutma.");
    }

    static CinemachineCamera CreateCamera(string name, Transform target, float pitch, float distance)
    {
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Map0 Hafta 5");
        // Y dönüşü 0 kalmalı: W ekranda yukarı yürüsün
        go.transform.rotation = Quaternion.Euler(pitch, 0f, 0f);

        CinemachineCamera cam = go.AddComponent<CinemachineCamera>();
        cam.Target.TrackingTarget = target;
        LensSettings lens = cam.Lens;
        lens.FieldOfView = 60f;
        cam.Lens = lens;

        // Position Composer: mesafe, gecikme, ölü bölge, ileri bakış
        CinemachinePositionComposer composer = go.AddComponent<CinemachinePositionComposer>();
        composer.CameraDistance = distance;
        composer.DeadZoneDepth = 1f;
        composer.Damping = Vector3.one;

        ScreenComposerSettings comp = composer.Composition;
        comp.ScreenPosition = Vector2.zero;
        comp.DeadZone.Enabled = true;
        comp.DeadZone.Size = new Vector2(0.1f, 0.1f);
        comp.HardLimits.Enabled = false;
        composer.Composition = comp;

        LookaheadSettings look = composer.Lookahead;
        look.Enabled = true;
        look.Time = 0.4f;      // 5 m/s × 0.4 s = 2 m önü
        look.Smoothing = 10f;
        look.IgnoreY = true;   // zıplama kamerayı itmesin
        composer.Lookahead = look;

        // Adım 9: sarsıntıyı dinler
        CinemachineImpulseListener listener = go.AddComponent<CinemachineImpulseListener>();
        listener.Gain = 1f;
        listener.UseCameraSpace = true;
        return cam;
    }

    // tehlikeye Impulse Source ekler; prefab içindeyse prefab'ın kendisine yazar (Apply All gibi)
    public static void AddShake(GameObject hazard)
    {
        if (PrefabUtility.IsPartOfPrefabInstance(hazard))
        {
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(hazard);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);
            Map0Hazard inPrefab = contents.GetComponentInChildren<Map0Hazard>(true);
            if (inPrefab != null) Configure(GetOrAdd(inPrefab.gameObject));
            PrefabUtility.SaveAsPrefabAsset(contents, path);
            PrefabUtility.UnloadPrefabContents(contents);
            return;
        }
        Configure(GetOrAdd(hazard));
    }

    static CinemachineImpulseSource GetOrAdd(GameObject go)
    {
        CinemachineImpulseSource source = go.GetComponent<CinemachineImpulseSource>();
        return source != null ? source : go.AddComponent<CinemachineImpulseSource>();
    }

    // küçük (0.5) ve kısa (0.2 sn) bir aşağı vuruş: "portakalı kaybettin"
    static void Configure(CinemachineImpulseSource source)
    {
        source.ImpulseDefinition.ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;
        source.ImpulseDefinition.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump;
        source.ImpulseDefinition.ImpulseDuration = 0.2f;
        source.DefaultVelocity = new Vector3(0f, -0.5f, 0f);
        EditorUtility.SetDirty(source);
    }

    static GameObject FindIncludingInactive(string name)
    {
        foreach (GameObject root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name == name) return root;
        return null;
    }
}
