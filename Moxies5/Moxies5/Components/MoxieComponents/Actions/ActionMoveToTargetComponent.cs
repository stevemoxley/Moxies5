using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Moxies5.Entities;
using Moxies5.Utilities;
using Moxies5.Serialization;
using Moxies5.Components.ItemComponents;
using Moxies5.Controllers;

namespace Moxies5.Components.MoxieComponents.Actions
{
    public class ActionMoveToTargetComponent: AbstractActionComponent
    {
        
        #region Fields
        SpatialComponent _targetLocationComponent;
        AbstractActionComponent _actionToMakeAtTarget; //This is the action that will be performed once you reach the target
        private Pathfinding pathFinder;
        private int currentNodeInPath = 0;
        private Vector2 currentNodeVector; //This is the vector of the node you are moving to
        private Timer stuckTimer = new Timer(3);
        private Timer checkDistanceTimer = new Timer(6);
        private int nodeSize = 36; //This must be an even number
        private bool insideTargetSensor = false;
        private int _maxDistanceBeforeFindingNewPath = 60;
        #endregion

        #region Properties
        public Vector2 CurrentNodeVector
        {
            get
            {
                return currentNodeVector;
            }
        }

        public List<Tile> Path
        {
            get
            {
                return pathFinder.Path;
            }
        }
        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionMoveToTargetComponent(Entity parentEntity, SpatialComponent targetLocationComponent, AbstractActionComponent actionToMakeAtTarget)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionMoveToTargetComponent";

            if (Moxie.HasComponent(typeof(MovementComponent)))
            {
                this._targetLocationComponent = targetLocationComponent;
                this._actionToMakeAtTarget = actionToMakeAtTarget;
                pathFinder = new Pathfinding(MainController.Game, null);
                pathFinder.useDiagonal = true;
                pathFinder.allowDiagonalMovement = false;
            }

            //Add the sensor collision
            if (_targetLocationComponent.Parent.HasComponent(typeof(SensorComponent)))
            {
                SensorComponent sensor = (SensorComponent)_targetLocationComponent.Parent.GetComponent(typeof(SensorComponent));
                PhysicsComponent moxiePhysicsComponent = (PhysicsComponent)Moxie.GetComponent(typeof(PhysicsComponent));
                sensor.Body.OnCollision += new FarseerPhysics.Dynamics.OnCollisionEventHandler(Body_OnCollision);
            }
            else
            {
                throw new Exception("Target parent must have a sensor component");
            }
        }

        bool Body_OnCollision(FarseerPhysics.Dynamics.Fixture fixtureA, FarseerPhysics.Dynamics.Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            //fixture B should be the moxie
            if (fixtureB.Body.UserData == Moxie)
            {
                insideTargetSensor = true;
                return true;
            }
            else
                return false;
        }

        public override void Update(GameTime gameTime)
        {
            SpatialComponent moxieSC = (SpatialComponent)Moxie.GetComponent(typeof(SpatialComponent));
            Vector2 moxieLocation = moxieSC.Position;

            #region Timer Updating
            checkDistanceTimer.Update(gameTime);
            stuckTimer.Update(gameTime);
            #endregion

            #region Check Distance to Target. Readjust if the Target has moved a lot
            if (checkDistanceTimer.Done)
            {
                if (pathFinder.Path.Count != 0)
                {
                    float distance = Vector2.Distance(_targetLocationComponent.Position, moxieLocation);
                    if (distance > _maxDistanceBeforeFindingNewPath)
                    {
                        //Find a path
                        FindPath();
                    }
                }
            }
            #endregion

            #region Pathfinding
            if (pathFinder.Path.Count == 0)
            {
                //Find a path
                FindPath();
            }
            //Move along the path
            else
            {
                Vector2 movementVector = Vector2.Zero;
                Rectangle nodeBounds = new Rectangle((int)currentNodeVector.X - (nodeSize / 2), (int)currentNodeVector.Y - (nodeSize / 2), nodeSize, nodeSize);

                //Check if we are inside the sensor range of the entity
                //Check if you have arrived at the location first
                if (!insideTargetSensor)
                {
                    if (nodeBounds.Contains((int)moxieLocation.X, (int)moxieLocation.Y))
                    {
                        //Arrived at the node
                        currentNodeInPath--; //Move to the next node
                        if (currentNodeInPath == 0)
                        {
                            Finish();
                            Moxie.ThoughtProcess.SetAction(_actionToMakeAtTarget);
                            return;
                        }
                        Tile nextNode = Path[currentNodeInPath - 1]; //Get the next tile node
                        currentNodeVector = new Vector2((nextNode.Location.X * Tile.tileWidth) + (Tile.tileWidth / 2), (nextNode.Location.Y * Tile.tileHeight) + (Tile.tileHeight / 2)); //Find the location of the next node
                        stuckTimer.Reset(); //Reset the stuck timer
                    }
                    //We havent arrived at the node yet. move to it
                    else
                    {
                        movementVector = new Vector2(currentNodeVector.X - moxieLocation.X, currentNodeVector.Y - moxieLocation.Y); //Get the movement vector to the node
                        movementVector.Normalize(); //Normalize the vector
                        MovementComponent moxieMC = (MovementComponent)Moxie.GetComponent(typeof(MovementComponent));
                        moxieMC.SetMovementVector(movementVector); //Set the movement vector
                        moxieMC.SetSpeed(moxieMC.NormalSpeed); ///Return the speed to normal

                        //Stuck heuristic
                        if (stuckTimer.Done)
                        {
                            //Add this target to the ignore list
                            Moxie.AddIgnoredEntity(_targetLocationComponent.Parent);
                            Finish();
                        }
                    }
                }
                else
                //Made it inside the target sensor
                {
                    Finish();
                    Moxie.ThoughtProcess.SetAction(_actionToMakeAtTarget);
                    return;
                }
            }

            #endregion

            base.Update(gameTime);

        }

        private void FindPath()
        {
            SpatialComponent moxieSpatialComponent = (SpatialComponent)Moxie.GetComponent(typeof(SpatialComponent));
            Tile targetTile = pathFinder.GetTileFromVector(_targetLocationComponent.Position);
            Tile startTile = pathFinder.GetTileFromVector(moxieSpatialComponent.Position);

            if (targetTile != null && startTile != null)
            {
                pathFinder.FindPath(startTile, targetTile, PathfindingController.Tiles);

                List<Tile> path = pathFinder.Path;

                if (path == null)
                {
                    throw new Exception("Path returned null");
                }
                if (path.Count > 0)
                {
                    currentNodeInPath = path.Count;
                    Tile nextNode = Path[currentNodeInPath - 1];
                    currentNodeVector = new Vector2((nextNode.Location.X * Tile.tileWidth) + (Tile.tileWidth / 2), (nextNode.Location.Y * Tile.tileHeight) + (Tile.tileHeight / 2));
                }
                else
                {
                    //Otherwise you can just move to the point without the help of the path finder
                }
            }
            else
            {
                throw new Exception("Target or Start Tile is null...fix it");
            }
        }


    }

}
