#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Construye automáticamente la escena completa del humedal.
// En Unity: menú superior "Humedal > Construir escena del juego".
public static class HumedalSceneBuilder
{
    const string Root = "Assets/Humedal/";
    const string ScenePath = Root + "Escenas/Humedal.unity";

    // Rectángulos de agua (x, y, ancho, alto) en unidades del mundo. Generados a partir del mapa.
    static readonly float[,] WaterRects = {
            {-11.0f,7.875f,2.0f,0.25f},
            {-1.875f,7.875f,0.25f,0.25f},
            {-0.25f,7.875f,0.5f,0.25f},
            {3.375f,7.75f,5.25f,0.5f},
            {-11.375f,7.625f,1.25f,0.25f},
            {6.125f,6.25f,0.25f,3.0f},
            {-11.625f,7.25f,0.75f,0.5f},
            {3.75f,7.25f,4.5f,0.5f},
            {6.375f,6.5f,0.25f,2.0f},
            {7.75f,7.125f,0.5f,0.75f},
            {8.375f,7.0f,0.25f,1.0f},
            {7.0f,6.875f,1.0f,0.75f},
            {8.125f,7.0f,0.25f,0.5f},
            {8.625f,6.875f,0.25f,0.75f},
            {-11.75f,6.75f,0.5f,0.5f},
            {4.125f,6.625f,3.75f,0.75f},
            {9.0f,6.375f,0.5f,0.75f},
            {-11.875f,6.375f,0.25f,0.25f},
            {6.75f,6.375f,0.5f,0.25f},
            {9.75f,6.125f,1.0f,0.75f},
            {4.25f,6.125f,3.5f,0.25f},
            {6.625f,6.125f,0.25f,0.25f},
            {10.375f,5.5f,0.25f,1.5f},
            {4.75f,5.625f,2.5f,0.75f},
            {9.875f,5.5f,0.75f,0.5f},
            {11.125f,5.0f,1.25f,1.0f},
            {5.0f,5.125f,2.0f,0.25f},
            {10.125f,5.125f,0.25f,0.25f},
            {11.875f,4.75f,0.25f,1.0f},
            {-10.875f,4.375f,0.25f,1.25f},
            {5.375f,4.875f,1.25f,0.25f},
            {-11.125f,4.25f,0.25f,1.0f},
            {-10.625f,4.125f,0.25f,1.25f},
            {5.5f,4.625f,0.5f,0.25f},
            {-9.75f,4.0f,1.5f,1.0f},
            {5.375f,4.25f,0.25f,0.5f},
            {11.25f,4.375f,1.0f,0.25f},
            {-8.875f,2.5f,0.25f,3.5f},
            {-11.375f,3.875f,0.25f,0.25f},
            {5.0f,3.0f,0.5f,2.0f},
            {-6.375f,0.5f,1.75f,6.5f},
            {-3.125f,3.25f,0.75f,1.0f},
            {4.5f,1.375f,0.5f,4.75f},
            {-9.625f,3.125f,1.25f,0.75f},
            {-7.375f,0.375f,0.25f,6.25f},
            {-4.625f,2.125f,0.75f,2.75f},
            {-3.625f,2.0f,0.25f,3.0f},
            {-2.625f,3.25f,0.25f,0.5f},
            {-8.125f,1.875f,1.25f,2.75f},
            {-5.25f,1.75f,0.5f,3.0f},
            {-4.0f,1.875f,0.5f,2.75f},
            {3.625f,1.25f,1.25f,4.0f},
            {2.875f,1.125f,0.25f,3.75f},
            {5.625f,2.875f,0.75f,0.25f},
            {-9.25f,2.5f,0.5f,0.5f},
            {-3.25f,1.625f,0.5f,2.25f},
            {5.5f,2.5f,0.5f,0.5f},
            {1.875f,1.125f,1.75f,2.75f},
            {-9.125f,1.875f,0.25f,0.75f},
            {-2.75f,1.0f,0.5f,2.0f},
            {0.875f,0.75f,0.25f,2.5f},
            {4.875f,0.5f,0.25f,3.0f},
            {-9.375f,1.625f,0.25f,0.25f},
            {-2.375f,0.875f,0.25f,1.75f},
            {-1.5f,0.625f,0.5f,2.25f},
            {9.0f,1.375f,1.0f,0.75f},
            {10.5f,1.5f,0.5f,0.5f},
            {-2.0f,0.75f,0.5f,1.5f},
            {5.125f,0.25f,0.25f,2.5f},
            {6.125f,0.5f,0.25f,2.0f},
            {9.875f,1.375f,0.75f,0.25f},
            {10.875f,1.0f,0.25f,1.0f},
            {5.625f,0.375f,0.75f,1.75f},
            {6.875f,0.625f,1.25f,1.25f},
            {8.375f,0.75f,0.25f,1.0f},
            {10.625f,0.75f,0.25f,1.0f},
            {7.875f,0.625f,0.75f,0.75f},
            {8.875f,0.75f,0.75f,0.5f},
            {-4.875f,0.5f,0.25f,0.5f},
            {11.125f,0.625f,0.25f,0.25f},
            {-8.0f,0.25f,1.0f,0.5f},
            {-4.125f,0.375f,0.25f,0.25f},
            {-3.125f,0.25f,0.25f,0.5f},
            {8.75f,0.375f,0.5f,0.25f},
            {-5.375f,0.125f,0.25f,0.25f},
            {-7.875f,-0.625f,0.75f,1.25f},
            {-2.875f,-0.125f,0.25f,0.25f},
            {6.75f,-0.125f,1.0f,0.25f},
            {-5.375f,-0.375f,0.25f,0.25f},
            {1.125f,-0.375f,0.25f,0.25f},
            {2.5f,-0.375f,0.5f,0.25f},
            {6.625f,-0.375f,0.75f,0.25f},
            {2.625f,-0.625f,0.25f,0.25f},
            {5.5f,-0.625f,0.5f,0.25f},
            {-8.375f,-1.0f,0.25f,0.5f},
            {3.75f,-0.875f,1.0f,0.25f},
            {5.375f,-0.875f,0.25f,0.25f},
            {-7.75f,-1.75f,0.5f,1.0f},
            {-5.375f,-4.625f,0.25f,6.25f},
            {-5.125f,-4.625f,0.25f,5.75f},
            {-4.625f,-4.25f,0.75f,4.5f},
            {-7.625f,-2.375f,0.25f,0.25f},
            {-4.125f,-4.125f,0.25f,3.75f},
            {-6.25f,-3.0f,1.5f,0.5f},
            {-3.25f,-4.125f,1.5f,2.25f},
            {-6.125f,-3.625f,1.25f,0.75f},
            {-1.625f,-3.75f,0.25f,1.0f},
            {-2.125f,-3.875f,0.75f,0.75f},
            {-1.375f,-4.0f,0.25f,0.5f},
            {-7.625f,-4.125f,0.25f,0.25f},
            {-6.0f,-4.125f,1.0f,0.25f},
            {-1.125f,-4.25f,0.25f,0.5f},
            {-7.875f,-5.375f,0.25f,2.25f},
            {-5.875f,-6.125f,0.75f,3.75f},
            {-2.375f,-4.375f,0.25f,0.25f},
            {-8.25f,-5.5f,0.5f,2.0f},
            {10.0f,-5.0f,1.5f,1.0f},
            {-9.125f,-5.25f,1.25f,1.0f},
            {-7.625f,-5.625f,0.25f,1.75f},
            {-6.375f,-6.375f,0.25f,3.25f},
            {10.875f,-5.125f,0.25f,0.75f},
            {11.75f,-5.125f,0.5f,0.75f},
            {-10.25f,-5.625f,1.0f,1.25f},
            {-7.0f,-6.125f,1.0f,2.25f},
            {9.125f,-5.125f,0.25f,0.25f},
            {11.25f,-5.25f,0.5f,0.5f},
            {-10.875f,-6.125f,0.25f,1.75f},
            {-3.75f,-5.5f,0.5f,0.5f},
            {5.375f,-6.0f,1.75f,1.5f},
            {-11.5f,-6.75f,1.0f,2.5f},
            {4.25f,-5.875f,0.5f,0.75f},
            {6.375f,-6.75f,0.25f,2.5f},
            {9.875f,-5.625f,1.25f,0.25f},
            {11.875f,-5.625f,0.25f,0.25f},
            {-9.5f,-5.875f,0.5f,0.25f},
            {6.875f,-6.875f,0.75f,2.25f},
            {9.75f,-5.875f,1.0f,0.25f},
            {7.625f,-7.0f,0.75f,2.0f},
            {9.375f,-6.125f,0.75f,0.25f},
            {-10.5f,-6.375f,0.5f,0.25f},
            {4.375f,-6.375f,0.25f,0.25f},
            {8.75f,-6.5f,1.5f,0.5f},
            {-10.625f,-6.625f,0.25f,0.25f},
            {-4.75f,-6.625f,0.5f,0.25f},
            {-4.875f,-7.125f,0.25f,0.75f},
            {5.5f,-7.0f,1.5f,0.5f},
            {8.625f,-7.125f,1.25f,0.75f},
            {-6.75f,-7.375f,0.5f,0.25f},
            {5.625f,-7.625f,1.25f,0.75f},
            {-6.625f,-7.75f,0.25f,0.5f},
            {8.5f,-7.75f,1.0f,0.5f}
    };

