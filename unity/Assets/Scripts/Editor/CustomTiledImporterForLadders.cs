using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
class CustomTiledImporterForLadders : Tiled2Unity.ICustomTiledImporter
{
    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props)
    {
        // Do nothing
    }

    public void CustomizePrefab(GameObject prefab)
    {
        // Find all the polygon colliders in the pefab
        var polygon2Ds = prefab.GetComponentsInChildren<PolygonCollider2D>();
        if (polygon2Ds == null)
            return;

        // Find all *ladder* polygon colliders
        int ladderMask = LayerMask.NameToLayer("Ladders");
        var ladderPolygons = from polygon in polygon2Ds
                             where polygon.gameObject.layer == ladderMask
                             select polygon;

        // For each ladder path in a ladder polygon collider
        // add a top, spine, and bottom edge collider
        foreach (var poly in ladderPolygons)
        {
            GameObject ladderTops = new GameObject("LadderTop-EdgeColliders");
            GameObject ladderSpines = new GameObject("LadderSpine-EdgeColliders");
            GameObject ladderBottoms = new GameObject("LadderBottom-EdgeColliders");
            ladderTops.layer = LayerMask.NameToLayer("LadderTops");
            ladderSpines.layer = LayerMask.NameToLayer("LadderSpines");
            ladderBottoms.layer = LayerMask.NameToLayer("LadderBottoms");

            // Create edge colliders for the ladder tops
            // We assume that every polygon path represents a ladder
            for (int p = 0; p < poly.pathCount; ++p)
            {
                Vector2[] points = poly.GetPath(p);
                points = points.Select(pt => new Vector2(pt.x + poly.transform.position.x, pt.y + poly.transform.position.y)).ToArray();

                float xmin = points.Min(pt => pt.x);
                float xmax = points.Max(pt => pt.x);
                float ymax = points.Max(pt => pt.y);
                float ymin = points.Min(pt => pt.y);
                float xcen = xmin + (xmax - xmin) * 0.5f;

                // Add our edge collider points for the ladder top
                EdgeCollider2D topEdgeCollider2d =
                    ladderTops.AddComponent<EdgeCollider2D>();
                topEdgeCollider2d.points = new Vector2[]
                {
                    new Vector2(xmin, ymax),
                    new Vector2(xmax, ymax),
                };

                // Add our edge collider points for the ladder spine
                EdgeCollider2D spineEdgeCollider2d =
                    ladderSpines.AddComponent<EdgeCollider2D>();
                spineEdgeCollider2d.points = new Vector2[]
                {
                    new Vector2(xcen, ymin),
                    new Vector2(xcen, ymax),
                };

                // Add our edge collider points for the ladder bottom
                EdgeCollider2D bottomEdgeCollider2d =
                    ladderBottoms.AddComponent<EdgeCollider2D>();
                bottomEdgeCollider2d.points = new Vector2[]
                {
                    new Vector2(xmin, ymin),
                    new Vector2(xmax, ymin),
                };
            }

            // Parent the ladder components to our ladder object
            ladderTops.transform.parent = poly.gameObject.transform;
            ladderSpines.transform.parent = poly.gameObject.transform;
            ladderBottoms.transform.parent = poly.gameObject.transform;
        }
    }
}

