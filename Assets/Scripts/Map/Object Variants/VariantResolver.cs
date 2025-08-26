using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VariantResolver : MonoBehaviour
{
    public enum ResolvePhase { Awake, Start, Manual }

    [Header("When to resolve")]
    [SerializeField] private ResolvePhase resolveWhen = ResolvePhase.Awake;

    [Header("Determinism (built-in)")]
    [Tooltip("If true, we compute a deterministic seed from seedBase + instanceKey + position.")]
    [SerializeField] private bool deterministic = true;
    [Tooltip("Base seed you can set per-prefab kind.")]
    [SerializeField] private int seedBase = 12345;
    [Tooltip("Per-instance key to keep rooms consistently resolved (e.g., 'Room_A1').")]
    [SerializeField] private string instanceKey = "";

    [Header("Optional custom RNG singleton (overrides built-in)")]
    [Tooltip("If assigned, must implement IRng (via a MonoBehaviour that exposes a public IRng property). Leave empty to use built-in deterministic RNG.")]
    [SerializeField] private MonoBehaviour rngProvider; // your adapter component that exposes an IRng

    [Header("Cull (Destroy) non-chosen variants AFTER resolve")]
    [Tooltip("If true, we'll Destroy() all inactive variants after resolving.\nDo this while a loading screen is up if you need to reclaim memory.")]
    [SerializeField] private bool cullInactiveAfterResolve = false;
    [Tooltip("How many frames to wait before culling (gives time for any post-resolve logic).")]
    [SerializeField] private int cullDelayFrames = 1;

    // Cache chosen set for debug/inspection if needed
    public PrefabSet ChosenSet { get; private set; }
    private IRng _rng;

    private void Awake()
    {
        if (resolveWhen == ResolvePhase.Awake)
            ResolveAll();
    }

    private void Start()
    {
        if (resolveWhen == ResolvePhase.Start)
            ResolveAll();
    }

    /// Call this manually if resolveWhen == Manual
    public void ResolveAll()
    {
        // 1) RNG setup
        _rng = TryGetCustomRng() ?? CreateBuiltInRng();

        // 2) Gather sets under this root
        var sets = GetComponentsInChildren<SetMarker>(includeInactive: true);
        if (sets == null || sets.Length == 0)
        {
            Debug.LogWarning("[PrefabSetResolver] No SetMarker children found.");
            return;
        }

        // 3) Choose active set
        ChosenSet = forceSet ? forcedSet : ChooseRandomSet(allowedSets);
        foreach (var s in sets) s.gameObject.SetActive(s.set == ChosenSet);

        // 4) Pick exactly one variant in every group inside the chosen set
        var chosenSetRoot = sets.First(s => s.set == ChosenSet).transform;
        var groups = chosenSetRoot.GetComponentsInChildren<GroupMarker>(includeInactive: true);

        foreach (var group in groups)
            ResolveGroup(group);

        // 5) Optionally cull leftovers (destroy inactive)
        if (cullInactiveAfterResolve)
            StartCoroutine(CullInactiveAfterDelay(chosenSetRoot, cullDelayFrames));
    }

    private IRng TryGetCustomRng()
    {
        if (!rngProvider) return null;
        // Your rngProvider script should expose a public IRng Rng { get; }
        // Example: public class MyRngAdapter : MonoBehaviour { public IRng Rng => YourSingleton.Instance; }
        var prop = rngProvider.GetType().GetProperty("Rng");
        if (prop != null && typeof(IRng).IsAssignableFrom(prop.PropertyType))
            return (IRng)prop.GetValue(rngProvider);
        Debug.LogWarning("[PrefabSetResolver] rngProvider does not expose a public IRng Rng property. Using built-in RNG.");
        return null;
    }

    private IRng CreateBuiltInRng()
    {
        int seed = seedBase;
        unchecked
        {
            seed = seed * 31 + (deterministic ? 1 : 0);
            seed = seed * 31 + (string.IsNullOrEmpty(instanceKey) ? 0 : instanceKey.GetHashCode());
            seed = seed * 31 + transform.position.GetHashCode();
            seed = seed * 31 + transform.GetInstanceID();
        }
        return new DefaultRng(seed);
    }

    private PrefabSet ChooseRandomSet(PrefabSet[] from)
    {
        if (from == null || from.Length == 0)
        {
            Debug.LogWarning("[PrefabSetResolver] allowedSets is empty. Defaulting to Black.");
            return PrefabSet.Black;
        }
        return from[_rng.NextInt(0, from.Length)];
    }

    private void ResolveGroup(GroupMarker group)
    {
        // variants can be nested under the group; includeInactive so we see disabled ones too
        var variants = group.GetComponentsInChildren<VariantEntry>(includeInactive: true).ToList();
        if (variants.Count == 0) return;

        // Best practice for Option A: all variants are INACTIVE in the prefab.
        // We'll activate exactly one.
        var chosen = WeightedPick(variants);

        foreach (var v in variants)
            v.Target.SetActive(v == chosen);
    }

    private VariantEntry WeightedPick(List<VariantEntry> options)
    {
        float total = 0f;
        foreach (var o in options)
            total += Mathf.Max(0f, o.weight);

        if (total <= 0f)
            return options[_rng.NextInt(0, options.Count)];

        float r = _rng.Next01() * total;
        float acc = 0f;
        foreach (var o in options)
        {
            acc += Mathf.Max(0f, o.weight);
            if (r <= acc) return o;
        }
        return options[options.Count - 1];
    }

    private IEnumerator CullInactiveAfterDelay(Transform chosenSetRoot, int delayFrames)
    {
        for (int i = 0; i < delayFrames; i++) yield return null;

        // Destroy every inactive VariantEntry's target in the chosen set
        var variants = chosenSetRoot.GetComponentsInChildren<VariantEntry>(includeInactive: true);
        foreach (var v in variants)
        {
            var go = v.Target;
            if (!go.activeSelf)
            {
                // If you’re culling during a loading screen, Destroy() is fine.
                // If you ever run this in editor edit mode, use DestroyImmediate with care.
                Destroy(go);
            }
        }
    }
}
