using UnityEngine;

public class ActionPoint : MonoBehaviour, IRespawnable {
  public void Respawn() {
    gameObject.SetActive(true);
  }
}
