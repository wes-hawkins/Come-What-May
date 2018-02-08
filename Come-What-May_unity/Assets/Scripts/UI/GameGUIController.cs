using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGUIController : MonoBehaviour {

	public static GameGUIController Inst = null;

	[SerializeField] private RectTransform diageticUIContainer = null; public RectTransform DiageticUIContainer { get { return diageticUIContainer; } }
	[SerializeField] private GameObject diageticUIPrefab = null;

	[SerializeField] private Sprite bracketJunk = null;
	[SerializeField] private Sprite bracketSmallShip = null;

	[SerializeField] private Sprite bracketShipForward = null;
	private DiageticUIElement shipForwardReticle;
	[SerializeField] private Sprite bracketGunsTarget = null;
	private DiageticUIElement gunsTargetReticle;

	private bool hitImpact = false;
	[SerializeField] private AudioClip targetHitClip = null;
	private AudioSource targetHitSource = null;

	[SerializeField] private RectTransform navigationCluster = null;
	[SerializeField] private RectTransform rotationIndicator = null;


	private void Awake() {
		Inst = this;

		gunsTargetReticle = NewDiageticUIElement(bracketGunsTarget, 60f, false);
		shipForwardReticle = NewDiageticUIElement(bracketShipForward, 30f, false);
		navigationCluster.SetParent(shipForwardReticle.MyRectTransform);
		navigationCluster.localPosition = Vector3.zero;
		navigationCluster.localScale = Vector3.one;

		targetHitSource = gameObject.AddComponent<AudioSource>();
		targetHitSource.clip = targetHitClip;
		targetHitSource.loop = true;
		targetHitSource.priority = 0;
	} // End of Awake().

	private void Update() {
		if(LocalPlayerController.Inst && LocalPlayerController.Inst.MyShip) {
			if(LocalPlayerController.Inst.MyShip.GunsTarget)
				gunsTargetReticle.SetPosition(LocalPlayerController.Inst.MyShip.GunsTarget.transform.position);
			
			shipForwardReticle.SetPosition(LocalPlayerController.Inst.transform.position + (LocalPlayerController.Inst.transform.forward * LocalPlayerController.Inst.DistanceToTarget));

			Vector2 rotationIndicatorAnchor = (Vector2.one * 0.5f) + (new Vector2(LocalPlayerController.Inst.RotationalThrottle.y, -LocalPlayerController.Inst.RotationalThrottle.x) * 0.5f);
			rotationIndicator.anchorMin = rotationIndicatorAnchor;
			rotationIndicator.anchorMax = rotationIndicatorAnchor;
		}

		gunsTargetReticle.gameObject.SetActive(LocalPlayerController.Inst && LocalPlayerController.Inst.MyShip && LocalPlayerController.Inst.MyShip.GunsTarget);


		if(hitImpact) {
			if(!targetHitSource.isPlaying){
				targetHitSource.Play();
				targetHitSource.loop = true;
			}
		}
		else if(targetHitSource.isPlaying)
			targetHitSource.loop = false;
		hitImpact = false;
	} // End of Update().

	public DiageticUIElement NewDiageticUIElement(Sprite sprite, float size, bool scale, string text = null) {
		GameObject newElement = Instantiate(diageticUIPrefab, diageticUIContainer.transform, false);
		DiageticUIElement newUIElement = newElement.GetComponent<DiageticUIElement>();
		newUIElement.Init(sprite, size, scale, text);
		return newUIElement;
	} // End of NewDiageticUIElement().


	public Sprite GetBracketSprite(EntityType entityType) {
		switch(entityType) {
			case EntityType.junk:
				return bracketJunk;
			case EntityType.smallShip:
				return bracketSmallShip;
		}
		return null;
	} // End of GetBracketSprite().

	public void HitImpactSound() {
		hitImpact = true;
	} // End of HitImpactSound().
	
} // End of GameGUIController.
