using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; } // глобальный доступ к вводу

    private GameInputActions _actions; // набор действий Input System

    private void Awake()
    {
        if (Instance != null && Instance != this) // защищаемся от второго экземпляра
        {
            Destroy(gameObject); // уничтожаем дубликат
            return; // выходим, чтобы не продолжать инициализацию
        }
        Instance = this; // сохраняем ссылку на себя
        _actions = new GameInputActions(); // создаём экземпляр действий
        _actions.Enable(); // активируем все действия
    }

    public Vector2 Move => _actions.Gameplay.Move.ReadValue<Vector2>(); // оси движения

    public bool Fire => _actions.Gameplay.Fire.triggered; // выстрел (нажат в этом кадре)
    public bool FireHeld => _actions.Gameplay.Fire.IsPressed(); // выстрел (удерживается)
    public bool FirePressed => Fire; // для обратной совместимости, если кто-то использует

    public Vector2 PointerScreen => Mouse.current.position.ReadValue(); // позиция курсора на экране
}

