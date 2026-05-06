using System;
using UnityEngine;

public class TileExplosionVisuals : MonoBehaviour {
    [SerializeField] private float lifetime;

    private float _destroyTime;
    private void Awake() {
        _destroyTime = Time.time + lifetime;
    }

    private void Update() {
        if(Time.time > _destroyTime) {
            Destroy(gameObject);
        }
    }
}