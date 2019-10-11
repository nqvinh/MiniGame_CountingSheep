using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    SpriteSheet,
    Prefab,
    Audio,
    Material,
    Texture,
    ParticleSystem
}

public class SimpleResourcesManager : Singleton<SimpleResourcesManager>
{
    

    Dictionary<string, Sprite> spriteFrameCache = new Dictionary<string, Sprite>();
    Dictionary<string, List<ParticleSystem>> particleSystemCache = new Dictionary<string, List<ParticleSystem>>();

    Dictionary<string, int> cursorPool = new Dictionary<string, int>();

    readonly string ResourceConfigPath = "Configs/ResourceConfig";

    ResourceConfig resourceConfig = null;

    public void LoadResource()
    {
        resourceConfig = Resources.Load<ResourceConfig>(ResourceConfigPath);
        int resourceLeng = resourceConfig.dataArray.Length;

        for (int i=0;i<resourceLeng;++i)
        {
            LoadActualResource(resourceConfig.dataArray[i]);
        }
    }

    void LoadActualResource(ResourceConfigData data)
    {
        switch (data.RESOURCETYPE)
        {
            case ResourceType.SpriteSheet:
                LoadSpriteSheet(data.Resourcepath+"/"+data.Resourcesname);
                break;
            case ResourceType.Prefab:
                break;
            case ResourceType.Audio:
                break;
            case ResourceType.Material:
                break;
            case ResourceType.Texture:
                break;
            case ResourceType.ParticleSystem:
                LoadParticleSystem(data.Resourcepath + "/" + data.Resourcesname, data.Preloadamount);
                break;
            default:
                break;
        }
    }

    #region SpriteSheet
    void LoadSpriteSheet(string path)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        int spriteCount = sprites.Length;
        for (int i=0;i<spriteCount;++i)
        {
            string spriteName = sprites[i].name;
            if (!spriteFrameCache.ContainsKey(spriteName))
            {
                spriteFrameCache.Add(spriteName, sprites[i]);
            }
            else
            {
                Debug.LogError(string.Format("Duplicate Sprite Frame Name {0} in Sheet{1}", spriteName, path));
            }
        }
    }

    public Sprite GetSprite(string spriteName)
    {
        if (spriteFrameCache.ContainsKey(spriteName))
        {
            return spriteFrameCache[spriteName];
        }
        Debug.LogError("Not Exist Sprite " + spriteName);
        return null;
    }
    #endregion

    #region ParticleSystem
    void LoadParticleSystem(string path,int preloadCount)
    {
        ParticleSystem particleSystem = Resources.Load<ParticleSystem>(path);

        particleSystemCache.Add(particleSystem.name, new List<ParticleSystem>());
        cursorPool.Add(particleSystem.name, 0);

        for (int i=0;i<preloadCount;++i)
        {
            ParticleSystem particle = Instantiate(particleSystem);
            particle.gameObject.SetActive(false);
            particleSystemCache[particleSystem.name].Add(particle);
            DontDestroyOnLoad(particle);
        }
    }


    public void ShowParticle(string name,Vector3 postion,float timeToHide = -1,System.Action showDoneCB = null)
    {
        if (!particleSystemCache.ContainsKey(name))
        {
            Debug.LogError("There is no particle name : "+ name);
            return;
        }

        ParticleSystem particleSystem = particleSystemCache[name][0];
        
        int currentCursor = cursorPool[name];

        particleSystem = particleSystemCache[name][currentCursor];
        currentCursor++;
        if (currentCursor >= particleSystemCache[name].Count)
            currentCursor = 0;

        particleSystem.gameObject.SetActive(true);
        particleSystem.transform.position = postion;
        particleSystem.Play();
        
        if (timeToHide > -1)
        {
            DOVirtual.DelayedCall(timeToHide, () =>
             {
                 particleSystem.Stop();
                 particleSystem.gameObject.SetActive(false);
                 if (showDoneCB != null)
                     showDoneCB();
             });
        }
    }
    #endregion
}
