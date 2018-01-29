using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Farseer;
using Nez.Svg;


namespace Nez.Samples
{
	[SampleScene( "SVG to Farseer Physics", 85, "This demo shows how you can use turn an SVG image into Farseer physics objects\nPress Space to start the physics simulation\nClick and drag to interact with physics shapes" )]
	public class FarseerSVGScene : SampleScene
	{
		public override void initialize()
		{
			clearColor = Color.Black;
			addRenderer( new DefaultRenderer() );
			Screen.setSize( 1280, 720 );

			Settings.maxPolygonVertices = 12;

			// create a physics world to manage the physics simulation
			var world = getOrCreateSceneComponent<FSWorld>()
				.setEnableMousePicking( true );
			world.setEnabled( false );

			// add a FSDebugView so that we can see the physics objects
			createEntity( "debug-view" )
				.addComponent( new PressKeyToPerformAction( Keys.Space, () => world.setEnabled( true ) ) )
				.addComponent( new FSDebugView( world ) )
				.appendFlags( FSDebugView.DebugViewFlags.ContactPoints );

			// load up the SVG document. This particular SVG only has one group so we can fetch it straight away
			var svgDoc = SvgDocument.open( TitleContainer.OpenStream( "Content/SVG/farseer-svg.svg" ) );
			var svgGroup = svgDoc.groups[0];

			// rectangles
			if( svgGroup.rectangles != null )
				addRectangles( svgGroup );

			// circles
			if( svgGroup.circles != null )
				addCircles( svgGroup );

			// lines
			if( svgGroup.lines != null )
				addLines( svgGroup );

			// paths: TODO: why is System.Drawing.Drawing2D.GraphicsPath never returning on macOS?!?!
			//if( svgGroup.paths != null )
			//	addPaths( svgGroup );

			// ellipses
			if( svgGroup.ellipses != null )
				addEllipses( svgGroup );

			// polygons
			if( svgGroup.polygons != null )
				addPolygons( svgGroup );
		}


		/// <summary>
		/// adds dynamic rectangle physics objects for each rect in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void addRectangles( SvgGroup group )
		{
			foreach( var rect in group.rectangles )
			{
				var boxEntity = createEntity( rect.id );
				boxEntity.setPosition( rect.center )
						 .setRotationDegrees( rect.rotationDegrees )
						 .addComponent( new FSRigidBody() )
						 .setBodyType( BodyType.Dynamic )
						 .addComponent( new FSCollisionBox( rect.width, rect.height ) );
			}
		}


		/// <summary>
		/// adds dynamic circle physics objects for each circle in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void addCircles( SvgGroup group )
		{
			foreach( var circle in group.circles )
			{
				createEntity( circle.id )
					.setPosition( circle.centerX, circle.centerY )
					.addComponent( new FSRigidBody() )
					.setBodyType( BodyType.Dynamic )
					.addComponent( new FSCollisionCircle( circle.radius ) );
			}
		}


		/// <summary>
		/// adds static edge physics objects for each line in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void addLines( SvgGroup group )
		{
			foreach( var line in group.lines )
			{
				var pts = line.getTransformedPoints();

				createEntity( line.id )
					.addComponent<FSRigidBody>()
					.addComponent( new FSCollisionEdge() )
					.setVertices( pts[0], pts[1] );
			}
		}


		/// <summary>
		/// adds static chain physics objects for each path in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void addPaths( SvgGroup group )
		{
			var svgPathBuilder = new SvgPathBuilder();

			foreach( var path in group.paths )
			{
				var pts = path.getTransformedDrawingPoints( svgPathBuilder );

				createEntity( path.id )
					.addComponent<FSRigidBody>()
					.addComponent( new FSCollisionChain() )
					.setVertices( pts );
			}
		}


		/// <summary>
		/// adds dynamic ellipse physics objects for each ellipse in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void addEllipses( SvgGroup group )
		{
			foreach( var ellipse in group.ellipses )
			{
				createEntity( ellipse.id )
					.setPosition( ellipse.centerX, ellipse.centerY )
					.addComponent<FSRigidBody>()
					.setBodyType( BodyType.Dynamic )
					.addComponent( new FSCollisionEllipse( ellipse.radiusX, ellipse.radiusY ) );
			}
		}


		/// <summary>
		/// adds dynamic polygon physics objects for each polygon in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void addPolygons( SvgGroup group )
		{
			foreach( var polygon in group.polygons )
			{
				createEntity( polygon.id )
					.setPosition( polygon.centerX, polygon.centerY )
					.addComponent<FSRigidBody>()
					.setBodyType( BodyType.Dynamic )
					.addComponent( new FSCollisionPolygon( polygon.getRelativePoints() ) );
			}
		}

	}
}