    static Material spriteMat;
    static TMP_FontAsset font;
    static Sprite fxDestello, fxHoja, fxAgua, alerta, teclaE, sombra;
    static Transform worldRoot;

    [MenuItem("Humedal/Construir escena del juego")]
    public static void Build()
    {
        var tmpSettings = Resources.Load<TMP_Settings>("TMP Settings");
        if (tmpSettings == null || TMP_Settings.defaultFontAsset == null)
        {
            EditorUtility.DisplayDialog("Falta TextMeshPro",
                "Primero importa los recursos de TextMeshPro:\n\nWindow > TextMeshPro > Import TMP Essential Resources\n\nLuego vuelve a usar Humedal > Construir escena del juego.", "OK");
            return;
        }
        font = TMP_Settings.defaultFontAsset;

#if !ENABLE_INPUT_SYSTEM
        EditorUtility.DisplayDialog("Input System",
            "El juego usa el nuevo Input System. Ve a Edit > Project Settings > Player > Active Input Handling y elige 'Input System Package (New)' o 'Both'. Unity se reiniciará.", "Entendido");
#endif

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        ConfigureSprites();
        AssetDatabase.Refresh();

        spriteMat = FindUnlitSpriteMaterial();
        fxDestello = S("Sprites/UI/efecto_destello.png");
        fxHoja = S("Sprites/UI/efecto_hoja.png");
        fxAgua = S("Sprites/UI/efecto_agua.png");
        alerta = S("Sprites/UI/icono_alerta.png");
        teclaE = S("Sprites/UI/tecla_e.png");
        sombra = S("Sprites/UI/sombra.png");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ---------- Mapa ----------
        var map = new GameObject("Mapa");
        var mapSr = map.AddComponent<SpriteRenderer>();
        mapSr.sprite = S("Sprites/Escenario/mapa_humedal.png");
        mapSr.sortingOrder = -32000;
        Mat(mapSr);

        var water = new GameObject("Colisiones_Agua");
        for (int i = 0; i < WaterRects.GetLength(0); i++)
        {
            var bc = water.AddComponent<BoxCollider2D>();
            bc.offset = new Vector2(WaterRects[i, 0], WaterRects[i, 1]);
            bc.size = new Vector2(WaterRects[i, 2], WaterRects[i, 3]);
        }
        // Bordes del mapa
        Vector2 half = mapSr.sprite.bounds.extents;
        AddWall(water, new Vector2(0, half.y + 0.5f), new Vector2(half.x * 2 + 2, 1));
        AddWall(water, new Vector2(0, -half.y - 0.5f), new Vector2(half.x * 2 + 2, 1));
        AddWall(water, new Vector2(half.x + 0.5f, 0), new Vector2(1, half.y * 2 + 2));
        AddWall(water, new Vector2(-half.x - 0.5f, 0), new Vector2(1, half.y * 2 + 2));

        worldRoot = new GameObject("Objetos_Interactuables").transform;

        // ---------- Jugador ----------
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-0.375f, -3.875f, 0);
        var psr = player.AddComponent<SpriteRenderer>();
        psr.sprite = S("Sprites/Personaje/caminar_abajo_0.png");
        Mat(psr);
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var pcol = player.AddComponent<BoxCollider2D>();
        pcol.size = new Vector2(0.5f, 0.25f);
        pcol.offset = new Vector2(0f, 0.14f);
        player.AddComponent<ToolManager>();
        var pc = player.AddComponent<PlayerController>();
        var so = new SerializedObject(pc);
        so.FindProperty("spriteRenderer").objectReferenceValue = psr;
        SetFrames(so, "walkDown", "abajo");
        SetFrames(so, "walkUp", "arriba");
        SetFrames(so, "walkRight", "derecha");
        SetFrames(so, "walkLeft", "izquierda");
        so.FindProperty("pinzaPose").objectReferenceValue = S("Sprites/Personaje/accion_pinza.png");
        so.FindProperty("sembrarPose").objectReferenceValue = S("Sprites/Personaje/accion_sembrar.png");
        so.FindProperty("agacharPose").objectReferenceValue = S("Sprites/Personaje/accion_agachar.png");
        so.FindProperty("celebrarPose").objectReferenceValue = S("Sprites/Personaje/accion_celebrar.png");
        so.ApplyModifiedPropertiesWithoutUndo();
        player.AddComponent<YSort>();
        AddShadow(player.transform, 0.9f);

