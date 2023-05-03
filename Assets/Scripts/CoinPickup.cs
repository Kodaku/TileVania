using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            StartCoroutine(DestroyCoin());
        }
    }

    private IEnumerator DestroyCoin() {
        yield return new WaitForSecondsRealtime(0.05f);
        FindObjectOfType<GameSession>().AddToScore(5);
        Destroy(gameObject);
    }
}
