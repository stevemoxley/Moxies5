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
using Moxies5.Components;
using Moxies5.Serialization;
using Moxies5.Utilities;
using Moxies5.Controllers;
using Moxies5.Components.MoxieComponents;

namespace Moxies5.Entities
{
    public enum BodySize
    {
        Baby,
        Small,
        Normal,
    }

    public class MoxieEntity : Entity, ISerialize
    {
        #region Fields

        private int _health;
        private int _sleep;
        private int _hunger;
        private int _fun;
        private int _age;
        private int _maxAge;
        private ThoughtProcess _thoughtProcess;

        private List<Entity> _entityIgnoreList = new List<Entity>();

        private BodySize _bodySize;

        #region Timers
        public Timer _sleepTimer;
        public Timer _hungerTimer;
        public Timer _funTimer;
        public Timer _ageTimer;
        public Timer _starvationTimer; //This will only tick when the moxie has zero hunger.
        public Timer _clearIgnoreListTimer; //This will clear the items on the ignore list when it ticks

        #endregion

        #endregion

        #region Properties
        public int Health
        {
            get
            {
                return _health;
            }
        }
        public int Sleep
        {
            get
            {
                return _sleep;
            }
        }
        public int Hunger
        {
            get
            {
                return _hunger;
            }
        }
        public int Fun
        {
            get
            {
                return _fun;
            }
        }

        /// <summary>
        /// Age of the Moxie in minutes
        /// </summary>
        public int Age
        {
            get
            {
                return _age;
            }
        }

        public ThoughtProcess ThoughtProcess
        {
            get
            {
                return _thoughtProcess;
            }
        }

        public List<Entity> EntityIgnoreList
        {
            get
            {
                return _entityIgnoreList;
            }
        }

        public MoxieGeneticsComponent Genetics
        {
            get
            {
                return (MoxieGeneticsComponent)GetComponent(typeof(MoxieGeneticsComponent));
            }

        }

        public BodySize BodySize
        {
            get
            {
                return _bodySize;
            }
            set
            {
                _bodySize = value;
            }
        }

        /// <summary>
        /// Max age of a Moxie before it dies
        /// </summary>
        public int MaxAge
        {
            get
            {
                return _maxAge;
            }
            set
            {
                _maxAge = value;
            }
        }

        #endregion

        #region Getters and Setters
        public void SetHealth(int health)
        {
            this._health = health;
            _health = (int)MathHelper.Clamp(_health, 0, 100);
        }
        public void SetSleep(int sleep)
        {
            this._sleep = sleep;
            _sleep = (int)MathHelper.Clamp(_sleep, 0, 100);
        }
        public void SetHunger(int hunger)
        {
            this._hunger = hunger;
            _hunger = (int)MathHelper.Clamp(_hunger, 0, 100);
        }
        public void SetFun(int fun)
        {
            this._fun = fun;
            _fun = (int)MathHelper.Clamp(_fun, 0, 100);
        }
        public void SetAge(int age)
        {
            this._age = age;
        }
        public void SetBodySize(BodySize size)
        {
            this._bodySize = size;
        }
        #endregion

        #region Constructor

        public MoxieEntity()
        {

            #region Timer Setup
            //These will changed based off the genetics of the Moxie
            _sleepTimer = new Timer(10);
            _hungerTimer = new Timer(3);
            _funTimer = new Timer(25);
            _starvationTimer = new Timer(5); 


            _ageTimer = new Timer(60); //This won't change. 
            _clearIgnoreListTimer = new Timer(30);

            #endregion

            #region Thought Process
            _thoughtProcess = new ThoughtProcess(this);
            #endregion

        }
        #endregion

        #region Methods

        /// <summary>
        /// XNA update method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (gameTime != null)
            {
                #region Timers

                #region Timer Updates
                _sleepTimer.Update(gameTime);
                _hungerTimer.Update(gameTime);
                _funTimer.Update(gameTime);
                _ageTimer.Update(gameTime);
                _starvationTimer.Update(gameTime);
                _clearIgnoreListTimer.Update(gameTime);
                #endregion

                #region Timer Actions
                //Some of these things will change based off of the genetics.
                //For example. Heavy eaters will get hungrier faster and heavy sleepers
                //will get sleepier faster
                if (_sleepTimer.Done)
                {
                    _sleep -= 1;

                    if (_sleep < 0)
                        _sleep = 0;
                }
                if (_hungerTimer.Done)
                {
                    _hunger -= 1;

                    if (_hunger < 0)
                        _hunger = 0;
                }
                if (_funTimer.Done)
                {
                    _fun -= 1;

                    if (_fun < 0)
                        _fun = 0;
                }
                if (_ageTimer.Done)
                {
                    _age++;
                }

                #region Starvation
                if (_hunger == 0)
                {
                    //Start and Reset the starvation Timer
                    if (!_starvationTimer.Running)
                    {
                        _starvationTimer.Start(); //Start the timer
                        _starvationTimer.Reset(); //Reset
                    }
                    else
                    {
                        if (_starvationTimer.Done)
                        {
                            _health--; //take health away from the Moxie
                        }
                    }
                }
                else if (_hunger > 0)
                {
                    if (_starvationTimer.Running)
                    {
                        _starvationTimer.Stop();
                        _starvationTimer.Reset();
                    }
                }
                #endregion

                #region Ignore List Clearing
                if (_clearIgnoreListTimer.Done)
                {
                    _entityIgnoreList.Clear();
                }
                #endregion

                #endregion

                #endregion

                //Thought processing should probably always be last
                #region Thought Process
                if (_thoughtProcess.Action == null)
                {
                    _thoughtProcess.FindAction(); //Find one
                }
                #endregion
            }

