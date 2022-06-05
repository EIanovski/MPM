using FibonacciHeap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class MPM
{
  public const int SOURCE = int.MinValue;
  public const int SINK = int.MaxValue;

  private static bool IsInnerNode(int node)
  {
    return node != SOURCE && node != SINK;
  }

  private static void UpdatePriorities(
    FibonacciHeap<int,int> h, 
    Dictionary<int,FibonacciHeapNode<int,int>> pointersToHeap, 
    Graph G,
    IEnumerable<int> nodes)
  {
    foreach (int node in nodes)
    {
      if (IsInnerNode(node))
      {
        FibonacciHeapNode<int, int> fibNode = pointersToHeap[node];
        h.DecreaseKey(fibNode, G.Capacity(node));
      }
    }
  }

  public static int MaxFlow(Graph G)
  {
    var maxFlow = new Dictionary<int, Dictionary<int, int>>();
    return MaxFlow(G, out maxFlow);
  }

  public static int MaxFlow(
    Graph G, 
    out Dictionary<int, Dictionary<int,int>> maxFlow)
  {
    Graph residualG = G.GetResidualGraph();
    Graph levelG = residualG.GetLevelGraph();
    while (levelG.Vertices.Contains(SINK))
    {
      var h = new FibonacciHeap<int, int>(0);
      var pointersToHeap = new Dictionary<int, FibonacciHeapNode<int, int>>();
      foreach (int node in levelG.Vertices)
      {
        if (IsInnerNode(node))
        {
          var fibNode = new FibonacciHeapNode<int, int>(node, levelG.Capacity(node));
          h.Insert(fibNode);
          pointersToHeap[node] = fibNode;
        }
      }
      Dictionary<int, Dictionary<int, int>> flow = levelG.GetEmptyFlow();
      while (!h.IsEmpty())
      {
        SaturateNode(h, levelG, residualG, flow, pointersToHeap);
      }
      G.AddFlow(flow);
      Debug.Assert(G.IsFlowValid());
      residualG = G.GetResidualGraph();
      levelG = residualG.GetLevelGraph();
    }
    maxFlow = G.GetFlow();
    return G.CountFlow();
  }

  internal static void SaturateNode(
    FibonacciHeap<int,int> h,
    Graph levelG,
    Graph residualG,
    Dictionary<int, Dictionary<int,int>> flow,
    Dictionary<int, FibonacciHeapNode<int,int>> pointersToHeap)
  {
    FibonacciHeapNode<int, int> nodeAndCapacity = h.RemoveMin();
    int node = nodeAndCapacity.Data;
    int capacity = nodeAndCapacity.Key;
    var nodesOnAugmentingPath = new List<int>();
    levelG.Push(node, capacity, flow, nodesOnAugmentingPath);
    levelG.Pull(node, capacity, flow, nodesOnAugmentingPath);
    Debug.Assert(IsFlowValid(residualG.Vertices, flow));
    IEnumerable<int> inNeighbours = levelG.InNeighbours(node);
    IEnumerable<int> outNeighbours = levelG.OutNeighbours(node);
    levelG.RemoveNode(node);
    UpdatePriorities(h, pointersToHeap, levelG, inNeighbours);
    UpdatePriorities(h, pointersToHeap, levelG, outNeighbours);
    UpdatePriorities(h, pointersToHeap, levelG, nodesOnAugmentingPath);
  }

  private static bool IsFlowValid(
    IEnumerable<int> nodes,
    Func<int,int,int> flow, 
    Func<int, int, int> capacity)
  {
    var inFlow = new Dictionary<int, int>();
    var outFlow = new Dictionary<int, int>();
    foreach (int node in nodes)
    {
      inFlow[node] = 0;
      outFlow[node] = 0;
    }
    foreach (int node in nodes)
    {
      foreach (int neighbour in nodes)
      {
        if (node!= neighbour)
        {
          if (flow(node, neighbour) > capacity(node, neighbour)
            || flow(node,neighbour) < 0)
          {
            return false;
          }
          outFlow[node] += flow(node, neighbour);
          inFlow[neighbour] += flow(node, neighbour);
        }
      }
    }
    return nodes.All(node => !IsInnerNode(node) || inFlow[node] == outFlow[node]);
  }

  internal static bool IsFlowValid(
    IEnumerable<int> nodes, 
    Dictionary<int, Dictionary<int, int>> flow)
  {
    Func<int, int, int> flowFunction = (node, neighbour) =>
      {
        if (flow.ContainsKey(node) && flow[node].ContainsKey(neighbour))
        {
          return flow[node][neighbour];
        }
        return 0;
      };
    Func<int, int, int> noCap = (node, neighbour) =>
      {
        return int.MaxValue;
      };
    return IsFlowValid(nodes, flowFunction, noCap);
  }

  public class Graph
  {
    private Dictionary<int, Dictionary<int, int>> OutCapacities = new Dictionary<int, Dictionary<int, int>>();
    private Dictionary<int, Dictionary<int, int>> InCapacities = new Dictionary<int, Dictionary<int, int>>();
    private Dictionary<int, Dictionary<int, int>> OutFlow = new Dictionary<int, Dictionary<int, int>>();
    private Dictionary<int, Dictionary<int, int>> InFlow = new Dictionary<int, Dictionary<int, int>>();
    internal HashSet<int> Vertices = new HashSet<int>();

    private void AddVertexIfMissing(int vertex)
    {
      if (!Vertices.Contains(vertex))
      {
        Vertices.Add(vertex);
        OutCapacities[vertex] = new Dictionary<int, int>();
        InCapacities[vertex] = new Dictionary<int, int>();
        OutFlow[vertex] = new Dictionary<int, int>();
        InFlow[vertex] = new Dictionary<int, int>();
      }
    }

    internal int OutDegree(int vertex)
    {
      return OutCapacities[vertex].Count;
    }

    internal int InDegree(int vertex)
    {
      return InCapacities[vertex].Count;
    }

    internal Graph GetLevelGraph()
    {
      var g = new Graph();
      var q = new Queue<int>();
      var nodeLevels = new Dictionary<int, int>();
      nodeLevels[SOURCE] = 0;
      q.Enqueue(SOURCE);
      while (q.Count > 0)
      {
        int currentNode = q.Dequeue();
        foreach (int neighbour in OutCapacities[currentNode].Keys)
        {
          int cap = OutCapacities[currentNode][neighbour];
          if (!nodeLevels.ContainsKey(neighbour)) {
            nodeLevels[neighbour] = nodeLevels[currentNode] + 1;
            q.Enqueue(neighbour);
          }
          if (nodeLevels[neighbour] == nodeLevels[currentNode] +1)
          {
            g.AddEdge(currentNode, neighbour, cap);
          }
        }
      }
      return g;
    }

    internal bool HasEdge(int firstVertex, int secondVertex, int cap, int flow = 0)
    {
      if (OutCapacities.ContainsKey(firstVertex)
        && InCapacities.ContainsKey(secondVertex)
        && OutCapacities[firstVertex].ContainsKey(secondVertex)
        && InCapacities[secondVertex].ContainsKey(firstVertex))
      {
        return OutCapacities[firstVertex][secondVertex] == cap
          && InCapacities[secondVertex][firstVertex] == cap
          && OutFlow[firstVertex][secondVertex] == flow
          && InFlow[secondVertex][firstVertex] == flow;
      }
      return false;
    }

    public void AddEdge(int firstVertex,
      int secondVertex,
      int cap,
      bool augmentExisting = false)
    {
      AddEdge(firstVertex, secondVertex, cap, 0, augmentExisting);
    }

    internal void AddEdge(int firstVertex,
      int secondVertex,
      int cap,
      int flow,
      bool augmentExisting = false)
    {
      AddVertexIfMissing(firstVertex);
      AddVertexIfMissing(secondVertex);
      AddEdgeLabel(OutCapacities, firstVertex, secondVertex, cap, augmentExisting);
      AddEdgeLabel(InCapacities, secondVertex, firstVertex, cap, augmentExisting);
      AddEdgeLabel(OutFlow, firstVertex, secondVertex, flow, augmentExisting);
      AddEdgeLabel(InFlow, secondVertex, firstVertex, flow, augmentExisting);
    }

    private void AddEdgeLabel(
      Dictionary<int, Dictionary<int, int>> labels,
      int firstVertex,
      int secondVertex,
      int valueOfLabel,
      bool augmentExisting = false)
    {
      if (augmentExisting)
      {
        if (!labels[firstVertex].ContainsKey(secondVertex))
        {
          labels[firstVertex][secondVertex] = 0;
        }
        labels[firstVertex][secondVertex] += valueOfLabel;
      }
      else
      {
        labels[firstVertex][secondVertex] = valueOfLabel;
      }
    }

    internal int Capacity(int node)
    {
      int inCapacity = InCapacities[node].Values.Sum();
      int outCapacity = OutCapacities[node].Values.Sum();
      return Math.Min(inCapacity, outCapacity);
    }

    internal void Push(
      int node, 
      int amount, 
      Dictionary<int, Dictionary<int, int>> flow,
      List<int> trace)
    {
      PushOrPull(node, amount, flow, trace, true);
    }

    internal void Pull(
      int node, 
      int amount, 
      Dictionary<int, Dictionary<int, int>> flow,
      List<int> trace)
    {
      PushOrPull(node, amount, flow, trace, false);
    }

    private void PushOrPull(
      int node, 
      int amount, 
      Dictionary<int, Dictionary<int, int>> flow,
      List<int> trace,
      bool pushing)
    {
      Dictionary<int, Dictionary<int,int>> capacities = 
        pushing ? OutCapacities : InCapacities;

      foreach (int neighbour in capacities[node].Keys.ToList())
      {
        int flowFrom = pushing ? node : neighbour;
        int flowTo = pushing ? neighbour : node;
        int amountFlow = Math.Min(capacities[node][neighbour], amount);
        if (amountFlow <= 0)
        {
          continue;
        }
        trace.Add(neighbour);
        PushOrPull(neighbour, amountFlow, flow, trace, pushing);
        amount -= amountFlow;
        flow[flowFrom][flowTo] += amountFlow;
        OutCapacities[flowFrom][flowTo] -= amountFlow;
        InCapacities[flowTo][flowFrom] -= amountFlow;
        if (amount == 0)
        {
          return;
        }
      }
    }

    internal void RemoveNode(int node)
    {
      Vertices.Remove(node);
      foreach (int neighbour in OutCapacities[node].Keys)
      {
        InCapacities[neighbour].Remove(node);
      }
      OutCapacities.Remove(node);
      foreach (int neighbour in InCapacities[node].Keys)
      {
        OutCapacities[neighbour].Remove(node);
      }
      InCapacities.Remove(node);
    }

    internal IEnumerable<int> InNeighbours(int node)
    {
      return InCapacities[node].Keys;
    }

    internal IEnumerable<int> OutNeighbours(int node)
    {
      return OutCapacities[node].Keys;
    }

    internal Graph GetResidualGraph()
    {
      var g = new Graph();
      foreach (int vertex in Vertices)
      {
        AddResidualEdges(g, vertex);
      }
      return g;
    }

    private void AddResidualEdges(Graph g, int vertex)
    {
      foreach (int neighbour in OutCapacities[vertex].Keys)
      {
        int flow = OutFlow[vertex][neighbour];
        int cap = OutCapacities[vertex][neighbour];
        int residualCap = cap - flow;
        if (residualCap > 0)
        {
          g.AddEdge(vertex, neighbour, residualCap, augmentExisting: true);
        }
        if (flow > 0)
        {
          g.AddEdge(neighbour, vertex, flow, augmentExisting : true);
        }
      }
    }

    internal void AddFlow(Dictionary<int, Dictionary<int, int>> flow)
    {
      foreach (int node in flow.Keys)
      {
        foreach (int target in flow[node].Keys)
        {
          if (!OutFlow[node].ContainsKey(target))
          {
            int backFlow = -flow[node][target];
            OutFlow[target][node] += backFlow;
            InFlow[node][target] += backFlow;
          }
          else
          {
            int extraFlow = flow[node][target];
            OutFlow[node][target] += extraFlow;
            InFlow[target][node] += extraFlow;
          }
        }
      }
    }

    internal bool IsFlowValid()
    {
      Func<int, int, int> flow = (node, neighbour) =>
      {
        if (OutFlow[node].ContainsKey(neighbour))
        {
          return OutFlow[node][neighbour];
        }
        return 0;
      };
      Func<int, int, int> cap = (node, neighbour) =>
      {
        if (OutCapacities[node].ContainsKey(neighbour))
        {
          return OutCapacities[node][neighbour];
        }
        return 0;
      };
      return MPM.IsFlowValid(Vertices, flow, cap);
    }

    internal int CountFlow()
    {
      return InFlow.ContainsKey(SINK) ? InFlow[SINK].Values.Sum() : 0;
    }

    internal Dictionary<int, Dictionary<int, int>> GetEmptyFlow()
    {
      var flow = new Dictionary<int, Dictionary<int, int>>();
      foreach (int node in Vertices)
      {
        flow[node] = new Dictionary<int, int>();
        foreach (int neighbour in OutCapacities[node].Keys)
        {
          flow[node][neighbour] = 0;
        }
      }
      return flow;
    }

    internal Dictionary<int, Dictionary<int, int>> GetFlow()
    {
      var flow = new Dictionary<int, Dictionary<int, int>>();
      foreach (int node in OutFlow.Keys)
      {
        flow[node] = new Dictionary<int, int>();
        foreach (int neighbour in OutFlow[node].Keys)
        {
          flow[node][neighbour] = OutCapacities[node][neighbour];
        }
      }
      return flow;
    }
  }
}
