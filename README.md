# MPM
A C# implementation of the Malhotra, Pramodh-Kumar and Maheshwari max flow algorithm, using Fibonacci heaps for the priority queue. This implementation is based on the exposition in http://www.cs.cornell.edu/courses/cs4820/2013sp/Handouts/DinicMPM.pdf and https://en.wikipedia.org/wiki/Dinic%27s_algorithm, since I could not make sense of the original paper.

Requirements:
The FibonacciHeap package by Anton Herzog (via NuGet package manager).

Usage:
Create an MPM.Graph object and add edges using MPM.AddEdge(int startVertex, int endVertex, int capacity). Make sure to include an MPM.SOURCE (int.MinValue) and an MPM.SINK (int.MaxValue) vertex. Run MPM.MaxFlow(MPM.Graph G) to get the value of the max flow, or MPM.MaxFlow(MPM.Graph G, out Dictionary<int,Dictionary<int,int>> flow) to get both the value and the flow in the form of a mapping firstVertex => secondVertex => value of flow.

E.g.
```
var G = new MPM.Graph();
G.Add(MPM.SOURCE, 0, 2);
G.Add(MPM.SOURCE, 1, 2);
G.Add(0, 2, 2);
G.Add(1, 2, 2);
G.Add(2, MPM.SINK, 3);
Assert(MPM.MaxFlow(G) == 3);
```
