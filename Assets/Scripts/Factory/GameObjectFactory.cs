using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameObjectFactory
{
    private readonly Dictionary<string, GameObject> _prefabs;
    
    public GameObjectFactory()
    {
        _prefabs = new Dictionary<string, GameObject>();
    }

    public async UniTask<GameObject> Build(string prefabName)
    {
        if (_prefabs.TryGetValue(prefabName, out var go) == false)
        {
            var load = Resources.LoadAsync<GameObject>(prefabName);

            while (load.isDone == false)
                await UniTask.DelayFrame(1);

            go = (GameObject)load.asset;
            _prefabs.Add(prefabName, go);
        }

        return Object.Instantiate(go);
    }
}
