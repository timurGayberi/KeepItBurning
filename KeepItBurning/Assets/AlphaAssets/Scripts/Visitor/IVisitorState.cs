public interface IVisitorState
{
    void Enter(Visitor visitor);
    void Update(Visitor visitor);
    void Exit(Visitor visitor);
}