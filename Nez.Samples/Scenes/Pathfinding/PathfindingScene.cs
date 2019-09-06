using Nez.Tiled;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	[SampleScene( "Pathfinding Tilemap", 100, "Right click to set the start point\nLeft click to set the end point\nBreadth First is the yellow path and Astar is the blue path\nTurn on Debug Render to see path generation times" )]
	public class PathfindingScene : SampleScene
	{
		public PathfindingScene() : base( true, true )
		{ }


		public override void Initialize()
		{
			ClearColor = Color.Black;
			SetDesignResolution( 640, 368, SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.SetSize( 1280, 736 );

			// load a TiledMap and a TiledMapComponent to display it
			var tiledEntity = CreateEntity( "tiled-map" );
			var tiledmap = Content.Load<TiledMap>( Nez.Content.DestructableMap.destructablemap );
			tiledEntity.AddComponent( new TiledMapComponent( tiledmap ) );

			// add a Pathfinder to handle pathfinding and debug display of the paths
			CreateEntity( "pathfinder" )
				.AddComponent( new Pathfinder( tiledmap ) );
		}
	}
}

