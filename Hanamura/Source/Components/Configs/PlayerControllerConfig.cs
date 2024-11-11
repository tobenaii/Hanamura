using MoonWorks.Input;

namespace Hanamura
{
    public readonly record struct PlayerControllerConfig(KeyCode MoveForward, KeyCode MoveBackward, KeyCode MoveLeft, KeyCode MoveRight);
}