using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using SFCore;
using System.Linq;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using FrogCore;
using HutongGames.PlayMaker;

namespace FartKnight;
public class FartHandler : MonoBehaviour
{
    private static GameObject fartPrehab;
    
    private int activeIndex = 0;
    private AudioClip _fartClip;
    private AudioSource _audioSource;
    private tk2dSpriteCollectionData _fartSC;
    private GameObject fartObject;

    private static int MAX_INSTANCES = 3;
    private static float TIME_INBETWEEN = 1.0f;
    private static bool ACTIVE = false;

    private void Start()
    {
        // Create prefab
        CreateCollections();
        base.StartCoroutine(CreateFartPrefab());
        _audioSource = HeroController.instance.gameObject.GetComponent<AudioSource>();
        _fartClip = LoadAudioClip();
    }

    // Runner (Call from mod class)
    public void Run()
    {

        if (activeIndex < MAX_INSTANCES && !ACTIVE)
        {
            ACTIVE = true;
            Modding.Logger.Log("[Fart Knight] Fart Trigger ", FartKnight.GS.LogLevel);
            // Run Hollow Knight animation
            HeroController.instance.GetComponent<KnightHandler>().Run();
            // Spawn collider
            StartCoroutine(CreateFartAnimation());
            // Play SFX (turned on/off via the global settings)
            if (FartKnight.GS.EnableSound)
            {
                _audioSource.PlayOneShot(_fartClip);
            }
            // Set ACTIVE false
            StartCoroutine(Disable());
        }
    }

    // Create the fart animation sprite
    // https://github.com/TTacco/Hollow-Point/blob/4498cc599c0770194993f811e592ec29ead06590/HollowPoint/AttackHandler.cs#L295
    private IEnumerator CreateFartAnimation()
    {
        activeIndex++;
        GameObject fart = SpawnFartFromKnight();
        yield return PlaySpriteAnimation(fart);
        Destroy(fart);
        activeIndex--;
        yield break;
    }

    private IEnumerator PlaySpriteAnimation(GameObject fart)
    {
        Modding.Logger.Log("[Fart Knight] PLAYING ANIMATION", FartKnight.GS.LogLevel);
        // Hooks up to the objects tk2dSpriteAnimation on add? Idk somehow
        tk2dSpriteAnimator _anim = fart.GetComponent<tk2dSpriteAnimator>();

        Modding.Logger.Log("[Fart Knight] Animation Clip: " + _anim.Library.clips[0].name, FartKnight.GS.LogLevel);
        Modding.Logger.Log("[Fart Knight] Animation Frames: " + _anim.Library.clips[0].frames.Length, FartKnight.GS.LogLevel);

        yield return _anim.PlayAnimWait("FartAnimation");
        Modding.Logger.Log("[Fart Knight] Animation Complete", FartKnight.GS.LogLevel);
    }

    private IEnumerator Disable()
    {
        // wait for our amount of time to re-enable
        yield return new WaitForSeconds(TIME_INBETWEEN);
        ACTIVE = false;
    }

    // https://github.com/TTacco/Hollow-Point/blob/master/HollowPoint/HollowPointPrefabs.cs#L269
    private GameObject SpawnFartFromKnight()
    {
        // Hollow knight direction
        float directionMultiplierX = (HeroController.instance.cState.facingRight) ? 1f : -1f;
        float wallClimbMultiplier = (HeroController.instance.cState.wallSliding) ? -1f : 1f;

        directionMultiplierX *= wallClimbMultiplier;
        // Instantiate the game object from prefab, note that this will get deleted once call method is complete
        // The prefab luckly does most of the lift, so we just need to edit a few items
        // Lower collision closer to the ground
        fartObject = Instantiate(fartPrehab, HeroController.instance.transform.position - new Vector3(directionMultiplierX * 0.0f, 0.75f, 0), new Quaternion(0, 0, 0, 0));


        // For some reason we need to manually inject the sprite collection into the animated sprite base after instantiation
        // Without collection the tk2dBaseSprite doesnt know where to get textures to update the object
        fartObject.GetComponent<tk2dAnimatedSprite>().Collection = _fartSC;

        // Make active
        fartObject.SetActive(true);
        fartObject.layer = 1;
        Modding.Logger.Log("[Fart Knight] Object Layer: " + fartObject.layer, FartKnight.GS.LogLevel);

        return fartObject;
    }

