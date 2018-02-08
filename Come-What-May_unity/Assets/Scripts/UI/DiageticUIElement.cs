using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiageticUIElement : MonoBehaviour {

	private RectTransform myRectTransform; public RectTransform MyRectTransform { get { return myRectTransform; } }
	private Image myImage; public Image MyImage { get { return myImage; } }
	private string label = "";
	[SerializeField] private TextMeshProUGUI text; public TextMeshProUGUI Text { get { return text; } }
	private Vector3 position = Vector3.zero; public Vector3 Position { get { return position; } }
	private float size; public float Size { get { return size; } }
	private bool scaleWithDistance; public bool ScaleWithDistance { get { return scaleWithDistance; } }

	public void Init(Sprite sprite, float _size, bool _scaleWithDistance, string _label = null) {
		MyImage.sprite = sprite;
		size = _size;
		scaleWithDistance = _scaleWithDistance;
		if(_label != null)
			label = _label;
		else
			Destroy(text);
		
		MyRectTransform.localScale = Vector3.one;
		MyRectTransform.sizeDelta = Vector2.one * size;
	} // End of constructor.

	private void Awake() {
		myRectTransform = GetComponent<RectTransform>();
		myImage = GetComponent<Image>();
	} // End of Awake().

	public void SetPosition(Vector3 newPosition) {
		position = newPosition;
	} // End of SetPosition().

	public void SetLabel(string newText) {
		label = newText;
	} // End of SetText().

	public void SetColor(Color newColor) {
		myImage.color = newColor;
	} // End of SetColor().

	private void LateUpdate() {
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(Position);
		if(NetworkPilot.Local && NetworkPilot.Local.MyShip && (screenPosition.z > 0f)) {
			MyRectTransform.localPosition = screenPosition;
			if(ScaleWithDistance)
				MyRectTransform.sizeDelta = Vector2.one * (1f / screenPosition.z) * size * 1400f * (Screen.height / 1000f);
			myImage.enabled = true;
			if(text) {
				text.enabled = true;
				text.text = label + "\n" + Vector3.Distance(NetworkPilot.Local.MyShip.transform.position, position).ToString("F0") + "m";
			}
		} else {
			myImage.enabled = false;
			if(text)
				text.enabled = false;
		}
	} // End of Update().

} // End of DiageticUIElement.
