using UnityEngine;
using System.Collections;

public static class Utilities {

	// Converts Y value to Screen.height - Y. Useful for stupid Unity-native mouse positions.
	public static Vector3 ScreenFixY(this Vector3 vector){
		return new Vector3(vector.x, Screen.height - vector.y, vector.z);
	} // End of ScreenFixY().

	public static Vector2 ScreenFixY(this Vector2 vector){
		return new Vector3(vector.x, Screen.height - vector.y);
	} // End of ScreenFixY().


	public static Vector3 ReflectX(this Vector3 vector){
		return new Vector3(-vector.x, vector.y, vector.z);
	} // End of ReflectX().


	public static Vector3 ZeroY(this Vector3 vector){
		return new Vector3(vector.x, 0f, vector.z);
	} // End of ReflectX().


	// Convers eulers to -180 to 180.
	public static Vector3 DeltaEulers(this Vector3 vector){
		return new Vector3(Mathf.DeltaAngle(0f, vector.x), Mathf.DeltaAngle(0f, vector.y), Mathf.DeltaAngle(0f, vector.z));
	} // End of DeltaEulers().


	public static Rect CenteredSquare(Vector2 center, float size){
		return CenteredRect(center, new Vector2(size, size));
	} // End of CenteredSquare().


	public static Rect CenteredRect(Vector2 center, Vector2 size){
		return new Rect(center.x - (size.x * 0.5f), center.y - (size.y * 0.5f), size.x, size.y);
	} // End of CenteredRect().


	// Changes the alpha of a color.
	public static Color SetAlpha(this Color color, float alpha){
		return new Color(color.r, color.g, color.b, alpha);
	} // End of SetAlpha().


	public static float Mps2Kmh(float metersPerSecond){
		return metersPerSecond * 3.6f;
	} // End of Ms2Kmh().
	

	// Maps a value to another value.
	public static float Map(float oldMin, float oldMax, float newMin, float newMax, float oldValue){
 
		float oldRange = (oldMax - oldMin);
		float newRange = (newMax - newMin);
		float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
 
		return(newValue);
	} // End of Map().

	public static string ColorRGBToHTML(Color color){
	    string hex = "#";
        hex += Mathf.FloorToInt(color.r * 255).ToString("x2");
        hex += Mathf.FloorToInt(color.g * 255).ToString("x2");
        hex += Mathf.FloorToInt(color.b * 255).ToString("x2");
        hex += Mathf.FloorToInt(color.a * 255).ToString("x2");
	    return hex;
    } // End of ColorToHex().

    public static string ColorText(Color color, string text){
        return "<color=" + ColorRGBToHTML(color) + ">" + text + "</color>";
    } // End of ColorText().


