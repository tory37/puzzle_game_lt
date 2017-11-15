using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MultiTag {
    Jumpable
}

public class MultiTagSystem : MonoBehaviour {

    [SerializeField]
    private List<MultiTag> tags;

    public bool HasTag(MultiTag tag) {
        if (tags.Contains(tag))
            return true;
        return false;
    }
}
