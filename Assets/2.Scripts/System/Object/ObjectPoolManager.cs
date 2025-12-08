using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolObjectData
{
    public string key;
    public GameObject prefab;
    public int count = 5;
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance = null;

    [SerializeField] List<PoolObjectData> _poolObjectDataList;

    Dictionary<GameObject, string> _keyByPoolObject;
    Dictionary<string, GameObject> _keyBySampleObject;
    Dictionary<string, Stack<GameObject>> _poolObjectByKey;

    void Awake()
    {
        instance = this;

        _keyByPoolObject = new Dictionary<GameObject, string>();
        _keyBySampleObject = new Dictionary<string, GameObject>();
        _poolObjectByKey = new Dictionary<string, Stack<GameObject>>();
        for(int i = 0; i < _poolObjectDataList.Count; i++)
        {

            CreatePool(_poolObjectDataList[i]);
        }
    }

    public void CreatePool(PoolObjectData data)
    {
        string parentObjectName = "Pool <" + data.key + ">";
        var parentObject = GameObject.Find(parentObjectName);

        GameObject newGameObject = null;
        if(_poolObjectByKey.ContainsKey(data.key))
        {

#if UNITY_EDITOR
            Debug.Log(data.key + " có key trùng lặp");
#endif
            for (int i = 0; i < data.count; i++)
            {
                newGameObject = Instantiate(_keyBySampleObject[data.key], parentObject.transform);
                _keyByPoolObject.Add(newGameObject, data.key);
            }

            return;
        }

        if (parentObject == null)
        {
            parentObject = new GameObject(parentObjectName);
            parentObject.transform.parent = transform;
        }

        var poolObject = new Stack<GameObject>();
        for (int i = 0; i < data.count; i++)
        {
            newGameObject = Instantiate(data.prefab, parentObject.transform);
            newGameObject.SetActive(false);
            poolObject.Push(newGameObject);
            _keyByPoolObject.Add(newGameObject, data.key);
        }
        _poolObjectByKey.Add(data.key, poolObject);
        _keyBySampleObject.Add(data.key, data.prefab);

#if UNITY_EDITOR
        Debug.Log("Tên object: " + data.key + "\nSố lượng object: " + parentObject.transform.childCount);
#endif
    }

    public GameObject GetPoolObject(string key, Vector2 pos, float scaleX = 1,float angle = 0)
    {
        if(!_poolObjectByKey.TryGetValue(key, out var poolObject))
        {

#if UNITY_EDITOR
            Debug.Log(key + " - Object này không tồn tại!");
#endif
            return null;
        }

        GameObject getPoolObject;
        if(poolObject.Count > 0)
        {

            getPoolObject = poolObject.Pop();
        }
        else
        {

            #if UNITY_EDITOR
                Debug.Log(key + " - Số lượng ít, thêm 1 object mới");
            #endif
            string parentObjectName = "Pool <" + key + ">";
            var parentObject = GameObject.Find(parentObjectName);
            getPoolObject = Instantiate(_keyBySampleObject[key], parentObject.transform);
            _keyByPoolObject.Add(getPoolObject, key);
        }

        getPoolObject.transform.position = new Vector3(pos.x, pos.y, getPoolObject.transform.position.z);
        getPoolObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        getPoolObject.transform.localScale = new Vector3(scaleX, 1, 1);
        getPoolObject.SetActive(true);

        return getPoolObject;
    }

    public void ReturnPoolObject(GameObject returnGameObject)
    {
        returnGameObject.SetActive(false);
        string key = _keyByPoolObject[returnGameObject];
        _poolObjectByKey[key].Push(returnGameObject);
    }
}
