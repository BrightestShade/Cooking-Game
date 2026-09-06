using UnityEngine;
[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
 
    
    //Store array for each sfx (to allow multiple variations)
    public AudioClip[] chop;
    public AudioClip[] deliveryFail;
    public AudioClip[] deliverySuccess;
    public AudioClip[] footstep;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickup;
    public AudioClip stoveSizzle; //single sfx so no array
    public AudioClip[] trash;
    public AudioClip[] warning;


}
