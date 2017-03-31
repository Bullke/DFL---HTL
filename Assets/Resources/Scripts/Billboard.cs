using System.Collections;
using UnityEngine;

/// <summary>
/// Orients an object to face the camera 
/// </summary>
[ExecuteInEditMode]
public class Billboard : MonoBehaviour
{
    /// <summary>
    /// Rotation about the billboard's forward axis
    /// </summary>
    public float billboardRotation;
	#region Private Methods

	/// <summary>
	/// Orient the object to face the camera
	/// <para/>
	/// Unity callback method
	/// </summary>
	void Update()
	{
		transform.forward = Camera.main.transform.forward;
        transform.Rotate(transform.forward, billboardRotation, Space.Self);
	}

	#endregion
}