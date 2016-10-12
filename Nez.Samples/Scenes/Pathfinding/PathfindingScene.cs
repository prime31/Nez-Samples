using Nez.Tiled;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	[SampleScene( "Pathfinding Tilemap", 100, "Right click to set the start point\nLeft click to set the end point\nBreadth First is the yellow path and Astar is the blue path\nTurn on Debug Render to see path generation times" )]
	public class PathfindingScene : SampleScene
	{
		public PathfindingScene() : base( true, true )
		{ }


		public override void initialize()
		{
			clearColor = Color.Black;
			setDesignResolution( 640, 368, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.setSize( 1280, 736 );

			// load a TiledMap and a TiledMapComponent to display it
			var tiledEntity = createEntity( "tiled-map" );
			var tiledmap = content.Load<TiledMap>( Content.DestructableMap.destructablemap );
			tiledEntity.addComponent( new TiledMapComponent( tiledmap ) );

			// add a Pathfinder to handle pathfinding and debug display of the paths
			createEntity( "pathfinder" )
				.addComponent( new Pathfinder( tiledmap ) );
		}
	}
}

