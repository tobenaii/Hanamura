using MoonWorks.Input;

namespace Hanamura
{
    public readonly record struct PlayerController(KeyCode MoveForward, KeyCode MoveBackward, KeyCode MoveLeft, KeyCode MoveRight)
    {
    }
}