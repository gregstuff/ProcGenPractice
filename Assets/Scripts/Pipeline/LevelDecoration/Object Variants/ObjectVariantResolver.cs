using ProcGenSys.Service.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.ObjectVariant
{
    public class ObjectVariantResolver : MonoBehaviour
    {
        private void Awake()
        {
            ResolveAll();
        }

        public void ResolveAll()
        {
            string? chosenSet = ChooseRandomSet();
            ResolveObjectsForSet(chosenSet);
        }

        private void ResolveObjectsForSet(string? chosenSet)
        {
            foreach (var group in GetComponentsInChildren<ObjectVariantGroupMarker>())
            {
                List<GameObject> candidates = new List<GameObject>();
                var sets = new HashSet<string>();
                foreach (Transform child in group.transform)
                {
                    child.TryGetComponent<ObjectVariantSetMarker>(out var setMarker);
                    if (setMarker != null) sets.Add(setMarker.Set);
                    candidates.Add(child.gameObject);
                }

                // filter only where one or more prefabs in the group have a set and a set is chosen
                // so, if there is a group where no child has a set defined, it will still pick one of the children
                if (sets.Count > 0 && chosenSet != null)
                {
                    candidates = candidates
                        .Where(cand => cand.TryGetComponent<ObjectVariantSetMarker>(out var setMarker) && setMarker.Set.Equals(chosenSet)).ToList();
                }

                var resolvedChild = candidates[RandomSingleton.Instance.NextInt(0, candidates.Count)];
                resolvedChild.SetActive(true);
            }
        }

        private string? ChooseRandomSet()
        {
            var sets = GetComponentsInChildren<ObjectVariantSetMarker>(true).Select(s => s.Set).Distinct().ToList();
            if (sets.Count == 0) return null;
            return sets[RandomSingleton.Instance.NextInt(0, sets.Count)];
        }

    }
}