    // https://github.com/TTacco/Hollow-Point/blob/master/HollowPoint/HollowPointPrefabs.cs#L92
    private IEnumerator CreateFartPrefab()
    {
        // Make sure our shit is ready
        do
        {
            yield return null;
        }
        while (HeroController.instance == null || GameManager.instance == null);
        Modding.Logger.Log("[Fart Knight] Instantiating Fart Prefab", FartKnight.GS.LogLevel);
        Resources.LoadAll<GameObject>("");

        //Prefab for the fart object, we will instantiate on button press
        fartPrehab = new GameObject("fartPrefabObject", 
               typeof(Rigidbody2D),
               typeof(BoxCollider2D),
               typeof(tk2dAnimatedSprite), // The sprite animation, HK uses this shitty non OSS animation package tk2d... idk cringe
               typeof(tk2dSpriteAnimator), // Helper for playing sprite animation
               typeof(FartBehavior), // Handles the on hit logic
               typeof(AudioSource),
               typeof(PlayMakerFSM), // For coloring collision correctly in debug mod
               typeof(MeshFilter),
               typeof(MeshRenderer)
         );

        // Here we will programmatically build this object
        // This means we need to explcitly set up a mesh to use with the render
        // I suppose one would normall asset bundle and load this? But idk how to do that :)
        fartPrehab.GetComponent<MeshFilter>().mesh = GetMesh();
        fartPrehab.GetComponent<MeshRenderer>().enabled = true;

        // Debug option: use regular sprite to see object placement if tk2d is not working
        // Add to prefab: typeof(SpriteRenderer), conflicts with mesh render in animatedsprite
        // var secondaryTexture1 = new Texture2D(64, 64);
        /* fartPrehab.GetComponent<SpriteRenderer>().sprite = Sprite.Create(secondaryTexture1,
            new Rect(0, 0, secondaryTexture1.width, secondaryTexture1.height),
            new Vector2(0.5f, 0.5f), 42);*/

        // Set up the animation clip
        tk2dSpriteAnimationClip _fartAnimationClip = new tk2dSpriteAnimationClip()
        {
            name = "FartAnimation",
            frames = new tk2dSpriteAnimationFrame[] {
            new() {spriteCollection = _fartSC, spriteId = 0},
            new() {spriteCollection = _fartSC, spriteId = 0},
            new() {spriteCollection = _fartSC, spriteId = 1},
            new() {spriteCollection = _fartSC, spriteId = 1},
            new() {spriteCollection = _fartSC, spriteId = 2},
            new() {spriteCollection = _fartSC, spriteId = 2},
            new() {spriteCollection = _fartSC, spriteId = 3},
            new() {spriteCollection = _fartSC, spriteId = 3},
            new() {spriteCollection = _fartSC, spriteId = 4},
            new() {spriteCollection = _fartSC, spriteId = 4},
            new() {spriteCollection = _fartSC, spriteId = 5},
            new() {spriteCollection = _fartSC, spriteId = 5},
            new() {spriteCollection = _fartSC, spriteId = 6},
            new() {spriteCollection = _fartSC, spriteId = 6},
            new() {spriteCollection = _fartSC, spriteId = 7},
            new() {spriteCollection = _fartSC, spriteId = 7},
            new() {spriteCollection = _fartSC, spriteId = 8},
            new() {spriteCollection = _fartSC, spriteId = 8}},
            fps = 12,
        };

        Modding.Logger.Log("[Fart Knight] Creating tk2dAnimatedSprite", FartKnight.GS.LogLevel);
        // Sprite animation is a collection  of clips that is the "Library" of the animated sprite
        // Useful: https://www.2dtoolkit.com/docs/latest/html/annotated.html
        tk2dSpriteAnimation spriteAnimation = fartPrehab.AddComponent<tk2dSpriteAnimation>();
        tk2dSpriteAnimationClip[] clips = { _fartAnimationClip };
        spriteAnimation.clips = clips;

        // Set up animated sprite
        tk2dAnimatedSprite animatedSprite = fartPrehab.GetComponent<tk2dAnimatedSprite>();
        animatedSprite.Library = spriteAnimation;
        animatedSprite.Collection = _fartSC; // Gets removed but whatever

        //Rigidbody
        fartPrehab.GetComponent<Rigidbody2D>().isKinematic = true;
        fartPrehab.transform.localScale = new Vector3(1.2f, 1.2f, 0);

        //Collider Changes
        BoxCollider2D fartCol = fartPrehab.GetComponent<BoxCollider2D>();
        fartCol.enabled = false; // false
        fartCol.isTrigger = true;
        fartCol.size = new Vector2(1.0f, 1.0f);
        fartCol.offset = new Vector2(0.0f, 0.0f);

        // FSM seems some hollow knight specific
        // Adding this so it appears blue on debug mod
        // https://github.com/TheMulhima/HollowKnight.DebugMod/blob/master/Source/Hitbox/HitboxRender.cs#L103
        PlayMakerFSM fsm = fartPrehab.GetComponent<PlayMakerFSM>();
        fsm.FsmName = "damages_enemy";

        // Not active
        fartPrehab.SetActive(false);
        DontDestroyOnLoad(fartPrehab);
        Modding.Logger.Log("[Fart Knight] Created Fart Prefab", FartKnight.GS.LogLevel);
    }