        // ---------- Objetos del humedal ----------
            Basura("bolsa", new Vector2(-8.375f,-2.875f), "Sprites/Basura/bolsa.png", "basura/bolsas.mp3", 0f, 2f, "la bolsa de basura");
            Basura("caja", new Vector2(5.875f,-2.375f), "Sprites/Basura/caja.png", "basura/caja.mp3", 0f, 2f, "la caja de cartón");
            Basura("lata", new Vector2(3.375f,4.625f), "Sprites/Basura/lata.png", "basura/latas.mp3", 0.35f, 2f, "la lata");
            Basura("papel", new Vector2(-0.375f,6.125f), "Sprites/Basura/papel.png", "basura/papel.mp3", 0f, 1f, "el papel");
            Basura("plastico", new Vector2(-9.125f,6.125f), "Sprites/Basura/plastico.png", "basura/plastico.mp3", 1.75f, 2f, "la botella de plástico");
            Basura("vidrio", new Vector2(-3.125f,-1.375f), "Sprites/Basura/vidrio.png", "basura/vidrio.mp3", 0f, 2f, "la botella de vidrio");
            Animal("rana", new Vector2(-3.875f,-0.875f), new[]{"Sprites/Animales/rana_0.png", "Sprites/Animales/rana_1.png"}, "Sprites/Animales/rana_feliz.png", "animales/rana.mp3", 0f, 2.5f, "la rana", false);
            Animal("sapo", new Vector2(-10.375f,0.375f), new[]{"Sprites/Animales/sapo_0.png", "Sprites/Animales/sapo_1.png"}, "Sprites/Animales/sapo_feliz.png", "animales/sapo.mp3", 0f, 3f, "el sapo", false);
            Animal("capibara", new Vector2(9.625f,3.375f), new[]{"Sprites/Animales/capibara_0.png", "Sprites/Animales/capibara_1.png"}, "Sprites/Animales/capibara_feliz.png", "animales/capibara.mp3", 0.75f, 2.5f, "el capibara", false);
            Animal("grillo", new Vector2(6.625f,-4.625f), new[]{"Sprites/Animales/grillo_0.png", "Sprites/Animales/grillo_1.png"}, "Sprites/Animales/grillo_feliz.png", "animales/grillo.mp3", 0f, 2.5f, "el grillo", false);
            Animal("pez", new Vector2(1.203f,0.656f), new[]{"Sprites/Animales/pez_0.png"}, "Sprites/Animales/pez_0.png", "animales/peces agua.mp3", 0f, 2.5f, "el pez", true);
            Planta("siembra_1", new Vector2(-2.625f,5.375f), "Sprites/Plantas/planta_flores_rosadas.png");
            Planta("siembra_2", new Vector2(2.125f,4.125f), "Sprites/Plantas/planta_junco_blanco.png");
            Planta("siembra_3", new Vector2(5.375f,-1.625f), "Sprites/Plantas/planta_flores_amarillas.png");
            Planta("siembra_4", new Vector2(2.625f,-4.625f), "Sprites/Plantas/planta_juncos_altos.png");
            Planta("siembra_5", new Vector2(-10.875f,-2.125f), "Sprites/Plantas/planta_junco.png");
            Planta("siembra_6", new Vector2(-2.875f,-5.875f), "Sprites/Plantas/planta_pasto.png");

