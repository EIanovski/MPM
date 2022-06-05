using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SolverFoundation.Common;
using System.Threading.Tasks;
using FibonacciHeap;

namespace Tests
{
	[TestClass]
	public class MaxFlowTests
	{
		[TestMethod]
		public void TestOutDegree()
		{
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(0, 1, 2);
			g.AddEdge(0, 2, 3);
			g.AddEdge(1, 2, 1);

			Assert.AreEqual(2, g.OutDegree(0));
			Assert.AreEqual(0, g.OutDegree(2));
		}

		[TestMethod]
		public void TestInDegree()
		{
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(0, 1, 2);
			g.AddEdge(0, 2, 3);
			g.AddEdge(1, 2, 1);

			Assert.AreEqual(2, g.InDegree(2));
			Assert.AreEqual(0, g.InDegree(0));
		}

		[TestMethod]
		public void TestAddEdge()
		{
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(0, 1, 2);
			g.AddEdge(0, 2, 3);
			g.AddEdge(1, 2, 1);

			Assert.AreEqual(2, g.OutDegree(0));
			Assert.AreEqual(0, g.InDegree(0));
			Assert.IsTrue(g.HasEdge(0, 1, 2));
			Assert.IsTrue(g.HasEdge(0, 2, 3));

			Assert.AreEqual(1, g.OutDegree(1));
			Assert.AreEqual(1, g.InDegree(1));
			Assert.IsTrue(g.HasEdge(1, 2, 1));

			Assert.AreEqual(2, g.InDegree(2));
			Assert.AreEqual(0, g.OutDegree(2));
		}

		[TestMethod]
		public void TestAddEdgeWithFlow()
		{
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(0, 1, 2, 3);
			g.AddEdge(0, 2, 3, 4);
			g.AddEdge(1, 2, 1, 0);

			Assert.AreEqual(2, g.OutDegree(0));
			Assert.AreEqual(0, g.InDegree(0));
			Assert.IsTrue(g.HasEdge(0, 1, 2, 3));
			Assert.IsTrue(g.HasEdge(0, 2, 3, 4));
			
			Assert.AreEqual(1, g.OutDegree(1));
			Assert.AreEqual(1, g.InDegree(1));
			Assert.IsTrue(g.HasEdge(1, 2, 1, 0));

			Assert.AreEqual(2, g.InDegree(2));
			Assert.AreEqual(0, g.OutDegree(2));
		}

		[TestMethod]
		public void TestAddFlow()
		{
			// First graph https://upload.wikimedia.org/wikipedia/commons/3/37/Dinic_algorithm_G1.svg
			// Second graph https://upload.wikimedia.org/wikipedia/commons/5/56/Dinic_algorithm_G2.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 0);
			g.AddEdge(MPM.SOURCE, 2, 10, 0);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 0);
			g.AddEdge(1, 4, 8, 0);
			g.AddEdge(2, 4, 9, 0);
			g.AddEdge(3, MPM.SINK, 10, 0);
			g.AddEdge(4, 3, 6, 0);
			g.AddEdge(4, MPM.SINK, 10, 0);

			Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
			flow[MPM.SOURCE] = new Dictionary<int, int>
			{
				{1, 10 },
				{2, 4 },
			};
			flow[1] = new Dictionary<int, int>
			{
				{ 3,4 },
				{4,6 }
			};
			flow[2] = new Dictionary<int, int>
			{
				{4,4 }
			};
			flow[3] = new Dictionary<int, int>
			{
				{MPM.SINK, 4 }
			};
			flow[4] = new Dictionary<int, int>
			{
				{MPM.SINK,10 }
			};

			g.AddFlow(flow);

			Assert.IsTrue(g.HasEdge(MPM.SOURCE, 1, 10, 10));
			Assert.IsTrue(g.HasEdge(MPM.SOURCE, 2, 10, 4));
			Assert.IsTrue(g.HasEdge(1, 2, 2, 0));
			Assert.IsTrue(g.HasEdge(1, 3, 4, 4));
			Assert.IsTrue(g.HasEdge(1, 4, 8, 6));
			Assert.IsTrue(g.HasEdge(2, 4, 9, 4));
			Assert.IsTrue(g.HasEdge(3, MPM.SINK, 10, 4));
			Assert.IsTrue(g.HasEdge(4, 3, 6, 0));
			Assert.IsTrue(g.HasEdge(4, MPM.SINK, 10, 10));
		}

		[TestMethod]
		public void TestAddBackFlow()
		{
			MPM.Graph G = new MPM.Graph();
			G.AddEdge(0, 1, 10, 10);

			Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>
			{
				{ 1, new Dictionary<int, int> { { 0, 5 } } }
			};
			G.AddFlow(flow);
			Assert.IsTrue(G.HasEdge(0, 1, 10, 5));
		}