            base.Update(gameTime);
        }

        public void AddIgnoredEntity(Entity ignore)
        {
            _entityIgnoreList.Add(ignore);
        }

        /// <summary>
        /// Generates a genome for new Moxies
        /// </summary>
        public void GenerateForFirstMoxies(GenderTrait gender)
        {
            MoxieGeneticsComponent genes = new MoxieGeneticsComponent(this);
            AddInitialComponent(genes);

            genes.DHungerTrait = HungerTraits.Normal;
            genes.RHungerTrait = HungerTraits.Normal;

            genes.DSleepTrait = SleepTraits.Normal;
            genes.RSleepTrait = SleepTraits.Normal;

            genes.DHealthTrait = HealthTraits.Normal;
            genes.RHealthTrait = HealthTraits.Normal;

            genes.DFunTrait = FunTraits.Normal;
            genes.RFunTrait = FunTraits.Normal;

            genes.DSocialTrait = SocialTraits.Normal;
            genes.RSocialTrait = SocialTraits.Normal;

            genes.DBreed = Breeds.Standard;
            genes.RBreed = Breeds.Standard;

            genes.DLifespanTrait = LifespanTraits.Normal;
            genes.RLifespanTrait = LifespanTraits.Normal;

            genes.DReproductivityTrait = ReproductivityTraits.One;
            genes.RReproductivityTrait = ReproductivityTraits.One;

            genes.DProductivityTrait = ProductivityTraits.Normal;
            genes.RProductivityTrait = ProductivityTraits.Normal;

            genes.DTempToleranceTrait = TempToleranceTraits.Normal;
            genes.RTempToleranceTrait = TempToleranceTraits.Normal;

            genes.DHumidityToleranceTrait = HumidityToleranceTraits.Normal;
            genes.RHumidityToleranceTrait = HumidityToleranceTraits.Normal;

            genes.Gender = gender;

            UIController.ChangeMoxieCount(1);

            SetMaxAge();

        }

        /// <summary>
        /// Sets the max age of the Moxie based on the DLifespanTrait
        /// </summary>
        public void SetMaxAge()
        {
            switch (Genetics.DLifespanTrait)
            {
                case LifespanTraits.Shortest:
                    {
                        _maxAge = MainController.Random.Next(5, 8);
                        break;
                    }
                case LifespanTraits.Short:
                    {
                        _maxAge = MainController.Random.Next(7, 10);
                        break;
                    }
                case LifespanTraits.Normal:
                    {
                        _maxAge = MainController.Random.Next(9,13);
                        break;
                    }
                case LifespanTraits.Long:
                    {
                        _maxAge = MainController.Random.Next(13, 19);
                        break;
                    }
                case LifespanTraits.Longest:
                    {
                        _maxAge = MainController.Random.Next(21,26);
                        break;
                    }
                default:
                    {
                        throw new Exception("No case found");
                    }
            }
        }

        /// <summary>
        /// Adds the initial components for the Moxie
        /// </summary>
        public void Initialize()
        {
            #region Components

            //Body Component
            MoxieBodyComponent moxieBodyComponent = new MoxieBodyComponent(this, Cameras.Dynamic);
            AddInitialComponent(moxieBodyComponent);

            //Eyes Component
            MoxieEyeComponent moxieEyesComponent = new MoxieEyeComponent(this, Cameras.Dynamic);
            moxieEyesComponent.SetEyeState(MoxieEyeComponent.EyeStates.Open);
            AddInitialComponent(moxieEyesComponent);

            //Mouth Component
            MoxieMouthComponent moxieMouthComponent = new MoxieMouthComponent(this, Cameras.Dynamic);
            AddInitialComponent(moxieMouthComponent);

            MovementComponent mc = new MovementComponent(this);
            mc.SetSpeed(10);
            mc.SetNormalSpeed(10);
            AddInitialComponent(mc);

            SpatialComponent sc = new SpatialComponent(this);
            AddInitialComponent(sc);

            //Physics component
            PhysicsComponent pc = new PhysicsComponent(this);
            pc.CreateCircleBody(20, 1);
            AddInitialComponent(pc);

            //Proximity sensor
            SensorComponent sensor = new SensorComponent(this);
            sensor.CreateSensorCircleBody(23, 1);
            AddInitialComponent(sensor);

            //Thought Bubble
            ThoughtBubbleComponent tbub = new ThoughtBubbleComponent(this);
            AddInitialComponent(tbub);

            #endregion

            #region Field Initialization
            _health = 100;
            _sleep = 100;
            _hunger = 81;
            _fun = 100;
            _age = 0;

            SetBodySize(BodySize.Normal);


            #endregion

        }

