using System;
using EW2;
using EW2.Spell;
using Lean.Pool;
using Map;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Zitga.ContextSystem;
using Object = UnityEngine.Object;

public static class ResourceUtils
{
    private static readonly ServiceContainer cacheContainer = new ServiceContainer();

    public static Sprite GetSpriteAtlas(string atlasName, string spriteName)
    {
        string fullPath = $"Art/Sprites/Atlas/{atlasName}";

        var atlas = Get<SpriteAtlas>(fullPath);

        return atlas ? atlas.GetSprite(spriteName) : null;
    }

    public static Sprite GetIconMoney(int moneyType)
    {
        return GetSpriteAtlas("money", moneyType.ToString());
    }

    public static Sprite GetBorderMoney(int moneyType)
    {
        switch (moneyType)
        {
            case MoneyType.Crystal:
            case MoneyType.Stamina:
            case MoneyType.Exp:
            case MoneyType.KeyRuneBasic:
            case MoneyType.KeySpellBasic:
                return GetSpriteAtlas("border_item", "border_1_rect");
            case MoneyType.ExpRune:
            case MoneyType.KeyRunePremium:
            case MoneyType.KeySpellPremium:
                return GetSpriteAtlas("border_item", "border_2_rect");
            case MoneyType.Diamond:
                return GetSpriteAtlas("border_item", "border_3_rect");
            default:
                return GetSpriteAtlas("border_item", "border_1_rect");
        }
    }

    public static Sprite GetBgrMoney(int moneyType)
    {
        switch (moneyType)
        {
            case MoneyType.Crystal:
            case MoneyType.Stamina:
            case MoneyType.Exp:
            case MoneyType.KeyRuneBasic:
            case MoneyType.KeySpellBasic:
                return GetSpriteAtlas("border_item", "bgr_rarity_1");
            case MoneyType.ExpRune:
            case MoneyType.KeyRunePremium:
            case MoneyType.KeySpellPremium:
                return GetSpriteAtlas("border_item", "bgr_rarity_2");
            case MoneyType.Diamond:
                return GetSpriteAtlas("border_item", "bgr_rarity_3");
            default:
                return GetSpriteAtlas("border_item", "bgr_rarity_1");
        }
    }

    public static Sprite GetIconMoneyReward(int moneyType)
    {
        return GetSpriteAtlas("money", moneyType.ToString());
    }

    public static Sprite GetSpriteEnemyGuide(int enemyId)
    {
        string fullPath = $"Art/Sprites/GuideEnemies/{enemyId}";

        var sprite = Get<Sprite>(fullPath);

        return sprite ? sprite : null;
    }

    public static Sprite GetSpriteTab(string nameImg)
    {
        return GetSpriteAtlas("tab_image", nameImg);
    }

    public static Sprite GetSpriteAvatar(int avatarId)
    {
        return GetSpriteAtlas("avatar", $"avatar_{avatarId}");
    }

    public static Sprite GetSpriteHeroIcon(string spriteName)
    {
        return GetSpriteAtlas("hero_icons", spriteName);
    }

    public static Sprite GetSpriteSpell(string spriteName)
    {
        return GetSpriteAtlas("spell", spriteName);
    }

    public static Sprite GetSprite(string dir, string spriteName)
    {
        string fullPath = $"Art/Sprites/{dir}/{spriteName}";

        var sprite = Get<Sprite>(fullPath);

        return sprite ? sprite : null;
    }

    private static GameObject GetUnitPrefab(string prefabName)
    {
        string fullPath = $"Art/Prefabs/GamePlays/Units/{prefabName}";

        return Get<GameObject>(fullPath);
    }

