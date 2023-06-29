# UnityNewInputSystemExercise
This repo contains unity new input system character movement exercise.

## Getting Started
These instructions will help you get started with playing the game on your local machine.

### The following software needs to be installed for the project to run:
- Unity (version: 2022.2.21f1)

## Gameplay
Here are the some instructions and keys of the game:

* Walk: W-A-S-D
* Run: left-Shift + Walk
* Jump: Space
* Double-Jump: Space during Jump
* Squat and take the key if exists: left-Ctrl
* Dash: E
* Open-Close the doors: F
* Change the look-up: Mouse


https://github.com/MGurcan/UnityNewInputSystemExercise/assets/78200658/d878033e-4682-4b50-b1f8-d9e1eda16bf7

# Difficulties Encountered and Solutions
----------------------------------------------------------------------------
#### 1. Creating 3D-Animation
My biggest problem was not being proficient in creating 3D animations. I managed to overcome this problem to some extent by copying and modifying animations from the ready-made free asset model I obtained. However, it is still difficult to say that they work very efficiently.
#### 2. Animation problem between run and walk during energy bar is over
To address the issue with the running and walking animation rapidly switching back and forth due to the user spamming the left-shift key, a cooldown mechanism was implemented. Now, when the energy bar is depleted, the player will have to wait for 3 seconds before being able to run again. During this cooldown period, the energy bar will gradually refill to a certain extent. This solution effectively resolves the animation problem and ensures a smoother transition between running and walking actions.
#### 3. Double Jump was spamming and cannot detect if space is pressed 2nd time or just holding down
During Double-Jump implementation; if the player spams Space or hold down Space it was spamming the double jump. Somehow I need to check the space is pressed more than one time or just spamming it down.
To check if the space is pressed then released and again pressed during space -> double jump following code added to implementation
    ```
    
        float jumpInputValue = input_control.Player_map.Jump.ReadValue<float>();

        if (jumpInputValue == 0 && isJumped)
            enableDoubleJump = true;

        if (jumpInputValue > 0)
        {
            if (!isJumped && isgrounded)
            {
                Playerjump();
                isJumped = true;
            }
            else if (enableDoubleJump && isJumped && cdDoubleJump.currCD <= 0)
            {
                PlayerDoubleJump();
                isJumped = false;
            }
        }
        
    ```
  In that way, I was able to check if the player is jumping and released the Space with this:
  if (jumpInputValue == 0 && isJumped)
            enableDoubleJump = true;
  #### 4. Pickup Keys From the Ground
  I had used functions like OnTriggerEnter and OnCollisionEnter during key pickup, but with these functions, I could only collect the key at the moment of the first collision. Therefore, it was not possible to pick up the key by simply hovering over it with an additional key (in this game, CTRL). I could only collect the key through collision without any extra action.
   
    ```
    
    private void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("Key") && isSquated)
      {
        Debug.Log("Key Detected");
        Destroy(other.gameObject);
      }
    }
    
    ```

    //Then I decided to use Raycast, and this way the problem was resolved.

    ```
    private void PickKey()
    {
        var layermask = 6;  //key layer
        layermask = ~layermask;
        var ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 5, layermask))
        {
            if (isSquated)
            {
                Destroy(hit.transform.gameObject);
                numberOfKeys += 1;
                KeyNumberText.text = "Keys: " + numberOfKeys;
            }      
        }
    }

    ```

  #### 5. Door Open-Close Spamming:
  Similar to problem-2(animation spamming) I added cooldown to solve this problem too.

  ```
IEnumerator RotateChildObject(Transform childTransform, float delay)
    {
        isRotating = true;
        Vector3 currentRotation = childTransform.localRotation.eulerAngles;

        if (currentRotation.y == 0 && numberOfKeys > 0)   //open
        {
            childTransform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y + 80f, currentRotation.z);

            numberOfKeys -= 1;
            KeyNumberText.text = "Keys: " + numberOfKeys;
            yield return new WaitForSeconds(delay);
        }
        else if (currentRotation.y == 80)   //close
        {
            childTransform.localRotation = Quaternion.Euler(currentRotation.x, 0f, 0);
            yield return new WaitForSeconds(delay);
        }
        isRotating = false;
    }
```
