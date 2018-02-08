using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Entity : NetworkBehaviour {

	public static List<Entity> allEntities = new List<Entity>();
	[SerializeField] protected string entityName = "New Entity";
	protected DiageticUIElement uiElement = null; public DiageticUIElement UIElement { get { return uiElement; } }
	[SerializeField] protected float roughSize = 5f; public float RoughSize { get { return roughSize; } }
	[SerializeField] protected EntityType entityType = EntityType.junk;
	protected Rigidbody myRigidbody = null; public Rigidbody MyRigidbody { get { return myRigidbody; } }


	protected virtual void Awake() {
		allEntities.Add(this);
	} // End of Awake().

	protected virtual void Start() {
		//uiElement = GameGUIController.Inst.NewDiageticUIElement(GameGUIController.Inst.GetBracketSprite(entityType), roughSize, true, entityName);
		uiElement = GameGUIController.Inst.NewDiageticUIElement(GameGUIController.Inst.GetBracketSprite(entityType), 20f, false, entityName);
		myRigidbody = gameObject.GetFirstComponentUpHierarchy<Rigidbody>();
		if(transform.parent)
			uiElement.gameObject.SetActive(false);
	} // End of Start().

	protected virtual void LateUpdate() {
		uiElement.SetPosition(transform.position);
	} // End of LateUpdate().
	
	protected virtual void OnDestroy() {
		allEntities.Remove(this);
		if(uiElement)
			Destroy(uiElement.gameObject);
	} // End of OnDestroy().

} // End of Entity.


public enum EntityType {
	junk,
	smallShip
} // End of BracketType.