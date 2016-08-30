using System;
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
		public override float width { get { return 1000; } }
		public override float height { get { return 1000; } }

		UnweightedGridGraph _gridGraph;
		List<Point> _breadthSearchPath;

		WeightedGridGraph _weightedGraph;
		List<Point> _weightedSearchPath;

		AstarGridGraph _astarGraph;
		List<Point> _astarSearchPath;

		TiledMap _tilemap;
		Point _start, _end;


		public Pathfinder( TiledMap tilemap )
		{
			_tilemap = tilemap;
			var layer = tilemap.getLayer<TiledTileLayer>( "main" );

			_start = new Point( 1, 1 );
			_end = new Point( 10, 10 );

			_gridGraph = new UnweightedGridGraph( layer );
			_breadthSearchPath = _gridGraph.search( _start, _end );

			_weightedGraph = new WeightedGridGraph( layer );
			_weightedSearchPath = _weightedGraph.search( _start, _end );

			_astarGraph = new AstarGridGraph( layer );
			_astarSearchPath = _astarGraph.search( _start, _end );

			Debug.drawTextFromBottom = true;
		}


		void IUpdatable.update()
		{
			// on left click set our path end time
			if( Input.leftMouseButtonPressed )
				_end = _tilemap.worldToTilePosition( Input.mousePosition );

			// on right click set our path start time
			if( Input.rightMouseButtonPressed )
				_start = _tilemap.worldToTilePosition( Input.mousePosition );

			// regenerate the path on either click
			if( Input.leftMouseButtonPressed || Input.rightMouseButtonPressed )
			{
				// time both path generations
				var first = Debug.timeAction( () =>
				{
					_breadthSearchPath = _gridGraph.search( _start, _end );
				} );

				var second = Debug.timeAction( () =>
				{
					_weightedSearchPath = _weightedGraph.search( _start, _end );
				} );

				var third = Debug.timeAction( () =>
				{
					_astarSearchPath = _astarGraph.search( _start, _end );
				} );

				// debug draw the times
				Debug.drawText( "Breadth First: {0}\nDijkstra: {1}\nAstar: {2}", first, second, third );
				Debug.log( "\nBreadth First: {0}\nDijkstra: {1}\nAstar: {2}", first, second, third );
			}
		}


		public override void render( Graphics graphics, Camera camera )
		{
			// if we have a path render all the nodes
			if( _breadthSearchPath != null )
			{
				foreach( var node in _breadthSearchPath )
				{
					var x = node.X * _tilemap.tileWidth + _tilemap.tileWidth * 0.5f;
					var y = node.Y * _tilemap.tileHeight + _tilemap.tileHeight * 0.5f;

					graphics.batcher.drawPixel( x + 2, y + 2, Color.Yellow, 4 );
				}
			}

			if( _weightedSearchPath != null )
			{
				foreach( var node in _weightedSearchPath )
				{
					var x = node.X * _tilemap.tileWidth + _tilemap.tileWidth * 0.5f;
					var y = node.Y * _tilemap.tileHeight + _tilemap.tileHeight * 0.5f;

					graphics.batcher.drawPixel( x - 2, y - 2, Color.Blue, 4 );
				}
			}

			if( _astarSearchPath != null )
			{
				foreach( var node in _astarSearchPath )
				{
					var x = node.X * _tilemap.tileWidth + _tilemap.tileWidth * 0.5f;
					var y = node.Y * _tilemap.tileHeight + _tilemap.tileHeight * 0.5f;

					graphics.batcher.drawPixel( x, y, Color.Orange, 4 );
				}
			}
		}

	}
}