    public static GameObject GetVfx(string type, string name, Vector3 position, Quaternion rotation,
        Transform parent = null, int preload = 0, bool isUsePool = true)
    {
        string fullPath = $"Art/Prefabs/VFX/{type}/{name}";

        var vfx = Get<GameObject>(fullPath);

        if (vfx == null)
        {
            if (Application.isMobilePlatform)
            {
                //throw new Exception($"effect is null: {type} => {name}");
            }
#if UNITY_EDITOR
            string path = $"Assets/EW2/Art/VFX/Prefabs/{type}/{name}.prefab";

            vfx = AssetDatabase.LoadAssetAtPath<GameObject>(path);
#endif
        }

        DestroyMe dm = vfx.GetComponent<DestroyMe>();
        if (!isUsePool)
        {
            GameObject clone = GameObject.Instantiate(vfx, position, rotation, parent);
            clone.transform.localPosition = position;
            clone.transform.localRotation = rotation;
            // Debug.LogError(vfx.name);
            return clone;
        }

        if (dm != null)
        {
            if (!dm.isNeccessory)
            {
                GameObject clone = GameObject.Instantiate(vfx, position, rotation, parent);
                clone.transform.localPosition = position;
                clone.transform.localRotation = rotation;
                // Debug.LogError(vfx.name);
                return clone;
            }
        }

        return LeanPool.Spawn(vfx, position, rotation, parent, preload);
    }

    public static GameObject GetVfx(string type, string name)
    {
        string fullPath = $"Art/Prefabs/VFX/{type}/{name}";

        var vfx = Get<GameObject>(fullPath);
        if (vfx == null)
        {
            if (Application.isMobilePlatform)
            {
                throw new Exception($"effect is null: {type} => {name}");
            }
#if UNITY_EDITOR
            string path = $"Assets/EW2/Art/VFX/Prefabs/{type}/{name}.prefab";

            vfx = AssetDatabase.LoadAssetAtPath<GameObject>(path);
#endif
        }

        DestroyMe dm = vfx.GetComponent<DestroyMe>();
        if (dm != null)
        {
            if (!dm.isNeccessory)
            {
                return GameObject.Instantiate(vfx);
            }
        }

        return LeanPool.Spawn(vfx);
    }

    public static GameObject GetUnitOther(string prefabName, Transform parent = null, bool isUsingPool = true)
    {
        string fullPath = $"Art/Prefabs/GamePlays/Others/{prefabName}";

        GameObject prefab = Get<GameObject>(fullPath);
        if (!isUsingPool)
        {
            return GameObject.Instantiate(prefab, parent);
        }

        return LeanPool.Spawn(prefab, parent);
    }

    public static GameObject GetUnit(string prefabName, Transform parent = null, bool isUsingPool = true)
    {
        GameObject prefab = GetUnitPrefab(prefabName);
        if (!isUsingPool)
        {
            return GameObject.Instantiate(prefab, parent);
        }

        return LeanPool.Spawn(prefab, parent);
    }

    public static GameObject GetUnit(string prefabName, Vector3 position, Quaternion rotation, Transform parent = null,
        bool isUsePool = true, int preLoad = 0)
    {
        GameObject prefab = GetUnitPrefab(prefabName);

        if (!isUsePool)
        {
            GameObject clone = GameObject.Instantiate(prefab, position, rotation, parent);
            clone.transform.localPosition = position;
            clone.transform.rotation = rotation;
            // Debug.LogError(prefab.name);
            return clone;
        }

        return LeanPool.Spawn(prefab, position, rotation, parent, preLoad);
    }

    public static GameObject GetEnemy(string prefabName, Vector3 position, Quaternion rotation, Transform parent = null,
        bool isUsePool = true, int preLoad = 0)
    {
        GameObject prefab = GetUnitPrefab(prefabName);
        
        if (prefab) prefab.SetActive(false);
        
        if (!isUsePool)
        {
            GameObject clone = GameObject.Instantiate(prefab, position, rotation, parent);
            clone.transform.localPosition = position;
            clone.transform.rotation = rotation;
            // Debug.LogError(prefab.name);
            return clone;
        }

        return LeanPool.Spawn(prefab, position, rotation, parent, preLoad);
    }

    public static GameObject GetSpellUnit(string prefabName, Vector3 position, Quaternion rotation,
        Transform parent = null)
    {
        string fullPath = $"Art/Prefabs/GamePlays/Spells/{prefabName}";

        var spell = Get<GameObject>(fullPath);

        return LeanPool.Spawn(spell, position, rotation, parent);
    }