		[TestMethod]
		public void TestAddFlow2()
		{
			// First graph https://upload.wikimedia.org/wikipedia/commons/5/56/Dinic_algorithm_G2.svg
			// Second graph https://upload.wikimedia.org/wikipedia/commons/7/71/Dinic_algorithm_G3.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 10);
			g.AddEdge(MPM.SOURCE, 2, 10, 4);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 4);
			g.AddEdge(1, 4, 8, 6);
			g.AddEdge(2, 4, 9, 4);
			g.AddEdge(3, MPM.SINK, 10, 4);
			g.AddEdge(4, 3, 6, 0);
			g.AddEdge(4, MPM.SINK, 10, 10);

			Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
			flow[MPM.SOURCE] = new Dictionary<int, int>
			{
				{2, 5 },
			};
			flow[2] = new Dictionary<int, int>
			{
				{4,5 }
			};
			flow[3] = new Dictionary<int, int>
			{
				{MPM.SINK, 5 }
			};
			flow[4] = new Dictionary<int, int>
			{
				{3,5 }
			};

			g.AddFlow(flow);

			Assert.IsTrue(g.HasEdge(MPM.SOURCE, 1, 10, 10));
			Assert.IsTrue(g.HasEdge(MPM.SOURCE, 2, 10, 9));
			Assert.IsTrue(g.HasEdge(1, 2, 2, 0));
			Assert.IsTrue(g.HasEdge(1, 3, 4, 4));
			Assert.IsTrue(g.HasEdge(1, 4, 8, 6));
			Assert.IsTrue(g.HasEdge(2, 4, 9, 9));
			Assert.IsTrue(g.HasEdge(3, MPM.SINK, 10, 9));
			Assert.IsTrue(g.HasEdge(4, 3, 6, 5));
			Assert.IsTrue(g.HasEdge(4, MPM.SINK, 10, 10));
		}

		[TestMethod]
		public void TestRemoveNode()
		{
			MPM.Graph G = new MPM.Graph();

			G.AddEdge(1, 0, 0);
			G.AddEdge(2, 0, 0);
			G.AddEdge(1, 2, 0);
			G.AddEdge(0, 3, 0);
			G.AddEdge(0, 4, 0);
			G.AddEdge(4, 3, 0);

			G.RemoveNode(0);

			Assert.IsTrue(G.HasEdge(1, 2, 0));
			Assert.IsTrue(G.HasEdge(4, 3, 0));
			Assert.IsFalse(G.HasEdge(1, 0, 0));
			Assert.IsFalse(G.HasEdge(2, 0, 0));
			Assert.IsFalse(G.HasEdge(0, 3, 0));
			Assert.IsFalse(G.HasEdge(0, 4, 0));
		}

		[TestMethod]
		public void TestResidualGraphWikipedia1()
		{
			// Flow graph https://upload.wikimedia.org/wikipedia/commons/3/37/Dinic_algorithm_G1.svg
			// Residual graph https://upload.wikimedia.org/wikipedia/commons/f/fd/Dinic_algorithm_Gf1.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 0);
			g.AddEdge(MPM.SOURCE, 2, 10, 0);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 0);
			g.AddEdge(1, 4, 8, 0);
			g.AddEdge(2, 4, 9, 0);
			g.AddEdge(3, MPM.SINK, 10, 0);
			g.AddEdge(4, 3, 6, 0);
			g.AddEdge(4, MPM.SINK, 10, 0);

			MPM.Graph resG = g.GetResidualGraph();

			Assert.AreEqual(2, resG.OutDegree(MPM.SOURCE));
			Assert.AreEqual(0, resG.InDegree(MPM.SOURCE));
			Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 1, 10));
			Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 2, 10));

			Assert.AreEqual(3, resG.OutDegree(1));
			Assert.AreEqual(1, resG.InDegree(1));
			Assert.IsTrue(resG.HasEdge(1, 2, 2));
			Assert.IsTrue(resG.HasEdge(1, 3, 4));
			Assert.IsTrue(resG.HasEdge(1, 4, 8));

			Assert.AreEqual(1, resG.OutDegree(2));
			Assert.AreEqual(2, resG.InDegree(2));
			Assert.IsTrue(resG.HasEdge(2, 4, 9));

			Assert.AreEqual(1, resG.OutDegree(3));
			Assert.AreEqual(2, resG.InDegree(3));
			Assert.IsTrue(resG.HasEdge(3, MPM.SINK, 10));

			Assert.AreEqual(2, resG.OutDegree(4));
			Assert.AreEqual(2, resG.InDegree(4));
			Assert.IsTrue(resG.HasEdge(4, MPM.SINK, 10));
			Assert.IsTrue(resG.HasEdge(4, 3, 6));

			Assert.AreEqual(2, resG.InDegree(MPM.SINK));
			Assert.AreEqual(0, resG.OutDegree(MPM.SINK));
		}

		[TestMethod]
		public void TestEmptyFlow()
		{
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10);
			g.AddEdge(MPM.SOURCE, 2, 10);
			g.AddEdge(1, 2, 2);
			g.AddEdge(1, 3, 4);
			g.AddEdge(1, 4, 8);
			g.AddEdge(2, 4, 9);
			g.AddEdge(3, MPM.SINK, 10);
			g.AddEdge(4, 3, 6);
			g.AddEdge(4, MPM.SINK, 10);

			Dictionary<int, Dictionary<int, int>> flow = g.GetEmptyFlow();
			Assert.IsTrue(flow[MPM.SOURCE].Keys.Count == 2);
			Assert.IsTrue(flow[MPM.SOURCE][1] == 0);
			Assert.IsTrue(flow[MPM.SOURCE][2] == 0);

			Assert.IsTrue(flow[1].Keys.Count == 3);
			Assert.IsTrue(flow[1][2] == 0);
			Assert.IsTrue(flow[1][3] == 0);
			Assert.IsTrue(flow[1][4] == 0);

			Assert.IsTrue(flow[2].Keys.Count == 1);
			Assert.IsTrue(flow[2][4] == 0);

			Assert.IsTrue(flow[3].Keys.Count == 1);
			Assert.IsTrue(flow[3][MPM.SINK] == 0);

			Assert.IsTrue(flow[4].Keys.Count == 2);
			Assert.IsTrue(flow[4][3] == 0);
			Assert.IsTrue(flow[4][MPM.SINK] == 0);
		}

		[TestMethod]
		public void TestPushingAndPullingCreatesValidFlow()
		{
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10);
			g.AddEdge(MPM.SOURCE, 2, 10);
			g.AddEdge(1, 2, 2);
			g.AddEdge(1, 3, 4);
			g.AddEdge(1, 4, 8);
			g.AddEdge(2, 4, 9);
			g.AddEdge(3, MPM.SINK, 10);
			g.AddEdge(4, 3, 6);
			g.AddEdge(4, MPM.SINK, 10);

			IEnumerable<int> vertices = new int[]
			{
				MPM.SOURCE, 1, 2, 3, 4, MPM.SINK
			};
			MPM.Graph levelG = g.GetLevelGraph();

			Dictionary<int, Dictionary<int, int>> flow = levelG.GetEmptyFlow();
			levelG.Push(1, 10, flow, new List<int>());
			levelG.Pull(1, 10, flow, new List<int>());
			Assert.IsTrue(MPM.IsFlowValid(vertices, flow));
			levelG.Push(2, 4, flow, new List<int>());
			levelG.Pull(2, 4, flow, new List<int>());
			Assert.IsTrue(MPM.IsFlowValid(vertices, flow));
		}

		[TestMethod]
		public void TestUnbalancedFlow()
		{
			MPM.Graph G = new MPM.Graph();
			G.AddEdge(MPM.SOURCE, 1, 5, 5);
			G.AddEdge(MPM.SOURCE, 2, 5, 5);
			G.AddEdge(1, 3, 5, 5);
			G.AddEdge(2, 3, 5, 5);
			G.AddEdge(3, MPM.SINK, 5, 5);
			Assert.IsFalse(G.IsFlowValid());
		}

		[TestMethod]
		public void TestUnbalancedFlow2()
		{
			IEnumerable<int> nodes = new int[]
			{
				MPM.SOURCE, 1, 2, 3, MPM.SINK
			};
			Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
			flow[MPM.SOURCE] = new Dictionary<int, int>();
			flow[MPM.SOURCE][1] = 5;
			flow[MPM.SOURCE][2] = 5;
			flow[1] = new Dictionary<int, int>();
			flow[1][3] = 5;
			flow[2] = new Dictionary<int, int>();
			flow[2][3] = 5;
			flow[3] = new Dictionary<int, int>();
			flow[3][MPM.SINK] = 5;
			Assert.IsFalse(MPM.IsFlowValid(nodes, flow));
		}

		[TestMethod]
		public void TestFlowViolatesCap()
		{
			MPM.Graph G = new MPM.Graph();
			G.AddEdge(MPM.SOURCE, 1, 5, 5);
			G.AddEdge(MPM.SOURCE, 2, 5, 5);
			G.AddEdge(1, 3, 5, 5);
			G.AddEdge(2, 3, 5, 5);
			G.AddEdge(3, MPM.SINK, 10, 5);
			Assert.IsFalse(G.IsFlowValid());
		}

		[TestMethod]
		public void TestFlowIsFine2()
		{
			IEnumerable<int> nodes = new int[]
			{
				MPM.SOURCE, 1, 2, 3, MPM.SINK
			};
			Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
			flow[MPM.SOURCE] = new Dictionary<int, int>();
			flow[MPM.SOURCE][1] = 5;
			flow[MPM.SOURCE][2] = 5;
			flow[1] = new Dictionary<int, int>();
			flow[1][3] = 5;
			flow[2] = new Dictionary<int, int>();
			flow[2][3] = 5;
			flow[3] = new Dictionary<int, int>();
			flow[3][MPM.SINK] = 10;
			Assert.IsTrue(MPM.IsFlowValid(nodes, flow));
		}

		[TestMethod]
		public void TestFlowIsFine()
		{
			MPM.Graph G = new MPM.Graph();
			G.AddEdge(MPM.SOURCE, 1, 5, 5);
			G.AddEdge(MPM.SOURCE, 2, 5, 5);
			G.AddEdge(1, 3, 5, 5);
			G.AddEdge(2, 3, 5, 5);
			G.AddEdge(3, MPM.SINK, 10, 10);
			Assert.IsTrue(G.IsFlowValid());
		}

		[TestMethod]
		public void TestWikipediaExample()
		{
			// Flow graph https://upload.wikimedia.org/wikipedia/commons/3/37/Dinic_algorithm_G1.svg

			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10);
			g.AddEdge(MPM.SOURCE, 2, 10);
			g.AddEdge(1, 2, 2);
			g.AddEdge(1, 3, 4);
			g.AddEdge(1, 4, 8);
			g.AddEdge(2, 4, 9);
			g.AddEdge(3, MPM.SINK, 10);
			g.AddEdge(4, 3, 6);
			g.AddEdge(4, MPM.SINK, 10);
			int answer = 19;
			Assert.AreEqual(answer, MPM.MaxFlow(g));
		}

		[TestMethod]
		public void TestLevelGraphWikipedia1()
		{
			// Residual graph https://upload.wikimedia.org/wikipedia/commons/f/fd/Dinic_algorithm_Gf1.svg
			// Level graph https://upload.wikimedia.org/wikipedia/commons/8/80/Dinic_algorithm_GL1.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10);
			g.AddEdge(MPM.SOURCE, 2, 10);
			g.AddEdge(1, 2, 2);
			g.AddEdge(1, 3, 4);
			g.AddEdge(1, 4, 8);
			g.AddEdge(2, 4, 9);
			g.AddEdge(3, MPM.SINK, 10);
			g.AddEdge(4, 3, 6);
			g.AddEdge(4, MPM.SINK, 10);

			MPM.Graph levelG = g.GetLevelGraph();
			Assert.AreEqual(2, levelG.OutDegree(MPM.SOURCE));
			Assert.AreEqual(0, levelG.InDegree(MPM.SOURCE));
			Assert.IsTrue(levelG.HasEdge(MPM.SOURCE,1, 10));
			Assert.IsTrue(levelG.HasEdge(MPM.SOURCE, 2, 10));

			Assert.AreEqual(2, levelG.OutDegree(1));
			Assert.AreEqual(1, levelG.InDegree(1));
			Assert.IsTrue(levelG.HasEdge(1, 3, 4));
			Assert.IsTrue(levelG.HasEdge(1, 4, 8));

			Assert.AreEqual(1, levelG.OutDegree(2));
			Assert.AreEqual(1, levelG.InDegree(2));
			Assert.IsTrue(levelG.HasEdge(2, 4, 9));

			Assert.AreEqual(1, levelG.OutDegree(3));
			Assert.AreEqual(1, levelG.InDegree(3));
			Assert.IsTrue(levelG.HasEdge(3, MPM.SINK, 10));

			Assert.AreEqual(1, levelG.OutDegree(4));
			Assert.AreEqual(2, levelG.InDegree(4));
			Assert.IsTrue(levelG.HasEdge(4, MPM.SINK, 10));

			Assert.AreEqual(0, levelG.OutDegree(MPM.SINK));
			Assert.AreEqual(2, levelG.InDegree(MPM.SINK));
		}

		[TestMethod]
		public void TestCapacity()
		{
			// Level graph https://upload.wikimedia.org/wikipedia/commons/9/97/Dinic_algorithm_GL2.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 10);
			g.AddEdge(MPM.SOURCE, 2, 10, 4);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 4);
			g.AddEdge(1, 4, 8, 6);
			g.AddEdge(2, 4, 9, 4);
			g.AddEdge(3, MPM.SINK, 10, 4);
			g.AddEdge(4, 3, 6, 0);
			g.AddEdge(4, MPM.SINK, 10, 10);

			MPM.Graph resG = g.GetResidualGraph();
			MPM.Graph levelG = resG.GetLevelGraph();

			Assert.AreEqual(0, levelG.Capacity(1));
			Assert.AreEqual(5, levelG.Capacity(2));
			Assert.AreEqual(6, levelG.Capacity(3));
			Assert.AreEqual(5, levelG.Capacity(4));
		}

		[TestMethod]
		public void TestCapacity2()
		{
			int[,] data = new int[,] {
				{ MPM.SOURCE, 2, 8 },
				{ MPM.SOURCE, 4, 7 },
				{ MPM.SOURCE, 6, 10 },
				{ MPM.SOURCE, 7, 12 },
				{ 2, 8, 4 },
				{ 2, 9, 3 },
				{ 2, 11, 8 },
				{ 3, 11, 2 },
				{ 3, 12, 2 },
				{ 4, 9, 2 },
				{ 4, 10, 3 },
				{ 4, 12, 5 },
				{ 6, 8, 4 },
				{ 6, 9, 2 },
				{ 7, 8, 3 },
				{ 7, 9, 6 },
				{ 7, 10, 4 },
				{ 8, MPM.SINK, 7 },
				{ 9, MPM.SINK, 6 },
				{ 9, 3, 4 },
				{ 10, MPM.SINK, 4 },
				{ 11, MPM.SINK, 9 },
				{ 12, MPM.SINK, 15 }
			};
			MPM.Graph flowGraph = DoubleArrayToGraph(data);
			MPM.Graph levelGraph = flowGraph.GetLevelGraph();
			Assert.AreEqual(4, levelGraph.Capacity(10));
		}


		[TestMethod]
		public void TestResidualGraphWikipedia2()
		{
			// Flow graph https://upload.wikimedia.org/wikipedia/commons/5/56/Dinic_algorithm_G2.svg
			// Residual graph https://upload.wikimedia.org/wikipedia/commons/4/43/Dinic_algorithm_Gf2.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 10);
			g.AddEdge(MPM.SOURCE, 2, 10, 4);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 4);
			g.AddEdge(1, 4, 8, 6);
			g.AddEdge(2, 4, 9, 4);
			g.AddEdge(3, MPM.SINK, 10, 4);
			g.AddEdge(4, 3, 6, 0);
			g.AddEdge(4, MPM.SINK, 10, 10);

			MPM.Graph resG = g.GetResidualGraph();
			Assert.AreEqual(1, resG.OutDegree(MPM.SOURCE));
			Assert.AreEqual(2, resG.InDegree(MPM.SOURCE));
			Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 2, 6));

			Assert.AreEqual(3, resG.OutDegree(1));
			Assert.AreEqual(2, resG.InDegree(1));
			Assert.IsTrue(resG.HasEdge(1, MPM.SOURCE, 10));
			Assert.IsTrue(resG.HasEdge(1, 2, 2));
			Assert.IsTrue(resG.HasEdge(1, 4, 2));

			Assert.AreEqual(2, resG.OutDegree(2));
			Assert.AreEqual(3, resG.InDegree(2));
			Assert.IsTrue(resG.HasEdge(2, MPM.SOURCE, 4));
			Assert.IsTrue(resG.HasEdge(2, 4, 5));

			Assert.AreEqual(2, resG.OutDegree(3));
			Assert.AreEqual(2, resG.InDegree(3));
			Assert.IsTrue(resG.HasEdge(3, MPM.SINK, 6));
			Assert.IsTrue(resG.HasEdge(3, 1, 4));

			Assert.AreEqual(3, resG.OutDegree(4));
			Assert.AreEqual(3, resG.InDegree(4));
			Assert.IsTrue(resG.HasEdge(4,2, 4));
			Assert.IsTrue(resG.HasEdge(4, 1, 6));
			Assert.IsTrue(resG.HasEdge(4, 3, 6));

			Assert.AreEqual(2, resG.OutDegree(MPM.SINK));
			Assert.AreEqual(1, resG.InDegree(MPM.SINK));
			Assert.IsTrue(resG.HasEdge(MPM.SINK, 3, 4));
			Assert.IsTrue(resG.HasEdge(MPM.SINK, 4, 10));
		}

		[TestMethod]
		public void TestLevelGraphWikipedia2()
		{
			// Residual graph https://upload.wikimedia.org/wikipedia/commons/4/43/Dinic_algorithm_Gf2.svg
			// Level graph https://upload.wikimedia.org/wikipedia/commons/9/97/Dinic_algorithm_GL2.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 10);
			g.AddEdge(MPM.SOURCE, 2, 10, 4);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 4);
			g.AddEdge(1, 4, 8, 6);
			g.AddEdge(2, 4, 9, 4);
			g.AddEdge(3, MPM.SINK, 10, 4);
			g.AddEdge(4, 3, 6, 0);
			g.AddEdge(4, MPM.SINK, 10, 10);

			MPM.Graph resG = g.GetResidualGraph();
			MPM.Graph levelG = resG.GetLevelGraph();

			Assert.AreEqual(1, levelG.OutDegree(MPM.SOURCE));
			Assert.AreEqual(0, levelG.InDegree(MPM.SOURCE));

			Assert.IsTrue(levelG.HasEdge(MPM.SOURCE, 2, 6));

			Assert.AreEqual(0, levelG.OutDegree(1));
			Assert.AreEqual(1, levelG.InDegree(1));


			Assert.AreEqual(1, levelG.OutDegree(2));
			Assert.AreEqual(1, levelG.InDegree(2));
			Assert.IsTrue(levelG.HasEdge(2, 4, 5));

			Assert.AreEqual(1, levelG.OutDegree(3));
			Assert.AreEqual(1, levelG.InDegree(3));
			Assert.IsTrue(levelG.HasEdge(3, MPM.SINK, 6));

			Assert.AreEqual(2, levelG.OutDegree(4));
			Assert.AreEqual(1, levelG.InDegree(4));
			Assert.IsTrue(levelG.HasEdge(4, 3, 6));
			Assert.IsTrue(levelG.HasEdge(4, 1, 6));

			Assert.AreEqual(0, levelG.OutDegree(MPM.SINK));
			Assert.AreEqual(1, levelG.InDegree(MPM.SINK));
		}

		[TestMethod]
		public void TestCountFlow()
		{
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 10);
			g.AddEdge(MPM.SOURCE, 2, 10, 9);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 4);
			g.AddEdge(1, 4, 8, 6);
			g.AddEdge(2, 4, 9, 9);
			g.AddEdge(3, MPM.SINK, 10, 9);
			g.AddEdge(4, 3, 6, 5);
			g.AddEdge(4, MPM.SINK, 10, 10);
			Assert.AreEqual(19, g.CountFlow());
		}

		[TestMethod]
		public void TestResidualGraphWikipedia3()
		{
			// Flow graph https://upload.wikimedia.org/wikipedia/commons/7/71/Dinic_algorithm_G3.svg
			// Residual graph https://upload.wikimedia.org/wikipedia/commons/2/20/Dinic_algorithm_Gf3.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 10);
			g.AddEdge(MPM.SOURCE, 2, 10, 9);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 4);
			g.AddEdge(1, 4, 8, 6);
			g.AddEdge(2, 4, 9, 9);
			g.AddEdge(3, MPM.SINK, 10, 9);
			g.AddEdge(4, 3, 6, 5);
			g.AddEdge(4, MPM.SINK, 10, 10);

			MPM.Graph resG = g.GetResidualGraph();
			Assert.AreEqual(1, resG.OutDegree(MPM.SOURCE));
			Assert.AreEqual(2, resG.InDegree(MPM.SOURCE));
			Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 2, 1));

			Assert.AreEqual(3, resG.OutDegree(1));
			Assert.AreEqual(2, resG.InDegree(1));
			Assert.IsTrue(resG.HasEdge(1, MPM.SOURCE, 10));
			Assert.IsTrue(resG.HasEdge(1, 2, 2));
			Assert.IsTrue(resG.HasEdge(1, 4, 2));

			Assert.AreEqual(1, resG.OutDegree(2));
			Assert.AreEqual(3, resG.InDegree(2));
			Assert.IsTrue(resG.HasEdge(2, MPM.SOURCE, 9));

			Assert.AreEqual(3, resG.OutDegree(3));
			Assert.AreEqual(2, resG.InDegree(3));
			Assert.IsTrue(resG.HasEdge(3, MPM.SINK, 1));
			Assert.IsTrue(resG.HasEdge(3, 1, 4));
			Assert.IsTrue(resG.HasEdge(3, 4, 5));

			Assert.AreEqual(3, resG.OutDegree(4));
			Assert.AreEqual(3, resG.InDegree(4));
			Assert.IsTrue(resG.HasEdge(4,2, 9));
			Assert.IsTrue(resG.HasEdge(4, 1, 6));
			Assert.IsTrue(resG.HasEdge(4, 3, 1));

			Assert.AreEqual(2, resG.OutDegree(MPM.SINK));
			Assert.AreEqual(1, resG.InDegree(MPM.SINK));
			Assert.IsTrue(resG.HasEdge(MPM.SINK, 3, 9));
			Assert.IsTrue(resG.HasEdge(MPM.SINK, 4, 10));
		}

		[TestMethod]
		public void TestLevelGraphWikipedia3()
		{
			// Residual graph https://upload.wikimedia.org/wikipedia/commons/2/20/Dinic_algorithm_Gf3.svg
			// Level graph https://upload.wikimedia.org/wikipedia/commons/9/95/Dinic_algorithm_GL3.svg
			MPM.Graph g = new MPM.Graph();
			g.AddEdge(MPM.SOURCE, 1, 10, 10);
			g.AddEdge(MPM.SOURCE, 2, 10, 9);
			g.AddEdge(1, 2, 2, 0);
			g.AddEdge(1, 3, 4, 4);
			g.AddEdge(1, 4, 8, 6);
			g.AddEdge(2, 4, 9, 9);
			g.AddEdge(3, MPM.SINK, 10, 9);
			g.AddEdge(4, 3, 6, 5);
			g.AddEdge(4, MPM.SINK, 10, 10);

			MPM.Graph resG = g.GetResidualGraph();
			MPM.Graph levelG = resG.GetLevelGraph();

			Assert.AreEqual(1, levelG.OutDegree(MPM.SOURCE));
			Assert.AreEqual(0, levelG.InDegree(MPM.SOURCE));
			Assert.IsTrue(levelG.HasEdge(MPM.SOURCE, 2, 1));

			Assert.AreEqual(0, levelG.OutDegree(2));
			Assert.AreEqual(1, levelG.InDegree(2));
		}

		[TestMethod]
		public void TestMultiGraphFlow()
		{
			MPM.Graph G = new MPM.Graph();
			G.AddEdge(MPM.SOURCE, 1, 5, augmentExisting: true);
			G.AddEdge(MPM.SOURCE, 1, 5, augmentExisting: true);
			G.AddEdge(1, MPM.SINK, 7, augmentExisting: true);
			Assert.AreEqual(7, MPM.MaxFlow(G));
		}

		[TestMethod]
		public void TestPrioritiesUpdated()
		{
			MPM.Graph G = new MPM.Graph();
			G.AddEdge(MPM.SOURCE, 1, 2);
			G.AddEdge(MPM.SOURCE, 2, 3);
			G.AddEdge(1, 3, 2);
			G.AddEdge(2, 3, 1);
			G.AddEdge(2, 4, 2);
			G.AddEdge(3, 5, 1);
			G.AddEdge(4, 5, 2);
			G.AddEdge(5, MPM.SINK, 3);

			FibonacciHeap<int, int> h = new FibonacciHeap<int, int>(0);
			var one = new FibonacciHeapNode<int, int>
				(1, G.Capacity(1));
			var two = new FibonacciHeapNode<int, int>
				(2, G.Capacity(2));
			var three = new FibonacciHeapNode<int, int>
				(3, G.Capacity(3));
			var four = new FibonacciHeapNode<int, int>
				(4, G.Capacity(4));
			var five = new FibonacciHeapNode<int, int>
				(5, G.Capacity(5));
			var pointersToHeap = new Dictionary<int, FibonacciHeapNode<int, int>>
			{
				{1, one },
				{2, two },
				{3, three },
				{4, four },
				{5, five }
			};
			foreach (FibonacciHeapNode<int,int> node in pointersToHeap.Values)
			{
				h.Insert(node);
			}
			MPM.SaturateNode(h, G, G, G.GetEmptyFlow(), pointersToHeap);
			Assert.AreEqual(2, five.Key);
			Assert.AreEqual(2, four.Key);
			Assert.AreEqual(2, two.Key);
			Assert.AreEqual(0, one.Key);
		}

		[TestMethod]
		public void GitHubExample()
		{
			int[,] data = new int[,] {
				{ MPM.SOURCE, 2, 16 },
				{ MPM.SOURCE, 3, 16 },
				{ 2, 14, 16 },
				{ 2, 13, 16 },
				{ 2, 12, 16 },
				{ 2, 11, 16 },
				{ 2, 10, 16 },
				{ 2, 9, 16 },
				{ 2, 8, 16 },
				{ 3, 5, 16 },
				{ 3, 6, 16 },
				{ 3, 8, 16 },
				{ 3, 14, 16 },
				{ 3, 13, 16 },
				{ 3, 12, 16 },
				{ 3, 4, 16 },
				{ 4, MPM.SINK, 3 },
				{ 5, MPM.SINK, 3 },
				{ 6, MPM.SINK, 3 },
				{ 7, MPM.SINK, 3 },
				{ 8, MPM.SINK, 3 },
				{ 9, MPM.SINK, 3 },
				{ 10, MPM.SINK, 3 },
				{ 11, MPM.SINK, 3 },
				{ 12, MPM.SINK, 3 },
				{ 13, MPM.SINK, 3 },
				{ 14, MPM.SINK, 3 }
			};
			MPM.Graph flowGraph = DoubleArrayToGraph(data);
			int answer = 30;
			Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
		}

		[TestMethod]
		public void Medium01WithLoop()
		{

			int[,] data = new int[,] {
				{ MPM.SOURCE, 2, 8 },
				{ MPM.SOURCE, 4, 7 },
				{ MPM.SOURCE, 6, 10 },
				{ MPM.SOURCE, 7, 12 },
				{ 2, 8, 4 },
				{ 2, 9, 3 },
				{ 2, 11, 8 },
				{ 3, 11, 2 },
				{ 3, 12, 2 },
				{ 4, 9, 2 },
				{ 4, 10, 3 },
				{ 4, 12, 5 },
				{ 12, 4, 5 },
				{ 6, 8, 4 },
				{ 6, 9, 2 },
				{ 7, 8, 3 },
				{ 7, 9, 6 },
				{ 7, 10, 4 },
				{ 8, MPM.SINK, 7 },
				{ 9, MPM.SINK, 6 },
				{ 9, 3, 4 },
				{ 10, MPM.SINK, 4 },
				{ 11, MPM.SINK, 9 },
				{ 12, MPM.SINK, 15 }
			};
			MPM.Graph flowGraph = DoubleArrayToGraph(data);
			int answer = 33;
			Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
		}

		[TestMethod]
		public void Medium01()
		{
			
			int[,] data = new int[,] {
				{ MPM.SOURCE, 2, 8 },
				{ MPM.SOURCE, 4, 7 },
				{ MPM.SOURCE, 6, 10 },
				{ MPM.SOURCE, 7, 12 },
				{ 2, 8, 4 },
				{ 2, 9, 3 },
				{ 2, 11, 8 },
				{ 3, 11, 2 },
				{ 3, 12, 2 },
				{ 4, 9, 2 },
				{ 4, 10, 3 },
				{ 4, 12, 5 },
				{ 6, 8, 4 },
				{ 6, 9, 2 },
				{ 7, 8, 3 },
				{ 7, 9, 6 },
				{ 7, 10, 4 },
				{ 8, MPM.SINK, 7 },
				{ 9, MPM.SINK, 6 },
				{ 9, 3, 4 },
				{ 10, MPM.SINK, 4 },
				{ 11, MPM.SINK, 9 },
				{ 12, MPM.SINK, 15 }
			};
			MPM.Graph flowGraph = DoubleArrayToGraph(data);
			int answer = 33;
			Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
		}

		public MPM.Graph DoubleArrayToGraph(int[,] data)
		{
			var flowGraph = new MPM.Graph();
			for (int i = 0; i < data.GetLength(0); i++)
			{
				flowGraph.AddEdge(data[i, 0], data[i, 1], data[i, 2]);
			}
			return flowGraph;
		}

		[TestMethod]
		public void Small01()
		{
			int[,] data = new int[,] {
				{ MPM.SOURCE, 2, 256 },
				{ MPM.SOURCE, 3, 256 },
				{ 2, 6, 256 },
				{ 6, MPM.SINK, 256 },
				{ 3, 7, 128 },
				{ 7, MPM.SINK, 256 },
				{ 2, 5, 128 },
				{ 5, 7, 128 },
				{ 3, 4, 128 },
				{ 4, 6, 128 }
			};
			MPM.Graph flowGraph = DoubleArrayToGraph(data);
			int answer = 512;
			Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
		}

		[TestMethod]
		public void ResidualGraphWithLoop()
		{
			MPM.Graph G = new MPM.Graph();
			G.AddEdge(0, 1, 2, 2);
			G.AddEdge(1, 2, 3, 2);
			G.AddEdge(2, 1, 1, 0);
			G.AddEdge(2, 3, 2, 2);

			MPM.Graph resG = G.GetResidualGraph();
			Assert.IsTrue(resG.HasEdge(2, 1, 3));
		}

		[TestMethod]
		public void Small05WithLoop()
		{
			int[,] data = new int[,] {
				{ MPM.SOURCE, 1, 1 },
				{ MPM.SOURCE, 2, 2 },
				{ 1, 3, 1 },
				{ 2, 3, 2 },
				{ 2, 4, 1 },
				{ 3, MPM.SINK, 2 },
				{ 4, MPM.SINK, 1 },
				{4, 5, 1 },
				{5,6,1 },
				{6,2,1 }
			};
			MPM.Graph G = DoubleArrayToGraph(data);
			int answer = 3;
			Assert.AreEqual(answer, MPM.MaxFlow(G));
		}

		[TestMethod]
		public void Small05()
		{
			int[,] data = new int[,] {
				{ MPM.SOURCE, 1, 1 },
				{ MPM.SOURCE, 2, 2 },
				{ 1, 3, 1 },
				{ 2, 3, 2 },
				{ 2, 4, 1 },
				{ 3, MPM.SINK, 2 },
				{ 4, MPM.SINK, 1 }
			};
			MPM.Graph G = DoubleArrayToGraph(data);			
			int answer = 3;
			Assert.AreEqual(answer, MPM.MaxFlow(G));
		}
	}
}
