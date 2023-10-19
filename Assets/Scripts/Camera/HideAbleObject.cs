using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HideAbleObject : MonoBehaviour
{
    private HideAbleObject hideObject;

    private static Dictionary<Collider, HideAbleObject> hideableObjectsMap = new Dictionary<Collider, HideAbleObject>();

    [SerializeField]
    public GameObject Renderers;

    public Collider Collider = null;

    private void Start()
    {
        Renderers = gameObject;
        Collider = GetComponent<Collider>();
        InitHideObject();
    }

    public static void InitHideObject()
    {
        foreach(var obj in hideableObjectsMap.Values)
        {
            if(obj != null&& obj.Collider!= null)
            {
                obj.SetVisible(true);
                obj.hideObject = null;
            }
        }


        hideableObjectsMap.Clear();

        foreach(var obj in FindObjectsOfType<HideAbleObject>())
        {
            if(obj.Collider != null)
            {
                hideableObjectsMap[obj.Collider] = obj;
            }
        }
    }


    public static HideAbleObject GetRootHideableByCollider(Collider collider)
    {
        HideAbleObject obj;

        if(hideableObjectsMap.TryGetValue(collider, out obj))
        {
            return GetRoot(obj);
        }
        else
        {
            return null;
        }
    }

    private static HideAbleObject GetRoot(HideAbleObject obj)
    {
        if(obj.hideObject ==null)
        {
            return obj;
        }
        else
        { 
            return GetRoot(obj.hideObject);
        }
    }
     
    public void SetVisible(bool visible)
    {
        Renderer rend = Renderers.GetComponent<Renderer>();

        if(rend != null && rend.gameObject.activeInHierarchy && hideableObjectsMap.ContainsKey(rend.GetComponent<Collider>()))
        {
            rend.shadowCastingMode = visible ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
        }
    }
}
