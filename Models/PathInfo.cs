﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.ConsistencyTracker.Models {

    [Serializable]
    public class PathInfo {

        public List<CheckpointInfo> Checkpoints { get; set; } = new List<CheckpointInfo>();
        public AggregateStats Stats { get; set; } = null;
        public string ParseError { get; set; }

        public static PathInfo GetTestPathInfo() {
            return new PathInfo() {
                Checkpoints = new List<CheckpointInfo>() {
                    new CheckpointInfo(){ Name="Start", Abbreviation="0M" },
                    new CheckpointInfo(){ Name="500 M", Abbreviation="500M" },
                },
            };
        }

        public override string ToString() {
            List<string> lines = new List<string>();

            foreach (CheckpointInfo cpInfo in Checkpoints) {
                lines.Add(cpInfo.ToString());
            }

            return string.Join("\n", lines);
        }

        public static PathInfo ParseString(string content, Action<string> logCallback) {
            logCallback($"[PathInfo.ParseString] Parsing path info string");
            List<string> lines = content.Trim().Split(new string[] { "\n" }, StringSplitOptions.None).ToList();

            PathInfo pathInfo = new PathInfo();

            foreach (string line in lines) {
                logCallback($"\tParsing line '{line}'");
                pathInfo.Checkpoints.Add(CheckpointInfo.ParseString(line));
            }

            return pathInfo;
        }
    }

    [Serializable]
    public class CheckpointInfo {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int RoomCount {
            get => Rooms.Count;
            private set {
            }
        }
        public List<RoomInfo> Rooms { get; set; } = new List<RoomInfo>();

        public AggregateStats Stats { get; set; } = null;
        public double GoldenChance { get; set; } = 1;

        public override string ToString() {
            string toRet = $"{Name};{Abbreviation};{Rooms.Count}";
            string debugNames = string.Join(",", Rooms);
            return $"{toRet};{debugNames}";
        }

        public static CheckpointInfo ParseString(string line) {
            List<string> parts = line.Trim().Split(new string[] { ";" }, StringSplitOptions.None).ToList();
            string name = parts[0];
            string abbreviation = parts[1];

            List<string> rooms = parts[3].Split(new string[] { "," }, StringSplitOptions.None).ToList();
            List<RoomInfo> roomInfo = new List<RoomInfo>();

            foreach (string room in rooms) {
                roomInfo.Add(new RoomInfo() { DebugRoomName = room });
            }

            return new CheckpointInfo() {
                Name = name,
                Abbreviation = abbreviation,
                Rooms = roomInfo,
            };
        }
    }

    [Serializable]
    public class RoomInfo {
        public string DebugRoomName { get; set; }
        public override string ToString() {
            return DebugRoomName;
        }

    }



    public class AggregateStats {
        public int CountSuccesses { get; set; } = 0;
        public int CountAttempts { get; set; } = 0;
        public int CountFailures {
            get {
                return CountAttempts - CountSuccesses;
            }
        }
        public float SuccessRate {
            get {
                if (CountAttempts == 0) return 0;

                return (float)CountSuccesses / CountAttempts;
            }
        }

        public int CountGoldenBerryDeaths { get; set; } = 0;
        public int CountGoldenBerryDeathsSession { get; set; } = 0;

        public float GoldenChance { get; set; } = 1;
    }
}
