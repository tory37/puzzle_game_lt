using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MultiTag
{
    // This is a solid object for player to jump off of
    Jumpable,
    // This means that the camera should not collide into this object
    MoveCameraOnCollision,
    // This means the renderer should turn off when a camera collides with it
    ToggleRendererOnCameraCollision,
    // Can be moved by telekinesis
    AffectedByTelekenisis

}

public class MultiTagSystem : MonoBehaviour
{

    [SerializeField]
    private List<MultiTag> tags;

    public bool HasTag(MultiTag tag)
    {
        return tags.Contains(tag);
    }
}
