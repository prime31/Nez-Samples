using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Nez.Samples
{
	/// <summary>
	/// applies forces to the SpringGrid as the Entity moves around. Also adds an explosive force when space is pressed
	/// </summary>
	public class GridModifier : Component, IUpdatable
	{
		SpringGrid _grid;
		Vector2 _lastPosition;


		public override void onAddedToEntity()
		{
			_grid = entity.scene.findEntity( "grid" ).getComponent<SpringGrid>();
		}


		void IUpdatable.update()
		{
			var velocity = entity.position - _lastPosition;
			_grid.applyExplosiveForce( 0.5f * velocity.Length(), entity.position, 80 );

			_lastPosition = entity.position;

			if( Input.isKeyPressed( Keys.Space ) )
				_grid.applyDirectedForce( new Vector3( 0, 0, 1000 ), new Vector3( entity.position.X, entity.position.Y, 0 ), 50 );
		}
	}
}

