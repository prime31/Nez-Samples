using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Farseer;
using Nez.Svg;


namespace Nez.Samples
{
	[SampleScene("SVG to Farseer Physics", 85,
		"This demo shows how you can turn an SVG image into Farseer physics objects\nPress Space to start the physics simulation\nClick and drag to interact with physics shapes")]
	public class FarseerSvgScene : SampleScene
	{
		public override void Initialize()
		{
			ClearColor = Color.Black;
			AddRenderer(new DefaultRenderer());
			Screen.SetSize(1280, 720);

			Settings.MaxPolygonVertices = 12;

			// create a physics world to manage the physics simulation
			var world = GetOrCreateSceneComponent<FSWorld>()
				.SetEnableMousePicking(true);
			world.SetEnabled(false);

			// add a FSDebugView so that we can see the physics objects
			CreateEntity("debug-view")
				.AddComponent(new PressKeyToPerformAction(Keys.Space, e => world.SetEnabled(true)))
				.AddComponent(new FSDebugView(world))
				.AppendFlags(FSDebugView.DebugViewFlags.ContactPoints);

			// load up the SVG document. This particular SVG only has one group so we can fetch it straight away
			var svgDoc = SvgDocument.Open(TitleContainer.OpenStream("Content/SVG/farseer-svg.svg"));
			var svgGroup = svgDoc.Groups[0];

			// rectangles
			if (svgGroup.Rectangles != null)
				AddRectangles(svgGroup);

			// circles
			if (svgGroup.Circles != null)
				AddCircles(svgGroup);

			// lines
			if (svgGroup.Lines != null)
				AddLines(svgGroup);

			// paths: TODO: why is System.Drawing.Drawing2D.GraphicsPath never returning on macOS?!?!
			//if( svgGroup.paths != null )
			//	addPaths( svgGroup );

			// ellipses
			if (svgGroup.Ellipses != null)
				AddEllipses(svgGroup);

			// polygons
			if (svgGroup.Polygons != null)
				AddPolygons(svgGroup);
		}


		/// <summary>
		/// adds dynamic rectangle physics objects for each rect in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void AddRectangles(SvgGroup group)
		{
			foreach (var rect in group.Rectangles)
			{
				var boxEntity = CreateEntity(rect.Id);
				boxEntity.SetPosition(rect.Center)
					.SetRotationDegrees(rect.RotationDegrees)
					.AddComponent(new FSRigidBody())
					.SetBodyType(BodyType.Dynamic)
					.AddComponent(new FSCollisionBox(rect.Width, rect.Height));
			}
		}


		/// <summary>
		/// adds dynamic circle physics objects for each circle in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void AddCircles(SvgGroup group)
		{
			foreach (var circle in group.Circles)
			{
				CreateEntity(circle.Id)
					.SetPosition(circle.CenterX, circle.CenterY)
					.AddComponent(new FSRigidBody())
					.SetBodyType(BodyType.Dynamic)
					.AddComponent(new FSCollisionCircle(circle.Radius));
			}
		}


		/// <summary>
		/// adds static edge physics objects for each line in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void AddLines(SvgGroup group)
		{
			foreach (var line in group.Lines)
			{
				var pts = line.GetTransformedPoints();

				CreateEntity(line.Id)
					.AddComponent<FSRigidBody>()
					.AddComponent(new FSCollisionEdge())
					.SetVertices(pts[0], pts[1]);
			}
		}


		/// <summary>
		/// adds static chain physics objects for each path in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void AddPaths(SvgGroup group)
		{
			var svgPathBuilder = new SvgPathBuilder();

			foreach (var path in group.Paths)
			{
				var pts = path.GetTransformedDrawingPoints(svgPathBuilder);

				CreateEntity(path.Id)
					.AddComponent<FSRigidBody>()
					.AddComponent(new FSCollisionChain())
					.SetVertices(pts);
			}
		}


		/// <summary>
		/// adds dynamic ellipse physics objects for each ellipse in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void AddEllipses(SvgGroup group)
		{
			foreach (var ellipse in group.Ellipses)
			{
				CreateEntity(ellipse.Id)
					.SetPosition(ellipse.CenterX, ellipse.CenterY)
					.AddComponent<FSRigidBody>()
					.SetBodyType(BodyType.Dynamic)
					.AddComponent(new FSCollisionEllipse(ellipse.RadiusX, ellipse.RadiusY));
			}
		}


		/// <summary>
		/// adds dynamic polygon physics objects for each polygon in the SVG
		/// </summary>
		/// <param name="group">Group.</param>
		void AddPolygons(SvgGroup group)
		{
			foreach (var polygon in group.Polygons)
			{
				CreateEntity(polygon.Id)
					.SetPosition(polygon.CenterX, polygon.CenterY)
					.AddComponent<FSRigidBody>()
					.SetBodyType(BodyType.Dynamic)
					.AddComponent(new FSCollisionPolygon(polygon.GetRelativePoints()));
			}
		}
	}
}