using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //References
    [Header("References")]
    public Transform trans;

    public Transform modelTrans;

    public CharacterController characterController;

    //Movement
    [Header("Movement")]
    [Tooltip("Units moved per second at maximum speed.")]
    public float movespeed = 24;

    [Tooltip("Time, in seconds, to reach maximum speed.")]
    public float timeToMaxSpeed = .26f;

    private float VelocityGainPerSecond
    {
        get
        {
            return movespeed / timeToMaxSpeed;
        }
    }

    [Tooltip("Time, in seconds, to go from maximum speed to stationary.")]
    public float timeToLoseMaxSpeed = .2f;

    private float VelocityLossPerSecond
    {
        get
        {
            return movespeed / timeToLoseMaxSpeed;
        }
    }

    [
        Tooltip(
            "Multiplier for momentum when attempting to move in a direction opposite the current traveling direction (e.g. trying to moveright when already moving left).")
    ]
    public float reverseMomentumMultiplier = 2.2f;

    private Vector3 movementVelocity = Vector3.zero;

    //Death and Respawning
    [Header("Death and Respawning")]
    [
        Tooltip(
            "How long after the player's death, in seconds, before they are respawned?")
    ]
    public float respawnWaitTime = 2f;

    private bool dead = false;

    private Vector3 spawnPoint;

    private Quaternion spawnRotation;

    // ######################### END OF VARIABLE DECLARATIONS #####################
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = trans.position;
        spawnRotation = modelTrans.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.T)) Die();
    }

    public void Respawn()
    {
        dead = false;
        trans.position = spawnPoint;
        enabled = true;
        characterController.enabled = true;
        modelTrans.rotation = spawnRotation;
        modelTrans.gameObject.SetActive(true);
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            Invoke("Respawn", respawnWaitTime);
            movementVelocity = Vector3.zero;
            enabled = false;
            characterController.enabled = false;
            modelTrans.gameObject.SetActive(false);
        }
    }

    // ###################### MOVEMENT ##########################
    private void Movement()
    {
        //If W or the up arrow key is held:
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (
                movementVelocity.z >= 0 //If we're already moving forward
            )
                //Increase Z velocity by VelocityGainPerSecond, but don't go higher
                // than 'movespeed':
                movementVelocity.z =
                    Mathf
                        .Min(movespeed,
                        movementVelocity.z +
                        VelocityGainPerSecond * Time.deltaTime); //Else if we're moving back
            else
                //Increase Z velocity by VelocityGainPerSecond, using the
                // reverseMomentumMultiplier, but don't raise higher than 0:
                movementVelocity.z =
                    Mathf
                        .Min(0,
                        movementVelocity.z +
                        VelocityGainPerSecond *
                        reverseMomentumMultiplier *
                        Time.deltaTime);
        } //If S or the down arrow key is held:
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (
                movementVelocity.z > 0 //If we're already moving forward
            )
                movementVelocity.z =
                    Mathf
                        .Max(0,
                        movementVelocity.z -
                        VelocityGainPerSecond *
                        reverseMomentumMultiplier *
                        Time.deltaTime); //If we're moving back or not moving at all
            else
                movementVelocity.z =
                    Mathf
                        .Max(-movespeed,
                        movementVelocity.z -
                        VelocityGainPerSecond * Time.deltaTime);
        } //If neither forward nor back are being held
        else
        {
            //We must bring the Z velocity back to 0 over time.
            if (
                movementVelocity.z > 0 //If we're moving up,
            )
                //Decrease Z velocity by VelocityLossPerSecond, but don't go any
                // lower than 0:
                movementVelocity.z =
                    Mathf
                        .Max(0,
                        movementVelocity.z -
                        VelocityLossPerSecond * Time.deltaTime); //If we're moving down,
            else
                //Increase Z velocity (back towards 0) by VelocityLossPerSecond,
                // but don't go any higher than 0:
                movementVelocity.z =
                    Mathf
                        .Min(0,
                        movementVelocity.z +
                        VelocityLossPerSecond * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (movementVelocity.x >= 0)
                movementVelocity.x =
                    Mathf
                        .Min(movespeed,
                        movementVelocity.x +
                        VelocityGainPerSecond * Time.deltaTime);
            else
                movementVelocity.x =
                    Mathf
                        .Min(0,
                        movementVelocity.x +
                        VelocityGainPerSecond *
                        reverseMomentumMultiplier *
                        Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (movementVelocity.x > 0)
                movementVelocity.x =
                    Mathf
                        .Max(0,
                        movementVelocity.x -
                        VelocityGainPerSecond *
                        reverseMomentumMultiplier *
                        Time.deltaTime); //If we're moving left or not moving at all
            else
                movementVelocity.x =
                    Mathf
                        .Max(-movespeed,
                        movementVelocity.x -
                        VelocityGainPerSecond * Time.deltaTime);
        } //If neither right nor left are being held
        else
        {
            //We must bring the X velocity back to 0 over time.
            if (
                movementVelocity.x > 0 //If we're moving right,
            )
                //Decrease X velocity by VelocityLossPerSecond, but don't go any
                // lower than 0:
                movementVelocity.x =
                    Mathf
                        .Max(0,
                        movementVelocity.x -
                        VelocityLossPerSecond * Time.deltaTime); //If we're moving left,
            else
                //Increase X velocity (back towards 0) by VelocityLossPerSecond,
                // but don't go any higher than 0:
                movementVelocity.x =
                    Mathf
                        .Min(0,
                        movementVelocity.x +
                        VelocityLossPerSecond * Time.deltaTime);
        }

        //If the player is moving in either direction (left/right or up/down):
        if (movementVelocity.x != 0 || movementVelocity.z != 0)
        {
            //Applying the movement velocity:
            characterController.Move(movementVelocity * Time.deltaTime);

            //Keeping the model holder rotated towards the last movement direction:
            modelTrans.rotation =
                Quaternion
                    .Slerp(modelTrans.rotation,
                    Quaternion.LookRotation(movementVelocity),
                    .18F);
        }
    }
}
