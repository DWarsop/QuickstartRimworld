using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;

namespace Quickstart
{
    public class QuickstartModSettings : ModSettings
    {
        public string DefaultScenario;
        public string DefaultStoryteller;
        public string DefaultDifficulty;
        public bool CommitmentMode;

        public bool SelfTend;

        public bool SetPriorities;
        public int FirefightPriority;
        public int PatientPriority;
        public int DoctorPriority;
        public int BedRestPriority;
        public int BasicPriority;
        public int WardenPriority;
        public int HandlePriority;
        public int CookPrority;
        public int HuntPriority;
        public int ConstructPriority;
        public int GrowPriority;
        public int MinePriority;
        public int PlantCutPriority;
        public int SmithPriority;
        public int TailorPriority;
        public int ArtPriority;
        public int CraftPriority;
        public int HaulPriority;
        public int CleanPriority;
        public int ResearchPriority;

        public bool SetAssignments;
        public List<string> Assignments = new List<string>(24);

        public override void ExposeData()
        {
            Scribe_Values.Look<string>(ref DefaultScenario, "defaultScenario", "Crashlanded");
            Scribe_Values.Look<string>(ref DefaultStoryteller, "defaultStoryteller", "Cassandra");
            Scribe_Values.Look<string>(ref DefaultDifficulty, "defaultDifficulty", "Rough");
            Scribe_Values.Look<bool>(ref CommitmentMode, "commitmentMode", true);

            Scribe_Values.Look<bool>(ref SelfTend, "selfTend", true);

            Scribe_Values.Look<bool>(ref SetPriorities, "setPriorities", false);
            Scribe_Values.Look<int>(ref FirefightPriority, "firefightPriority", 1);
            Scribe_Values.Look<int>(ref PatientPriority, "patientPriority", 1);
            Scribe_Values.Look<int>(ref DoctorPriority, "doctorPriority", 1);
            Scribe_Values.Look<int>(ref BedRestPriority, "bedrestPriority", 2);
            Scribe_Values.Look<int>(ref BasicPriority, "basicPriority", 3);
            Scribe_Values.Look<int>(ref WardenPriority, "wardenPriority", 3);
            Scribe_Values.Look<int>(ref HandlePriority, "handlePriority", 3);
            Scribe_Values.Look<int>(ref CookPrority, "cookPriority", 2);
            Scribe_Values.Look<int>(ref HuntPriority, "huntPriority", 3);
            Scribe_Values.Look<int>(ref ConstructPriority, "constructPriority", 2);
            Scribe_Values.Look<int>(ref GrowPriority, "growPriority", 2);
            Scribe_Values.Look<int>(ref MinePriority, "minePriority", 2);
            Scribe_Values.Look<int>(ref PlantCutPriority, "plantcutPriority", 2);
            Scribe_Values.Look<int>(ref SmithPriority, "smithPriority", 3);
            Scribe_Values.Look<int>(ref TailorPriority, "tailorPriority", 3);
            Scribe_Values.Look<int>(ref ArtPriority, "artPriority", 3);
            Scribe_Values.Look<int>(ref CraftPriority, "craftPriority", 3);
            Scribe_Values.Look<int>(ref HaulPriority, "haulPriority", 1);
            Scribe_Values.Look<int>(ref CleanPriority, "cleanPriority", 1);
            Scribe_Values.Look<int>(ref ResearchPriority, "researchPriority", 3);

            Scribe_Values.Look<bool>(ref SetAssignments, "setAssignments", false);
            Scribe_Collections.Look<string>(ref this.Assignments, "assignments", LookMode.Undefined);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.Assignments == null)
            {
                this.Assignments = new List<string>(24);
                for (int index = 0; index < 24; index++)
                {
                    this.Assignments.Add("Anything");
                }
            }

            base.ExposeData();
        }
    }
}