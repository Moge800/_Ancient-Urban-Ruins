using AncientMarket_Libraray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarketAI_Libraray
{
    public static class LevelPather
    {
        public static List<AMMapPortal> GetPathPortal(Map root, AMMapPortal destination)
        {
            List<AMMapPortal> result = new List<AMMapPortal>();
            List<Map> destinationToRoot = GetAllParentMaps(destination);
            if (destinationToRoot.Contains(root))
            {
                result.AddRange(GetPathLine(root, destination));
            }
            else if (root.Parent is MapParent_Custom custom)
            {
                List<Map> rootParent = GetAllParentMaps(destination);
                Map BranchMap = destination.Map;
                while (BranchMap.Parent as MapParent_Custom != null)
                {
                    if (!rootParent.Contains(BranchMap))
                    {
                        BranchMap = (BranchMap.Parent as MapParent_Custom).entrance.Map;
                    }
                    else
                    {
                        break;
                    }
                }
                result.AddRange(GetPathLineReverse(custom.Map, BranchMap));
            }
            else
            {
                StringBuilder log = new StringBuilder("?There is a strange bug in Ancient Market Mod");
                log.AppendLine(destination.Label);

                Log.Error(log.ToString().Trim());
               
            }
            return result;
        }
        public static List<AMMapPortal> GetPathLineReverse(Map root, Map destination)
        {
            List<AMMapPortal> result = new List<AMMapPortal>();
            Map curMap = root;
            while (curMap.Parent as MapParent_Custom != null)
            {
                if (curMap.Parent is MapParent_Custom map)
                {
                    result.Add(map.Exit);
                    if (curMap == destination)
                    {
                        break;
                    }
                    curMap = map.sourceMap;
                }
            }
            return result;
        }
        public static List<AMMapPortal> GetPathLine(Map root, AMMapPortal destination)
        {
            List<AMMapPortal> result = new List<AMMapPortal>() { destination };
            Map curMap = destination.Map;
            while (curMap.Parent as MapParent_Custom != null)
            {
                if (curMap.Parent is MapParent_Custom map)
                {
                    result.Add(map.entrance);
                    curMap = map.sourceMap;
                    if (curMap == root)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        public static List<Map> GetAllParentMaps(AMMapPortal portal)
        {
            List<Map> result = new List<Map>();
            if (portal.Map.Parent is MapParent_Custom map)
            {
                result.AddRange(GetParent(map));
            }
            else 
            {
                result.Add(portal.Map); 
            }
            return result;
        }

        public static List<Map> GetParent(MapParent_Custom map)
        {
            List<Map> result = new List<Map>() { map.Map};
            if (map.sourceMap.Parent is MapParent_Custom map2)
            {
                result.AddRange(GetParent(map2));
            }
            else 
            {
                result.Add(map.sourceMap);
            }
            return result;
        }
    }
}
