#if UNIRX_SUPPORT
using System;
using System.Collections.Generic;
using MoraeGames.Library.Manager.ResourceFactory;
using UniRx;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace MoraeGames.Library.Manager.Sound
{
    [Serializable]
    public class SoundClip
    {
        [SerializeField] public AudioClip audioClip;
        [SerializeField, Range(0, 1)] public float defaultVolume = 0.5f;
    }

    public class SoundManager : IStartable
    {
        #region Fields
        
        readonly string KEY_BGM = "BGM";
        readonly string KEY_SFX = "SFX";
        
        AudioSource audioSourceForBgm;
        List<AudioSource> audioSourcesForSfx = new();
        bool isMuteBGM;
        float volumeBGM;
        bool isMuteEffect;
        float volumeEffect;
        GameObject audioPlayer;
        Dictionary<string, AudioClip> CachedClips = new();

        #endregion

        #region Properties

#if ADDRESSABLE_SUPPORT
        public bool UseAddressable { get; set; } = false;
#endif 

        public bool IsMuteBGM
        {
            get => isMuteBGM;
            set
            {
                isMuteBGM = value;

                audioSourceForBgm.mute = isMuteBGM;
            }
        }

        public FloatReactiveProperty VolumeBGM { get; } = new(1);

        public bool IsMuteEffect
        {
            get => isMuteEffect;
            set
            {
                isMuteEffect = value;

                foreach (var audioSourceForEffect in audioSourcesForSfx)
                {
                    audioSourceForEffect.mute = isMuteEffect;
                }
            }
        }

        public FloatReactiveProperty VolumeEffect { get; } = new(1);

        public AudioClip CurrentBGM { get; private set; }

        #endregion

        #region Entry Point

        void IStartable.Start()
        {
            VolumeBGM.Value = PlayerPrefs.HasKey(KEY_BGM) ? PlayerPrefs.GetFloat(KEY_BGM) : 1f;
            VolumeEffect.Value = PlayerPrefs.HasKey(KEY_SFX) ? PlayerPrefs.GetFloat(KEY_SFX) : 1f;
            VolumeBGM.Subscribe(value => PlayerPrefs.SetFloat(KEY_BGM, value));
            VolumeEffect.Subscribe(value => PlayerPrefs.SetFloat(KEY_SFX, value));
            Observable.Merge(VolumeBGM.AsUnitObservable(), VolumeEffect.AsUnitObservable()).Subscribe(_ => PlayerPrefs.Save());
            
            audioPlayer = new GameObject("AudioPlayer");
            Object.DontDestroyOnLoad(audioPlayer);
            AddAudioSourceForBgm();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// BGM을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 사운드 정보</param>
        /// <param name="volume">볼륨(0일 시 기본 값)</param>
        public void PlayBgm(string clipKey)
        {
            if (!CachedClips.ContainsKey(clipKey))
            {
#if ADDRESSABLE_SUPPORT
                if (UseAddressable)
                    CachedClips[clipKey] = ResourceFactory<AudioClip>.Builder.ByAddressable.Build(clipKey).Load();
                else
#endif
                CachedClips[clipKey] = ResourceFactory<AudioClip>.Builder.Build(clipKey).Load();
            }
            var clip = CachedClips[clipKey];
            
            if (CurrentBGM == clip) return;

            CurrentBGM = clip;
            audioSourceForBgm.mute = isMuteBGM;
            PlaySound(audioSourceForBgm, clip, VolumeBGM.Value);
        }

        /// <summary>
        /// 효과음을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 사운드 정보</param>
        /// <param name="volume">볼륨(0일 시 기본 값)</param>
        public void PlaySfx(string clipKey, float volume = 0.5f)
        {
            if (!CachedClips.ContainsKey(clipKey))
            {
#if ADDRESSABLE_SUPPORT
                if (UseAddressable)
                    CachedClips[clipKey] = ResourceFactory<AudioClip>.Builder.ByAddressable.Build(clipKey).Load();
                else
#endif
                CachedClips[clipKey] = ResourceFactory<AudioClip>.Builder.Build(clipKey).Load();
            }
            var clip = CachedClips[clipKey];
            
            if (isMuteEffect) return;

            for (int i = 0; i < audioSourcesForSfx.Count; i++)
            {
                if (!audioSourcesForSfx[i].isPlaying)
                {
                    PlaySound(audioSourcesForSfx[i], clip, volume * VolumeEffect.Value);
                    return;
                }
            }

            var newAudioSource = AddAudioSourceForSfx();
            PlaySound(newAudioSource, clip, volume * VolumeEffect.Value);
        }

        #endregion

        #region Private Methods

        void AddAudioSourceForBgm()
        {
            audioSourceForBgm = audioPlayer.AddComponent<AudioSource>();
            audioSourceForBgm.loop = true;
            audioSourceForBgm.playOnAwake = false;
            VolumeBGM.Subscribe(volume => audioSourceForBgm.volume = volume).AddTo(audioSourceForBgm);
        }
    
        AudioSource AddAudioSourceForSfx()
        {
            var audioSource = audioPlayer.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSourcesForSfx.Add(audioSource);
            return audioSource;
        }

        void PlaySound(AudioSource audioSource, AudioClip clip, float volume)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }

        #endregion
    }
}
#endif