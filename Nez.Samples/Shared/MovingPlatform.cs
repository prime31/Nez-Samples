using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Nez.Samples
{
	public class MovingPlatform : Component, IUpdatable
	{
		float _minX;
		float _maxX;
		float _minY;
		float _maxY;
		float _speedFactor;


		public MovingPlatform( float minY, float maxY, float speedFactor = 2f )
		{
			_minY = minY;
			_maxY = maxY;
			_speedFactor = speedFactor;
		}


		public override void onAddedToEntity()
		{
			_minX = entity.transform.position.X;
			_maxX = _minX + 100;
		}


		void IUpdatable.update()
		{
			var x = Mathf.pingPong( Time.time, 1f );
			var xToTheSpeedFactor = Mathf.pow( x, _speedFactor );
			var alpha = 1f - xToTheSpeedFactor / xToTheSpeedFactor + Mathf.pow( 1 - x, _speedFactor );

			var deltaY = Nez.Tweens.Lerps.lerp( _minY, _maxY, alpha ) - entity.transform.position.Y;
			var deltaX = Nez.Tweens.Lerps.lerp( _minX, _maxX, alpha ) - entity.transform.position.X;

			// TODO: probably query Physics to fetch the actors that we will intersect instead of blindly grabbing them all
			var ridingActors = getAllRidingActors();

			moveSolid( new Vector2( deltaX, deltaY ), ridingActors );
		}


		void moveSolid( Vector2 motion, List<Entity> ridingActors )
		{
			if( motion.X == 0 && motion.Y == 0 )
				return;

			moveSolidX( motion.X, ridingActors );
			moveSolidY( motion.Y, ridingActors );
		}


		void moveSolidX( float amount, List<Entity> ridingActors )
		{
			var moved = false;
			entity.transform.position += new Vector2( amount, 0 );

			var platformCollider = entity.getComponent<Collider>();
			var colliders = new HashSet<Collider>( Physics.boxcastBroadphaseExcludingSelf( platformCollider ) );
			foreach( var collider in colliders )
			{
				float pushAmount;
				if( amount > 0 )
					pushAmount = platformCollider.bounds.right - collider.bounds.left;
				else
					pushAmount = platformCollider.bounds.left - collider.bounds.right;

				var mover = collider.entity.getComponent<Mover>();
				if( mover != null )
				{
					moved = true;
					CollisionResult collisionResult;
					if( mover.move( new Vector2( pushAmount, 0 ), out collisionResult ) )
					{
						collider.entity.destroy();
						return;
					}
				}
				else
				{
					collider.entity.position += new Vector2( pushAmount, 0 );
				}
			}


			foreach( var ent in ridingActors )
			{
				if( !moved )
					ent.position += new Vector2( amount, 0 );
			}
		}


		void moveSolidY( float amount, List<Entity> ridingActors )
		{
			var moved = false;
			entity.transform.position += new Vector2( 0, amount );

			var platformCollider = entity.getComponent<Collider>();
			var colliders = new HashSet<Collider>( Physics.boxcastBroadphaseExcludingSelf( platformCollider ) );
			foreach( var collider in colliders )
			{
				float pushAmount;
				if( amount > 0 )
					pushAmount = platformCollider.bounds.bottom - collider.bounds.top;
				else
					pushAmount = platformCollider.bounds.top - collider.bounds.bottom;

				var mover = collider.entity.getComponent<Mover>();
				if( mover != null )
				{
					moved = true;
					CollisionResult collisionResult;
					if( mover.move( new Vector2( 0, pushAmount ), out collisionResult ) )
					{
						collider.entity.destroy();
						return;
					}
				}
				else
				{
					collider.entity.position += new Vector2( 0, pushAmount );
				}
			}


			foreach( var ent in ridingActors )
			{
				if( !moved )
					ent.position += new Vector2( 0, amount );
			}
		}


		/// <summary>
		/// brute force search for Entities on top of this Collider. Not a great approach.
		/// </summary>
		/// <returns>The all riding actors.</returns>
		List<Entity> getAllRidingActors()
		{
			var list = new List<Entity>();
			var platformCollider = entity.getComponent<Collider>();

			var entities = entity.scene.findEntitiesWithTag( 0 );
			for( var i = 0; i < entities.Count; i++ )
			{
				var collider = entities[i].getComponent<Collider>();
				if( collider == platformCollider || collider == null )
					continue;

				CollisionResult collisionResult;
				if( collider.collidesWith( platformCollider, new Vector2( 0f, 1f ), out collisionResult ) )
					list.Add( entities[i] );
			}

			return list;
		}

	}
}

