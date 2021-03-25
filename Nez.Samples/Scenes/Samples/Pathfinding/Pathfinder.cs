using Nez.AI.Pathfinding;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.Tiled;


namespace Nez.Samples
{
	/// <summary>
	/// simple Component that finds a path on click and displays it via a series of rectangles
	/// </summary>
	public class Pathfinder : RenderableComponent, IUpdatable
	{
		// make sure we arent culled
		public override float Width => 1000;

		public override float Height => 1000;

		UnweightedGridGraph _gridGraph;
		List<Point> _breadthSearchPath;

		WeightedGridGraph _weightedGraph;
		List<Point> _weightedSearchPath;

		AstarGridGraph _astarGraph;
		List<Point> _astarSearchPath;

		TmxMap _tilemap;
		Point _start, _end;


		public Pathfinder(TmxMap tilemap)
		{
			_tilemap = tilemap;
			var layer = tilemap.GetLayer<TmxLayer>("main");

			_start = new Point(1, 1);
			_end = new Point(10, 10);

			_gridGraph = new UnweightedGridGraph(layer);
			_breadthSearchPath = _gridGraph.Search(_start, _end);

			_weightedGraph = new WeightedGridGraph(layer);
			_weightedSearchPath = _weightedGraph.Search(_start, _end);

			_astarGraph = new AstarGridGraph(layer);
			_astarSearchPath = _astarGraph.Search(_start, _end);

			Debug.DrawTextFromBottom = true;
		}

		void IUpdatable.Update()
		{
			// on left click set our path end time
			if (Input.LeftMouseButtonPressed)
				_end = _tilemap.WorldToTilePosition(Input.MousePosition);

			// on right click set our path start time
			if (Input.RightMouseButtonPressed)
				_start = _tilemap.WorldToTilePosition(Input.MousePosition);

			// regenerate the path on either click
			if (Input.LeftMouseButtonPressed || Input.RightMouseButtonPressed)
			{
				// time both path generations
				var first = Debug.TimeAction(() => { _breadthSearchPath = _gridGraph.Search(_start, _end); });

				var second = Debug.TimeAction(() => { _weightedSearchPath = _weightedGraph.Search(_start, _end); });

				var third = Debug.TimeAction(() => { _astarSearchPath = _astarGraph.Search(_start, _end); });

				// debug draw the times
				Debug.DrawText("Breadth First: {0}\nDijkstra: {1}\nAstar: {2}", first, second, third);
				Debug.Log("\nBreadth First: {0}\nDijkstra: {1}\nAstar: {2}", first, second, third);
			}
		}

		public override void Render(Batcher batcher, Camera camera)
		{
			// if we have a path render all the nodes
			if (_breadthSearchPath != null)
			{
				foreach (var node in _breadthSearchPath)
				{
					var x = node.X * _tilemap.TileWidth + _tilemap.TileWidth * 0.5f;
					var y = node.Y * _tilemap.TileHeight + _tilemap.TileHeight * 0.5f;

					batcher.DrawPixel(x + 2, y + 2, Color.Yellow, 4);
				}
			}

			if (_weightedSearchPath != null)
			{
				foreach (var node in _weightedSearchPath)
				{
					var x = node.X * _tilemap.TileWidth + _tilemap.TileWidth * 0.5f;
					var y = node.Y * _tilemap.TileHeight + _tilemap.TileHeight * 0.5f;

					batcher.DrawPixel(x - 2, y - 2, Color.Blue, 4);
				}
			}

			if (_astarSearchPath != null)
			{
				foreach (var node in _astarSearchPath)
				{
					var x = node.X * _tilemap.TileWidth + _tilemap.TileWidth * 0.5f;
					var y = node.Y * _tilemap.TileHeight + _tilemap.TileHeight * 0.5f;

					batcher.DrawPixel(x, y, Color.Orange, 4);
				}
			}
		}
	}
}