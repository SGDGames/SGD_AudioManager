# SGD_AudioManager
Unity3D / 2021.1.11f1 / Windows10

 AudioManager
 

- Addressables를 사용하는 간단한 오디오매니저입니다.
- A simple audio manager that uses Addressables.
 <br/>


- PoolAudio(string key, AudioClip clip = null, float volume = 1, bool mute = false, bool loop = false, int multiChannel = 1)
- key - 고유 키
- clip - 클립, null일경우 Addressables에서 key값을 로드함/If null, the key value is loaded from Addressables.
- volume - 볼륨
- mute - 뮤트
- loop - 루프
- multiChanel - 최대 동시 재생수
 <br/>
 
- PlayAudio(string key, float volume = 1, bool loop = false)
- key - 고유 키 
- volume - 볼륨
- loop - 루프

 

