using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {
  public GameObject[] enemies;

  public void Respawn () {
    foreach (GameObject respawnable in enemies) {
      var enemy = (IRespawnable)respawnable.gameObject.GetComponent(typeof(IRespawnable));
      enemy.Respawn();
    }
  }
}
