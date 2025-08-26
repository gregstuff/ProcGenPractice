using DungeonGeneration.Service.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VariantResolver : MonoBehaviour
{
    private void Awake()
    {
        ResolveAll();
    }

    public void ResolveAll()
    {
        var chosenSet = ChooseRandomSet();
        ResolveObjectsForSet(chosenSet);
    }

    private void ResolveObjectsForSet(string chosenSet)
    {
        foreach (var group in GetComponentsInChildren<GroupMarker>())
        {
            if (notForSet(group.gameObject, chosenSet)) continue;

            List<GameObject> candidates = new List<GameObject>();
            foreach (Transform child in group.transform)
            {
                if (!notForSet(child.gameObject, chosenSet)) candidates.Add(child.gameObject);
            }
            var resolvedChild = candidates[RandomSingleton.Instance.NextInt(0, candidates.Count)];
            resolvedChild.SetActive(true);
        }
    }

    private bool notForSet(GameObject candidate, string set)
    {
        return candidate.TryGetComponent<SetMarker>(out var setMarker) && !setMarker.Set.Equals(set);
    }

    private string ChooseRandomSet()
    {
        var sets = GetComponentsInChildren<SetMarker>().Select(s => s.Set).Distinct().ToList();
        if (sets.Count == 0) throw new System.Exception("For a variant you need to define at least one set");
        return sets[RandomSingleton.Instance.NextInt(0, sets.Count)];
    }

}
