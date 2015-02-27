using System.Collections.Generic;
using UnityEngine;

namespace Game.Code.Scripts.Meshing
{
    public class TravelingSalesmanProblem<T> where T : class, ISalesmanNode
    {

        public static List<T> Sort(List<T> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return nodes;

            List<T> r = new List<T>(nodes.Count);
            List<T> ignore = new List<T>(nodes.Count);

            T firstNode = nodes[0];
            r.Add(firstNode);
            ignore.Add(firstNode);

            T lastNode = FindClosestNode(firstNode, ignore, nodes);
            ignore.Add(lastNode);

            ChainAddClosestNodes(firstNode, r, ignore, nodes);

            r.Add(lastNode);
            return r;
        }

        private static T FindClosestNode(T node, ICollection<T> ignoreNodes, IList<T> nodes)
        {
            float dis = 1000;
            T closest = null;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] == null || nodes[i] == node || ignoreNodes.Contains(nodes[i]))
                    continue;

                float dis2 = Vector3.Distance(node.Position, nodes[i].Position);

                if (dis2 < dis)
                {
                    dis = dis2;
                    closest = nodes[i];
                }
            }

            return closest;
        }

        private static void ChainAddClosestNodes(T node, ICollection<T> toAdd, ICollection<T> ignoreNodes, IList<T> nodes)
        {
            var closestNode = FindClosestNode(node, ignoreNodes, nodes);

            if (closestNode != null)
            {
                toAdd.Add(closestNode);
                ignoreNodes.Add(closestNode);
                ChainAddClosestNodes(closestNode, toAdd, ignoreNodes, nodes);
            }
        }
    }

    public interface ISalesmanNode
    {
        Vector3 Position { get; }
    }
}
