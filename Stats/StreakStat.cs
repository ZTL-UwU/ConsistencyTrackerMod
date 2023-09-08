﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.ConsistencyTracker.Enums;
using Celeste.Mod.ConsistencyTracker.Models;

namespace Celeste.Mod.ConsistencyTracker.Stats {

    /*
     Stats to implement:
     {room:successRate} - Success Rate of the current room
     {checkpoint:successRate} - Average Success Rate of current checkpoint
     {chapter:successRate} - Average Success Rate of the entire chapter

         */
    
    public class StreakStat : Stat {
        public static string RoomCurrentStreak = "{room:currentStreak}";
        public static string RoomCurrentStreakBest = "{room:currentStreakBest}";
        public static string CheckpointCurrentStreak = "{checkpoint:currentStreak}";
        public static string ListRoomStreaks = "{list:roomStreaks}";

        public static List<string> IDs = new List<string>() { RoomCurrentStreak, RoomCurrentStreakBest, CheckpointCurrentStreak, ListRoomStreaks };

        public StreakStat() : base(IDs) {}

        public override string FormatStat(PathInfo chapterPath, ChapterStats chapterStats, string format) {
            if (chapterPath == null) { //Player doesnt have path
                format = StatManager.MissingPathFormat(format, RoomCurrentStreak);
                format = StatManager.MissingPathFormat(format, RoomCurrentStreakBest);
                format = StatManager.MissingPathFormat(format, CheckpointCurrentStreak);
                format = StatManager.MissingPathFormat(format, ListRoomStreaks);
                return format;
            } else if (chapterPath.CurrentRoom == null) { //or is not on the path
                format = StatManager.NotOnPathFormat(format, RoomCurrentStreak);
                format = StatManager.NotOnPathFormat(format, RoomCurrentStreakBest);
                format = StatManager.NotOnPathFormat(format, CheckpointCurrentStreak);
                format = StatManager.NotOnPathFormat(format, ListRoomStreaks);
                return format;
            }

            int streak = chapterStats.GetRoom(chapterPath.CurrentRoom.DebugRoomName).SuccessStreak;
            int bestStreak = chapterStats.GetRoom(chapterPath.CurrentRoom.DebugRoomName).SuccessStreakBest;

            List<int> roomStreaks = new List<int>();
            //Checkpoint
            int cpLowestStreak = 100;
            foreach (CheckpointInfo cpInfo in chapterPath.Checkpoints) {
                int cpLowestCheck = 100;
                bool isInCp = false;

                foreach (RoomInfo rInfo in cpInfo.Rooms) {
                    RoomStats rStats = chapterStats.GetRoom(rInfo.DebugRoomName);
                    int rStreak = rStats.SuccessStreak;

                    roomStreaks.Add(rStreak);

                    if (rStreak < cpLowestStreak) {
                        cpLowestCheck = rStreak;
                    }
                    if (rInfo.DebugRoomName == chapterPath.CurrentRoom.DebugRoomName) {
                        isInCp = true;
                    }
                }

                if (isInCp) {
                    cpLowestStreak = cpLowestCheck;
                }
            }


            format = format.Replace(RoomCurrentStreak, $"{streak}");
            format = format.Replace(RoomCurrentStreakBest, $"{bestStreak}");
            format = format.Replace(CheckpointCurrentStreak, $"{cpLowestStreak}");


            if (StatManager.ListOutputFormat == ListFormat.Plain) {
                string output = string.Join(", ", roomStreaks);
                format = format.Replace(ListRoomStreaks, $"{output}");

            } else if (StatManager.ListOutputFormat == ListFormat.Json) {
                string output = string.Join(", ", roomStreaks);
                format = format.Replace(ListRoomStreaks, $"[{output}]");
            }

            return format;
        }

        public override string FormatSummary(PathInfo chapterPath, ChapterStats chapterStats) {
            return null;
        }


        //success-rate;Room SR: {room:successRate} | CP: {checkpoint:successRate} | Total: {chapter:successRate}
        public override List<KeyValuePair<string, string>> GetPlaceholderExplanations() {
            return new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(RoomCurrentStreak, Dialog.Clean("CCT_STAT_STREAK_EXPLANATIONS_ROOM_CURRENT_STREAK")),
                new KeyValuePair<string, string>(RoomCurrentStreakBest, Dialog.Clean("CCT_STAT_STREAK_EXPLANATIONS_ROOM_CURRENT_STREAK_BEST")),
                new KeyValuePair<string, string>(CheckpointCurrentStreak, Dialog.Clean("CCT_STAT_STREAK_EXPLANATIONS_CHECKPOINT_CURRENT_STREAK")),
            };
        }
        public override List<StatFormat> GetDefaultFormats() {
            return new List<StatFormat>() {
                new StatFormat(Dialog.Clean("CCT_STAT_STREAK_FORMAT_TITLE_BASIC_CURRENT_STREAK"), $"{Dialog.Clean("CCT_STAT_STREAK_FORMAT_CONTENT_BASIC_CURRENT_STREAK_1")}: {RoomCurrentStreak}, {Dialog.Clean("CCT_STAT_STREAK_FORMAT_CONTENT_BASIC_CURRENT_STREAK_2")}: {CheckpointCurrentStreak}")
            };
        }
    }
}
