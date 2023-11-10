using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using SFCore;

namespace FartKnight;
public class KnightHandler: MonoBehaviour
{
    private tk2dSpriteAnimator _anim;
    private tk2dSpriteCollectionData _fartKnightSC;
    private bool addedAnimations = false;
    private bool running = false;

    private void Awake()
    {
        // Adopting from press G to Dab
        // https://github.com/Link459/PressGToDab/blob/master/PressGToDab/Emoter.cs#L13
        // Initialize variables or states before the application starts
        this._anim = HeroController.instance.gameObject.GetComponent<tk2dSpriteAnimator>();
    }

    private void Start()
    {
        // instantiated during gameplay
        CreateCollections();
        if (!addedAnimations)
        {
            AddAnimations();
            addedAnimations = true;
        }

    }

    // Method for Mod class
    public void Run()
    {
        Modding.Logger.Log("[Fart Knight] Fire Knight Animation", FartKnight.GS.LogLevel);
        base.StartCoroutine(PlayFart());
    }

    // Adopting from press G to Dab
    // https://github.com/Link459/PressGToDab/blob/master/PressGToDab/Emoter.cs#L48
    // Similar to HK Vocals
    // https://github.com/Hallownest-Vocalized/Hallownest-Vocalized/blob/f7955bee3ad2c7dc3059da720d460f71121b11e4/HKVocal/EasterEggs/PaleFlower.cs#L321
    // Note: we do not take input control just animation control, so player can still move
    private IEnumerator PlayFart()
    {
        //HeroController.instance.RelinquishControl();
        HeroController.instance.StopAnimationControl();
        yield return _anim.PlayAnimWait("KnightFartAnimation");
        // HeroController.instance.RegainControl();
        HeroController.instance.StartAnimationControl();
        Modding.Logger.Log("[Fart Knight] Knight animation complete", FartKnight.GS.LogLevel);
        yield break;
    }

    // Following HK Vocal
    // https://github.com/Hallownest-Vocalized/Hallownest-Vocalized/blob/f7955bee3ad2c7dc3059da720d460f71121b11e4/HKVocal/EasterEggs/PaleFlower.cs#L75
    // Similar to press G to Dab
    private void CreateCollections()
    {
        // Creates sprite collection in tk2d, basically collection of textures
        // Some info here: https://www.2dtoolkit.com/docs/latest/tutorial/creating_a_sprite_collection.html
        Texture2D Idle = Satchel.AssemblyUtils.GetTextureFromResources("FartKnight.Resources.Knite_Sprite.png");
        GameObject IdleGo = new GameObject("Knight Fart Sprite Collection");

        int num_frames = 6;
        float width = (float)Idle.height;
        float height = (float)Idle.height;
        string[] names = new string[num_frames];
        Rect[] rects = new Rect[num_frames];
        Vector2[] anchors = new Vector2[num_frames];
        bool[] rotated = new bool[num_frames];
        for (int i = 0; i < num_frames; i++)
        {
            names[i] = i.ToString();
            rects[i] = new Rect(width * (float)i, 0, width, height);
            anchors[i] = new Vector2(64f, 32f);
            rotated[i] = false;
        }
        // https://github.com/RedFrog6002/FrogCore/
        //FrogCore.Utils.CreateTk2dSpriteCollection(Idle, names, rects, anchors, IdleGo);
        _fartKnightSC = FrogCore.Utils.CreateFromTexture(IdleGo, Idle, tk2dSpriteCollectionSize.PixelsPerMeter(64f), new Vector2(width * num_frames, height), names, rects, null, anchors, rotated);
        _fartKnightSC.hasPlatformData = false;
        Modding.Logger.Log("[Fart Knight] Created Knight Collections!", FartKnight.GS.LogLevel);
    }

    // Following HK Vocal
    // https://github.com/Hallownest-Vocalized/Hallownest-Vocalized/blob/f7955bee3ad2c7dc3059da720d460f71121b11e4/HKVocal/EasterEggs/PaleFlower.cs#L75
    public void AddAnimations()
    {
        tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip()
        {
            name = "KnightFartAnimation",
            frames = new tk2dSpriteAnimationFrame[] {
            new() {spriteCollection = _fartKnightSC, spriteId = 0},
            new() {spriteCollection = _fartKnightSC, spriteId = 0},
            new() {spriteCollection = _fartKnightSC, spriteId = 1},
            new() {spriteCollection = _fartKnightSC, spriteId = 1},
            new() {spriteCollection = _fartKnightSC, spriteId = 2},
            new() {spriteCollection = _fartKnightSC, spriteId = 3},
            new() {spriteCollection = _fartKnightSC, spriteId = 4},
            new() {spriteCollection = _fartKnightSC, spriteId = 4},
            new() {spriteCollection = _fartKnightSC, spriteId = 5},
            new() {spriteCollection = _fartKnightSC, spriteId = 5}},
            fps = 16
        };
        this._anim.Library.clips = this._anim.Library.clips.Append(idleClip).ToArray();
    }
}
