// Behavior originally contributed by Natfoth.
//
// DOCUMENTATION:
//     http://www.thebuddyforum.com/mediawiki/index.php?title=Honorbuddy_Custom_Behavior:_MyCTM
//
using System;
using System.Collections.Generic;
using System.Threading;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

using TreeSharp;
using Action = TreeSharp.Action;


namespace Styx.Bot.Quest_Behaviors
{
    public class MoveTo : CustomForcedBehavior
    {
        public MoveTo(Dictionary<string, string> args)
            : base(args)
        {
			try
			{
                Destination = GetXYZAttributeAsWoWPoint("", true, null) ?? WoWPoint.Empty;
				Precision = GetAttributeAsDouble("Precision", false, 0, double.MaxValue, null) ?? 0;
				UseMount = GetAttributeAsBoolean("UseMount", false, null) ?? true;
				
			}

			catch (Exception except)
			{
				UtilLogMessage("error", "BEHAVIOR MAINTENANCE PROBLEM: " + except.Message
										+ "\nFROM HERE:\n"
										+ except.StackTrace + "\n");
				IsAttributeProblem = true;
			}
        }


        // Attributes provided by caller
        public WoWPoint                 Destination { get; private set; }
		public double					Precision	{ get; private set; }
		public bool 					UseMount { get; private set; }

        // Private variables for internal state
		public WoWPoint[]               Path { get; private set; }
        private bool            _isBehaviorDone;
		private bool            OnSpot;
		private int             _pathIndex;
        private Composite       _root;

        // Private properties
        private LocalPlayer         Me { get { return (ObjectManager.Me); } }
		private bool canmount { get { return (Styx.Logic.Mount.CanMount()); } }
		
		WoWPoint NextPoint
		{
			get
			{
				if (Me.Location.Distance(Path[_pathIndex]) <= Precision && _pathIndex < Path.Length - 1)
				{
					_pathIndex++;
				}
				return Path[_pathIndex];
			}	
		}
        #region Overrides of CustomForcedBehavior

        protected override Composite CreateBehavior()
        {
            return _root ?? (_root =
                 new PrioritySelector(

                            new Decorator(ret => OnSpot,
                                new Sequence(
                                    new Action(ret => TreeRoot.StatusText = "Finished!"),
                                    new WaitContinue(120,
                                        new Action(delegate
                                        {
                                            _isBehaviorDone = true;
                                            return RunStatus.Success;
                                        }))
                                    )),
							new Decorator(ret =>UseMount && Me.GetSkill(Styx.SkillLine.Riding).CurrentValue != 0 && canmount && !Me.Mounted && !Me.IsCasting && Destination.Distance(Me.Location) > 100,
                                new Sequence(
                                        new Action(ret => TreeRoot.StatusText = "Mounting"),
                                        new Action(ret => WoWMovement.MoveStop()),
                                        new Action(ret => Thread.Sleep(100)),
										new Action(ret => Styx.Logic.Mount.MountUp())
                                    )
                                ),
							new Decorator(ret => !Me.IsCasting && Precision != 0 && Me.Location.Distance(Destination) > Precision && Path == null,
								new Action(ret => Path = Navigator.GeneratePath(Me.Location, Destination))),
							new Decorator(ret =>!Me.IsCasting && Precision != 0 && Me.Location.Distance(Destination) > Precision,
                                new Sequence(
                                        new Action(ret => TreeRoot.StatusText = "Moving To Location - X: " + NextPoint.X + " Y: " + NextPoint.Y + " Distance = " + Me.Location.Distance(NextPoint)),
                                        new Action(ret => WoWMovement.ClickToMove(NextPoint)),
                                        new Action(ret => Thread.Sleep(50))
                                    )
                                ),
							
                            new Decorator(ret =>!Me.IsCasting && Precision == 0 && Destination.Distance(Me.Location) > 3,
                                new Sequence(
                                        new Action(ret => TreeRoot.StatusText = "Moving To Location - X: " + Destination.X + " Y: " + Destination.Y),
                                        new Action(ret => Navigator.MoveTo(Destination)),
                                        new Action(ret => Thread.Sleep(50))
                                    )
                                ),
							new Action(delegate
							{
								OnSpot = true;
							})
                    ));
        }


        public override bool IsDone
        {
            get
            {
                return (_isBehaviorDone);
            }
        }


        public override void OnStart()
        {
            OnStart_HandleAttributeProblem();

            if (!IsDone)
            {
                TreeRoot.GoalText = "MoveTo Running";
            }
        }

        #endregion
    }
}

