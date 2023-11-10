using HutongGames.PlayMaker.Actions;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FartKnight;
public class HitBox: MonoBehaviour
{
    private GameObject bulletPrefab;
    private static GameObject fartObjectPrefab;
    private int layer_id = 0;
    private GameObject fartObject;

    // public FartObject fartObject;
    // public FartObject[] fartObject;


    [Serializable]
    public struct ObjectsToInst
    {
        public Terrain terrain;
        public float yOffset;
        public int numberOfObjectsToCreate;
        public bool parent;
        public bool randomScale;
        public float setRandScaleXMin, setRandScaleXMax;
        public float setTandScaleYMin, setTandScaleYMax;
        public float setTandScaleZMin, setRandScaleZMax;
    }

    private void Awake()
    {
        Modding.Logger.Log("AYYYYYYY1111");
        //  initialize variables or states before the application starts

        fartObjectPrefab = new GameObject("fartObject", typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(FartBehavior), typeof(AudioSource));

        //Rigidbody
        fartObjectPrefab.GetComponent<Rigidbody2D>().isKinematic = true;
        fartObjectPrefab.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        Modding.Logger.Log("AYYYYYYY222");
        //Collider Changes
        BoxCollider2D bulletCol = fartObjectPrefab.GetComponent<BoxCollider2D>();
        bulletCol.enabled = true; // false
        bulletCol.isTrigger = true;
        bulletCol.size = new Vector2(1.0f, 1.0f);
        bulletCol.offset = new Vector2(0.0f, -0.5f);

        Modding.Logger.Log("YEET");
        // https://github.com/TheMulhima/HollowKnight.DebugMod/blob/master/Source/Hitbox/HitboxRender.cs#L103
        PlayMakerFSM fsm = fartObjectPrefab.AddComponent<PlayMakerFSM>();
        fsm.FsmName = "damages_enemy";
        // this.fartObject.SetActive(false);
        Modding.Logger.Log("AYYYYYYY");
        Modding.Logger.Log(fartObjectPrefab);
        DontDestroyOnLoad(fartObjectPrefab);
    }
    private void Start()
    {
        Modding.Logger.Log("~~~~Started here!!!!!");
        Modding.Logger.Log(fartObject);
        // StartCoroutine(CreateBulletPrefab());
        // StartCoroutine(GetFSMPrefabsAndParticles());
        // ModHooks.Instance.ObjectPoolSpawnHook += Instance_ObjectPoolSpawnHook;
    }

