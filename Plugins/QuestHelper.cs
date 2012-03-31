using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using Levelbot.Actions.Combat;
using Styx;
using Styx.Helpers;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Profiles;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.Logic.Questing;
using Styx.Logic.POI;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace QuestHelper
{
    public class QuestHelper : HBPlugin, IDisposable
    {
		#region uselessbutnecessaryshit
		public override string Name { get { return "QuestHelper"; } }
        public override string Author { get { return "Project-WoW"; } }
        public override Version Version { get { return new Version(1, 0, 0); } }
		#endregion
		private static LocalPlayer Me { get { return ObjectManager.Me; } }
		public int counter = 0;
		private bool Dothis = true;
		private bool onstart = true;
		private static Stopwatch feedPetTimer = new Stopwatch();
		private static Stopwatch afktimer = new Stopwatch();
		private static Stopwatch dothis = new Stopwatch();
		private static Stopwatch InCombatTimer = new Stopwatch();
		private static WoWPoint lastpoint = new WoWPoint();
		private List<ulong> localblacklist = new List<ulong>();
		private WoWPoint stuckloc1 = new WoWPoint(-266.5306, -3525.131, 41.87179);
		private WoWPoint safeloc1 = new WoWPoint(-256.2392, -3543.411, 21.53597);
		private WoWPoint stuckloc2 = new WoWPoint(-7145.544, -2345.563, -215.0251);
		private WoWPoint safeloc2 = new WoWPoint(-7140.273, -2293.389, -267.7866);
		public int PNDErrorCount = 0;
		public int LOSErrorCount = 0;
		public int TooCloseErrorCount = 0;
		public List<WoWUnit> q26026mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => ((u.Entry == 3992 || u.Entry == 3993 || u.Entry == 4202 || u.Entry == 4070) && !u.Dead && u.DistanceSqr < 5 * 5 ))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q27330mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 46067 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q24814mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 38302 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q25688mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 41191 && !u.Dead && u.IsCasting))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q13988mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 34615 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q25112mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 5419 && u.Dead && !localblacklist.Contains(u.Guid)))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q25111mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 5429 && u.Dead && !localblacklist.Contains(u.Guid)))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q24737mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => ((u.Entry == 6517 || u.Entry == 6518 || u.Entry == 6519 || u.Entry == 6527) && u.Dead && !localblacklist.Contains(u.Guid)))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q24699mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 38307 && u.Dead && !localblacklist.Contains(u.Guid)))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q25701mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => ((u.Entry == 41280 || u.Entry == 41279) && u.CurrentHealth <= 1))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q25115mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 5426 && u.Dead && !localblacklist.Contains(u.Guid)))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q25591mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 41083 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> goblinrezmob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 36608 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q24735mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 9999 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q8346mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == 15274 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> q24963mob
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => ((u.Entry == 5472 || u.Entry == 5474) && u.CurrentHealth < 700 && !u.Dead && u.IsHostile))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> tamelist
		{
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry ==  3475 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> badtargets
		{
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry ==  35205 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> newtarget
				{
					get
					{
						return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.IsHostile && !u.Dead && u.Entry != 35205))
                                    .OrderBy(u => u.Distance).ToList();
					}
				}
        public override void Pulse()
        {
			ObjectManager.Update();
            
			if (onstart)
			{
				Log("loaded");
				Lua.Events.AttachEvent("CHAT_MSG_RAID_BOSS_WHISPER", HandleRBW);
				Lua.Events.AttachEvent("CHAT_MSG_MONSTER_EMOTE", HandleME);
				Lua.Events.AttachEvent("CHAT_MSG_MONSTER_SAY", HandleMS);
				Lua.Events.AttachEvent("UI_ERROR_MESSAGE", HandleUEM);
				onstart = false;
			}
			#region stuck issues
			if (!Me.Dead && Me.Location.Distance(stuckloc1) < 5)
			{
				Log("In proximity of general stuck location. Moving out");
				WoWMovement.ClickToMove(safeloc1);
				Thread.Sleep(3000);
				Styx.Logic.Pathing.Navigator.Clear();
			}
			if (Me.IsGhost && Me.Location.Distance(stuckloc2) < 10)
			{
				Log("In proximity of general stuck location. Moving out");
				WoWMovement.ClickToMove(safeloc2);
				Thread.Sleep(5000);
				Styx.Logic.Pathing.Navigator.Clear();
			}
			#endregion
			#region 26026
			if ((!Me.Dead && Me.QuestLog.GetQuestById(26026) != null && q26026mob.Count > 0) || (Me.Combat && Me.ZoneId == 400 && Me.CurrentTarget != null && Me.CurrentTarget.DistanceSqr < 5 * 5 && Me.CurrentTarget.Entry != 40959))
			{	
				Log("AntiFuckin'Disengage Procedure running");
				WoWMovement.MoveStop();
				while (Me.Combat && Me.CurrentTarget != null && !Me.CurrentTarget.Dead && Me.CurrentTarget.DistanceSqr < 5 * 5)
				{
					if (!Me.IsAutoAttacking)
					{
						SpellManager.Cast(6603);
					}
					Me.CurrentTarget.Face();
					SpellManager.Cast(34026);
					Thread.Sleep(1000);
					SpellManager.Cast(2973);
					Thread.Sleep(2000);
				}
				
			}
            #endregion
			#region 13988
			if (!Me.Dead && Me.QuestLog.GetQuestById(13988) != null && !Me.Combat && q13988mob.Count == 0)
			{
				Log("Where the fuck did that bird go?");
				WoWMovement.MoveStop();
				Thread.Sleep(100);
				Lua.DoString("UseItemByName(46782)");
				Thread.Sleep(2000);
			}
			#endregion
			#region 25688
			if (!Me.Dead && Me.QuestLog.GetQuestById(25688) != null && !Me.QuestLog.GetQuestById(25688).IsCompleted && q25688mob.Count > 0)
			{
				Log(q25688mob[0].Name+" Disrupting ressurect. Kill it");
                q25688mob[0].Target();
                Thread.Sleep(200);
                Lua.DoString("PetAttack()");
				if (q25688mob[0].Location.Distance(Me.Pet.Location) < 5 && SpellManager.Spells.ContainsKey("Kill Command") && !SpellManager.Spells["Kill Command"].Cooldown)
				{
					Log("Kill Command");
					SpellManager.Cast("Kill Command");
				}
			}
			#endregion
			#region 26028
			if (Me.QuestLog.GetQuestById(26028) != null && Me.Combat && Me.CurrentTarget != null)
			{
				Log("CC override");
				Me.CurrentTarget.Face();
				Lua.DoString("RunMacroText('/click VehicleMenuBarActionButton1','0')");
				Lua.DoString("RunMacroText('/click VehicleMenuBarActionButton3','0')");
				Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromMinutes(1));
				Me.ClearTarget();
			}
			#endregion
			while (Me.QuestLog.GetQuestById(24958) != null && !Me.QuestLog.GetQuestById(24958).IsCompleted && Me.CurrentTarget != null && Me.CurrentTarget.Entry == 38855 && Me.CurrentTarget.IsAlive)
			{
				Log("Firing at "+Me.CurrentTarget.Name);
				WoWMovement.MoveStop();
				Me.CurrentTarget.Face();
				Thread.Sleep(100);
				Lua.DoString("UseItemByName(52043)");
				Thread.Sleep(100);
			}
			#region 25165
			if (Me.QuestLog.GetQuestById(25165) != null && !Me.QuestLog.GetQuestById(25165).IsCompleted && Me.CurrentTarget != null && Me.CurrentTarget.Entry == 3125 && Me.CurrentTarget.Location.Distance(Me.Location) <= 30 && counter == 0 && Me.CurrentTarget.IsAlive)
			{
				WoWMovement.MoveStop();
				SpellManager.Cast(75);
				while (!Me.CurrentTarget.IsCasting)
				{
					Log("Waiting for "+Me.CurrentTarget.Name +" to start a cast");
					Thread.Sleep(1000);
				}
				Log("Placing totem for "+Me.CurrentTarget.Name);
				Lua.DoString("UseItemByName(52505)");
				Thread.Sleep(2000);
				counter++;
			}
			if (Me.QuestLog.GetQuestById(25165) != null && !Me.QuestLog.GetQuestById(25165).IsCompleted && !Me.Combat)
			{
				counter = 0;
			}
			
			#endregion
			#region 27336
			if (Me.QuestLog.GetQuestById(27336) != null && !Me.QuestLog.GetQuestById(27336).IsCompleted && Me.Combat && Dothis && Me.CurrentTarget != null && (Me.CurrentTarget.Entry == 4344 || Me.CurrentTarget.Entry == 4345))
			{
				Dothis = false;
				Log("Placing totem for "+Me.CurrentTarget.Name);
				Lua.DoString("UseItemByName(33101)");
				SpellManager.Cast(781);
				Thread.Sleep(1500);
				SpellManager.Cast(3044);
			}
			if (Me.QuestLog.GetQuestById(27336) != null && !Me.QuestLog.GetQuestById(27336).IsCompleted && !Me.Combat && !Dothis)
			{
				Dothis = true;
			}
			if (Me.QuestLog.GetQuestById(27336) != null && !Me.QuestLog.GetQuestById(27336).IsCompleted && Me.Combat && Me.CurrentTarget != null && (Me.CurrentTarget.Entry == 4344 || Me.CurrentTarget.Entry == 4345) && Me.CurrentTarget.CurrentHealth < 500)
			{
				Log("Calling pet back to get the killing blow");
				Lua.DoString("PetStopAttack()");
				WoWMovement.MoveStop();
				Me.CurrentTarget.Face();
				while (Me.Combat && Me.CurrentTarget != null && !Me.CurrentTarget.Dead && Me.CurrentTarget.CurrentHealth < 500)
				{
					SpellManager.Cast(53351);
					SpellManager.Cast(2973);
					Thread.Sleep(300);
				}
			}
			#endregion
			#region 13961
			if ((!dothis.IsRunning || dothis.Elapsed.Seconds > 5) && Me.QuestLog.GetQuestById(13961) != null && !Me.QuestLog.GetQuestById(13961).IsCompleted && Me.CurrentTarget != null && Me.CurrentTarget.Entry == 34503 && Me.CurrentTarget.Location.Distance(Me.Location) <= 15 && !Me.HasAura("Dragging a Razormane"))
			{
				Log("Using net on "+Me.CurrentTarget.Name);
				dothis.Reset();
				dothis.Start();
				Lua.DoString("UseItemByName(46722)");
				Thread.Sleep(100);
				Blacklist.Add(Me.CurrentTarget.Guid, TimeSpan.FromMinutes(1));
				Thread.Sleep(100);
				Me.ClearTarget();
				
			}
			if (Me.HasAura("Dragging a Razormane"))
			{
				Log("Dragging a Razormane");
				dothis.Reset();
			}
			#endregion
			#region tame
			if (!Me.Dead && !Me.IsGhost && Me.QuestLog.GetQuestById(881) != null && Me.CarriedItems.Any(i => i.Entry == 10327) && !Me.GotAlivePet)
			{
				Log("taming new pet!");
				while (!Me.GotAlivePet)
				{
					ObjectManager.Update();
					if (tamelist.Count > 0)
					{
						tamelist[0].Target();
						tamelist[0].Face();
						if (tamelist[0].Location.Distance(Me.Location) > 30)
						{
							Navigator.MoveTo(tamelist[0].Location);
						}
						if (tamelist[0].Location.Distance(Me.Location) <= 30 && !Me.IsCasting)
						{
							WoWMovement.MoveStop();
							SpellManager.Cast("Tame Beast");
						}
					}
					if (tamelist.Count == 0 && !Me.IsCasting)
					{
						WoWPoint tameloc = new WoWPoint(-22.75822, -2395.158, 91.66674);
						if (tameloc.Distance(Me.Location) > 1)
						{
							Navigator.MoveTo(tameloc);
						}
						if (tameloc.Distance(Me.Location) <= 1)
						{
							WoWMovement.MoveStop();
							Thread.Sleep(2000);
							Lua.DoString("UseItemByName(10327)");
						}
					}
				}
				Me.ClearTarget();
			}
			#endregion
			#region 13621
			if (!Me.Dead && Me.QuestLog.GetQuestById(13621) != null && !Me.QuestLog.GetQuestById(13621).IsCompleted)
			{
				if (!afktimer.IsRunning)
				{
					Log("checking stuck");
					Log("saving current position");
					WoWPoint prevloc = new WoWPoint(Me.X, Me.Y, Me.Z);
					lastpoint = prevloc;
					afktimer.Reset();
					afktimer.Start();
				}
				if (afktimer.Elapsed.Seconds > 55 && lastpoint == Me.Location)
				{
					Log("stuck");
					afktimer.Reset();
					ProfileManager.LoadNew(Path.Combine(Path.GetDirectoryName(ProfileManager.XmlLocation), "ashenvale.xml"), false);
				}
				if (afktimer.Elapsed.Seconds > 55 && lastpoint != Me.Location)
				{
					Log("all good");
					afktimer.Reset();
				}
			}
			if (!Me.Dead && Me.QuestLog.GetQuestById(13621) != null && Me.QuestLog.GetQuestById(13621).IsCompleted)
			{
				afktimer.Reset();
			}
			#endregion
			#region mounted & under attack
			if (Me.Mounted && Me.Combat && !Me.CarriedItems.Any(i => i.Entry == 56799))
			{
				Log("Dismount & fight");
				Mount.Dismount();
			}
			#endregion
			#region 13980
			if (!Me.Dead && Me.QuestLog.GetQuestById(13980) != null && !Me.QuestLog.GetQuestById(13980).IsCompleted && !Me.HasAura("Jinx's Elf Detection Ray"))
			{
				Log("Using Jinx's Goggles");
				Lua.DoString("UseItemByName(46776)");
			}
			#endregion
			#region 26066
			while (Me.QuestLog.GetQuestById(26066) != null && !Me.QuestLog.GetQuestById(26066).IsCompleted && Me.Combat && Me.CurrentTarget != null && (Me.CurrentTarget.Entry == 11915 || Me.CurrentTarget.Entry == 11917))
			{
				if (Me.CurrentTarget.Location.Distance(Me.Location) > 4)
				{
					Navigator.MoveTo(Me.CurrentTarget.Location);
				}
				else
				{
					Log("Using Whip on " +Me.CurrentTarget.Name);
					WoWMovement.MoveStop();
					Lua.DoString("UseItemByName(56794)");
				}
			}
			#endregion
			#region 25757
			if (!Me.Dead && Me.QuestLog.GetQuestById(25757) != null && !Me.QuestLog.GetQuestById(25757).IsCompleted && Me.Combat && Me.CurrentTarget != null && Me.CurrentTarget.Entry == 41196 && Me.CurrentTarget.CurrentHealth == 1)
			{
				Log("Moving to loot "+ Me.CurrentTarget.Name);
				while (!Me.CurrentTarget.WithinInteractRange)
				{
					Navigator.MoveTo(Me.CurrentTarget.Location);
				}
				WoWMovement.MoveStop();
				Thread.Sleep(1000);
				Me.CurrentTarget.Interact();
				Me.ClearTarget();
			}
			#endregion
			if (Me.Race == WoWRace.Goblin && Me.ZoneId == 4720 && Me.HasAura("Near Death!") && goblinrezmob.Count > 0)
			{
				goblinrezmob[0].Interact();
				Thread.Sleep(1000);
				Lua.DoString("RunMacroText('/click QuestFrameCompleteQuestButton')");
			}
			#region 25628
			if (!Me.Dead && Me.QuestLog.GetQuestById(25628) != null && Me.Combat && Me.CurrentTarget != null && Me.CurrentTarget.Entry == 40959)
			{
				Lua.DoString("UseItemByName(55158)");
				Thread.Sleep(1000);
				Me.ClearTarget();
			}
			#endregion
			#region 27330
			if (!Me.Dead && Me.QuestLog.GetQuestById(27330) != null && Me.Combat && q27330mob.Count > 3 && Me.CurrentTarget.Entry == 45447)
			{
				Log("Ending combat");
				Lua.DoString("StopAttack()");
				Lua.DoString("PetStopAttack()");
				Me.ClearTarget();
			}
			if (!Me.Dead && Me.QuestLog.GetQuestById(27330) != null && Me.Combat && q27330mob.Count == 1 && Me.CurrentTarget.Entry == 45447)
			{
				Log("Switching target");
				q27330mob[0].Target();
			}
			
			#endregion
			#region 24814
			if (!Me.Dead && Me.QuestLog.GetQuestById(24814) != null && !Me.QuestLog.GetQuestById(24814).IsCompleted && Me.Combat && q24814mob.Count > 0 && Me.CurrentTarget.Entry == 38306)
			{
				Log("Switching target");
				q24814mob[0].Target();
			}
			#endregion
			#region 25591
			if (!Me.Dead && Me.QuestLog.GetQuestById(25591) != null && Me.Combat && q25591mob.Count == 1 && Me.CurrentTarget.Entry != 41083)
			{
				Log("Switching target");
				q25591mob[0].Target();
			}
			
			#endregion
			#region 25112
			if (!Me.Dead && Me.QuestLog.GetQuestById(25112) != null && !Me.QuestLog.GetQuestById(25112).IsCompleted && q25112mob.Count > 0)
			{
				Log("Harvesting Basillisk");
				Thread.Sleep(1000);
				q25112mob[0].Target();
				Lua.DoString("UseItemByName(52715)");
				Thread.Sleep(1000);
				localblacklist.Add(q25112mob[0].Guid);
				Me.ClearTarget();
			}
			#endregion
			#region 25701
			if (!Me.Dead && Me.QuestLog.GetQuestById(25701) != null && !Me.QuestLog.GetQuestById(25701).IsCompleted && q25701mob.Count > 0)
			{
				Log("Using knife");
				Thread.Sleep(1000);
				q25701mob[0].Target();
				Lua.DoString("UseItemByName(56012)");
				Thread.Sleep(1000);
			}
			#endregion
			#region 25111
			if (!Me.Dead && Me.QuestLog.GetQuestById(25111) != null && !Me.QuestLog.GetQuestById(25111).IsCompleted && q25111mob.Count > 0)
			{
				Log("Harvesting Fire Roc");
				Thread.Sleep(1000);
				q25111mob[0].Target();
				Lua.DoString("UseItemByName(52715)");
				Thread.Sleep(1000);
				localblacklist.Add(q25111mob[0].Guid);
				Me.ClearTarget();
			}
			#endregion
			#region 25115
			if (!Me.Dead && Me.QuestLog.GetQuestById(25115) != null && !Me.QuestLog.GetQuestById(25115).IsCompleted && q25115mob.Count > 0)
			{
				Log("Harvesting Blisterpaw Hyena");
				Thread.Sleep(1000);
				q25115mob[0].Target();
				Lua.DoString("UseItemByName(52715)");
				Thread.Sleep(1000);
				localblacklist.Add(q25115mob[0].Guid);
				Me.ClearTarget();
			}
			#endregion
			#region 24737
			if (!Me.Dead && Me.QuestLog.GetQuestById(24737) != null && !Me.QuestLog.GetQuestById(24737).IsCompleted && q24737mob.Count > 0)
			{
				Log("Harvesting Tar");
				while (Me.Location.Distance(q24737mob[0].Location) > 5)
				{
					Navigator.MoveTo(q24737mob[0].Location);
				}
				WoWMovement.MoveStop();
				Thread.Sleep(300);
				q24737mob[0].Target();
				Lua.DoString("UseItemByName(50742)");
				Thread.Sleep(2000);
				localblacklist.Add(q24737mob[0].Guid);
				Me.ClearTarget();
			}
			#endregion
			#region 24699
			if (!Me.Dead && Me.QuestLog.GetQuestById(24699) != null && !Me.QuestLog.GetQuestById(24699).IsCompleted && q24699mob.Count > 0)
			{
				Log("Harvesting Tar");
				while (Me.Location.Distance(q24699mob[0].Location) > 5)
				{
					Navigator.MoveTo(q24699mob[0].Location);
				}
				WoWMovement.MoveStop();
				Thread.Sleep(300);
				q24699mob[0].Target();
				Lua.DoString("UseItemByName(50746)");
				Thread.Sleep(2000);
				localblacklist.Add(q24699mob[0].Guid);
				Me.ClearTarget();
			}
			#endregion
			#region 13850
			while (!Me.Dead && Me.QuestLog.GetQuestById(13850) != null && !Me.QuestLog.GetQuestById(13850).IsCompleted && Me.Combat && Me.CurrentTarget != null && Me.CurrentTarget.Entry == 6508 && !Me.CurrentTarget.Dead && Me.HealthPercent > 30)
			{
				Log("Suicide mission");
				Lua.DoString("PetStopAttack()");
				while (Me.CurrentTarget.Location.Distance(Me.Location) > 5)
				{
					Navigator.MoveTo(Me.CurrentTarget.Location);
				}
				WoWMovement.MoveStop();
				if (!Me.IsAutoAttacking)
				{
					SpellManager.Cast(6603);
				}
				Thread.Sleep(1000);
			}
			#endregion
			#region 13850
			while (!Me.Dead && Me.QuestLog.GetQuestById(24697) != null && !Me.QuestLog.GetQuestById(24697).IsCompleted && Me.Combat && Me.CurrentTarget != null && (Me.CurrentTarget.Entry == 9162 || Me.CurrentTarget.Entry == 9163) && Me.CurrentTarget.HasAura("Throw Meat"))
			{
				Log("Running back to spike pit");
				WoWPoint pitloc = new WoWPoint(-7530.954, -1458.899, -279.552);
				while (Me.Location.Distance(pitloc) > 2)
				{
					Navigator.MoveTo(pitloc);
				}
				WoWMovement.MoveStop();
				Thread.Sleep(1000);
			}
			#endregion
			#region 24963
			while (!Me.Dead && Me.QuestLog.GetQuestById(24963) != null && !Me.QuestLog.GetQuestById(24963).IsCompleted && q24963mob.Count > 0)
			{
				Log("Feeding " + q24963mob[0].Name);
				SpellManager.StopCasting();
				q24963mob[0].Target();
				Thread.Sleep(100);
				Lua.DoString("UseItemByName(52044)");
			}
			#endregion
			#region 8346
			if (!Me.Dead && Me.QuestLog.GetQuestById(8346) != null && !Me.QuestLog.GetQuestById(8346).IsCompleted && q8346mob.Count > 0 && Me.Location.Distance(q8346mob[0].Location) < 7)
			{
				Log("Casting Arcane Torrent on " + q8346mob[0].Name);
				SpellManager.StopCasting();
				q8346mob[0].Target();
				Thread.Sleep(100);
				Lua.DoString("CastSpellByName('Arcane Torrent')");
			}
			#endregion
			#region stupid bot behaviors
			if (!Me.Dead && PNDErrorCount > 5)
			{
				Log("Calling pet");
				Thread.Sleep(2000);
				Lua.DoString("CastSpellByName('Call Pet 1')");
				PNDErrorCount = 0;
			}
			if (!Me.Dead && LOSErrorCount > 5 && Me.CurrentTarget != null)
			{
				Log("moving closer to target to get in LoS");
				Navigator.MoveTo(Me.CurrentTarget.Location);
				LOSErrorCount = 0;
				Thread.Sleep(2000);
			}
			if (!Me.Dead && TooCloseErrorCount > 5 && Me.CurrentTarget != null && !Me.IsAutoAttacking)
			{
				Log("Attacking target");
				Lua.DoString("CastSpellByName('Auto Attack')");
				TooCloseErrorCount = 0;
			}
			#endregion
			#region combat overrides
			//If someone knows a better way to avoid combat or override current CC behavior please contact me.
			if (Me.Combat && Me.QuestLog.GetQuestById(24695) == null)
			{
				if (Me.HealthPercent < 30)
                {
                    Healing.UseHealthPotion();
                }
				if (Me.CurrentTarget == null && badtargets.Count > 0 && (!InCombatTimer.IsRunning || InCombatTimer.Elapsed.Seconds > 5))
				{
					InCombatTimer.Start();
					if (InCombatTimer.Elapsed.Seconds > 5)
					{
						Log("stuck in combat with " +badtargets[0].Name);
						newtarget[0].Target();
						InCombatTimer.Reset();
					}
				}
				foreach (WoWUnit t in Targeting.Instance.TargetList.Where(t => Me.Level > 11 && Me.GotAlivePet && t.CurrentTargetGuid == Me.Guid && Me.Pet.CurrentTargetGuid != t.Guid && Me.CurrentTarget != null && t.CanSelect))
                {
                    Log(t.Name+" Is touching me in funny places! Let my pet join the fun");
                    WoWUnit LastTarget = Me.CurrentTarget;
                    Lua.DoString("StopAttack()");
                    t.Target();
                    Thread.Sleep(200);
                    Lua.DoString("PetAttack()");
					if (Me.Combat && t.Location.Distance(Me.Pet.Location) > 7)
					{
						ObjectManager.Update();
						Thread.Sleep(2000);
						Lua.DoString("PetAttack()");
					}
					if (SpellManager.Spells.ContainsKey("Intimidation") && !SpellManager.Spells["Intimidation"].Cooldown)
					{
						Log("Using Intimidation");
						SpellManager.Cast("Intimidation");
					}
					if (SpellManager.Spells.ContainsKey("Kill Command") && !SpellManager.Spells["Kill Command"].Cooldown)
					{
						Log("Kill Command");
						SpellManager.Cast("Kill Command");
					}
                    LastTarget.Target();
                    Thread.Sleep(500);
                }
				foreach (WoWUnit t in Targeting.Instance.TargetList.Where(t => Me.Level > 11 && Me.GotAlivePet && t.CurrentTargetGuid == Me.Guid && t.Entry == 41064 && Me.CurrentTarget != null))
				{
					Log(t.Name+" Argo on me. Lets try to survive");
					Lua.DoString("StopAttack()");
					Thread.Sleep(200);
					Lua.DoString("PetAttack()");
					while (Me.Combat && t.Location.Distance(Me.Location) < 15 && !t.Dead)
					{
						ObjectManager.Update();
						Thread.Sleep(500);
						Lua.DoString("PetAttack()");
						WoWMovement.Move(WoWMovement.MovementDirection.Backwards);
						if (SpellManager.Spells.ContainsKey("Intimidation") && !SpellManager.Spells["Intimidation"].Cooldown)
						{
							Log("Using Intimidation");
							SpellManager.Cast("Intimidation");
						}
						if (SpellManager.Spells.ContainsKey("Kill Command") && !SpellManager.Spells["Kill Command"].Cooldown)
						{
							Log("Kill Command");
							SpellManager.Cast("Kill Command");
						}
					}
					WoWMovement.MoveStop(WoWMovement.MovementDirection.Backwards);
					
				}
			}
			#endregion
			while (Me.BagItems.Exists(i => i.Entry == 49932) && Me.QuestLog.GetQuestById(24606) == null)
			{
				Styx.Logic.Inventory.Frames.Quest.QuestFrame quest = new Styx.Logic.Inventory.Frames.Quest.QuestFrame();
				Lua.DoString("UseItemByName(49932)");
				Thread.Sleep(1000);
				quest.AcceptQuest();
			}
        }
		private void HandleRBW(object sender, LuaEventArgs args)
        {
			if (args.Args[0].ToString() == "Slap Ringo before he passes out!" && !Me.Dead && Me.QuestLog.GetQuestById(24735) != null && !Me.QuestLog.GetQuestById(24735).IsCompleted && q24735mob.Count > 0)
			{
				Log("Spank 'da bitch!");
				while (Me.Location.Distance(q24735mob[0].Location) > 5)
				{
					Navigator.MoveTo(q24735mob[0].Location);
				}
				WoWMovement.MoveStop();
				q24735mob[0].Interact();
				StyxWoW.SleepForLagDuration();
			}
			if (!Me.Dead && Me.QuestLog.GetQuestById(14122) != null && !Me.QuestLog.GetQuestById(14122).IsCompleted)
			{
				//Logging.Write(""+args.Args[0]);
				if (args.Args[0].ToString().Contains("Amazing G-Ray"))
				{
					Thread.Sleep(1000);
					Lua.DoString("UseAction(121, 'target', 'LeftButton')");
				}
				if (args.Args[0].ToString().Contains("Blastcrackers"))
				{
					Thread.Sleep(1000);
					Lua.DoString("UseAction(122, 'target', 'LeftButton')");
				}
				if (args.Args[0].ToString().Contains("Ear-O-Scope"))
				{
					Thread.Sleep(1000);
					Lua.DoString("UseAction(123, 'target', 'LeftButton')");
				}
				if (args.Args[0].ToString().Contains("Infinifold Lockpick"))
				{
					Thread.Sleep(1000);
					Lua.DoString("UseAction(124, 'target', 'LeftButton')");
				}
				if (args.Args[0].ToString().Contains("mite Drill"))
				{
					Thread.Sleep(1000);
					Lua.DoString("UseAction(125, 'target', 'LeftButton')");
				}
			}
        }
		private void HandleMS(object sender, LuaEventArgs args)
        {
			if ((args.Args[0].ToString() == "Hey!  A little help over here!" ||
			args.Args[0].ToString() == "Umm... I think I might need a hand here..." ||
			args.Args[0].ToString() == "If we don't slow down soon, I might pass out." ||
			args.Args[0].ToString() == "Ugh... maybe if you could lightly slap me on the cheek...") && !Me.Dead && Me.QuestLog.GetQuestById(24735) != null && !Me.QuestLog.GetQuestById(24735).IsCompleted && q24735mob.Count > 0)
			{
				Log("Slap 'da bitch!");
				while (Me.Location.Distance(q24735mob[0].Location) > 5)
				{
					Navigator.MoveTo(q24735mob[0].Location);
				}
				WoWMovement.MoveStop();
				q24735mob[0].Interact();
				StyxWoW.SleepForLagDuration();
			}
			if (args.Args[0].ToString() == "Squire!  Please stand next to me as I request the blessing of this Light-blessed spirit." && !Me.Dead && Me.QuestLog.GetQuestById(24706) != null && !Me.QuestLog.GetQuestById(24706).IsCompleted)
			{
				Log("Let the idiot get his blessing");
				Lua.DoString("PetStopAttack()");
				SpellManager.StopCasting();
				Me.ClearTarget();
				if (!SpellManager.Spells["Freezing Trap"].Cooldown)
				{
					SpellManager.Cast("Freezing Trap");
				}
				Thread.Sleep(10000);
			}
		}
		private void HandleME(object sender, LuaEventArgs args)
        {
			if (args.Args[0].ToString() == "%s faints and falls onto the ground." && !Me.Dead && Me.QuestLog.GetQuestById(24735) != null && !Me.QuestLog.GetQuestById(24735).IsCompleted && q24735mob.Count > 0)
			{
				Log("Pour some water on that lazy ass!");
				while (Me.Location.Distance(q24735mob[0].Location) > 20)
				{
					Navigator.MoveTo(q24735mob[0].Location);
				}
				WoWMovement.MoveStop();
				SpellManager.StopCasting();
				Thread.Sleep(1000);
				Lua.DoString("UseItemByName(11804)");
				Thread.Sleep(6000);
			}
        }
		private void HandleUEM(object sender, LuaEventArgs args)
		{
			if (args.Args[0].ToString() == "Your pet is not dead.")
			{
				PNDErrorCount++;
			}
			if (args.Args[0].ToString() == "Target not in line of sight")
			{
				LOSErrorCount++;
			}
			if (args.Args[0].ToString() == "Target too close")
			{
				TooCloseErrorCount++;
			}
		}
		public override void OnButtonPress(){}
		private static void Log(string format, params object[] args)
        {
            Logging.Write(Color.DarkViolet, "[QuestHelper] " + format, args);
        }
        
    }
}

