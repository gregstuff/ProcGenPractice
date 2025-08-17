using UnityEngine;
using DungeonGeneration.Map.Enum;
using System;

public class StickyUIPanel
{
    private const float PALETTE_WIDTH = 150f;
    private const float PALETTE_ITEM_HEIGHT = 20f;
    private const float HEADER_BUTTONS_HEIGHT = 50f; 
    private const float MARGIN = 10f;

    public static void Construct(
        TilePaletteUIModel tilePalette,
        Action onAddButtonClicked,
        Action onSaveButtonClicked,
        Action onLoadButtonClicked,
        Rect windowRect)
    {
        float paletteHeight = System.Enum.GetValues(typeof(TileType)).Length * PALETTE_ITEM_HEIGHT * 1.5f;

        Rect currentOffset = new Rect(windowRect.width - PALETTE_WIDTH - 10f, 10f, PALETTE_WIDTH, paletteHeight);

        TilePaletteUI.Construct(tilePalette, currentOffset);

        currentOffset = new Rect()
        {
            x = currentOffset.x,
            y = currentOffset.y + paletteHeight + MARGIN,
            width = currentOffset.width,
            height = currentOffset.height
        };

        HeaderButtonsUI.Construct(
            onAddButtonClicked,
            onSaveButtonClicked,
            onLoadButtonClicked,
            currentOffset);
    }
}