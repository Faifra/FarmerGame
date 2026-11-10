using UnityEngine;

public interface IInteractableScript
{
    string InteractionPrompt { get; }

    void Interact();
}
