using UnityEngine;

/// <summary>
/// This class represents an interactable object in the game world and needs to be inherited by other classes.
/// </summary>
public abstract class Interactable : MonoBehaviour {
    /// <summary>
    /// Called when the player interacts with this object.
    /// </summary>
    public abstract void Interact();
}
