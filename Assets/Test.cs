using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "Scriptable Objects/Test")]
public class Test : ScriptableObject
{
    public string testString;
    public int testInt;
    public float testFloat;
    public bool testBool;
    public Color testColor;
    public Vector3 testVector3;
    public Quaternion testQuaternion;
    public GameObject testGameObject;
    public Transform testTransform;
    public Rigidbody testRigidbody;
    public Collider testCollider;
    public MeshRenderer testMeshRenderer;
    public MeshFilter testMeshFilter;
    public MeshCollider testMeshCollider;
    public Material testMaterial;
    public Texture2D testTexture2D;
    public AudioClip testAudioClip;
    public AnimationCurve testAnimationCurve;
    public Gradient testGradient;
    public List<string> testList;
    public Dictionary<string, int> testDictionary;
    public HashSet<string> testHashSet;
  Debug.Log("Test");
}
