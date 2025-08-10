
// Utility class for common spawn helpers
using DungeonGeneration.Map.SO;
using static DecorationPatternMatcher;
using UnityEngine;

public static class DecorationSpawnUtils
{
    public static void SpawnDecoration(DecorationPlacement placement, Transform parent, TilesetConfigSO tileset)
    {
        if (placement.Prefab == null) return;

        GameObject obj = GameObject.Instantiate(placement.Prefab, placement.SpawnPosition, placement.Rotation, parent);
        obj.transform.localScale = tileset.tileScale;

        // Additional setup based on rule type
        if (placement.Rule is ActorRuleSO actorRule)
        {
            // Apply AI config, e.g., obj.GetComponent<ActorAI>().Initialize(actorRule.AIConfig);
        }
        else if (placement.Rule is InteractableRuleSO interactRule)
        {
            // Apply interactable config
        }
        // Static needs no extra setup
    }

    public static Vector3 RaycastForPlacement(Vector3 startPos, SpawnDirection direction, float rayDistance = 10f)
    {
        // Placeholder for raycasting to align with walls/floors
        // E.g., Ray ray = new Ray(startPos + Vector3.up * 0.1f, GetDirectionVector(direction));
        // if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        // {
        //     return hit.point;
        // }
        return startPos; // Fallback
    }

    private static Vector3 GetDirectionVector(SpawnDirection direction)
    {
        switch (direction)
        {
            case SpawnDirection.TopWall: return Vector3.forward;
            case SpawnDirection.BottomWall: return Vector3.back;
            case SpawnDirection.LeftWall: return Vector3.left;
            case SpawnDirection.RightWall: return Vector3.right;
            default: return Vector3.zero;
        }
    }
}