    // Loads wav file into audioclip
    // https://github.com/SFGrenade/SFCore/blob/master/Util/WavUtils.cs
    private AudioClip LoadAudioClip()
    {
        Modding.Logger.Log("[Fart Knight] Loading audio clip from resources", FartKnight.GS.LogLevel);
        var assembly = Assembly.GetExecutingAssembly();
        // In Resources folder
        Stream s = assembly.GetManifestResourceStream("FartKnight.Resources.fart.wav");
        AudioClip clip = SFCore.Utils.WavUtils.ToAudioClip(s, "fart");
        return clip;
    }

    // Following HK Vocal
    // https://github.com/Hallownest-Vocalized/Hallownest-Vocalized/blob/f7955bee3ad2c7dc3059da720d460f71121b11e4/HKVocal/EasterEggs/PaleFlower.cs#L75
    private void CreateCollections()
    {
        // Creates sprite collection in tk2d, basically collection of textures
        // Some info here: https://www.2dtoolkit.com/docs/latest/tutorial/creating_a_sprite_collection.html
        Texture2D Idle = Satchel.AssemblyUtils.GetTextureFromResources("FartKnight.Resources.Fart_Sprite.png");
        GameObject IdleGo = new GameObject("Fart Sprite Collection");

        int num_frames = 9;
        float width = (float)Idle.height;
        float height = (float)Idle.height;
        string[] names = new string[num_frames];
        Rect[] rects = new Rect[num_frames];
        Vector2[] anchors = new Vector2[num_frames];
        for (int i = 0; i < num_frames; i++)
        {
            names[i] = i.ToString();
            rects[i] = new Rect(width * (float)i, 0, width, height);
            anchors[i] = new Vector2(128f, 128f);
        }
        // https://github.com/RedFrog6002/FrogCore/
        //FrogCore.Utils.CreateTk2dSpriteCollection(Idle, names, rects, anchors, IdleGo);
        _fartSC = FrogCore.Utils.CreateFromTexture(IdleGo, Idle, tk2dSpriteCollectionSize.PixelsPerMeter(128f), new Vector2(width * num_frames, height), names, rects, null, anchors, new bool[num_frames]);
        _fartSC.hasPlatformData = false;
        Modding.Logger.Log("[Fart Knight] Created Collections!", FartKnight.GS.LogLevel);
    }

    // https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
    private Mesh GetMesh(float scale=0.5f)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(scale*1, 0, 0),
            new Vector3(0, scale*1, 0),
            new Vector3(scale*1, scale*1, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }
}
