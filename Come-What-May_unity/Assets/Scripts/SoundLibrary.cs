using UnityEngine;
using System.Collections;
using System;


public enum BulkSound {
	none,
	footstep_grass,
	footstep_stone,
	footstep_water
} // End of BulkSound.

public class SoundLibrary : MonoBehaviour {

	static SoundLibrary instance;
	public static SoundLibrary Inst = null;

	// Sound effects
	[Header("Gear Manipulation")]
	[SerializeField] private SpatialAudioClip equipItemSAC = null; public SpatialAudioClip EquipItemSAC { get { return equipItemSAC; } }
	[SerializeField] private SpatialAudioClip retrieveItemSAC = null; public SpatialAudioClip RetrieveItemSAC { get { return retrieveItemSAC; } }
	[SerializeField] private SpatialAudioClip dequipItemSAC = null; public SpatialAudioClip DequipItemSAC { get { return dequipItemSAC; } }
	[SerializeField] private SpatialAudioClip tossItemSAC = null; public SpatialAudioClip TossItemSAC { get { return tossItemSAC; } }
	[SerializeField] private SpatialAudioClip equipFirearmSAC = null; public SpatialAudioClip EquipFirearmSAC { get { return equipFirearmSAC; } }
	[SerializeField] private SpatialAudioClip stashItemSAC = null; public SpatialAudioClip StashItemSAC { get { return stashItemSAC; } }
	[SerializeField] private SpatialAudioClip loadMagazineSAC = null; public SpatialAudioClip LoadMagazineSAC { get { return loadMagazineSAC; } }
	[SerializeField] private SpatialAudioClip attachOpticSAC = null; public SpatialAudioClip AttachOpticSAC { get { return attachOpticSAC; } }
	[SerializeField] private SpatialAudioClip wearClothingSAC = null; public SpatialAudioClip WearClothingSAC { get { return wearClothingSAC; } }
	[SerializeField] private SpatialAudioClip changeActiveBagSAC = null; public SpatialAudioClip ChangeActiveBagSAC { get { return changeActiveBagSAC; } }




	class SoundCollection{
		public BulkSound soundType;
		public SpatialAudioClip[] spatialClips;
	} // End of SoundCollection.
	SoundCollection[] collections;


	void Awake(){
		Inst = this;

		// Make a collection for each sound type (except for 'none').
		collections = new SoundCollection[Enum.GetValues(typeof(BulkSound)).Length - 1];
		for(int i = 0; i < collections.Length; i++){
			collections[i] = new SoundCollection();
			collections[i].soundType = (BulkSound)(i + 1);

			AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Audio/" + Enum.GetNames(typeof(BulkSound))[i + 1]);
			collections[i].spatialClips = new SpatialAudioClip[audioClips.Length];
			for(int j = 0; j < audioClips.Length; j++){
				collections[i].spatialClips[j] = new SpatialAudioClip(audioClips[j]);

				RolloffClass rolloff = RolloffClass.quiet;
				float clipVolume = 1f;
				float pitchMod = 0f;
				switch(collections[i].soundType){
					case BulkSound.footstep_grass :
						rolloff = RolloffClass.quiet;
						clipVolume = 0.2f;
						pitchMod = 0.3f;
						break;
					case BulkSound.footstep_stone :
						rolloff = RolloffClass.quiet;
						clipVolume = 0.4f;
						pitchMod = 0.3f;
						break;
					case BulkSound.footstep_water :
						rolloff = RolloffClass.quiet;
						clipVolume = 0.6f;
						pitchMod = 0.4f;
						break;
					default :
						rolloff = RolloffClass.quiet;
						break;
				}

				collections[i].spatialClips[j].rolloffClass = rolloff;
				collections[i].spatialClips[j].clipVolume = clipVolume;
				collections[i].spatialClips[j].pitchModulation = pitchMod;
			}
		}
	} // End of Awake().


	public SpatialAudioClip GetClip(BulkSound soundType){
		for(int i = 0; i < collections.Length; i++){
			if(collections[i].soundType == soundType) {
				SpatialAudioClip spatialClip = collections[i].spatialClips[UnityEngine.Random.Range(0, collections[i].spatialClips.Length)];

				// Return a copy so if we tweak it, we don't modify the one stored in memory.
				return new SpatialAudioClip(spatialClip.clip, spatialClip.clipVolume, spatialClip.pitchModulation, spatialClip.rolloffClass);
			}
		}
		return null;
	} // End of GetClip().

} // End of SoundLibrary.

