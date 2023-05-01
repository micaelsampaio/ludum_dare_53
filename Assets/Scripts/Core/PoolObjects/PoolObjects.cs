using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core.Pooling
{
  public class PoolObjects
  {
    private Queue<GameObject> queue = new Queue<GameObject>();
    public PoolObjects() { }

    public PoolObjects(Pool p)
    {
      Add(p);
    }

    public PoolObjects(GameObject gameObject, int amount)
    {
      Add(gameObject, amount);
    }

    public void Add(Pool p)
    {
      if (p.prefab.scene != null && !string.IsNullOrEmpty(p.prefab.scene.name)) p.prefab.SetActive(false); // hide if has scene

      if (!p.increment && queue.Count > 0) return;

      for (var i = 0; i < p.amount; ++i)
      {
        var clone = InstantiatePrefab(p.prefab);

        if (p.processors != null && p.processors.Length > 0)
        {
          foreach (var processor in p.processors) processor(clone);
        }
      }
    }

    public void Add(GameObject g, int amount)
    {
      if (g.scene != null && !string.IsNullOrEmpty(g.scene.name)) g.SetActive(false); // hide if has scene

      for (var i = 0; i < amount; ++i)
      {
        InstantiatePrefab(g);
      }
    }

    public void AddObject(GameObject g)
    {
      if (g.scene != null && !string.IsNullOrEmpty(g.scene.name)) g.SetActive(false); // hide if has scene

      ValidatePrefab(g);
    }

    public GameObject Get()
    {
      if (queue.Count > 1)
      {
        var obj = queue.Dequeue();
        if (obj.activeSelf == true)
        {
          Debug.LogError("Brooo WTF --> " + obj.name); // if it eneters here huston we have a fucking problem
        }
        return obj;
      }
      else
      {
        var go = queue.Peek();
        InstantiatePrefab(go);
        Debug.LogWarning(go + " DONT EXISTS IN POOL");
        return queue.Dequeue();
      }
    }

    public void Enqueue(GameObject obj)
    {
      obj.SetActive(false);
      queue.Enqueue(obj);
    }
    public void Enqueue(PoolObject obj)
    {
      obj.gameObject.SetActive(false);
      queue.Enqueue(obj.gameObject);
    }

    public T Get<T>()
    {
      return Get().GetComponent<T>();
    }
    public List<T> GetAll<T>()
    {
      var list = new List<T>(queue.Count);
      for (int i = 0; i < list.Count; ++i)
      {
        list[i] = queue.Dequeue().GetComponent<T>();
      }

      for (int i = 0; i < list.Count; ++i)
      {
        queue.Enqueue(list[i] as GameObject);
      }
      return list;
    }
    public void Shuffle(int time = 2)
    {
      GameObject[] list = queue.ToArray();

      for (var j = 0; j < time; j++)
      {

        for (var i = 0; i < list.Length; i++)
        {
          var temp = list[i];
          var index = UnityEngine.Random.Range(0, list.Length);
          list[i] = list[index];
          list[index] = temp;
        }
      }

      queue = new Queue<GameObject>(list);
    }

    private GameObject InstantiatePrefab(GameObject go)
    {
      var clone = UnityEngine.Object.Instantiate(go);
      clone.SetActive(false);

      if (clone.TryGetComponent<I_PoolObject>(out var pool))
      {
        pool.Load(this);
      }

      queue.Enqueue(clone);
      return clone;
    }
    private void ValidatePrefab(GameObject clone)
    {
      clone.SetActive(false);

      if (clone.TryGetComponent<I_PoolObject>(out var pool))
      {
        pool.Load(this);
      }

      queue.Enqueue(clone);
    }

  }

  [Serializable]
  public class Pool
  {
    public string key;
    public GameObject prefab;
    public int amount;
    public bool increment = true;
    public PoolObjects _pool;
    public Action<GameObject>[] processors;

    public void Register()
    {
      _pool = PoolManager.Instance.RegisterPool(this);
    }

    public void Enqueue(GameObject obj)
    {
      _pool.Enqueue(obj);
    }

    public T Get<T>()
    {
      return _pool.Get<T>();
    }
    public GameObject Get()
    {
      return _pool.Get();
    }

    public bool IsNotNull() => prefab != null;
  }

  [Serializable]
  public class PoolSpawnData
  {
    public string key;
    public string spawnKey;
    public string spawnSound;
    public string spawnEffect;
    public Vector3 position;
    public Vector3 rotation;
    public bool parent = false;

    public T Spawn<T>(Transform parent)
    {
      var pool = PoolManager.Instance.GetPool(key);
      var clone = pool.Get<Transform>();
      var parentTransform = parent.transform;

      clone.transform.position = parentTransform.position + parentTransform.forward * position.z + parentTransform.right * position.x + parentTransform.up * position.y;
      clone.transform.rotation = Quaternion.Euler(parent.rotation.eulerAngles + rotation);

      if (typeof(T) == typeof(GameObject))
      {
        return (T)(object)clone.gameObject;
      }

      return clone.GetComponent<T>();
    }

    public GameObject Spawn(Transform parent)
    {
      return Spawn<GameObject>(parent);
    }
  }


  [Serializable]
  public class CharacterData
  {
    public Pool[] PoolObjects;
    public Sound[] Sounds;
    public PoolSpawnData[] SpawnObjectsConfig;
    public Dictionary<string, PoolSpawnData> SpawnObjects;

    public void Load(GameObject parent)
    {
      if (PoolManager.Instance == null) throw new Exception("NO POOLMANAGER");

      if (PoolObjects != null)
      {
        for (int i = 0; i < PoolObjects.Length; ++i)
        {
          PoolManager.Instance.RegisterPool(PoolObjects[i]);
          //character.AddPool(PoolObjects[i]);
        }
      }

      if (Sounds != null && parent.TryGetComponent<SoundController>(out var soundController))
      {
        for (int i = 0; i < Sounds.Length; ++i)
        {
          soundController.RegisterSound(Sounds[i]);
        }
      }

      if (SpawnObjectsConfig != null && SpawnObjectsConfig.Length > 0)
      {
        SpawnObjects = new Dictionary<string, PoolSpawnData>();
        foreach (var pool in SpawnObjectsConfig)
        {
          var key = string.IsNullOrEmpty(pool.spawnKey) ? pool.key : pool.spawnKey;
          SpawnObjects[key] = pool;
        }
        SpawnObjectsConfig = null;
      }
    }

    public T Spawn<T>(string key, Transform target, Transform owner)
    {
      if (SpawnObjects.TryGetValue(key, out var pool))
      {
        return pool.Spawn<T>(pool.parent ? owner ?? target : target ?? owner);
      }
      return default(T);
    }

    public PoolSpawnData GetSpawnData(string key)
    {
      if (SpawnObjects.TryGetValue(key, out var pool))
      {
        return pool;
      }

      return null;
    }
  }
}