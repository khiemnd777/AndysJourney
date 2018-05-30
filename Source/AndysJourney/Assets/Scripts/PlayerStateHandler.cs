using UnityEngine;

public enum PlayerState
{
    Idling, Walking, Carrying
}

public class PlayerStateHandler : MonoBehaviour
{
    public PlayerState state;
}