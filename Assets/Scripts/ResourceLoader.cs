using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TaskString = Cysharp.Threading.Tasks.UniTask<string>;
public abstract class IResourseLoader<T>
{
        public T Resourse;
    
        public virtual async Cysharp.Threading.Tasks.UniTask<T> Load(string storage, string bundle = null)
        {
            return Resourse;
        }
}
public class DefaultTextLoader : IResourseLoader<string>
{
        public string[] Lines;
        public int SavedLine;
        public override async TaskString Load(string storage, string bundle = null)
        {
            TextAsset text = await Addressables.LoadAssetAsync<TextAsset>(storage);
            Lines = text.text.Split("\n");
            return null;
        }
    }

public class ResourceLoader : MonoBehaviour
{
    [Serializable]
    struct TextSave
    {
        private string name;
        private int savedline;
    }
    private const ResourceType DefaultResourceType = ResourceType.LocalStatic;
    public static Dictionary<string, DefaultTextLoader> textLoader = new Dictionary<string, DefaultTextLoader>();

    //存各脚本被读到哪里了
    public void Save()
    {
        
    }
    public static async TaskString LoadText(string storage)
    {       
            if (!textLoader.ContainsKey(storage))
            {
                print("loading: "+storage);
                textLoader.Add(storage, new DefaultTextLoader());
            }
            await textLoader[storage].Load(storage);
            return null;
    }
}
public enum ResourceType
{
        LocalStatic = 0,
        LocalStreaming = 1,
        WWW = 2,
        AssetBundle = 3,
        Unknown = 99
}
