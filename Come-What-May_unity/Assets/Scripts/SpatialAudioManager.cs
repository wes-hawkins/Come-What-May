using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpatialAudioManager : MonoBehaviour {

	public static SpatialAudioManager Inst = null;

	public static List<SpatialAudioSource> allSpatialSources = new List<SpatialAudioSource>();
	private Vector3 listenerPos = Vector3.zero; public Vector3 ListenerPos { get { return listenerPos; } }

	// A sound effect with clipVolume '1' will play at this level.
	// This allows for mastering levels of audio when adding a sound effect that is quiet, even at 'full volume.'
	// That quiet clip can now have a clip volume greater than 1.
	private float standardLevelCoefficient = 0.4f; public float StandardLevelCoefficient { get { return standardLevelCoefficient; } }

	public void Awake() {
		Inst = this;
	} // End of Awake().
	

	// Plays a one-shot audio clip without spatial mapping (UI, etc.)
	public static SpatialAudioSource PlayUIClip(AudioClip clip, float volume = 1f){
		return GenerateAudio(new SpatialAudioClip(clip, volume), Vector3.zero, volume, null, false, SoundSpace.ui);
	}
	// Plays a one-shot audio clip in the 'soundtrack' soundspace.
	public static SpatialAudioSource PlayMusicClip(AudioClip clip){
		return GenerateAudio(new SpatialAudioClip(clip), Vector3.zero, 1f, null, false, SoundSpace.soundtrack);
	}
	// Plays a one-shot audio clip at a point in space.
	public static SpatialAudioSource PlayClipAtPoint(SpatialAudioClip spatialClip, Vector3 point, float volume = 1f){
		return GenerateAudio(spatialClip, point, volume, null, false, SoundSpace.world);
	}
	// Attaches a one-shot clip to a transform.
	public static SpatialAudioSource PlayClipOnTransform(SpatialAudioClip spatialClip, Transform sourceTrans, float volume = 1f){
		return GenerateAudio(spatialClip, Vector3.zero, volume, sourceTrans, false, SoundSpace.world);
	}
	// Attaches a looping clip to a transform--does not play by default.
	public static SpatialAudioSource AttachClipToTransform(SpatialAudioClip spatialClip, Transform sourceTrans, float volume = 1f){
		return GenerateAudio(spatialClip, Vector3.zero, volume, sourceTrans, true, SoundSpace.world);
	}
	// Instantiates the sound in the game.
	public static SpatialAudioSource GenerateAudio(SpatialAudioClip spatialClip, Vector3 point, float volume = 1f, Transform sourceTrans = null, bool loop = false, SoundSpace soundSpace = SoundSpace.world){
		GameObject newSoundObject = new GameObject("_sound!", typeof(AudioSource)/*, typeof(AudioLowPassFilter)*/);

		SpatialAudioSource newSpatialSource = new SpatialAudioSource();
		allSpatialSources.Add(newSpatialSource);
		newSpatialSource.source = newSoundObject.GetComponent<AudioSource>();
		//newSpatialSource.lowPass = newSoundObject.GetComponent<AudioLowPassFilter>();
		newSpatialSource.audioGObject = newSoundObject;
		newSpatialSource.spatialClip = spatialClip;
		newSpatialSource.loop = loop;
		newSpatialSource.boundToTransform = loop;
		newSpatialSource.point = point;
		newSpatialSource.sourceTransform = sourceTrans;
		newSpatialSource.soundSpace = soundSpace;
		newSpatialSource.source.clip = spatialClip.clip;
		newSpatialSource.volume = volume;

		newSpatialSource.pitch = Mathf.Pow(2f, Random.Range(-spatialClip.pitchModulation, spatialClip.pitchModulation));
		
		if(soundSpace == SoundSpace.ui){
			newSpatialSource.source.bypassListenerEffects = true;
			newSpatialSource.source.bypassEffects = true;
		}

		//if(loop)
			//newSpatialSource.source.time = spatialClip.clip.length;

		newSpatialSource.Update();

		return newSpatialSource;
	} // End of PlayClipAtPoint().


	void Update(){
		// Run Update() on all of the spatial audio sources (as they are not monobehaviours).
		for(int i = 0; i < allSpatialSources.Count; i++){
			SpatialAudioSource curSource = allSpatialSources[i];
			curSource.Update();
		}

		if(LocalPlayerController.LocalShip)
			listenerPos = LocalPlayerController.LocalShip.transform.position;
		else if(Camera.main)
			listenerPos = Camera.main.transform.position;
	} // End of Update().

	public static void ClearAllSources() {
		while(allSpatialSources.Count > 0)
			allSpatialSources[0].Destroy();
	} // End of ClearAllSources().
	
} // End of SoundManager.


