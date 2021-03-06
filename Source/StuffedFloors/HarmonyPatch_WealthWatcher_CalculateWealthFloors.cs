﻿// HarmonyPatch_WealthWatcher_CalculateWealthFloors.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;

namespace StuffedFloors
{
    /**
     * The vanilla game stores cached market values in WealthWatcher.cachedTerrainMarketValue,
     * and indexes this list by def.index. Somehow, even though this cache is initialized after
     * our terrains are generated, these indexes are out of sync. I suspect this is caused by 
     * subclassing TerrainDef with FloorTypeDef. 
     * 
     * Regardless, the cache is fairly pointless, as we don't need to check value for every cell
     * like vanilla does, we only need to check it for each unique terrainDef. In addition, this
     * routine is performed once every 5000 ticks, so less than once per minute. Looping over a 
     * map isn't _that_ expensive, so the below approach is simplified.
     * 
     *      - Fluffy.
     */
    [HarmonyPatch( typeof( WealthWatcher ), "CalculateWealthFloors" )]
    public class HarmonyPatch_WealthWatcher_CalculateWealthFloors
    {
        public static bool Prefix( Map ___map, ref float __result )
        {
            var terrainGrid = ___map.terrainGrid.topGrid;
            var fogGrid = ___map.fogGrid.fogGrid;
            var n = terrainGrid.Length;
            var counts = new Dictionary<TerrainDef, int>();
            var total = 0f;

            // note that an argument for checking for ownership could be made, but that doesn't
            // exist for floors, so it's a moot point.
            for ( int i = 0; i < n; i++ )
            {
                if ( !fogGrid[i] )
                {
                    if ( counts.ContainsKey( terrainGrid[i] ) )
                        counts[terrainGrid[i]] += 1;
                    else
                        counts[terrainGrid[i]] = 1;
                }
            }

            foreach ( var terrainCount in counts )
                total += terrainCount.Key.GetStatValueAbstract( StatDefOf.MarketValue ) * terrainCount.Value;
            __result = total;

            return false;
        }
    }
}