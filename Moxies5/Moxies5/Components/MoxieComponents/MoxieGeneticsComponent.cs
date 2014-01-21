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
using Moxies5.Controllers;

namespace Moxies5.Components.MoxieComponents
{
    #region Trait Enums

    //Each trait will have an enum
    //The enum will rate the traits with 0 being the worst
    //When a Moxie looks for a mate they will rate the potential
    //Partners. A moxie will only mate with a partner that is within
    //a range of its rating
    public enum HungerTraits
    {
        Glutton = 0,
        HeavyEater = 1,
        Normal = 2,
        LightEater = 3,
        Dieter = 4
    }

    public enum SleepTraits
    {
        Narcoleptic = 0,
        HeavySleeper = 1,
        LightSleeper = 2,
        Normal = 3,
    }

    public enum FunTraits
    {
        Boring = 0,
        Normal = 1,
        Energetic = 2
    }

    public enum SocialTraits
    {
        Evil = 0,
        Rude = 1,
        Normal = 2,
        Nice = 3,
        Angelic = 4
    }

    public enum HealthTraits
    {
        Sickly = 0,
        Normal = 1,
        Healthy = 2
    }

    public enum TempToleranceTraits
    {
        Arctic,
        Cold,
        Normal,
        Hot,
        Sweltering
    }

    public enum LifespanTraits
    {
        Shortest = 0,
        Short = 1,
        Normal = 2,
        Long = 3,
        Longest = 4
    }

