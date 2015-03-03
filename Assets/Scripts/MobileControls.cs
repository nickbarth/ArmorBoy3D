using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  public void OnPointerDown(PointerEventData data)
  { 
    if (gameObject.name == "LeftButton") {
      GameManager.Player.MoveSpeed = -1f;
    }

    if (gameObject.name == "RightButton") {
      GameManager.Player.MoveSpeed  = 1f;
    }

    if (gameObject.name == "JumpButton") {
      GameManager.Player.Jump();
    }

    if (gameObject.name == "AttackButton") {
      GameManager.Player.Attack();
    }
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    if (gameObject.name == "LeftButton") {
      GameManager.Player.MoveSpeed = 0f;
    }

    if (gameObject.name == "RightButton") {
      GameManager.Player.MoveSpeed = 0f;
    }
  }
}
