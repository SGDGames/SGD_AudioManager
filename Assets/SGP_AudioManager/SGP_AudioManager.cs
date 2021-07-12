using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SGP_Util
{


    public class SGP_AudioManager : MonoBehaviour
    {
        public static SGP_AudioManager i;

        // Start is called before the first frame update
        private GameObject _tempObject;

        private Dictionary<string, AudioData> _audioPool = new Dictionary<string, AudioData>();

        private void Awake()
        {
            if (i != null)
                Destroy(gameObject);
            i = this;
            _tempObject = new GameObject();
            _tempObject.transform.SetParent(transform);
        }

        public void PoolAudio(string key, AudioClip clip = null, float volume = 1, bool mute = false, bool loop = false,
            int multiChannel = 1)
        {
            if (_audioPool.ContainsKey(key))
            {
                Debug.Log("[SGP_AudioManagerLog] " + key + " was the key prepared.");
                return;
            }

            var localclip = clip;
            if (localclip == null)
            {
                var load = Addressables.LoadAssetAsync<AudioClip>(key);
                load.Completed += (audio) =>
                {
                    _audioPool[key] =
                        new AudioData(key, _tempObject, load.Result, volume, mute, loop, multiChannel);
                };

            }
        }

        public bool Dispose(string key)
        {
            if (_audioPool.ContainsKey(key))
            {
                _audioPool[key].Dispose();
                _audioPool.Remove(key);
                return true;
            }

            return false;

        }

/* ResourceLoad.... now using
 public void PoolAudio(string key,AudioClip clip = null,float volume = 1,bool mute = false,bool loop = false,int multiChannel = 1)
{
    if (_audioPool.ContainsKey(key))
    {
        Debug.Log("[SGP_AudioManagerLog] " +key +" was the key prepared.");
        return;
    }

    var localclip = clip;
    if (localclip == null)
    {
        var load = Resources.Load<AudioClip>(key);
        
            _audioPool[key] = new AudioData(key,_tempObject,load,volume,mute,loop,multiChannel);

    }
}
    public PoolStatus PoolAudio(string key,AudioClip clip = null,float volume = 1,bool mute = false,bool loop = false,int multiChannel = 1)
    {
        if (_audioPool.ContainsKey(key))
        {
            Debug.Log("[SGP_AudioManagerLog] " +key +" was the key prepared.");
            return PoolStatus.Prepared;
        }

        var localclip = clip;
        if (localclip == null)
        {
            localclip = Resources.Load(key) as AudioClip;
        }

        if (localclip == null)
        {
            Debug.Log("[SGP_AudioManagerLog] " +key +"s clip is null.");
            return PoolStatus.ClipIsNull;
        }

        _audioPool[key] = new AudioData(key,_tempObject,localclip,volume,mute,loop,multiChannel);
        return PoolStatus.Success;
    }

    public bool LoadAndPlayAudio(string key,AudioClip clip = null,float volume = 1,bool mute = false,bool loop = false,int multiChannel = 1)
    {
        if (PoolAudio(key, clip, volume, mute, loop, multiChannel) == PoolStatus.ClipIsNull)
        {
            return false;
        }

        return PlayAudio(key,volume,loop);
       
    }
    */
        public bool PlayAudio(string key, float volume = 1, bool loop = false)
        {
            AudioData audioData;
            if (!_audioPool.TryGetValue(key, out audioData))
            {
                Debug.Log("[SGP_AudioManagerLog] " + key + " is not loaded.");
                return false;
            }

            audioData.Volume = volume;
            audioData.Loop = loop;
            audioData.Play();
            return true;
        }

        /* 당장 필요 없다. 필요 시 구현
        public void AddStartEvent(string key,Action callback,bool onlyone = true)
        {
            
        }
        public void AddPrograssEvent(string key,Action<float> callback,bool onlyonecycle = true)
        {
            
        }
        public void AddEndEvent(string key, Action callback,bool onlyone = true)
        {
            
        }
        */
    }

    public class AudioData
    {
        public AudioData()
        {
        }

        public AudioData(string key, GameObject temp, AudioClip clip, float volume = 1, bool mute = false,
            bool loop = false, int multiChannel = 1)
        {
            Key = key;

            MultiChannel = multiChannel;
            SourceInit(temp, clip);
            Volume = volume;
            Mute = mute;
            Loop = loop;
        }

        private void SourceInit(GameObject temp, AudioClip clip)
        {
            _source = new AudioSourceData[MultiChannel];
            for (var i = 0; i < _source.Length; i++)
            {
                _source[i] = new AudioSourceData(temp.AddComponent<AudioSource>(), this);
                _source[i].playOnAwake = false;
                _source[i].clip = clip;
            }
        }

        private int _multiIndex = 0;

        public void Dispose()
        {
            for (var i = 0; i < _source.Length; i++)
            {
                _source[i].Dispose();

            }
        }

        public void Play()
        {
            _source[_multiIndex].Play();
            _multiIndex++;
            if (_multiIndex >= MultiChannel) _multiIndex = 0;
        }

        public void EndEvent()
        {
            if (Loop)
                Play();
            else
                Stop();
        }


        public void Stop()
        {
            foreach (var _s in _source) _s.Stop();
        }


        public int MultiChannel = 1;
        public string Key;
        private AudioSourceData[] _source;

        public AudioSourceData GetAudioSourceData(int i)
        {
            if (i >= MultiChannel)
                return null;
            return _source[i];
        }

        private float _volume = 1;

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                foreach (var _s in _source) _s.volume = _s.volume * _volume;
            }
        }

        public bool _mute = false;

        public bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                foreach (var _s in _source) _s.mute = _mute;
            }
        }

        public bool _loop = false;

        public bool Loop
        {
            get => _loop;
            set => _loop = value;
        }
    }

    public class AudioSourceData
    {
        public AudioSourceData()
        {
        }

        private AudioData _audioData;

        public AudioSourceData(AudioSource source, AudioData audioData)
        {
            _source = source;
            _audioData = audioData;
        }

        private AudioSource _source;

        public void Dispose()
        {
            GameObject.Destroy(_source.clip);
            GameObject.Destroy(_source);
        }

        public void Play()
        {
            if (Status != AudioSourceStatus.Null)
            {
                Status = AudioSourceStatus.Play;
                _source.Play();
            }
        }

        public void Stop()
        {
            if (Status != AudioSourceStatus.Null)
            {
                Status = AudioSourceStatus.Ready;
                _source.Stop();
            }
        }

        public float volume
        {
            get => _source.volume;
            set => _source.volume = value;
        }

        public bool mute
        {
            get => _source.mute;
            set => _source.mute = value;
        }

        public bool loop
        {
            get => _source.loop;
            set => _source.loop = value;
        }

        public bool playOnAwake
        {
            get => _source.playOnAwake;
            set => _source.playOnAwake = value;
        }

        public AudioClip clip
        {
            get => _source.clip;
            set
            {
                _source.clip = value;
                if (value != null)
                {
                    _lenght = value.length;
                    Status = AudioSourceStatus.Ready;
                }
                else
                {
                    Status = AudioSourceStatus.Null;
                }
            }
        }

        private float _lenght;
        public float Prograss => _source.time / _lenght;

        public AudioSourceStatus Status;

        public enum AudioSourceStatus
        {
            Null,
            Ready,
            Play
        }
    }
}