        // ---------- Cámara ----------
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.18f, 0.12f);
        camGo.AddComponent<AudioListener>();
        var follow = camGo.AddComponent<CameraFollow>();
        var fso = new SerializedObject(follow);
        fso.FindProperty("target").objectReferenceValue = player.transform;
        fso.FindProperty("mapBounds").objectReferenceValue = mapSr;
        fso.ApplyModifiedPropertiesWithoutUndo();

        // ---------- Managers ----------
        var gm = new GameObject("GameManager");
        var gmc = gm.AddComponent<GameManager>();
        gm.AddComponent<LevelManager>();
        var gso = new SerializedObject(gmc);
        gso.FindProperty("targetTrash").intValue = 6;
        gso.FindProperty("targetPlants").intValue = 6;
        gso.FindProperty("targetAnimals").intValue = 5;
        gso.ApplyModifiedPropertiesWithoutUndo();

        BuildSoundManager();
        BuildUI();

        Directory(Root + "Escenas");
        EditorSceneManager.SaveScene(scene, ScenePath);
        var list = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(ScenePath, true) };
        foreach (var s in EditorBuildSettings.scenes) if (s.path != ScenePath) list.Add(s);
        EditorBuildSettings.scenes = list.ToArray();

        EditorUtility.DisplayDialog("¡Listo!", "La escena Humedal se creó en Assets/Humedal/Escenas.\n\nPresiona Play para jugar.", "Jugar");
    }

    // ================== Objetos ==================
    static void Basura(string name, Vector2 pos, string sprite, string sound, float start, float max, string display)
    {
        var go = Item("Basura_" + name, pos, S(sprite), InteractableType.Basura, out var sr, out var so);
        so.FindProperty("displayName").stringValue = display;
        so.FindProperty("itemSound").objectReferenceValue = Clip(sound);
        so.FindProperty("soundStart").floatValue = start;
        so.FindProperty("soundMaxDuration").floatValue = max;
        so.FindProperty("effectSprite").objectReferenceValue = fxDestello;
        so.ApplyModifiedPropertiesWithoutUndo();
        AddShadow(go.transform, 0.7f);
    }

    static void Planta(string name, Vector2 pos, string grown)
    {
        var go = Item("Planta_" + name, pos, S("Sprites/Plantas/tierra_para_sembrar.png"), InteractableType.Planta, out var sr, out var so);
        so.FindProperty("displayName").stringValue = "el espacio para sembrar";
        so.FindProperty("doneSprite").objectReferenceValue = S(grown);
        so.FindProperty("effectSprite").objectReferenceValue = fxHoja;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void Animal(string name, Vector2 pos, string[] idle, string happy, string sound, float start, float max, string display, bool inWater)
    {
        var go = Item("Animal_" + name, pos, S(idle[0]), InteractableType.Animal, out var sr, out var so);
        so.FindProperty("displayName").stringValue = display;
        so.FindProperty("itemSound").objectReferenceValue = Clip(sound);
        so.FindProperty("soundStart").floatValue = start;
        so.FindProperty("soundMaxDuration").floatValue = max;
        var frames = so.FindProperty("idleFrames");
        frames.arraySize = idle.Length;
        for (int i = 0; i < idle.Length; i++) frames.GetArrayElementAtIndex(i).objectReferenceValue = S(idle[i]);
        so.FindProperty("doneSprite").objectReferenceValue = S(happy);
        so.FindProperty("effectSprite").objectReferenceValue = inWater ? fxAgua : fxDestello;
        if (inWater) so.FindProperty("bobAmount").floatValue = 0.06f;

        float top = sr.sprite != null ? sr.sprite.bounds.size.y : 0.6f;
        var al = Child(go.transform, "Alerta", alerta, new Vector3(0, top + 0.35f, 0), 0.5f, 31000);
        so.FindProperty("alertIcon").objectReferenceValue = al;
        so.ApplyModifiedPropertiesWithoutUndo();
        if (!inWater) AddShadow(go.transform, 0.8f);
    }

    static GameObject Item(string name, Vector2 pos, Sprite sprite, InteractableType type, out SpriteRenderer sr, out SerializedObject so)
    {
        var go = new GameObject(name);
        go.transform.SetParent(worldRoot);
        go.transform.position = pos;
        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        Mat(sr);
        go.AddComponent<YSort>();
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 1.0f;
        col.offset = new Vector2(0f, 0.3f);
        var it = go.AddComponent<InteractableItem>();
        so = new SerializedObject(it);
        so.FindProperty("type").enumValueIndex = (int)type;
        so.FindProperty("spriteRenderer").objectReferenceValue = sr;
        float top = sprite != null ? sprite.bounds.size.y : 0.6f;
        var prompt = Child(go.transform, "Tecla_E", teclaE, new Vector3(0.55f, top + 0.15f, 0), 0.6f, 32000);
        prompt.SetActive(false);
        so.FindProperty("promptIcon").objectReferenceValue = prompt;
        return go;
    }

    static GameObject Child(Transform parent, string name, Sprite sprite, Vector3 localPos, float scale, int order)
    {
        var c = new GameObject(name);
        c.transform.SetParent(parent, false);
        c.transform.localPosition = localPos;
        c.transform.localScale = Vector3.one * scale;
        var sr = c.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = order;
        Mat(sr);
        return c;
    }

    static void AddShadow(Transform parent, float width)
    {
        var c = Child(parent, "Sombra", sombra, new Vector3(0, 0.03f, 0), width, 0);
        var ys = c.AddComponent<YSort>();
        var yso = new SerializedObject(ys);
        yso.FindProperty("offset").intValue = -5;
        yso.ApplyModifiedPropertiesWithoutUndo();
    }

    static void AddWall(GameObject go, Vector2 offset, Vector2 size)
    {
        var bc = go.AddComponent<BoxCollider2D>();
        bc.offset = offset;
        bc.size = size;
    }

    static void SetFrames(SerializedObject so, string prop, string dir)
    {
        var p = so.FindProperty(prop);
        p.arraySize = 9;
        for (int i = 0; i < 9; i++)
            p.GetArrayElementAtIndex(i).objectReferenceValue = S("Sprites/Personaje/caminar_" + dir + "_" + i + ".png");
    }

    // ================== Sonido ==================
    static void BuildSoundManager()
    {
        var go = new GameObject("SoundManager");
        var sfx = go.AddComponent<AudioSource>(); sfx.playOnAwake = false;
        var bgm = go.AddComponent<AudioSource>(); bgm.playOnAwake = false; bgm.volume = 0.35f; bgm.loop = true;
        var amb = go.AddComponent<AudioSource>(); amb.playOnAwake = false; amb.volume = 0.2f; amb.loop = true;
        var sm = go.AddComponent<SoundManager>();
        var so = new SerializedObject(sm);
        so.FindProperty("sfxSource").objectReferenceValue = sfx;
        so.FindProperty("bgmSource").objectReferenceValue = bgm;
        so.FindProperty("ambientSource").objectReferenceValue = amb;
        so.FindProperty("trashSound").objectReferenceValue = Clip("herramientas/pinzas.mp3");
        so.FindProperty("plantSound").objectReferenceValue = Clip("arreglo planta/plantas.mp3");
        so.FindProperty("animalSound").objectReferenceValue = Clip("efectos/curar animal.mp3");
        so.FindProperty("winSound").objectReferenceValue = Clip("efectos/humedla_complet.mp3");
        so.FindProperty("goalSound").objectReferenceValue = Clip("efectos/destello_logro.mp3");
        so.FindProperty("pinzaSound").objectReferenceValue = Clip("herramientas/pinzas.mp3");
        so.FindProperty("palaSound").objectReferenceValue = Clip("herramientas/pala.mp3");
        so.FindProperty("backgroundMusic").objectReferenceValue = Clip("fondo/c. fondo.mp3");
        so.FindProperty("ambientLoop").objectReferenceValue = Clip("efectos/agua_fluyendo.mp3");
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ================== Interfaz ==================
    static readonly Color Cream = new Color(1f, 0.95f, 0.8f);
    static readonly Color Brown = new Color(0.3f, 0.19f, 0.09f);
    static readonly Color DarkGreen = new Color(0.16f, 0.36f, 0.12f);

    static void BuildUI()
    {
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        var ui = canvasGo.AddComponent<UIManager>();
        var so = new SerializedObject(ui);
        RectTransform c = canvasGo.GetComponent<RectTransform>();

        // Retrato y contadores (arriba a la izquierda)
        Img(c, "Retrato", "retrato", new Vector2(0, 1), new Vector2(24, -20), new Vector2(170, 146), false);
        string[] panels = { "panel_basura", "panel_plantas", "panel_animales" };
        string[] props = { "trashText", "plantsText", "animalsText" };
        for (int i = 0; i < 3; i++)
        {
            var p = Img(c, "Contador_" + panels[i], panels[i], new Vector2(0, 1), new Vector2(205, -22 - i * 88), new Vector2(260, 84), false);
            var t = Txt(p, "Texto", "0 / 0", 38, Cream, TextAlignmentOptions.Center, FontStyles.Bold);
            Stretch(t.rectTransform, new Vector2(0.36f, 0.08f), new Vector2(0.97f, 0.92f));
            so.FindProperty(props[i]).objectReferenceValue = t;
        }

        // Barra de herramientas (abajo al centro)
        var bar = Img(c, "Barra_Herramientas", "panel_grande", new Vector2(0.5f, 0), new Vector2(0, 14), new Vector2(520, 190), true);
        bar.pivot = new Vector2(0.5f, 0);
        bar.anchoredPosition = new Vector2(0, 14);
        string[] icons = { "icono_pinza", "icono_pala", "icono_mano" };
        string[] names = { "1  Pinza", "2  Pala", "3  Mano" };
        var iconProp = so.FindProperty("toolIcons");
        var frameProp = so.FindProperty("toolFrames");
        iconProp.arraySize = 3; frameProp.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            float x = (i - 1) * 150f;
            var frame = Img(bar, "Marco_" + i, "marco_seleccion", new Vector2(0.5f, 0.5f), new Vector2(x, 18), new Vector2(128, 128), false);
            frame.gameObject.SetActive(false);
            var icon = Img(bar, "Herramienta_" + i, icons[i], new Vector2(0.5f, 0.5f), new Vector2(x, 18), new Vector2(110, 105), false);
            var label = Txt(bar, "Nombre_" + i, names[i], 26, Brown, TextAlignmentOptions.Center, FontStyles.Bold);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            label.rectTransform.sizeDelta = new Vector2(150, 34);
            label.rectTransform.anchoredPosition = new Vector2(x, -60);
            iconProp.GetArrayElementAtIndex(i).objectReferenceValue = icon.GetComponent<Image>();
            frameProp.GetArrayElementAtIndex(i).objectReferenceValue = frame.gameObject;
        }

        // Pista de interacción (sobre la barra)
        var hint = Img(c, "Pista", "panel_grande", new Vector2(0.5f, 0), new Vector2(0, 215), new Vector2(820, 80), true);
        hint.pivot = new Vector2(0.5f, 0); hint.anchoredPosition = new Vector2(0, 215);
        var hintT = Txt(hint, "Texto", "", 28, Brown, TextAlignmentOptions.Center, FontStyles.Bold);
        Stretch(hintT.rectTransform, new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f));
        so.FindProperty("hintRoot").objectReferenceValue = hint.gameObject;
        so.FindProperty("hintText").objectReferenceValue = hintT;

        // Mensajes (arriba al centro)
        var msg = Img(c, "Mensaje", "panel_grande", new Vector2(0.5f, 1), new Vector2(0, -24), new Vector2(860, 96), true);
        msg.pivot = new Vector2(0.5f, 1); msg.anchoredPosition = new Vector2(0, -24);
        var msgT = Txt(msg, "Texto", "", 32, Brown, TextAlignmentOptions.Center, FontStyles.Bold);
        Stretch(msgT.rectTransform, new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f));
        so.FindProperty("messageRoot").objectReferenceValue = msg.gameObject;
        so.FindProperty("messageText").objectReferenceValue = msgT;

        // Pantalla de inicio
        var intro = Overlay(c, "Panel_Inicio");
        var ip = Img(intro, "Panel", "panel_grande", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1180, 700), true);
        Title(ip, "¡Restaura el humedal!", 66, DarkGreen, -60);
        Body(ip, "El humedal está lleno de basura y sus animales necesitan ayuda.\n\n" +
                 "<b>WASD / flechas</b>: moverte      <b>E</b>: interactuar      <b>R</b>: reiniciar\n" +
                 "<b>1 Pinza</b>: recoger basura    <b>2 Pala</b>: sembrar    <b>3 Mano</b>: ayudar animales", 30, -150, 220);
        Stages(ip, -400, false);
        var play = Img(ip, "Boton", "boton_play", new Vector2(0.5f, 0), new Vector2(-230, 70), new Vector2(80, 80), false);
        var pt = Txt(ip, "Comenzar", "Presiona ENTER para comenzar", 34, DarkGreen, TextAlignmentOptions.Left, FontStyles.Bold);
        pt.rectTransform.anchorMin = pt.rectTransform.anchorMax = new Vector2(0.5f, 0);
        pt.rectTransform.sizeDelta = new Vector2(520, 60);
        pt.rectTransform.anchoredPosition = new Vector2(80, 70);
        so.FindProperty("introPanel").objectReferenceValue = intro.gameObject;

        // Pantalla de victoria
        var win = Overlay(c, "Panel_Victoria");
        var wp = Img(win, "Panel", "panel_grande", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100, 640), true);
        Title(wp, "¡Humedal restaurado!", 70, DarkGreen, -60);
        Body(wp, "Recogiste la basura, sembraste plantas nativas y ayudaste a los animales.\n¡Gracias por cuidar el humedal!", 32, -160, 130);
        Stages(wp, -330, true);
        var wt = Txt(wp, "Reiniciar", "Presiona R para jugar de nuevo", 32, Brown, TextAlignmentOptions.Center, FontStyles.Bold);
        wt.rectTransform.anchorMin = wt.rectTransform.anchorMax = new Vector2(0.5f, 0);
        wt.rectTransform.sizeDelta = new Vector2(700, 50);
        wt.rectTransform.anchoredPosition = new Vector2(0, 60);
        so.FindProperty("winPanel").objectReferenceValue = win.gameObject;
        win.gameObject.SetActive(false);

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static RectTransform Overlay(RectTransform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        Stretch(rt, Vector2.zero, Vector2.one);
        go.GetComponent<Image>().color = new Color(0, 0, 0, 0.55f);
        return rt;
    }

    static void Title(RectTransform p, string text, int size, Color col, float y)
    {
        var t = Txt(p, "Titulo", text, size, col, TextAlignmentOptions.Center, FontStyles.Bold);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, 1);
        t.rectTransform.sizeDelta = new Vector2(1000, 90);
        t.rectTransform.anchoredPosition = new Vector2(0, y - 30);
    }

    static void Body(RectTransform p, string text, int size, float y, float h)
    {
        var t = Txt(p, "Texto", text, size, Brown, TextAlignmentOptions.Center, FontStyles.Normal);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, 1);
        t.rectTransform.sizeDelta = new Vector2(1000, h);
        t.rectTransform.anchoredPosition = new Vector2(0, y - h / 2f);
    }

    static void Stages(RectTransform p, float y, bool three)
    {
        string[] st = three ? new[] { "etapa_sucio", "etapa_medio", "etapa_limpio" } : new[] { "etapa_sucio", "etapa_limpio" };
        float w = 210, gap = 80;
        float total = st.Length * w + (st.Length - 1) * gap;
        for (int i = 0; i < st.Length; i++)
        {
            float x = -total / 2f + w / 2f + i * (w + gap);
            Img(p, "Etapa_" + i, st[i], new Vector2(0.5f, 1), new Vector2(x, y), new Vector2(w, 133), false);
            if (i < st.Length - 1)
                Img(p, "Flecha_" + i, "flecha", new Vector2(0.5f, 1), new Vector2(x + w / 2f + gap / 2f, y), new Vector2(50, 40), false);
        }
    }

    static RectTransform Img(RectTransform parent, string name, string sprite, Vector2 anchor, Vector2 pos, Vector2 size, bool sliced)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = new Vector2(anchor.x == 0 ? 0 : (anchor.x == 1 ? 1 : 0.5f), anchor.y == 1 ? 1 : (anchor.y == 0 ? 0 : 0.5f));
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = go.GetComponent<Image>();
        img.sprite = S("Sprites/UI/" + sprite + ".png");
        img.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
        img.preserveAspect = !sliced;
        if (sliced && size.y < 150f) img.pixelsPerUnitMultiplier = 1.7f;
        img.raycastTarget = false;
        return rt;
    }

    static TextMeshProUGUI Txt(RectTransform parent, string name, string text, float size, Color col, TextAlignmentOptions align, FontStyles style)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.font = font;
        t.text = text;
        t.fontSize = size;
        t.color = col;
        t.alignment = align;
        t.fontStyle = style;
        t.raycastTarget = false;
        t.richText = true;
        return t;
    }

    static void Stretch(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min; rt.anchorMax = max;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    // ================== Importación de sprites ==================
    [MenuItem("Humedal/Reconfigurar sprites")]
    public static void ConfigureSprites()
    {
        foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Root + "Sprites" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;

            bool ui = path.Contains("/UI/");
            bool map = path.Contains("/Escenario/");
            float ppu = 100f;
            if (path.Contains("/Personaje/")) ppu = 100f;
            else if (path.Contains("/Basura/")) ppu = 130f;
            else if (path.Contains("/Animales/")) ppu = 100f;
            else if (path.Contains("/Plantas/")) ppu = 60f;
            else if (map) ppu = 64f;

            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = ppu;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.maxTextureSize = 2048;

            var s = new TextureImporterSettings();
            ti.ReadTextureSettings(s);
            s.spriteAlignment = (int)((ui || map) ? SpriteAlignment.Center : SpriteAlignment.BottomCenter);
            s.spriteMeshType = SpriteMeshType.FullRect;
            ti.SetTextureSettings(s);

            if (path.EndsWith("panel_grande.png")) ti.spriteBorder = new Vector4(64, 56, 64, 56);
            ti.SaveAndReimport();
        }
    }

    // ================== Utilidades ==================
    static Sprite S(string rel)
    {
        var sp = AssetDatabase.LoadAssetAtPath<Sprite>(Root + rel);
        if (sp == null) Debug.LogWarning("No se encontró el sprite: " + Root + rel);
        return sp;
    }

    static AudioClip Clip(string rel)
    {
        var c = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "Audio/" + rel);
        if (c == null) Debug.LogWarning("No se encontró el audio: " + Root + "Audio/" + rel);
        return c;
    }

    static void Mat(SpriteRenderer sr)
    {
        if (spriteMat != null) sr.sharedMaterial = spriteMat;
    }

    // En URP se usa el material "Sprite-Unlit-Default" para que se vea sin luces 2D
    static Material FindUnlitSpriteMaterial()
    {
        if (GraphicsSettings.currentRenderPipeline == null) return null;
        foreach (var guid in AssetDatabase.FindAssets("Sprite-Unlit-Default t:Material"))
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            if (m != null) return m;
        }
        return null;
    }

    static void Directory(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets/Humedal", "Escenas");
    }
}
#endif
