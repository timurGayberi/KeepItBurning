using UnityEngine;

public class ChocolateState : IVisitorState
{
    private Sprite chocolateSprite;
    public ChocolateState(Sprite chocolateSprite)
    {
        this.chocolateSprite = chocolateSprite;
    }

    public void Enter(Visitor visitor)
    {
        Debug.Log("Want Chocolate");
        visitor.ResetTimer();
        visitor.ShowThoughtBubble(chocolateSprite);
    }

    public void Update(Visitor visitor)
    {
        if (visitor.TickTimer())
            visitor.SetState(new AngryState());
    }

    public void Exit(Visitor visitor)
    {
        Debug.Log("Got Chocolate");
        visitor.HideThoughtBubble();
    }
}

public class SausageState : IVisitorState
{
    private Sprite sausageSprite;
    public SausageState(Sprite sausageSprite)
    {
        this.sausageSprite = sausageSprite;
    }

    public void Enter(Visitor visitor)
    {
        Debug.Log("Want Sausage");
        visitor.ResetTimer();
        visitor.ShowThoughtBubble(sausageSprite);
    }

    public void Update(Visitor visitor)
    {
        if (visitor.TickTimer())
            visitor.SetState(new AngryState());
    }

    public void Exit(Visitor visitor)
    {
        Debug.Log("Got Sausage");
        visitor.HideThoughtBubble();
    }
}

public class MarshmallowState : IVisitorState
{
    private Sprite marshmallowSprite;
    public MarshmallowState(Sprite marshmallowSprite)
    {
        this.marshmallowSprite = marshmallowSprite;
    }

    public void Enter(Visitor visitor)
    {
        Debug.Log("Want Marshmallow");
        visitor.ResetTimer();
        visitor.ShowThoughtBubble(marshmallowSprite);
    }

    public void Update(Visitor visitor)
    {
        if (visitor.TickTimer())
            visitor.SetState(new AngryState());
    }

    public void Exit(Visitor visitor)
    {
        Debug.Log("Got Marshmallow");
        visitor.HideThoughtBubble();
    }
}

public class AngryState : IVisitorState
{
    public void Enter(Visitor visitor)
    {
        Debug.Log("Visitor Leaving");
        visitor.HideThoughtBubble();
        GameObject.Destroy(visitor.gameObject, 1.5f);
    }

    public void Update(Visitor visitor) { }
    public void Exit(Visitor visitor) { }
}

public class HappyState : IVisitorState
{
    public void Enter(Visitor visitor)
    {
        Debug.Log("Visitor Happy");
        visitor.HideThoughtBubble();
        GameObject.Destroy(visitor.gameObject, 1.5f);
    }

    public void Update(Visitor visitor) { }
    public void Exit(Visitor visitor) { }
}

public class IdleState : IVisitorState
{
    public void Enter(Visitor visitor)
    {
        visitor.HideThoughtBubble();
        Debug.Log(visitor.name + " waiting.");
    }

    public void Update(Visitor visitor) { }
    public void Exit(Visitor visitor) { }
}