﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Player;

public class TargettingCamera : MonoBehaviour
{
    public bool freezeX;

    private bool isXFrozen;


    public bool freezeY;
    private bool isYFrozen;

    public Transform target;
    public PlayerCharacter player;
    public float targetSmoothing;
    public float playerSmoothing;

    public float sizeSmoothing;
    public Vector3 velocity;

    public Vector3 targetOffset;
    public Vector3 leftOffset;
    public Vector3 rightOffset;
    public bool isCentered;

    private Camera targetSettings;

    private Camera defaultSettings;

    void Start() {
        defaultSettings = GetComponent<Camera>();
        if (player == null) {
            player = FindObjectOfType<PlayerCharacter>();
            if (player == null) return;
        }
        ClearTarget();
        ClearFreezeX();
        ClearFreezeY();
    }

    void FixedUpdate() {
        if (target == null) {
            return;
        }

        if (player == null) {
            player = FindObjectOfType<PlayerCharacter>();
            if (player == null) return;
        }

        
        float futureSize = targetSettings.orthographicSize;
        float smoothing = (target == player.transform) ? playerSmoothing : targetSmoothing;
        Vector3 futurePos = getFuturePosition();
        
        // interpolate camera zoom
        Camera.main.orthographicSize = interpolate(Camera.main.orthographicSize, targetSettings.orthographicSize, sizeSmoothing);
        
        // interpolate camera position
        transform.position = Vector3.SmoothDamp(transform.position, futurePos, ref velocity, smoothing);
    }


    private Vector3 getFuturePosition() {
        Vector3 pos;

        // if following the player
        if (target == player.transform) {
            pos = player.transform.position;

            // choose appropriate camera offset.
            if (isCentered) {
                pos += targetOffset;
            } if (player.movement.isFacingRight) {
                pos += rightOffset;
            } else {
                pos += leftOffset;
            }
            
        // moving to a predefined location
        } else {
            pos = target.transform.position + targetOffset;
        }

        if (isXFrozen) {
            pos.x = Camera.main.transform.position.x;
        }

        if (isYFrozen) {
            pos.y = Camera.main.transform.position.y;
        }

        return pos;
    }


    private float interpolate(float x, float y, float a) {
        return x*a + y*(1-a);
    }


    public void SetPlayerSmoothing(float smoothing) {
        playerSmoothing = smoothing;
    }

    public void SetTargetSmoothing(float smoothing) {
        targetSmoothing = smoothing;
    }

    public void SetTarget(Camera cameraSettings) {
        //Debug.Log("Setting Target!");
        targetSettings = cameraSettings;
        isCentered = true;
        target = cameraSettings.transform;
    }

    public void ClearTarget() {
        //Debug.Log("Clearing Target!");
        targetSettings = defaultSettings;
        target = player.transform;
        isCentered = false;
    }

    public void SetCentered(bool centered) {
        isCentered = centered;
    }

    public void SetFreezeX(bool freeze) { isXFrozen = freeze; }
    public void SetFreezeY(bool freeze) { isYFrozen = freeze; }

    public void ClearFreezeX() { isXFrozen = freezeX; }
    public void ClearFreezeY() { isYFrozen = freezeY; }
    public void FreezeX() { isXFrozen = true; }

    public void FreezeY() { isYFrozen = true; }

    public void UnfreezeX() { isXFrozen = false; }

    public void UnfreezeY() { isYFrozen = false; }
}
