using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Quickstart
{
    public class QuickstartMod : Mod
    {
        private QuickstartModSettings settings;
        private QuickstartModOptionCategoryDef selectedCategory;
        private float optionsViewRectHeight;
        private Vector2 optionsScrollPosition;

        public QuickstartMod(ModContentPack content) : base(content)
        {
            this.AddOptionCategoryDefs();
            this.selectedCategory = DefDatabase<QuickstartModOptionCategoryDef>.GetNamed(QuickstartModOptionCategoryDef.General);

            this.settings = GetSettings<QuickstartModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            var optionCategories = DefDatabase<QuickstartModOptionCategoryDef>.AllDefsListForReading;

            int num1 = 0;
            foreach (var optionCategory in optionCategories)
            {
                this.DoCategoryRow(new Rect(0.0f, (float)num1 * 50f, 160f, 48f).ContractedBy(4f), optionCategory);
                num1++;
            }

            float num2 = 60f;
            this.DoCategoryOptions(new Rect(177f, 0.0f, (float)((double)inRect.width - 160.0 - 17.0), inRect.height - num2), this.selectedCategory);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Quick Start";
        }

        private void AddOptionCategoryDefs()
        {
            DefDatabase<QuickstartModOptionCategoryDef>.Add(new List<QuickstartModOptionCategoryDef>
            {
                new QuickstartModOptionCategoryDef
                {
                    defName = QuickstartModOptionCategoryDef.General,
                    label = "General",
                    texPath = "UI/Icons/Options/OptionsGeneral"
                },
                new QuickstartModOptionCategoryDef
                {
                    defName = QuickstartModOptionCategoryDef.Scenario,
                    label = "Scenario",
                    texPath = "UI/Icons/Options/OptionsGameplay"
                },
                new QuickstartModOptionCategoryDef
                {
                    defName = QuickstartModOptionCategoryDef.Priorities,
                    label = "Priorities",
                    texPath = "UI/Icons/Options/OptionsControls"
                },
                new QuickstartModOptionCategoryDef
                {
                    defName = QuickstartModOptionCategoryDef.Health,
                    label = "Health",
                    texPath = "UI/Icons/ColonistBar/MedicalRest"
                },
                new QuickstartModOptionCategoryDef
                {
                    defName = QuickstartModOptionCategoryDef.Assignments,
                    label = "Assignments",
                    texPath = "UI/Icons/ColonistBar/Idle"
                }
            });
        }

        private void DoCategoryRow(Rect r, QuickstartModOptionCategoryDef optionCategory)
        {
            Widgets.DrawOptionBackground(r, optionCategory == this.selectedCategory);
            if (Widgets.ButtonInvisible(r))
            {
                this.selectedCategory = optionCategory;
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
            float posTop = r.y + (float)(((double)r.height - 20.0) / 2.0);
            float iconPosLeft = r.x + 10f;
            GUI.DrawTexture(new Rect(iconPosLeft, posTop, 20f, 20f), (Texture)ContentFinder<Texture2D>.Get(optionCategory.texPath));

            float labelPosLeft = iconPosLeft + 30f;
            Widgets.Label(new Rect(labelPosLeft, posTop, r.width - labelPosLeft, r.height), optionCategory.label.CapitalizeFirst());
        }

        private void DoCategoryOptions(Rect inRect, QuickstartModOptionCategoryDef optionCategory)
        {
            bool flag = (double)this.optionsViewRectHeight > (double)inRect.height;
            Rect outRect = new Rect(inRect);
            Rect viewRect = new Rect(outRect.x, outRect.y, outRect.width - (flag ? 26f : 0.0f), this.optionsViewRectHeight);
            Widgets.BeginScrollView(outRect, ref this.optionsScrollPosition, viewRect);
            var listingStandard = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listingStandard.Begin(rect);
            listingStandard.verticalSpacing = 5f;
            listingStandard.Gap();

            switch (optionCategory.defName)
            {
                case QuickstartModOptionCategoryDef.General:
                    this.DoGeneralSettings(listingStandard);
                    break;
                case QuickstartModOptionCategoryDef.Priorities:
                    this.DoPrioritySettings(listingStandard);
                    break;
                case QuickstartModOptionCategoryDef.Health:
                    this.DoHealthSettings(listingStandard);
                    break;
                case QuickstartModOptionCategoryDef.Scenario:
                    this.DoScenarioSettings(listingStandard);
                    break;
                case QuickstartModOptionCategoryDef.Assignments:
                    this.DoAssignmentSettings(listingStandard);
                    break;
            }

            this.optionsViewRectHeight = listingStandard.CurHeight;
            listingStandard.End();
            Widgets.EndScrollView();
        }

        private void DoGeneralSettings(Listing_Standard listingStandard)
        {
            if (listingStandard.ButtonText("Start Quickstart Game"))
            {
                this.StartQuickstartGame();
            }
        }

        private void DoScenarioSettings(Listing_Standard listingStandard)
        {
            var scenarioLabel = DefDatabase<ScenarioDef>.GetNamed(settings.DefaultScenario).label;
            if (listingStandard.ButtonTextLabeled("Scenario", scenarioLabel, TextAnchor.MiddleLeft))
            {
                var scenarios = DefDatabase<ScenarioDef>.AllDefsListForReading.Where(s => s.defName != "Tutorial").ToList();
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (var scenario in scenarios)
                {
                    options.Add(new FloatMenuOption(scenario.label, (Action)(() =>
                    {
                        settings.DefaultScenario = scenario.defName;
                        scenarioLabel = scenario.label;
                    })));
                }
                Find.WindowStack.Add((Window)new FloatMenu(options));
            }

            var storytellerLabel = DefDatabase<StorytellerDef>.GetNamed(settings.DefaultStoryteller).label;
            if (listingStandard.ButtonTextLabeled("Storyteller", storytellerLabel, TextAnchor.MiddleLeft))
            {
                var storytellers = DefDatabase<StorytellerDef>.AllDefsListForReading.Where(s => s.defName != "Tutor").ToList();
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (var storyteller in storytellers)
                {
                    options.Add(new FloatMenuOption(storyteller.label, (Action)(() =>
                    {
                        settings.DefaultStoryteller = storyteller.defName;
                        storytellerLabel = storyteller.label;
                    })));
                }
                Find.WindowStack.Add((Window)new FloatMenu(options));
            }

            var difficultyLabel = DefDatabase<DifficultyDef>.GetNamed(settings.DefaultDifficulty).LabelCap;
            if (listingStandard.ButtonTextLabeled("Difficulty", difficultyLabel, TextAnchor.MiddleLeft))
            {
                var difficulties = DefDatabase<DifficultyDef>.AllDefsListForReading.Where(d => d.defName != "Custom").ToList();
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (var difficulty in difficulties)
                {
                    options.Add(new FloatMenuOption(difficulty.LabelCap, (Action)(() =>
                    {
                        settings.DefaultDifficulty = difficulty.defName;
                        difficultyLabel = difficulty.LabelCap;
                    })));
                }
                Find.WindowStack.Add((Window)new FloatMenu(options));
            }

            listingStandard.CheckboxLabeled("Commitment Mode", ref settings.CommitmentMode);
        }

        private void DoPrioritySettings(Listing_Standard listingStandard)
        {
            listingStandard.CheckboxLabeled("Set Work Priorities", ref settings.SetPriorities);

            settings.FirefightPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Firefight Priority: {settings.FirefightPriority}", settings.FirefightPriority, 0.0f, 4.0f));
            settings.PatientPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Patient Priority: {settings.PatientPriority}", settings.PatientPriority, 0.0f, 4.0f));
            settings.DoctorPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Doctor Priority: {settings.DoctorPriority}", settings.DoctorPriority, 0.0f, 4.0f));
            settings.BedRestPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Bed Rest Priority: {settings.BedRestPriority}", settings.BedRestPriority, 0.0f, 4.0f));
            settings.BasicPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Basic Priority: {settings.BasicPriority}", settings.BasicPriority, 0.0f, 4.0f));
            settings.WardenPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Warden Priority: {settings.WardenPriority}", settings.WardenPriority, 0.0f, 4.0f));
            settings.HandlePriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Handle Priority: {settings.HandlePriority}", settings.HandlePriority, 0.0f, 4.0f));
            settings.CookPrority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Cook Priority: {settings.CookPrority}", settings.CookPrority, 0.0f, 4.0f));
            settings.HuntPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Hunt Priority: {settings.HuntPriority}", settings.HuntPriority, 0.0f, 4.0f));
            settings.ConstructPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Construct Priority: {settings.ConstructPriority}", settings.ConstructPriority, 0.0f, 4.0f));
            settings.GrowPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Grow Priority: {settings.GrowPriority}", settings.GrowPriority, 0.0f, 4.0f));
            settings.MinePriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Mine Priority: {settings.MinePriority}", settings.MinePriority, 0.0f, 4.0f));
            settings.PlantCutPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Plant Cut Priority: {settings.PlantCutPriority}", settings.PlantCutPriority, 0.0f, 4.0f));
            settings.SmithPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Smith Priority: {settings.SmithPriority}", settings.SmithPriority, 0.0f, 4.0f));
            settings.TailorPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Tailor Priority: {settings.TailorPriority}", settings.TailorPriority, 0.0f, 4.0f));
            settings.ArtPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Art Priority: {settings.ArtPriority}", settings.ArtPriority, 0.0f, 4.0f));
            settings.CraftPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Craft Priority: {settings.CraftPriority}", settings.CraftPriority, 0.0f, 4.0f));
            settings.HaulPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Haul Priority: {settings.HaulPriority}", settings.HaulPriority, 0.0f, 4.0f));
            settings.CleanPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Clean Priority: {settings.CleanPriority}", settings.CleanPriority, 0.0f, 4.0f));
            settings.ResearchPriority = Mathf.RoundToInt(listingStandard.SliderLabeled($"Research Priority: {settings.ResearchPriority}", settings.ResearchPriority, 0.0f, 4.0f));
        }

        private void DoHealthSettings(Listing_Standard listingStandard)
        {
            listingStandard.CheckboxLabeled("Self Tend", ref settings.SelfTend);
        }

        private void DoAssignmentSettings(Listing_Standard listingStandard)
        {
            listingStandard.CheckboxLabeled("Set Assignments", ref settings.SetAssignments);

            TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(new Rect(0.0f, 50.0f, 191f, 65f));

            var assignmentHeaderRect = new Rect(0.0f, 90.0f, listingStandard.ColumnWidth, 30f);
            float widgetLabelWidth = assignmentHeaderRect.width / 24f;

            var widgetLabelRect = new Rect(assignmentHeaderRect.x + 5.0f, assignmentHeaderRect.y, widgetLabelWidth, assignmentHeaderRect.height);
            for (int hour = 0; hour < 24; ++hour)
            {
                Widgets.Label(widgetLabelRect, hour.ToString());
                widgetLabelRect.x += widgetLabelWidth;
            }

            var assignmentRect = new Rect(0.0f, 110.0f, listingStandard.ColumnWidth, 30f);
            float widgetWidth = assignmentRect.width / 24f;
            var widgetRect = new Rect(assignmentRect.x, assignmentRect.y, widgetWidth, assignmentRect.height);
            for (int hour = 0; hour < 24; ++hour)
            {
                this.DoAssignmentCell(widgetRect, hour);
                widgetRect.x += widgetWidth;
            }
        }

        private void DoAssignmentCell(Rect widgetRect, int hour)
        {
            var selectedAssignment = TimeAssignmentSelector.selectedAssignment;
            var settingsAssignment = settings.Assignments[hour];
            var currentAssignmentDef = DefDatabase<TimeAssignmentDef>.GetNamed(settingsAssignment);
            GUI.DrawTexture(widgetRect, currentAssignmentDef.ColorTexture);
            GUI.DrawTexture(new Rect(widgetRect.x + widgetRect.width - 2f, widgetRect.y, 2f, widgetRect.height), BaseContent.BlackTex);

            bool mouseButton = Input.GetMouseButton(0);

            if (!mouseButton)
                MouseoverSounds.DoRegion(widgetRect);
            if (!Mouse.IsOver(widgetRect))
                return;

            Widgets.DrawBox(widgetRect, 2);

            if (!mouseButton || currentAssignmentDef == selectedAssignment || selectedAssignment == null)
                return;

            SoundDefOf.Designate_DragStandard_Changed_NoCam.PlayOneShotOnCamera();
            settings.Assignments[hour] = selectedAssignment.defName;
        }

        private void StartQuickstartGame()
        {
            LongEventHandler.QueueLongEvent(delegate
            {
                Current.ProgramState = ProgramState.Entry;
                Current.Game = new Game();
                Current.Game.InitData = new GameInitData();

                this.SetGameScenario();
                this.SetGameWorld();

                Find.Scenario.PostIdeoChosen();

                this.SetGameInitData();
                Find.Scenario.PreMapGenerate();
            }, "Play", "GeneratingMap", doAsynchronously: true, null);
        }

        private void SetGameScenario()
        {
            var scenarioDef = DefDatabase<ScenarioDef>.GetNamed(settings.DefaultScenario);
            var storytellerDef = DefDatabase<StorytellerDef>.GetNamed(settings.DefaultStoryteller);
            var difficultyDef = DefDatabase<DifficultyDef>.GetNamed(settings.DefaultDifficulty);

            Current.Game.Scenario = scenarioDef.scenario;
            Find.Scenario.PreConfigure();
            Current.Game.storyteller = new Storyteller(storytellerDef, difficultyDef);
        }

        private void SetGameWorld()
        {
            Current.Game.World = WorldGenerator.GenerateWorld(0.3f, GenText.RandomSeedString(), OverallRainfall.Normal, OverallTemperature.Normal, OverallPopulation.Normal, LandmarkDensity.Normal);
        }

        private void SetGameInitData()
        {
            if (settings.CommitmentMode)
            {
                Find.GameInitData.permadeath = true;
                Find.GameInitData.permadeathChosen = true;
            }
            Find.GameInitData.ChooseRandomStartingTile();
            Find.GameInitData.PrepForMapGen();

            this.SetGamePlaySettings();
            this.SetPawnSettings();
        }

        private void SetGamePlaySettings()
        {
            Current.Game.playSettings.useWorkPriorities = settings.SetPriorities;
        }

        private void SetPawnSettings()
        {
            foreach (var pawn in Find.GameInitData.startingAndOptionalPawns)
            {
                pawn.playerSettings.selfTend = settings.SelfTend;

                foreach (WorkTypeDef w in DefDatabase<WorkTypeDef>.AllDefs)
                {
                    this.SetPawnWorkPriority(pawn, w);
                    this.SetPawnAssignments(pawn);
                }
            }
        }

        private void SetPawnWorkPriority(Pawn pawn, WorkTypeDef workType)
        {
            if (!settings.SetPriorities || pawn.WorkTypeIsDisabled(workType))
            {
                return;
            }

            var settingsPriority = 3;

            switch (workType.defName)
            {
                case "Firefighter":
                    settingsPriority = settings.FirefightPriority;
                    break;
                case "Patient":
                    settingsPriority = settings.PatientPriority;
                    break;
                case "Doctor":
                    settingsPriority = settings.DoctorPriority;
                    break;
                case "PatientBedRest":
                    settingsPriority = settings.BedRestPriority;
                    break;
                case "BasicWorker":
                    settingsPriority = settings.BasicPriority;
                    break;
                case "Warden":
                    settingsPriority = settings.WardenPriority;
                    break;
                case "Handling":
                    settingsPriority = settings.HandlePriority;
                    break;
                case "Cooking":
                    settingsPriority = settings.CookPrority;
                    break;
                case "Hunting":
                    settingsPriority = settings.HuntPriority;
                    break;
                case "Construction":
                    settingsPriority = settings.ConstructPriority;
                    break;
                case "Growing":
                    settingsPriority = settings.GrowPriority;
                    break;
                case "Mining":
                    settingsPriority = settings.MinePriority;
                    break;
                case "PlantCutting":
                    settingsPriority = settings.PlantCutPriority;
                    break;
                case "Smithing":
                    settingsPriority = settings.SmithPriority;
                    break;
                case "Tailoring":
                    settingsPriority = settings.TailorPriority;
                    break;
                case "Art":
                    settingsPriority = settings.ArtPriority;
                    break;
                case "Crafting":
                    settingsPriority = settings.CraftPriority;
                    break;
                case "Hauling":
                    settingsPriority = settings.HaulPriority;
                    break;
                case "Cleaning":
                    settingsPriority = settings.CleanPriority;
                    break;
                case "Research":
                    settingsPriority = settings.ResearchPriority;
                    break;
            }

            pawn.workSettings.SetPriority(workType, settingsPriority);
        }

        private void SetPawnAssignments(Pawn pawn)
        {
            if (!settings.SetAssignments)
            {
                return;
            }

            for (int i = 0; i < 24; i++)
            {
                var assignmentDef = DefDatabase<TimeAssignmentDef>.GetNamed(settings.Assignments[i]);
                if (assignmentDef != null)
                {
                    pawn.timetable.SetAssignment(i, assignmentDef);
                }
            }
        }
    }
}