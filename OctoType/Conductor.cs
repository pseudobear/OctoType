﻿using System;
using OctoType.Audio;
using OctoType.Utils;
namespace OctoType.Audio {
    public class Conductor {

        /// <summary>
        /// The main variable for keeping track of the location inside of a song
        /// ***EVERYTHING*** should be using this variable to keep time
        /// </summary>
        public float SongTime { private set; get; }
        public float AdjustedSongTime { private set; get; }
        public float Bpm { private set; get; }

        /// <summary>
        /// The amount of milliseconds before the song actually starts playing
        /// after opening the file
        /// </summary>
        public float Offset { set; private get; }
        public bool IsPlaying { private set; get; }

        private float lastReportedPlayheadPosition;
        private float previousFrameTime;
        private int barNum;
        private int songPlaying;
        private int adjustmentFactor = 10;

        private AudioManager _audio;

        public Conductor(float bpm, float offset, ref AudioManager audioManager) {
            barNum = 0;
            SongTime = 0;
            Bpm = bpm;
            Offset = offset;
            _audio = audioManager;
        }

        public void StartSong(string name) {
            if(IsPlaying) return;
            IsPlaying = true;
            lastReportedPlayheadPosition = 0;
            songPlaying = _audio.PlaySong(name);
            previousFrameTime = TimeUtils.Timer;
        }
        
        // call this every frame
        public void UpdateSongTime() {
            SongTime += TimeUtils.Timer - previousFrameTime;
            previousFrameTime = TimeUtils.Timer;
            if(Math.Abs(_audio.GetSongPosition(songPlaying) - lastReportedPlayheadPosition) > 1) {
                lastReportedPlayheadPosition = _audio.GetSongPosition(songPlaying);
                Console.Write("difference is now " + (lastReportedPlayheadPosition - (SongTime - Offset)));
                Console.WriteLine(" UPDATED PLAYHEAD POSITION TO " +lastReportedPlayheadPosition);

                // averaging SongTime and playhead position for correction
                SongTime = (SongTime + adjustmentFactor * lastReportedPlayheadPosition) / (adjustmentFactor + 1);
                Console.WriteLine("Corrected for difference: " + SongTime);
                Console.WriteLine("difference is now " + (lastReportedPlayheadPosition - (SongTime - Offset)));
            }
            AdjustedSongTime = SongTime - Offset;
        }
    }
}