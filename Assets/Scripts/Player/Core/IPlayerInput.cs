using UnityEngine;

public interface IPlayerInput
{
    float MoveDirection { get; }
    bool JumpPressed { get; }
    bool SlidePressed { get; }
    bool StartGamePressed { get; }
}