public enum SoundSpace{
	world, // Default
	soundtrack, // Non-spatialized, not affected by fading, but affected by time scaling.
	ui // Non-spatialized, not affectred by time scaling.
} // End of SoundSpace.


public enum RolloffClass {
	veryQuiet = 60,
	quiet = 120,
	medium = 250,
	loud = 500,
	veryLoud = 1000
} // End of LoudnessCategory.


[System.Serializable]
public class SpatialAudioClip {

	public AudioClip clip;
	public float clipVolume = 1f;
	public RolloffClass rolloffClass = RolloffClass.quiet;
	public float pitchModulation = 0f; // How much the pitch will be randomly shifted (1 = 0.5-2x, 2 = 0.25-4x, etc.

	public SpatialAudioClip(){}
	public SpatialAudioClip(AudioClip clip, float volume = 1f, float pitchModulation = 0f, RolloffClass rolloffClass = RolloffClass.quiet){
		this.clip = clip;
		clipVolume = volume;
		this.rolloffClass = rolloffClass;
		this.pitchModulation = pitchModulation;
	}

	public static implicit operator bool(SpatialAudioClip exists){
		return exists.clip != null;
	}

} // End of LocativeSound.


// Actual sound playback volume AND rolloff distance will decrease as 'volume' is reduced.
public class SpatialAudioSource {

	public GameObject audioGObject;
	public SpatialAudioClip spatialClip;
	public AudioSource source;
	//public AudioLowPassFilter lowPass;
	public Vector3 point;
	// This sound will be destroyed IFF the source transform is destroyed.
	public bool boundToTransform;
	public bool loop { get { return source.loop; } set { source.loop = value; } }
	public float time { get { return source.time; } set { source.time = value; } }
	public float length { get { return source.clip.length; } }
	public float pitch = 1f;
	public float volume = 1f;
	public SoundSpace soundSpace;

	public Transform sourceTransform;

	bool init = false;
	public bool IsPlaying {get{return source.isPlaying;}}

	public void Update(){
		// If our source was destroyed, we can go as well. RIP
		if(!source) {
			Destroy();
			return;
		}

		if(!SpatialAudioManager.Inst)
			return;

		if(sourceTransform)
			point = sourceTransform.position;

		float distToListener = 0f;
		if((soundSpace == SoundSpace.world))
			distToListener = Vector3.Distance(point, SpatialAudioManager.Inst.ListenerPos);
		
		if(soundSpace == SoundSpace.ui)
			source.volume = spatialClip.clipVolume * volume;
		else if(soundSpace == SoundSpace.soundtrack)
			source.volume = spatialClip.clipVolume * volume;
		else {
			float maxDistance = (float)spatialClip.rolloffClass * volume;
			source.volume = Mathf.Pow(1f - Mathf.InverseLerp(0f, maxDistance, distToListener), 2f) * spatialClip.clipVolume * volume * SpatialAudioManager.Inst.StandardLevelCoefficient;
		}


		if(soundSpace != SoundSpace.ui && (Time.timeScale > 0f))
			source.pitch = Time.timeScale * pitch;
		else if(soundSpace == SoundSpace.ui)
			source.pitch = pitch;
		else if(Time.timeScale == 0f)
			source.pitch = 0f;

		//lowPass.cutoffFrequency = 20000f / (distToListener * 0.02f);

		// Start a one-shot audio clip (now that we've set the volume).
		if(!init && !boundToTransform){
			source.Play();
			init = true;
		}

		// We done (or our source is destroyed), so we ded
		if((!source.isPlaying && !boundToTransform) || (boundToTransform && !sourceTransform)){
			Destroy();
		}
	} // End of Update().

	// Safely stops/starts the sound, can be called constantly.
	public void PlayControl(bool play, bool randomizeTime = false){
		if(play && !IsPlaying)
			Play(randomizeTime);
		else if(!play && IsPlaying)
			Stop();
	} // End of PlayControl().

	public void Play(bool randomizeTime = false){
		source.clip = spatialClip.clip;
		pitch = Mathf.Pow(2f, Random.Range(-spatialClip.pitchModulation, spatialClip.pitchModulation));
		//if(randomizeTime)
			//source.time = Random.Range(0f, source.clip.length);
		source.Play();
	} // End of Play().

	public void Stop(){
		if(source)
			source.Stop();
	} // End of Stop().

	public void Destroy() {
		GameObject.Destroy(audioGObject);
		SpatialAudioManager.allSpatialSources.Remove(this);
	} // End of Destroy().

	public static implicit operator bool(SpatialAudioSource exists){
		return exists != null;
	}

} // End of SpatialSource.