	public static void SetLayerRecursively(this GameObject obj, int layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) 
            child.gameObject.SetLayerRecursively(layer);
    } // End of SetLayerRecursively().


	public static void SetRenderRecursively(this GameObject obj, bool render) {
        Renderer thisRenderer = obj.GetComponent<Renderer>();
		if(thisRenderer)
			thisRenderer.enabled = render;
        foreach (Transform child in obj.transform) 
            child.gameObject.SetRenderRecursively(render);
    } // End of SetLayerRecursively().


	public static T GetFirstComponentUpHierarchy<T>(this GameObject obj) where T : Component{
		// Hey, WE have that object! Return it!
		if(obj.GetComponent<T>())
			return obj.GetComponent<T>();
		// We don't have it, but we have a parent to check!
		else if(obj.transform.parent){
			return obj.transform.parent.gameObject.GetFirstComponentUpHierarchy<T>();
		// We don't have it, no parent to check, return empty-handed. :(
		}else
			return null;
	} // End of GetFirstComponentUpHierarchy().


	public static float LinToSmoothLerp(float input) {
		return 0.5f + ((Mathf.Sin(Mathf.PI * (Mathf.Clamp01(input) - 0.5f)) * 0.5f));
	} // End of LinearLerpToSmooth().

	// from 0 to 1, with smoothing towards 1.
	public static float EaseOut(float input) {
		return Mathf.Sqrt(1f - Mathf.Pow(input - 1f, 2f));
	} // End of EaseOut().

	public static float EaseIn(float input) {
		return 1f - Mathf.Sqrt(1f - Mathf.Pow(input, 2f));
	} // End of EaseIn().

	public static float SmoothPingPong01(float input) {
		return 0.5f + (Mathf.Sin((-0.5f * Mathf.PI) + input * 2f * Mathf.PI) * 0.5f);
	} // End of LinearLerpToSmooth().


	public static float LerpPiece(float start, float end, float t) {
		return 0f;
	} // End of LerpPiece().


	public static float CrowDist(Vector3 a, Vector3 b) {
		return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
	} // End of CrowDist().


	public static Quaternion CrowDirection(Transform transform) {
		return Quaternion.LookRotation(Vector3.Scale(transform.forward, new Vector3(1f, 0f, 1f)));
	} // End of CrowDirection().

	public static Quaternion CrowDirection(Vector3 vector) {
		return Quaternion.LookRotation(Vector3.Scale(vector, new Vector3(1f, 0f, 1f)));
	} // End of CrowDirection().


	public static Vector3 CalculateBezierPoint(Vector3 firstPoint, Vector3 firstHandle, Vector3 secondHandle, Vector3 secondPoint, float t){
		float u = 1f - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;
 
		Vector3 p = uuu * firstPoint; //first term
		p += 3 * uu * t * firstHandle; //second term
		p += 3 * u * tt * secondHandle; //third term
		p += ttt * secondPoint; //fourth term
 
		return p;
	} // End of CalculateBezierPoint().


	public static float AzimuthTo_rad(Vector3 start, Vector3 end) {
		return Mathf.Atan2(end.x - start.x, end.z - start.z);
	} // End of AzimuthTo().

	public static float AzimuthTo_deg(Vector3 start, Vector3 end) {
		return Mathf.Atan2(end.x - start.x, end.z - start.z) * Mathf.Rad2Deg;
	} // End of AzimuthTo().


	public static void EnhancedTorqueToRotation(Rigidbody rigidbody, Quaternion targetRotation, float rate = 0.7f, float damping = 10f) {
		Quaternion deltaRotation = Quaternion.Inverse(rigidbody.transform.rotation) * targetRotation;
		Vector3 deltaAngles = GetRelativeAngles(deltaRotation.eulerAngles);
		Vector3 worldDeltaAngles = rigidbody.transform.TransformDirection(deltaAngles);
 
		// alignmentSpeed controls how fast you rotate the body towards the target rotation
		// alignmentDamping prevents overshooting the target rotation
		// Values used: alignmentSpeed = 0.025, alignmentDamping = 0.2
		Vector3 torqueToAdd = rate * worldDeltaAngles - damping * rigidbody.angularVelocity;
		rigidbody.AddTorque(torqueToAdd, ForceMode.Acceleration);

	} // End of EnhancedTorqueToRotation().

	// Convert angles above 180 degrees into negative/relative angles
	public static Vector3 GetRelativeAngles(Vector3 angles){
		Vector3 relativeAngles = angles;
		if (relativeAngles.x > 180f)
		relativeAngles.x -= 360f;
		if (relativeAngles.y > 180f)
		relativeAngles.y -= 360f;
		if (relativeAngles.z > 180f)
		relativeAngles.z -= 360f;
 
		return relativeAngles;
	} // End of GetRelativeAngles().


	public static T CopyComponent<T>(T original, GameObject destination) where T : Component{
         System.Type type = original.GetType();
         var dst = destination.GetComponent(type) as T;
         if (!dst) dst = destination.AddComponent(type) as T;
         var fields = type.GetFields();
         foreach (var field in fields)
         {
             if (field.IsStatic) continue;
             field.SetValue(dst, field.GetValue(original));
         }
         var props = type.GetProperties();
         foreach (var prop in props)
         {
             if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
             prop.SetValue(dst, prop.GetValue(original, null), null);
         }
         return dst as T;
    } // End of CopyComponent().


	// "Blows an object apart." Strips it and all of its children down into 'wreckage' and flings
    //   it all into the air with rigidbody and collider components.
    public static void Dismantle(GameObject anObject){ // Defaulted explosion origin.
        Utilities.Dismantle(anObject.gameObject, anObject.transform.position, Vector3.zero);}
	public static void Dismantle(GameObject anObject, Vector3 expOrigin){ // Defaulted explosion origin.
        Utilities.Dismantle(anObject.gameObject, expOrigin, Vector3.zero);}
    public static void Dismantle(GameObject currentObject, Vector3 expOrigin, Vector3 initVelocity){
		// Objects marked "DestroyOnDismantle" are simply deleted when dismantled.
		if(currentObject.CompareTag("DestroyOnDismantle")){
            MonoBehaviour.Destroy(currentObject);
            return;
        }

		Rigidbody currentRigidbody = currentObject.GetComponent<Rigidbody>();
		
        Transform[] children = currentObject.GetComponentsInChildren<Transform>();
		Vector3 passRigidVel = currentRigidbody? currentRigidbody.velocity : initVelocity;
	    foreach(Transform child in children){
            if((child != currentObject.transform) && !child.CompareTag("junk") && !currentObject.CompareTag("DestroyOnDismantle"))
    		    Utilities.Dismantle(child.gameObject, expOrigin, passRigidVel);
        }
		
        if(currentObject.CompareTag("junk")){
            return;
        }
	
        currentObject.tag = "junk";
	    currentObject.name = "_junk " + currentObject.name;
        //currentObject.transform.parent = GameManager.junkContainer;

        currentObject.layer = LayerMask.NameToLayer("Wreckage");
	
	    bool hasMeshRenderer = false;
        bool hasMeshCollider = false;
	    Rigidbody myRigidbody = null;
		
	    Component[] components = currentObject.GetComponents<Component>();
	    for(int i = 0; i < components.Length; i++){
            Component curComponent = components[i];
            System.Type componentType = curComponent.GetType();

            // Keep the component if it's one of the following... else destroy it.
            if(componentType == typeof(Transform)){}
            else if(componentType == typeof(MeshFilter)){}
            else if(componentType == typeof(MeshRenderer))
                hasMeshRenderer = true;
            else if(componentType == typeof(MeshCollider))
                hasMeshCollider = true;
            else if(componentType == typeof(BoxCollider)){}
            else if(componentType == typeof(SphereCollider)){}
            else if(componentType == typeof(CapsuleCollider)){}
            else if(componentType == typeof(Rigidbody))
                myRigidbody = curComponent.GetComponent<Rigidbody>();
            else if(componentType == typeof(Camera)){}
            else
                GameObject.Destroy(curComponent);
	    }
	
	    if(hasMeshRenderer && !currentObject.CompareTag("DestroyOnDismantle")){
		    if(!myRigidbody)
			    myRigidbody = currentObject.AddComponent<Rigidbody>();
			
            MeshFilter myMeshFilter = currentObject.GetComponent<MeshFilter>();
            if(myMeshFilter){
                Vector3 size = myMeshFilter.mesh.bounds.size;
                myRigidbody.mass = size.x * size.y * size.z;
            }

            myRigidbody.mass = 1f;

		    myRigidbody.AddExplosionForce(5f, expOrigin, 50f, 0f, ForceMode.Impulse);
			myRigidbody.velocity = passRigidVel;
            myRigidbody.AddTorque(Random.rotation * Vector3.forward * 100f);
		
            if(hasMeshCollider)
    		    currentObject.GetComponent<MeshCollider>().convex = true;

		    //currentObject.AddComponent<WreckageScript>();
	    }
	    else
		    GameObject.Destroy(currentObject);
    } // End of Dismantle().


	// Returns the pitch and azimuth of a direction.
	public static Vector2 PitchAzimuth(Vector3 direction) {
		Vector2 anglesToTarget = Vector2.zero;
		anglesToTarget.x = Mathf.Atan2(direction.y, Utilities.CrowDist(Vector3.zero, direction)) * Mathf.Rad2Deg;
		anglesToTarget.y = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
		return anglesToTarget;
	} // End of PitchAzimuth();


	public static float Length(this UnityEngine.AI.NavMeshPath path) {
		float length = 0f;
		for(int i = 1; i < path.corners.Length; i++)
			length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
		return length;
	} // End of Length().


	public static void DrawRotatedGizmoCube(Vector3 position, Quaternion rotation, Vector3 scale){
		Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
 
		Gizmos.matrix *= cubeTransform;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = oldGizmosMatrix;
	} // End of DrawRotatedGizmoCube().


	public static Vector2 WorldToCanvasPoint(this Canvas canvas, Camera camera, Vector3 worldPoint) {
		//first you need the RectTransform component of your canvas
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		//then you calculate the position of the UI element
		//  0,0 for the canvas is at the center of the screen
		//  whereas WorldToViewPortPoint treats the lower left corner as 0,0.
		//  Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
		Vector2 viewportPosition = camera.WorldToViewportPoint(worldPoint);
		Vector2 screenPos = new Vector2(
			((viewportPosition.x * canvasRect.sizeDelta.x * (canvasRect.sizeDelta.x / Screen.width) * canvasRect.localScale.x * camera.rect.width) - (canvasRect.sizeDelta.x * 0.5f * camera.rect.width)),
			((viewportPosition.y * canvasRect.sizeDelta.y * (canvasRect.sizeDelta.y / Screen.height) * canvasRect.localScale.y * camera.rect.height) - (canvasRect.sizeDelta.y * 0.5f * camera.rect.height))
		);
 
		return screenPos;
	} // End of WorldToCanvasPoint().

	public static Ray CanvasToWorldRay(this Canvas canvas, Camera camera, Vector2 canvasPoint) {
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		// Solved for 'viewportPosition' given 'canvasPoint' from WorldToCanvasPoint.
		Vector2 viewportPosition = new Vector2(
			((canvasPoint.x + (canvasRect.sizeDelta.x * 0.5f * camera.rect.width)) / (canvasRect.localScale.x * camera.rect.width)) / (canvasRect.sizeDelta.x * (canvasRect.sizeDelta.x / Screen.width)),
			((canvasPoint.y + (canvasRect.sizeDelta.y * 0.5f * camera.rect.height)) / (canvasRect.localScale.y * camera.rect.height)) / (canvasRect.sizeDelta.y * (canvasRect.sizeDelta.y / Screen.height))
		);

		return camera.ViewportPointToRay(viewportPosition);
	} // End of WorldToCanvasPoint().


	// This afford a sequence of one-shot events with a set time *BETWEEN* each event (NOT a specific elapsed time!)
	// Must have four local variables:
	//   float elapsedTime - the total time that has elapsed, against which event calls will be checked.
	//   int state - the current state of the machine.
	//   int incrementStep - used to index the steps in the sequence. Set this to 0 every frame before the event checks are called.
	//   float incrementTime - used to determine the firing times for events. Set this to 0 every frame before the event checks are called.
	//
	// EXAMPLE:
	//
	// float elapsedTime = 0f;
	// int state = 0;
	// incrementStep = 0;
	// float incrementTime = 0f;
	//
	// void Update(){
	//   elapsedTime += Time.deltaTime;
	//   incrementStep = 0;
	//   incrementTime = 0f;
	//   if(SequenceOfEvents(2f, state, incrementStep, incrementTime, elapsedTime)
	//      print("This happens immediately. In 2 seconds...");
	//   if(SequenceOfEvents(2f, state, incrementStep, incrementTime, elapsedTime)
	//      print("...this stuff happens! Then 3.5 seconds later, the next stuff will happen. And so on.");
	// } // End of Update().
	//
	public static bool SequenceOfEvents(float delayUnitNext, ref int state, ref int incrementStep, ref float incrementTime, float elapsedTime) {
		incrementStep++;
		if((elapsedTime > incrementTime) && (state < incrementStep)) {
			state++;
			incrementTime += delayUnitNext;
			return true;
		} else {
			incrementTime += delayUnitNext;
			return false;
		}
	} // End of SequenceOfEvents().


	// Remaps a value in a range to another range (unclamped)
    public static float UnclampedRemap(float value, float fromMin, float fromMax, float toMin, float toMax){
        return toMin + (((value - fromMin) / (fromMax - fromMin)) * (toMax - toMin));
    }


	public static int RoundUpToTens(int value){
		return 10 * ((value + 9) / 10);
	} // End of RoundUpToTens().

	public static int RoundDownToTens(int value){
		return 10 * (value / 10);
	} // ENd of RoundDownToTens().


	// x, y, dx, dy, x1, y1, x2, y2
	public static bool RayIntersectLineSegment(Vector2 rayOrigin, Vector2 rayDirection, Vector2 segmentPointA, Vector2 segmentPointB, out Vector2 intersection){
		float r, s, d;
		//Make sure the lines aren't parallel, can use an epsilon here instead
		// Division by zero in C# at run-time is infinity. In JS it's NaN
		if ((rayDirection.y / rayDirection.x) != ((segmentPointB.y - segmentPointA.y) / (segmentPointB.x - segmentPointA.x))){
			d = ((rayDirection.x * (segmentPointB.y - segmentPointA.y)) - rayDirection.y * (segmentPointB.x - segmentPointA.x));
			if (d != 0){
				r = (((rayOrigin.y - segmentPointA.y) * (segmentPointB.x - segmentPointA.x)) - (rayOrigin.x - segmentPointA.x) * (segmentPointB.y - segmentPointA.y)) / d;
				s = (((rayOrigin.y - segmentPointA.y) * rayDirection.x) - (rayOrigin.x - segmentPointA.x) * rayDirection.y) / d;
				if (r >= 0 && s >= 0 && s <= 1){
					intersection = new Vector2(rayOrigin.x + r * rayDirection.x, rayOrigin.y + r * rayDirection.y);
					return true;
				}
			}
		}
		intersection = Vector2.zero;
		return false;
	}


	public static float VolumeOfMesh(Mesh mesh, Transform scaleBy){
		float volume = 0;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < mesh.triangles.Length; i += 3){
			Vector3 p1 = vertices[triangles[i + 0]];
			Vector3 p2 = vertices[triangles[i + 1]];
			Vector3 p3 = vertices[triangles[i + 2]];
			volume += SignedVolumeOfTriangle(p1, p2, p3);
		}
		if(scaleBy)
			volume *= scaleBy.lossyScale.x * scaleBy.transform.lossyScale.y * scaleBy.transform.lossyScale.z;
		return Mathf.Abs(volume);
	} // End of VolumeOfMesh().


	public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3){
		float v321 = p3.x * p2.y * p1.z;
		float v231 = p2.x * p3.y * p1.z;
		float v312 = p3.x * p1.y * p2.z;
		float v132 = p1.x * p3.y * p2.z;
		float v213 = p2.x * p1.y * p3.z;
		float v123 = p1.x * p2.y * p3.z;
		return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
	} // End of SignedVolumeOfTriangle().


	// Sets the position/rotation of an attachment to the position of a targetAnchor, offset by the attachment's parented attachmentAnchor.
	public static void AttachToAnchor(Transform attachment, Transform attachmentAnchor, Transform targetAnchor) {
		attachment.transform.rotation = targetAnchor.rotation * Quaternion.Inverse(attachmentAnchor.localRotation);
		attachment.transform.position = targetAnchor.position + (attachment.transform.rotation * -attachmentAnchor.localPosition);
	} // End of AttachToAnchor().


} // End of Utilities.
