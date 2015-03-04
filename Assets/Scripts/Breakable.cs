using UnityEngine;

public class Breakable : MonoBehaviour {
  // Config
  public GameObject Pieces;

  public void Break () {
    Object explosion = Instantiate(Pieces, transform.position, Quaternion.identity);
    Destroy(gameObject);
    Destroy(explosion, 1f);
  }

  void OnTriggerEnter(Component component) {
    if (component.CompareTag("Player") && Player.Attacking) {
      Break();
    }
  }
}
