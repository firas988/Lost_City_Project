using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "Scriptable Objects/Test")]
public class Test : ScriptableObject
{
    public Material testMaterial;
    public Texture2D testTexture2D;
    public AudioClip testAudioClip;
    public AnimationCurve testAnimationCurve;
    public Gradient testGradient;

    public void Test()
    {
        Debug.Log("Test");
    }
}
