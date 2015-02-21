using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	Player player;

	void Awake() {
		GameObject armorBoy = GameObject.Find("ArmorBoy");
		player = armorBoy.GetComponent<Player>();
	}

	public void OnPointerDown(PointerEventData data)
	{	
		if (gameObject.name == "LeftButton") {
			player.MoveSpeed(-1f);
		}
		
		if (gameObject.name == "RightButton") {
			player.MoveSpeed(1f);
		}

		if (gameObject.name == "JumpButton") {
			player.Jump();
		}

		if (gameObject.name == "AttackButton") {
			player.Attack();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (gameObject.name == "LeftButton") {
			player.MoveSpeed(0f);
		}
		
		if (gameObject.name == "RightButton") {
			player.MoveSpeed(0f);
		}
	}
}