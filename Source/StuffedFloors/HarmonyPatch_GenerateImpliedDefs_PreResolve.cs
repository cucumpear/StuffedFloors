﻿// Karel Kroeze
// HarmonyPatch_GenerateImpliedDefs_PreResolve.cs
// 2017-05-07

using Harmony;
using RimWorld;
using Verse;

namespace StuffedFloors
{
    [HarmonyPatch( typeof(DefGenerator), "GenerateImpliedDefs_PreResolve" )]
    public class HarmonyPatch_GenerateImpliedDefs_PreResolve
    {
        public static void Prefix()
        {
            foreach ( FloorTypeDef floortype in DefDatabase<FloorTypeDef>.AllDefsListForReading )
            {
                foreach ( ThingDef stuff in Controller.GetStuffDefsFor( floortype.stuffCategories ) )
                {
                    // create and prepare def
                    var terrain = floortype.GetStuffedTerrainDef( stuff );
                    terrain.PostLoad();

                    // add to databases
                    DefDatabase<TerrainDef>.Add( terrain );
                    floortype.terrains.Add( terrain );
                }
            }
        }
    }
}