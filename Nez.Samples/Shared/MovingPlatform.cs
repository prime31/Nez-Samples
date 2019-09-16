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


		public MovingPlatform(float minY, float maxY, float speedFactor = 2f)
		{
			_minY = minY;
			_maxY = maxY;
			_speedFactor = speedFactor;
		}


		public override void OnAddedToEntity()
		{
			_minX = Entity.Position.X;
			_maxX = _minX + 100;
		}


		void IUpdatable.Update()
		{
			var x = Mathf.PingPong(Time.TotalTime, 1f);
			var xToTheSpeedFactor = Mathf.Pow(x, _speedFactor);
			var alpha = 1f - xToTheSpeedFactor / xToTheSpeedFactor + Mathf.Pow(1 - x, _speedFactor);

			var deltaY = Tweens.Lerps.Lerp(_minY, _maxY, alpha) - Entity.Position.Y;
			var deltaX = Tweens.Lerps.Lerp(_minX, _maxX, alpha) - Entity.Position.X;

			// TODO: probably query Physics to fetch the actors that we will intersect instead of blindly grabbing them all
			var ridingActors = GetAllRidingActors();

			MoveSolid(new Vector2(deltaX, deltaY), ridingActors);
		}


		void MoveSolid(Vector2 motion, List<Entity> ridingActors)
		{
			if (motion.X == 0 && motion.Y == 0)
				return;

			MoveSolidX(motion.X, ridingActors);
			MoveSolidY(motion.Y, ridingActors);
		}


		void MoveSolidX(float amount, List<Entity> ridingActors)
		{
			var moved = false;
			Entity.Position += new Vector2(amount, 0);

			var platformCollider = Entity.GetComponent<Collider>();
			var colliders = new HashSet<Collider>(Physics.BoxcastBroadphaseExcludingSelf(platformCollider));
			foreach (var collider in colliders)
			{
				float pushAmount;
				if (amount > 0)
					pushAmount = platformCollider.Bounds.Right - collider.Bounds.Left;
				else
					pushAmount = platformCollider.Bounds.Left - collider.Bounds.Right;

				var mover = collider.Entity.GetComponent<Mover>();
				if (mover != null)
				{
					moved = true;
					CollisionResult collisionResult;
					if (mover.Move(new Vector2(pushAmount, 0), out collisionResult))
					{
						collider.Entity.Destroy();
						return;
					}
				}
				else
				{
					collider.Entity.Position += new Vector2(pushAmount, 0);
				}
			}


			foreach (var ent in ridingActors)
			{
				if (!moved)
					ent.Position += new Vector2(amount, 0);
			}
		}


		void MoveSolidY(float amount, List<Entity> ridingActors)
		{
			var moved = false;
			Entity.Position += new Vector2(0, amount);

			var platformCollider = Entity.GetComponent<Collider>();
			var colliders = new HashSet<Collider>(Physics.BoxcastBroadphaseExcludingSelf(platformCollider));
			foreach (var collider in colliders)
			{
				float pushAmount;
				if (amount > 0)
					pushAmount = platformCollider.Bounds.Bottom - collider.Bounds.Top;
				else
					pushAmount = platformCollider.Bounds.Top - collider.Bounds.Bottom;

				var mover = collider.Entity.GetComponent<Mover>();
				if (mover != null)
				{
					moved = true;
					CollisionResult collisionResult;
					if (mover.Move(new Vector2(0, pushAmount), out collisionResult))
					{
						collider.Entity.Destroy();
						return;
					}
				}
				else
				{
					collider.Entity.Position += new Vector2(0, pushAmount);
				}
			}


			foreach (var ent in ridingActors)
			{
				if (!moved)
					ent.Position += new Vector2(0, amount);
			}
		}


		/// <summary>
		/// brute force search for Entities on top of this Collider. Not a great approach.
		/// </summary>
		/// <returns>The all riding actors.</returns>
		List<Entity> GetAllRidingActors()
		{
			var list = new List<Entity>();
			var platformCollider = Entity.GetComponent<Collider>();

			var entities = Entity.Scene.FindEntitiesWithTag(0);
			for (var i = 0; i < entities.Count; i++)
			{
				var collider = entities[i].GetComponent<Collider>();
				if (collider == platformCollider || collider == null)
					continue;

                if (collider.CollidesWith(platformCollider, new Vector2(0f, 1f), out CollisionResult collisionResult))
                    list.Add(entities[i]);
            }

			return list;
		}
	}
}