    public enum ReproductivityTraits
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3
    }

    public enum ProductivityTraits
    {
        Slowest = 0,
        Slower = 1,
        Slow = 2,
        Normal = 3,
        Faster = 4,
        Fast = 5,
        Fastest = 6
    }

    public enum HumidityToleranceTraits
    {
        Wet,
        Damp,
        Normal,
        Dry,
        Arid
    }

    //It is possible for a Moxie to have no gender.
    //This Moxie won't be able to reproduce
    /// <summary>
    /// The gender of the Moxie
    /// </summary>
    public enum GenderTrait
    {
        Male,
        Female,
        None
    }

    /// <summary>
    /// Mutations are special flags that Moxies can carry
    /// They can be good or bad. Not sure how they will be
    /// considered for Reproduction Rating
    /// </summary>
    [Flags]
    public enum MutationTraits
    {
        None = 0,
        ///0x1
        ///0x2
        ///0x4
        ///0x8
        ///0x01
        ///0x02
        ///0x04
        ///0x08
    }
    #endregion

    #region Breed Enum
    public enum Breeds
    {
        Standard,
        Mini,
        Fiery,
        Icy,
        Beautiful,
        Charming,
        Sacred,
        Glitz,
        Demonic,
        Modest,
        Diligent,
        Foul,
        Manic,
        Beastly,
        Genius,
        Radioactive,
        Noble,
        Majestic,
        Ancient,
    }
    #endregion

    public class MoxieGeneticsComponent: Component, ISerialize
    {
        
        #region Fields

        public HungerTraits DHungerTrait{ get; set; }
        public HungerTraits RHungerTrait{ get; set; }

        public SleepTraits DSleepTrait{ get; set; }
        public SleepTraits RSleepTrait{ get; set; }

        public FunTraits DFunTrait{ get; set; }
        public FunTraits RFunTrait{ get; set; }

        public SocialTraits DSocialTrait{ get; set; }
        public SocialTraits RSocialTrait{ get; set; }

        public HealthTraits DHealthTrait{ get; set; }
        public HealthTraits RHealthTrait{ get; set; }

        public LifespanTraits DLifespanTrait{ get; set; }
        public LifespanTraits RLifespanTrait{ get; set; }

        public ReproductivityTraits DReproductivityTrait{ get; set; }
        public ReproductivityTraits RReproductivityTrait{ get; set; }

        public ProductivityTraits DProductivityTrait{ get; set; }
        public ProductivityTraits RProductivityTrait{ get; set; }

        public TempToleranceTraits DTempToleranceTrait{ get; set; }
        public TempToleranceTraits RTempToleranceTrait{ get; set; }

        public HumidityToleranceTraits DHumidityToleranceTrait{ get; set; }
        public HumidityToleranceTraits RHumidityToleranceTrait{ get; set; }

        public Breeds DBreed { get; set; }
        public Breeds RBreed { get; set; }

        public MutationTraits Mutations{ get; set; }

        public GenderTrait Gender{ get; set; }

        public const int MaxReproductivityRating = 28;

        private Color _bodyColor;

        #endregion

        #region Properties
        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public MoxieGeneticsComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "MoxieGeneticsComponent";

            _bodyColor = Color.White;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Calculates and returns the reproduction rating
        /// </summary>
        /// <returns>The reproduction rating</returns>
        public int CalculateReproductionRating()
        {
            int _reproductionRating = -1;
            int hungerValue = (int)DHungerTrait;
            int sleepValue = (int)DSleepTrait;
            int funValue = (int)DFunTrait;
            int socialValue = (int)DSocialTrait;
            int healthValue = (int)DHealthTrait;
            int lifespanValue = (int)DLifespanTrait;
            int reproductivityValue = (int)DReproductivityTrait;
            int productivityValue = (int)DProductivityTrait;

            _reproductionRating = hungerValue + sleepValue + funValue + socialValue + healthValue + lifespanValue + reproductivityValue + productivityValue;

            return _reproductionRating;
        }

        /// <summary>
        /// Returns the sell value of the Moxie based on the genetics
        /// </summary>
        /// <returns>Sell value of the Moxie</returns>
        public int CalculateSellValue()
        {
            throw new NotImplementedException();
        }

        public SaveObject Serialize(int ID)
        {
            MoxieGeneticsComponentSave save = new MoxieGeneticsComponentSave();
            save.Serialize(this, ID);
            return save;
        }



        /// <summary>
        /// Adds a new mutation 
        /// </summary>
        /// <param name="newMutation"></param>
        public void AddMutation(MutationTraits newMutation)
        {
            if(!Mutations.HasFlag(newMutation))
                Mutations |= newMutation;
        }

        /// <summary>
        /// Removes a mutation
        /// </summary>
        /// <param name="rMutation"></param>
        public void RemoveMutation(MutationTraits rMutation)
        {
            if (Mutations.HasFlag(rMutation))
                Mutations &= ~rMutation;
        }

        /// <summary>
        /// Check if the Moxie has a mutation
        /// </summary>
        /// <param name="mutation">The mutation in question</param>
        /// <returns>True or false</returns>
        public bool HasMutation(MutationTraits mutation)
        {
            if (Mutations.HasFlag(mutation))
                return true;
            else return false;
        }

        /// <summary>
        /// This will set the Genetics based on the genes of the two parents
        /// </summary>
        /// <param name="fatherGenes">The fathers Genetics component</param>
        /// <param name="motherGenes">The mother's Genetics component</param>
        public void SetGeneticsFromParents(MoxieGeneticsComponent fatherGenes, MoxieGeneticsComponent motherGenes)
        {
            //Steps to getting the genes
            //Roll a dice between 1-4
            //1) Take the dominant traits of both parents. The father's gene will be Dominant and mother's recessive.
            //2) Take the dominant trait of the father and the recessive trait of the mother. The father's gene will be dominant.
            //3) Take the domiant trait of the mother and the recessive trait of the father. The mother's gene will be dominnat.
            //4) Take the recessive trait of both parents. The father's gene will be Dominant and the mother's recessive.

            #region Hunger
            int hungerRoll = MainController.Random.Next(1, 5);
            if (hungerRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (hungerRoll == 1)
            {
                DHungerTrait = fatherGenes.DHungerTrait;
                RHungerTrait = motherGenes.DHungerTrait;
            }
            else if (hungerRoll == 2)
            {
                DHungerTrait = fatherGenes.DHungerTrait;
                RHungerTrait = motherGenes.RHungerTrait;
            }
            else if (hungerRoll == 3)
            {
                DHungerTrait = fatherGenes.RHungerTrait;
                RHungerTrait = motherGenes.DHungerTrait;
            }
            else if (hungerRoll == 4)
            {
                DHungerTrait = fatherGenes.RHungerTrait;
                RHungerTrait = motherGenes.RHungerTrait;
            }
            #endregion

            #region Sleep
            int sleepRoll = MainController.Random.Next(1, 5);
            if (sleepRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (sleepRoll == 1)
            {
                DSleepTrait = fatherGenes.DSleepTrait;
                RSleepTrait = motherGenes.DSleepTrait;
            }
            else if (sleepRoll == 2)
            {
                DSleepTrait = fatherGenes.DSleepTrait;
                RSleepTrait = motherGenes.RSleepTrait;
            }
            else if (sleepRoll == 3)
            {
                DSleepTrait = fatherGenes.RSleepTrait;
                RSleepTrait = motherGenes.DSleepTrait;
            }
            else if (sleepRoll == 4)
            {
                DSleepTrait = fatherGenes.RSleepTrait;
                RSleepTrait = motherGenes.RSleepTrait;
            }
            #endregion

            #region Fun
            int funRoll = MainController.Random.Next(1, 5);
            if (funRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (funRoll == 1)
            {
                DFunTrait = fatherGenes.DFunTrait;
                RFunTrait = motherGenes.DFunTrait;
            }
            else if (funRoll == 2)
            {
                DFunTrait = fatherGenes.DFunTrait;
                RFunTrait = motherGenes.RFunTrait;
            }
            else if (funRoll == 3)
            {
                DFunTrait = fatherGenes.RFunTrait;
                RFunTrait = motherGenes.DFunTrait;
            }
            else if (funRoll == 4)
            {
                DFunTrait = fatherGenes.RFunTrait;
                RFunTrait = motherGenes.RFunTrait;
            }
            #endregion

            #region Social
            int socialRoll = MainController.Random.Next(1, 5);
            if (socialRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (socialRoll == 1)
            {
                DSocialTrait = fatherGenes.DSocialTrait;
                RSocialTrait = motherGenes.DSocialTrait;
            }
            else if (socialRoll == 2)
            {
                DSocialTrait = fatherGenes.DSocialTrait;
                RSocialTrait = motherGenes.RSocialTrait;
            }
            else if (socialRoll == 3)
            {
                DSocialTrait = fatherGenes.RSocialTrait;
                RSocialTrait = motherGenes.DSocialTrait;
            }
            else if (socialRoll == 4)
            {
                DSocialTrait = fatherGenes.RSocialTrait;
                RSocialTrait = motherGenes.RSocialTrait;
            }
            #endregion

            #region Health
            int healthRoll = MainController.Random.Next(1, 5);
            if (healthRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (healthRoll == 1)
            {
                DHealthTrait = fatherGenes.DHealthTrait;
                RHealthTrait = motherGenes.DHealthTrait;
            }
            else if (healthRoll == 2)
            {
                DHealthTrait = fatherGenes.DHealthTrait;
                RHealthTrait = motherGenes.RHealthTrait;
            }
            else if (healthRoll == 3)
            {
                DHealthTrait = fatherGenes.RHealthTrait;
                RHealthTrait = motherGenes.DHealthTrait;
            }
            else if (healthRoll == 4)
            {
                DHealthTrait = fatherGenes.RHealthTrait;
                RHealthTrait = motherGenes.RHealthTrait;
            }
            #endregion

            #region TempTolerance
            int tempRoll = MainController.Random.Next(1, 5);
            if (tempRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (tempRoll == 1)
            {
                DTempToleranceTrait = fatherGenes.DTempToleranceTrait;
                RTempToleranceTrait = motherGenes.DTempToleranceTrait;
            }
            else if (tempRoll == 2)
            {
                DTempToleranceTrait = fatherGenes.DTempToleranceTrait;
                RTempToleranceTrait = motherGenes.RTempToleranceTrait;
            }
            else if (tempRoll == 3)
            {
                DTempToleranceTrait = fatherGenes.RTempToleranceTrait;
                RTempToleranceTrait = motherGenes.DTempToleranceTrait;
            }
            else if (tempRoll == 4)
            {
                DTempToleranceTrait = fatherGenes.RTempToleranceTrait;
                RTempToleranceTrait = motherGenes.RTempToleranceTrait;
            }
            #endregion

            #region Lifespan
            int lifespanRoll = MainController.Random.Next(1, 5);
            if (lifespanRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (lifespanRoll == 1)
            {
                DLifespanTrait = fatherGenes.DLifespanTrait;
                RLifespanTrait = motherGenes.DLifespanTrait;
            }
            else if (lifespanRoll == 2)
            {
                DLifespanTrait = fatherGenes.DLifespanTrait;
                RLifespanTrait = motherGenes.RLifespanTrait;
            }
            else if (lifespanRoll == 3)
            {
                DLifespanTrait = fatherGenes.RLifespanTrait;
                RLifespanTrait = motherGenes.DLifespanTrait;
            }
            else if (lifespanRoll == 4)
            {
                DLifespanTrait = fatherGenes.RLifespanTrait;
                RLifespanTrait = motherGenes.RLifespanTrait;
            }
            #endregion

            #region Reproductivity
            int reproductivityRoll = MainController.Random.Next(1, 5);
            if (reproductivityRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (reproductivityRoll == 1)
            {
                DReproductivityTrait = fatherGenes.DReproductivityTrait;
                RReproductivityTrait = motherGenes.DReproductivityTrait;
            }
            else if (reproductivityRoll == 2)
            {
                DReproductivityTrait = fatherGenes.DReproductivityTrait;
                RReproductivityTrait = motherGenes.RReproductivityTrait;
            }
            else if (reproductivityRoll == 3)
            {
                DReproductivityTrait = fatherGenes.RReproductivityTrait;
                RReproductivityTrait = motherGenes.DReproductivityTrait;
            }
            else if (reproductivityRoll == 4)
            {
                DReproductivityTrait = fatherGenes.RReproductivityTrait;
                RReproductivityTrait = motherGenes.RReproductivityTrait;
            }
            #endregion

            #region Productivity
            int productivityRoll = MainController.Random.Next(1, 5);
            if (productivityRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (productivityRoll == 1)
            {
                DProductivityTrait = fatherGenes.DProductivityTrait;
                RProductivityTrait = motherGenes.DProductivityTrait;
            }
            else if (productivityRoll == 2)
            {
                DProductivityTrait = fatherGenes.DProductivityTrait;
                RProductivityTrait = motherGenes.RProductivityTrait;
            }
            else if (productivityRoll == 3)
            {
                DProductivityTrait = fatherGenes.RProductivityTrait;
                RProductivityTrait = motherGenes.DProductivityTrait;
            }
            else if (productivityRoll == 4)
            {
                DProductivityTrait = fatherGenes.RProductivityTrait;
                RProductivityTrait = motherGenes.RProductivityTrait;
            }
            #endregion

            #region HumidityTolerance
            int humidityRoll = MainController.Random.Next(1, 5);
            if (humidityRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (humidityRoll == 1)
            {
                DHumidityToleranceTrait = fatherGenes.DHumidityToleranceTrait;
                RHumidityToleranceTrait = motherGenes.DHumidityToleranceTrait;
            }
            else if (humidityRoll == 2)
            {
                DHumidityToleranceTrait = fatherGenes.DHumidityToleranceTrait;
                RHumidityToleranceTrait = motherGenes.RHumidityToleranceTrait;
            }
            else if (humidityRoll == 3)
            {
                DHumidityToleranceTrait = fatherGenes.RHumidityToleranceTrait;
                RHumidityToleranceTrait = motherGenes.DHumidityToleranceTrait;
            }
            else if (humidityRoll == 4)
            {
                DHumidityToleranceTrait = fatherGenes.RHumidityToleranceTrait;
                RHumidityToleranceTrait = motherGenes.RHumidityToleranceTrait;
            }
            #endregion

            #region Breed
            int breedRoll = MainController.Random.Next(1, 5);
            if (breedRoll == 5)
                throw new Exception("Roll cant be 5.");
            else if (breedRoll == 1)
            {
                DBreed = fatherGenes.DBreed;
                RBreed = motherGenes.DBreed;
            }
            else if (breedRoll == 2)
            {
                DBreed = fatherGenes.DBreed;
                RBreed = motherGenes.RBreed;
            }
            else if (breedRoll == 3)
            {
                DBreed = fatherGenes.RBreed;
                RBreed = motherGenes.DBreed;
            }
            else if (breedRoll == 4)
            {
                DBreed = fatherGenes.RBreed;
                RBreed = motherGenes.RBreed;
            }
            #endregion 

            #region Breed Mutation
            MutateBreed(fatherGenes.DBreed, motherGenes.DBreed);
            #endregion
        }

        /// <summary>
        /// This will mutate the breed of the Moxie
        /// </summary>
        private void MutateBreed(Breeds FatherDBreed, Breeds MotherDBreed)
        {
            //Roll a random number
            int rand = MainController.Random.Next(1, 101);

            #region Two Normals Makes a diligent
            if ((FatherDBreed == Breeds.Standard && MotherDBreed == Breeds.Standard))
            {
                int chanceToMutate = 15;
                if (rand <= chanceToMutate)
                {
                    DBreed = Breeds.Diligent;
                    RBreed = Breeds.Diligent;

              

                    //Change any other traits

                    //Unlock the money maker item
                    UIController.UnlockItem(Item.MONEY_MAKING_BASIC);

                    _bodyColor = Color.DodgerBlue;
                }
            }


            #endregion
        }

        public void SetBodyColor()
        {
            MoxieBodyComponent body = (MoxieBodyComponent)Parent.GetComponent(typeof(MoxieBodyComponent));
            body.SetColor(_bodyColor);
        }

        public bool TestIfMoxieIsPossibleMate(MoxieEntity possibleMate)
        {
            int bestReproductionRating = -1;
            int thisMoxieReproductionRating = CalculateReproductionRating();
            int marginOfAcceptance = (int)(thisMoxieReproductionRating * .1); //The mate has to be within the margin of acceptance of the other mate

            int reproductionRating = possibleMate.Genetics.CalculateReproductionRating();
            if (reproductionRating <= thisMoxieReproductionRating + marginOfAcceptance && reproductionRating >= thisMoxieReproductionRating - marginOfAcceptance)
            {
                if (reproductionRating > bestReproductionRating)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class MoxieGeneticsComponentSave : SaveObject
    {
        public HungerTraits DHungerTrait { get; set; }
        public HungerTraits RHungerTrait { get; set; }

        public SleepTraits DSleepTrait { get; set; }
        public SleepTraits RSleepTrait { get; set; }

        public FunTraits DFunTrait { get; set; }
        public FunTraits RFunTrait { get; set; }

        public SocialTraits DSocialTrait { get; set; }
        public SocialTraits RSocialTrait { get; set; }

        public HealthTraits DHealthTrait { get; set; }
        public HealthTraits RHealthTrait { get; set; }

        public LifespanTraits DLifespanTrait { get; set; }
        public LifespanTraits RLifespanTrait { get; set; }

        public ReproductivityTraits DReproductivityTrait { get; set; }
        public ReproductivityTraits RReproductivityTrait { get; set; }

        public ProductivityTraits DProductivityTrait { get; set; }
        public ProductivityTraits RProductivityTrait { get; set; }

        public TempToleranceTraits DTempToleranceTrait { get; set; }
        public TempToleranceTraits RTempToleranceTrait { get; set; }

        public HumidityToleranceTraits DHumidityToleranceTrait { get; set; }
        public HumidityToleranceTraits RHumidityToleranceTrait { get; set; }

        public MutationTraits Mutations { get; set; }

        public GenderTrait Gender { get; set; }

        public Breeds DBreed { get; set; }
        public Breeds RBreed { get; set; }


        public override void Serialize(object _moxieGeneticsComponent, int ID)
        {
            MoxieGeneticsComponent moxieGeneticsComponent = (MoxieGeneticsComponent)_moxieGeneticsComponent;

            this.DHungerTrait = moxieGeneticsComponent.DHungerTrait;
            this.RHungerTrait = moxieGeneticsComponent.RHungerTrait;

            this.DSleepTrait = moxieGeneticsComponent.DSleepTrait;
            this.RSleepTrait = moxieGeneticsComponent.RSleepTrait;

            this.DFunTrait = moxieGeneticsComponent.DFunTrait;
            this.RFunTrait = moxieGeneticsComponent.RFunTrait;

            this.DSocialTrait = moxieGeneticsComponent.DSocialTrait;
            this.RSocialTrait = moxieGeneticsComponent.RSocialTrait;

            this.DHealthTrait = moxieGeneticsComponent.DHealthTrait;
            this.RHealthTrait = moxieGeneticsComponent.RHealthTrait;

            this.DLifespanTrait = moxieGeneticsComponent.DLifespanTrait;
            this.RLifespanTrait = moxieGeneticsComponent.RLifespanTrait;

            this.DReproductivityTrait = moxieGeneticsComponent.DReproductivityTrait;
            this.RReproductivityTrait = moxieGeneticsComponent.RReproductivityTrait;

            this.DProductivityTrait = moxieGeneticsComponent.DProductivityTrait;
            this.RProductivityTrait = moxieGeneticsComponent.RProductivityTrait;

            this.DHumidityToleranceTrait = moxieGeneticsComponent.DHumidityToleranceTrait;
            this.RHumidityToleranceTrait = moxieGeneticsComponent.RHumidityToleranceTrait;

            this.DTempToleranceTrait = moxieGeneticsComponent.DTempToleranceTrait;
            this.RTempToleranceTrait = moxieGeneticsComponent.RTempToleranceTrait;

            this.DBreed = moxieGeneticsComponent.DBreed;
            this.RBreed = moxieGeneticsComponent.RBreed;

            this.Mutations = moxieGeneticsComponent.Mutations;

            this.Gender = moxieGeneticsComponent.Gender;

            base.Serialize(_moxieGeneticsComponent, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            MoxieGeneticsComponent genes = new MoxieGeneticsComponent(null);
            MoxieGeneticsComponentSave mgcSave = (MoxieGeneticsComponentSave)save;

            genes.DHungerTrait = mgcSave.DHungerTrait;
            genes.RHungerTrait = mgcSave.RHungerTrait;

            genes.DSleepTrait = mgcSave.DSleepTrait;
            genes.RSleepTrait = mgcSave.RSleepTrait;

            genes.DFunTrait = mgcSave.DFunTrait;
            genes.RFunTrait = mgcSave.RFunTrait;

            genes.DSocialTrait = mgcSave.DSocialTrait;
            genes.RSocialTrait = mgcSave.RSocialTrait;

            genes.DHealthTrait = mgcSave.DHealthTrait;
            genes.RHealthTrait = mgcSave.RHealthTrait;

            genes.DLifespanTrait = mgcSave.DLifespanTrait;
            genes.RLifespanTrait = mgcSave.RLifespanTrait;

            genes.DReproductivityTrait = mgcSave.DReproductivityTrait;
            genes.RReproductivityTrait = mgcSave.RReproductivityTrait;

            genes.DProductivityTrait = mgcSave.DProductivityTrait;
            genes.RProductivityTrait = mgcSave.RProductivityTrait;

            genes.DTempToleranceTrait = mgcSave.DTempToleranceTrait;
            genes.RTempToleranceTrait = mgcSave.RTempToleranceTrait;

            genes.DHumidityToleranceTrait = mgcSave.DHumidityToleranceTrait;
            genes.RHumidityToleranceTrait = mgcSave.RHumidityToleranceTrait;

            genes.Gender = mgcSave.Gender;

            genes.Mutations = mgcSave.Mutations;

            genes.DBreed = mgcSave.DBreed;
            genes.RBreed = mgcSave.RBreed;

            return genes;
        }

    }


}