        private void SetScaleOfBodyComponents(float scaleOfBodyComponents, float sizeOfPhysicsComponent)
        {
            MoxieBodyComponent body = (MoxieBodyComponent)GetComponent(typeof(MoxieBodyComponent));
            body.SetScale(scaleOfBodyComponents);

            MoxieMouthComponent mouth = (MoxieMouthComponent)GetComponent(typeof(MoxieMouthComponent));
            mouth.SetScale(scaleOfBodyComponents);

            MoxieEyeComponent eye = (MoxieEyeComponent)GetComponent(typeof(MoxieEyeComponent));
            eye.SetScale(scaleOfBodyComponents);

            PhysicsComponent PC = (PhysicsComponent)GetComponent(typeof(PhysicsComponent));
            if (!PhysicsController.BodyRemoveListContains(PC.Body) && PC.Body != null)
            {
                PhysicsController.RemoveBody(PC.Body);
                PC.CreateCircleBody(sizeOfPhysicsComponent, 1);
            }

            SensorComponent sensor = (SensorComponent)GetComponent(typeof(SensorComponent));
            if (!PhysicsController.BodyRemoveListContains(sensor.Body) && sensor.Body != null)
            {
                PhysicsController.RemoveBody(sensor.Body);
                sensor.CreateSensorCircleBody(sizeOfPhysicsComponent + 3, 1);
                if(PC.Body != null)
                    sensor.Body.IgnoreCollisionWith(PC.Body);
            }

            if (HasComponent(typeof(MoxieOutlineComponent)))
            {
                MoxieOutlineComponent moc = (MoxieOutlineComponent)GetComponent(typeof(MoxieOutlineComponent));
                moc.SetScale(scaleOfBodyComponents);
            }
            
            
        }

        #region Serialization
        public new SaveObject Serialize(int ID)
        {
            MoxieEntitySave save = new MoxieEntitySave();
            save.Serialize(this, ID);
            return save;
        }


        #endregion

        #endregion
    }


    #region Save Object Class
    public class MoxieEntitySave : SaveObject
    {
        public int Health;
        public int Sleep;
        public int Hunger;
        public int Fun;
        public int Age;
        public int MaxAge;

        public float SleepTimerTime;
        public float HungerTimerTime;
        public float FunTimerTime;
        public float AgeTimerTime;
        public float StarvationTimerTime;
        public float ClearEntityIgnoreListTime;

        public BodySize BodySize;

        public override void Serialize(object toSerialize, int ID)
        {
            MoxieEntity entity = (MoxieEntity)toSerialize;
            this.Health = entity.Health;
            this.Sleep = entity.Sleep;
            this.Hunger = entity.Hunger;
            this.Fun = entity.Fun;
            this.Age = entity.Age;
            this.MaxAge = entity.MaxAge;

            this.SleepTimerTime = entity._sleepTimer.ElapsedTime;
            this.HungerTimerTime = entity._hungerTimer.ElapsedTime;
            this.FunTimerTime = entity._funTimer.ElapsedTime;
            this.AgeTimerTime = entity._ageTimer.ElapsedTime;
            this.StarvationTimerTime = entity._starvationTimer.ElapsedTime;
            this.ClearEntityIgnoreListTime = entity._clearIgnoreListTimer.ElapsedTime;

            this.BodySize = entity.BodySize;

            base.Serialize(toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            MoxieEntity moxie = new MoxieEntity();
            MoxieEntitySave mSave = (MoxieEntitySave)save;
            moxie.SetHealth(mSave.Health);
            moxie.SetSleep(mSave.Sleep);
            moxie.SetHunger(mSave.Hunger);
            moxie.SetFun(mSave.Fun);
            moxie.SetAge(mSave.Age);
            moxie._sleepTimer.ElapsedTime = mSave.SleepTimerTime;
            moxie._hungerTimer.ElapsedTime = mSave.HungerTimerTime;
            moxie._funTimer.ElapsedTime = mSave.FunTimerTime;
            moxie._ageTimer.ElapsedTime = mSave.AgeTimerTime;
            moxie._starvationTimer.ElapsedTime = mSave.StarvationTimerTime;
            moxie._clearIgnoreListTimer.ElapsedTime = mSave.ClearEntityIgnoreListTime;
            moxie.BodySize = mSave.BodySize;
            moxie.MaxAge = mSave.MaxAge;
            return moxie;
        }

    }
#endregion
}
