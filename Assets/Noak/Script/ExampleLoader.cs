using UnityEngine;

public class ExampleLoader : MonoBehaviour
{
    public CharacterPartAttacher attacher;
    public GameObject torsoPrefab;

    void Start()
    {
        attacher.AttachPart(torsoPrefab);
    }
}
