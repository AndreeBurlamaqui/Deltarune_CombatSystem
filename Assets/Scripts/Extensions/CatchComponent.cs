using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CatchComponent
{

    public static bool TryGetComponentInParent<T>(this GameObject _gameObject, out T parentComponent, bool includeInactive = false)
    {
        if (_gameObject.GetComponentInParent<T>(includeInactive) != null)
        {
            parentComponent = _gameObject.GetComponentInParent<T>(includeInactive);
            return true;
        }
        else
        {
            parentComponent = default(T);
            return false;
        }
    }

    public static bool TryGetComponentInParents<T>(this GameObject _gameObject, out T[] parentComponent, bool includeInactive = false)
    {
        if (_gameObject.GetComponentsInParent<T>(includeInactive).Length > 0)
        {
            parentComponent = _gameObject.GetComponentsInParent<T>(includeInactive);
            return true;
        }
        else
        {
            parentComponent = default(T[]);
            return false;
        }
    }

    public static bool TryGetComponentInChildren<T>(this GameObject _gameObject, out T childrenComponent, bool includeInactive = false)
    {
        if (_gameObject.GetComponentInChildren<T>(includeInactive) != null)
        {
            childrenComponent = _gameObject.GetComponentInChildren<T>(includeInactive);
            return true;
        }
        else
        {
            childrenComponent = default(T);
            return false;
        }
    }

    public static bool TryGetComponentInChildrens<T>(this GameObject _gameObject, out T[] childrenComponent, bool includeInactive = false)
    {
        if (_gameObject.GetComponentsInChildren<T>(includeInactive).Length > 0)
        {
            childrenComponent = _gameObject.GetComponentsInChildren<T>(includeInactive);
            return true;
        }
        else
        {
            childrenComponent = default(T[]);
            return false;
        }
    }

    public static bool TryGetComponentInChildrensWithTag<T>(this GameObject _gameObject, string searchTag, out T[] childrenComponent, bool includeInactive = false)
    {
        if (_gameObject.GetComponentsInChildren<T>(includeInactive).Length > 0)
        {
            List<T> foundChilds = new List<T>();

            foreach (Transform childs in _gameObject.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (childs.CompareTag(searchTag) && childs.GetComponents<T>().Length > 0)
                {
                    foundChilds.AddRange(childs.GetComponents<T>());
                }
            }

            if (foundChilds.Count > 0)
            {
                childrenComponent = foundChilds.ToArray();
                return true;
            }
            else
            {
                childrenComponent = default(T[]);
                return false;
            }
        }
        else
        {
            childrenComponent = default(T[]);
            return false;
        }
    }
}
