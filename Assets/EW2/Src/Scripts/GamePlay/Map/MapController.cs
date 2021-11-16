using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EW2;
using Unity.Mathematics;
using UnityEngine;

namespace Map
{
    public class MapController : MonoBehaviour
    {
        private int mapId = -1;

        public int MapId
        {
            get => mapId;
            set => mapId = value;
        }

        public List<LineController> lines;

        public List<Transform> pointSpawnHero;

        private List<PointCallSpawn> pointButtonSpawn;

        public List<PointCallSpawn> PointButtonSpawn
        {
            get
            {
                if (pointButtonSpawn == null)
                {
                    pointButtonSpawn = new List<PointCallSpawn>();

                    pointButtonSpawn = GetComponentsInChildren<PointCallSpawn>().ToList();
                }

                return pointButtonSpawn;
            }
        }

        private BorderMapController borderMapController;

        public BorderMapController BorderMap
        {
            get
            {
                if (borderMapController == null)
                    borderMapController = GetComponentInChildren<BorderMapController>();

                return borderMapController;
            }
        }
    }
}