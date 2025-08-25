using DungeonGeneration.Service.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ProcGen/Decorators/Debug Texture Decorator")]
public class DebugTextureDecoratorSO : DecoratorSO
{

    private static readonly string RENDERER_NAME = "DecorationTexture";

    [SerializeField] private DecorationMatcherSO _decorationMatcher;
    [SerializeField] private Renderer _decorationRenderer;
    private Renderer _renderer;
    private int _cellHeight;
    private int _cellWidth;
    private float _resolvedWidth;
    private float _resolvedHeight;
    private bool[,] _blockedGrid;
    private float _mapScale;
    public override void ApplyDecorations(ICapabilityProvider level)
    {
        GetOrCreateRenderer();
        InitValues(level);
        var matches = GetDecorationMatches(level);
        DrawLayout(matches);
    }

    private IEnumerable<IDecorationMatch> GetDecorationMatches(ICapabilityProvider level)
    {
        return _decorationMatcher.GetDecorationMatches(level);
    }

    private void InitValues(ICapabilityProvider level)
    {
        if (!level.TryGet<Dimensions>(out var dimensions)
            || !level.TryGet<Scale>(out var mapScale)
            || !level.TryGet<BlockMask>(out var blockMask))
        {
            throw new System.Exception($"Level generation missing data required for Debug Texture Decorator");
        }

        _cellWidth = dimensions.MapDimensions.x;
        _cellHeight = dimensions.MapDimensions.y;
        _mapScale = mapScale.MapScale;
        _resolvedWidth = _cellWidth * _mapScale;
        _resolvedHeight = _cellHeight * _mapScale;

        _blockedGrid = blockMask.Mask;
    }

    private void GetOrCreateRenderer()
    {
        var existing = GameObject.FindGameObjectWithTag(RENDERER_NAME);

        if (existing != null && existing.TryGetComponent<Renderer>(out var existingRenderer))
        {
            _renderer = existingRenderer;
            return;
        }
            
        _renderer = ObjectSpawnerSingleton.Instance.Spawn(_decorationRenderer);
    }

    void DrawLayout(IEnumerable<IDecorationMatch> decorationMatches)
    {
        var positions = decorationMatches.Select(match => match.SpawnPosition).ToHashSet();

        //initialize using cell width / cell height and stretch after initialization and pixel assignment
        _renderer.transform.localScale = new Vector3(_cellHeight, _cellWidth, 1);
        var layoutTexture = (Texture2D)_renderer.sharedMaterial.mainTexture;
        layoutTexture.Reinitialize(_cellWidth, _cellHeight);
        layoutTexture.FillWithColor(Color.black);

        for (int y = 0; y < _cellHeight; ++y)
        {
            for (int x = 0; x < _cellWidth; ++x)
            {
                if (positions.Contains(new Vector2Int(x,y)))
                {
                    layoutTexture.SetPixel(x, y, Color.red);
                }
                else
                {
                    layoutTexture.SetPixel(x, y, _blockedGrid[y, x] ? Color.black : Color.white);
                }
            }
        }

        layoutTexture.SaveAsset();

        //position and scale the renderer component
        _renderer.transform.localScale = new Vector3(_resolvedWidth, _resolvedHeight, 1);
        _renderer.transform.position = new Vector3((_resolvedWidth / 2) - _mapScale, 1, (_resolvedHeight / 2) - _mapScale);
        _renderer.transform.localRotation = Quaternion.Euler(90, 0, 0);

    }

}
