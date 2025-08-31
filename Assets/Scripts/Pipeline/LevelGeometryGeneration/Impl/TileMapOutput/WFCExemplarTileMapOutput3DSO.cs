using ProcGenSys.Common.LevelBundle;
using ProcGenSys.WFC.Marker;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelGeometryGeneration
{
    [CreateAssetMenu(menuName = "ProcGen/Output/Tile/2D/WFC Exemplar Tile Map 3D")]
    public class WFCExemplarTileMapOutput3DSO : TileMapOutput3DSO
    {
        public override void OutputMap(ICapabilityProvider level)
        {
            base.OutputMap(level);

            //add meta data to output....
            var dungeonParent = GameObject.FindGameObjectWithTag(DUNGEON_PARENT_TAG);

            if (dungeonParent == null) throw new System.Exception($"Dungeon Parent was null in {typeof(WFCExemplarTileMapOutput3DSO)}");

            if (!level.TryGet<Dimensions>(out var mapDimensions)
                || !level.TryGet<Scale>(out var mapScale)
                || !level.TryGet<TileLayer>(out var tiles))
            {
                throw new System.Exception($"Map Data does not meet requirements for {typeof(WFCExemplarTileMapOutput3DSO)}");
            }

            var exemplar = dungeonParent.AddComponent<WFCLevelExemplar>();

            exemplar.SetGrid(mapDimensions.MapDimensions, mapScale.MapScale, tiles.Tiles);
        }
    }
}
