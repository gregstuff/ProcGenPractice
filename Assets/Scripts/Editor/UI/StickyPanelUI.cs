using UnityEngine;
using System;

public class StickyUIPanel
{
    private const float PALETTE_HEIGHT = 150f;
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
        //get num tile rules here

        Rect currentOffset = new Rect(windowRect.width - PALETTE_WIDTH - 10f, 10f, PALETTE_WIDTH, PALETTE_HEIGHT);

        TilePaletteUI.Construct(tilePalette, currentOffset);

        currentOffset = new Rect()
        {
            x = currentOffset.x,
            y = currentOffset.y + PALETTE_HEIGHT + MARGIN,
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