    public static GameObject GetStatus(string prefabName, Transform parent = null)
    {
        string fullPath = $"Art/Prefabs/GamePlays/Status/{prefabName}";

        GameObject prefab = Get<GameObject>(fullPath);
        return LeanPool.Spawn(prefab, parent);
    }

    public static GameObject GetVfxHero(string prefabName, Vector3 position, Quaternion rotation,
        Transform parent = null)
    {
        return GetVfx("Hero", prefabName, position, rotation, parent);
    }

    public static GameObject GetVfxTower(string prefabName, Vector3 position, Quaternion rotation,
        Transform parent = null, int preload = 0)
    {
        return GetVfx("Tower", prefabName, position, rotation, parent, preload);
    }

    public static GameObject GetTutorial(string prefabName)
    {
        string fullPath = $"Art/Prefabs/Tutorial/{prefabName}";

        return Get<GameObject>(fullPath);
    }

    public static MapController GetMapPrefab(int worldId, int mapId)
    {
        string prefabName = $"map_campaign_{worldId}_{mapId}";
        string fullPath = $"Art/Prefabs/Maps/{prefabName}";

        GameObject prefab = Get<GameObject>(fullPath);
        var go = LeanPool.Spawn(prefab);
        if (go != null)
        {
            return go.GetComponent<MapController>();
        }

        return null;
    }

    public static MapController GetMapDefenseModePrefab(int mapId)
    {
        string prefabName = $"map_herodefense_{mapId}";
        string fullPath = $"Art/Prefabs/HeroDefenseMap/{prefabName}";

        GameObject prefab = Get<GameObject>(fullPath);
        var go = LeanPool.Spawn(prefab);
        if (go != null)
        {
            return go.GetComponent<MapController>();
        }

        return null;
    }
    
    public static RewardUI GetRewardUi(ResourceType type)
    {
        string fullPath = $"Art/Prefabs/Rewards/{(int)type}";

        GameObject prefab = Get<GameObject>(fullPath);

        LeanPool.SetPersist(prefab);

        var go = LeanPool.Spawn(prefab);

        if (go != null)
        {
            return go.GetComponent<RewardUI>();
        }

        return null;
    }

    public static void GetEffectBuildTower(Transform posInstance)
    {
        GetVfx("Effects", "fx_common_tower_build_upgrade", posInstance.position, Quaternion.identity);
    }

    public static GameObject GetHeroClasses(Transform parent = null)
    {
        string fullPath = $"Art/Prefabs/HeroRoom/info_classes";

        GameObject prefab = Get<GameObject>(fullPath);
        return LeanPool.Spawn(prefab, parent);
    }


    public static AudioClip LoadSound(string path)
    {
        return Get<AudioClip>(path);
    }

    public static Sprite GetHeroIcon(int heroId)
    {
        return GetSprite("HeroIcons", $"hero_icon_{heroId}");
    }

    public static Shader GetShader(string nameShader)
    {
        string fullPath = $"Art/Shaders/{nameShader}";

        var shader = Resources.Load<Shader>(fullPath);

        return shader;
    }

    public static GameObject GetCutScene(Transform parent = null)
    {
        string fullPath = $"Art/Prefabs/Cutscene/Cutscene_awaken_miniboss_map_09";
        GameObject prefab = Get<GameObject>(fullPath);
        return LeanPool.Spawn(prefab, parent);
    }

    public static Font GetFont(string fontName)
    {
        string fullPath = $"Fonts/{fontName}";
        return Get<Font>(fullPath);
    }

    public static Sprite GetRankIconLargeTournament(int rank)
    {
        return GetSpriteAtlas("icon_rank_tournament", $"icon_rank_{rank}_large");
    }
    
    public static Sprite GetRankIconSmallTournament(int rank)
    {
        return GetSpriteAtlas("icon_rank_tournament", $"icon_rank_{rank}_small");
    }
    
    public static T Get<T>(string path) where T : Object
    {
        T cache = cacheContainer.Resolve<T>(path);

        if (cache == null)
        {
            cache = Resources.Load<T>(path);

            if (cache != null)
            {
                cacheContainer.Register(path, cache);
            }

            //Debug.Assert(cache != null, "Object is not exist: " + path);
        }

        return cache;
    }
}