    // Main fart runner
    public void FartRun()
    {
        fartObject = Instantiate(fartObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        DontDestroyOnLoad(fartObject);
        Modding.Logger.Log("6666" + fartObject);
        if (!fartObject.activeSelf)
        {
            Modding.Logger.Log("Collider Go");
            base.StartCoroutine(this.RunCollider());
        }
    }

    // https://github.com/TTacco/Hollow-Point/blob/4498cc599c0770194993f811e592ec29ead06590/HollowPoint/AttackHandler.cs#L295
    private IEnumerator RunCollider()
    {
        
        GameObject bullet = SpawnBulletFromKnight(0);
        yield return new WaitForSeconds(2.0f); //0.12f This yield will determine the time inbetween shots
        // Destroy(bullet, 2.0f);
        fartObject.SetActive(false);
        Modding.Logger.Log("Collider Destroy");

    }

    // https://github.com/TTacco/Hollow-Point/blob/master/HollowPoint/HollowPointPrefabs.cs#L269
    private GameObject SpawnBulletFromKnight(float bulletDegreeDirection)
    {
        //SpawnObjectFromDictionary("FireballImpact", HeroController.instance.transform.position, Quaternion.identity);
        //Instantiate(fireballImpactPrefab, HeroController.instance.transform.position, Quaternion.identity).SetActive(true);

        bulletDegreeDirection = bulletDegreeDirection % 360;
        float directionOffsetY = 0;

        //If the player is aiming upwards, change the bullet offset of where it will spawn
        //Otherwise the bullet will spawn too high or inbetween the knight
        /*bool directionalOffSetBool = (dirOrientation == DirectionalOrientation.Vertical || dirOrientation == DirectionalOrientation.Diagonal);
        bool firingUpwards = (bulletDegreeDirection > 0 && bulletDegreeDirection < 180);
        if (directionalOffSetBool && firingUpwards) directionOffsetY = 0.8f;
        else if (directionalOffSetBool && !firingUpwards) directionOffsetY = -1.1f;*/

        float directionMultiplierX = (HeroController.instance.cState.facingRight) ? 1f : -1f;
        float wallClimbMultiplier = (HeroController.instance.cState.wallSliding) ? -1f : 1f;

        //Checks if the player is firing upwards/downwards, and enables the x offset so the bullets spawns directly ontop of the knight
        //from the gun's barrel instead of spawning to the upper right/left of them 
        /*if (dirOrientation == DirectionalOrientation.Vertical) directionMultiplierX = 0.2f * directionMultiplierX;

        if (dirOrientation == DirectionalOrientation.Center)
        {
            directionMultiplierX = 0f;
            directionOffsetY = 0f;
        }*/

        directionMultiplierX *= wallClimbMultiplier;

        // GameObject bullet = Instantiate(bulletPrefab, HeroController.instance.transform.position - new Vector3(directionMultiplierX * 1.0f, 0, 0), new Quaternion(0, 0, 0, 0));
        //BulletBehaviour bb = bullet.GetComponent<BulletBehaviour>();
        // bb.bulletDegreeDirection = bulletDegreeDirection;
        //bb.heatOnHit = HeatHandler.currentHeat;
        //bb.size = Stats.instance.currentWeapon.bulletSize;

        fartObject.transform.position = HeroController.instance.transform.position - new Vector3(directionMultiplierX * 1.0f, 0, 0);
        Modding.Logger.Log(fartObject.transform.position);
        Modding.Logger.Log(HeroController.instance.GetComponent<BoxCollider2D>().size);
        fartObject.SetActive(true);
        fartObject.layer = layer_id; // 8 = ground
        Modding.Logger.Log("Layer: " + fartObject.layer); 


        return fartObject;
    }

    private IEnumerator CreateBulletPrefab()
    {
        do
        {
            yield return null;
        }
        while (HeroController.instance == null || GameManager.instance == null);
        Modding.Logger.Log("---- yoooooo");
        //projectileSprites = new Dictionary<String, Sprite>();
        //prefabDictionary = new Dictionary<string, GameObject>();

        Resources.LoadAll<GameObject>("");
        /*foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            try
            {
                //Log(go.name);
                //if (go.name.Equals("shadow burst") && blood == null)
                if (go.name.Equals("particle_orange blood") && blood == null)
                {
                    //globalPrefabDict.Add("blood", Instantiate(go));
                    blood = go;
                    //blood.SetActive(false);
                    Modding.Logger.Log(go.name);
                }
                else if (go.name.Equals("Gas Explosion Recycle M") && !prefabDictionary.ContainsKey("Gas Explosion Recycle M"))
                {
                    //globalPrefabDict.Add("explosion medium", Instantiate(go));
                    explosion = go;
                    //explosion.SetActive(false);
                    prefabDictionary.Add("Gas Explosion Recycle M", go);
                    Modding.Logger.Log(go.name);

                }
                else if (go.name.Equals("Dung Explosion") && !prefabDictionary.ContainsKey("Dung Explosion"))
                {
                    prefabDictionary.Add("Dung Explosion", go);
                    Modding.Logger.Log(go.name);
                }
                else if (go.name.Equals("Knight Spore Cloud") && !prefabDictionary.ContainsKey("Knight Spore Cloud"))
                {
                    prefabDictionary.Add("Knight Spore Cloud", go);
                    Modding.Logger.Log(go.name);
                }
                else if (go.name.Equals("Knight Dung Cloud") && !prefabDictionary.ContainsKey("Knight Dung Cloud"))
                {
                    prefabDictionary.Add("Knight Dung Cloud", go);
                    Modding.Logger.Log(go.name);
                }
                else if (go.name.Equals("soul_particles") && !prefabDictionary.ContainsKey("soul_particles"))
                {
                    prefabDictionary.Add("soul_particles", go);
                    Modding.Logger.Log(go.name);
                }
                else if (go.name.Equals("Focus Effects") && !prefabDictionary.ContainsKey("Focus Effects"))
                {
                    prefabDictionary.Add("Focus Effects", go);
                    Modding.Logger.Log(go.name);
                }

            }
            catch (Exception e)
            {
                Modding.Logger.Log(e);
            }

        }*/

        // LoadAssets.spriteDictionary.TryGetValue("sprite_bullet_soul.png", out Texture2D bulletTexture);
        // LoadAssets.spriteDictionary.TryGetValue("bulletSpriteFade.png", out Texture2D fadeTexture);

        //Prefab instantiationBulletBehaviour
        bulletPrefab = new GameObject("bulletPrefabObject", typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(FartBehavior), typeof(AudioSource));
        // bulletPrefab = new GameObject("bulletPrefabObject", typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(BulletBehaviour), typeof(AudioSource));
        /*bulletPrefab.GetComponent<SpriteRenderer>().sprite = Sprite.Create(bulletTexture,
            new Rect(0, 0, bulletTexture.width, bulletTexture.height),
            new Vector2(0.5f, 0.5f), 42);*/

        string[] textureNames = { "specialbullet.png", "furybullet.png", "shadebullet.png", "sprite_bullet_dagger.png", "sprite_bullet_dung.png", "sprite_bullet_voids.png" };
        //Special bullet sprite
        /*
        LoadAssets.spriteDictionary.TryGetValue("specialbullet.png", out Texture2D specialBulletTexture);
        projectileSprites.Add("specialbullet.png", Sprite.Create(specialBulletTexture,
            new Rect(0, 0, specialBulletTexture.width, specialBulletTexture.height),
            new Vector2(0.5f, 0.5f), 42));
        */

        /*foreach (string tn in textureNames)
        {
            try
            {
                LoadAssets.spriteDictionary.TryGetValue(tn, out Texture2D specialBulletTexture);
                projectileSprites.Add(tn, Sprite.Create(specialBulletTexture,
                    new Rect(0, 0, specialBulletTexture.width, specialBulletTexture.height),
                    new Vector2(0.5f, 0.5f), 42));
            }
            catch (Exception e)
            {
                Log(e);
            }
        }*/

        //Rigidbody
        bulletPrefab.GetComponent<Rigidbody2D>().isKinematic = true;
        bulletPrefab.transform.localScale = new Vector3(1.2f, 1.2f, 0);

        //Collider Changes
        BoxCollider2D bulletCol = bulletPrefab.GetComponent<BoxCollider2D>();
        bulletCol.enabled = true; // false
        bulletCol.isTrigger = true;
        bulletCol.size = new Vector2(1.0f, 1.0f);
        bulletCol.offset = new Vector2(0.0f, -0.5f);

        Modding.Logger.Log("YEET");
        // https://github.com/TheMulhima/HollowKnight.DebugMod/blob/master/Source/Hitbox/HitboxRender.cs#L103
        PlayMakerFSM fsm = bulletPrefab.AddComponent<PlayMakerFSM>();

        fsm.FsmName = "damages_enemy";
        bulletPrefab.SetActive(false);
        DontDestroyOnLoad(bulletPrefab);

        // TODO move so I can create multiple
        
        
        Modding.Logger.Log("---- init bullet" + bulletPrefab.LocateMyFSM("damages_enemy"));
    }

}
