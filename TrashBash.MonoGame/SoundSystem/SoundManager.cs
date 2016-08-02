using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace TrashBash.MonoGame.SoundSystem
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundEffect> sounds =
            new Dictionary<string, SoundEffect>();

        private static Song currentSong;

        private static float soundVolume = 1f;

        private static ContentManager content;

        private static AudioEngine audioEngine;

        private static WaveBank waveBank;

        private static SoundBank soundBank;

        public static int soundsPlaying = 0;

        public static void Initialize(ContentManager cman)
        {
            content = cman;
            audioEngine = new AudioEngine("Content\\SoundEffects\\trashbash.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\SoundEffects\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\SoundEffects\\Sound Bank.xsb");
        }

        public static Cue PlaySound(string name)
        {
            Cue cue = soundBank.GetCue(name);
            cue.Play();
            soundsPlaying++;
            return cue;
        }

        public static void StopSound(Cue cue)
        {
            cue.Stop(AudioStopOptions.Immediate);
            soundsPlaying--;
        }

        public static void PlayRandomSong()
        {
            MediaPlayer.Stop();
            MediaLibrary ml = new MediaLibrary();
            SongCollection songs = ml.Songs;
            Random rand = new Random();
            Song songToPlay = songs[rand.Next(songs.Count - 1)];
            while (songToPlay.IsProtected)
            {
                songToPlay = songs[rand.Next(songs.Count)];
            }
            MediaPlayer.Play(songs[rand.Next(songs.Count)]);
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        public static void PlayMusic(string name)
        {
            currentSong = null;
            try
            {
                currentSong = content.Load<Song>("Content/Music/" + name);
            }
            catch (Exception e)
            {
                if (currentSong == null)
                    throw e;
            }
            MediaPlayer.Play(currentSong);
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                PlayRandomSong();
            }
        }

        public static void AddSound(string assetName)
        {
            sounds.Add(assetName, content.Load<SoundEffect>("Content/SoundEffects/" + assetName));
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }

        public static void SetSoundFxVolume(float volume)
        {
            soundVolume = volume;
        }

        public static void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        public static void Update()
        {
            audioEngine.Update();
        }
    }
}
