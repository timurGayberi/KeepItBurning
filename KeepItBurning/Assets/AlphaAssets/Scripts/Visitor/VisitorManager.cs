using UnityEngine;

public class VisitorManager : MonoBehaviour
{
    public GameObject visitorPrefab;

    public Sprite chocolateSprite;
    public Sprite marshmellowSprite;
    public Sprite sausageSprite;

    private void Start()
    {
        SpawnVisitor();
    }

    void SpawnVisitor()
    {
        // instantiate visitor
        var visitorObj = Instantiate(visitorPrefab);
        var visitor = visitorObj.GetComponent<Visitor>();

        int random = Random.Range(0, 2);
        if (random == 0) visitor.SetState(new ChocolateState(chocolateSprite));

        if (random == 1) visitor.SetState(new MarshmallowState(marshmellowSprite));

        if (random == 2) visitor.SetState(new SausageState(sausageSprite));
    }
}