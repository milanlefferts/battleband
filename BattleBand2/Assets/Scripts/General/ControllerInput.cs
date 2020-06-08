using System;
using UnityEngine;
using System.Linq;

namespace General.ControllerInput
{
    public enum Dpad { Up, Right, Down, Left, None };

    public class ControllerInput
    {
        public KeyCode buttonUp, buttonDown, buttonRight, buttonLeft, dpadUp, dpadDown, dpadRight, dpadLeft, buttonLeftTrigger, buttonRestart, buttonMenu, buttonRightTrigger;
        public bool up, down, right, left;
        public bool hasGamepad;
        private bool dpadAxisInUse;
        private string dpadAxisX, dpadAxisY;
        public Dpad currentDirection;

        public void SetupControls()
        {
            string[] gamePads = Input.GetJoystickNames();
            if (gamePads.Length > 0)
            {
                foreach (string gamepad in gamePads)
                {
                    if (gamepad == "Wireless Controller") // PS4 controls
                    {
                        SetupPS4Controller();
                        //SetupKeyboardControls(); // TEST
                        Debug.Log(gamepad + " detected. Setting PS4 controls.");
                        break;
                    }
                    else if (gamepad == "Controller (Xbox One For Windows)" || gamepad == "Controller (XBOX 360 For Windows)")
                    {
                        SetupXBOX360Controller();
                        Debug.Log(gamepad + " detected. Setting XBOX360 controls.");
                        break;
                    }
                }
            }

            if (!hasGamepad)    // Set Keyboard Controls
            {
                Debug.Log(" No gamepad detected. Setting Keyboard controls.");
                SetupKeyboardControls();
            }

            currentDirection = Dpad.None; // Resets base input direction
        }

        private void SetupKeyboardControls()
        {

            hasGamepad = false;
            EventManager.Instance.SetControllerUI(0);

            dpadAxisX = "";
            dpadAxisY = "";

            buttonLeftTrigger = KeyCode.LeftShift;
            buttonRightTrigger = KeyCode.RightShift;
            buttonRestart = KeyCode.Backspace;
            buttonMenu = KeyCode.Escape;

            buttonUp = KeyCode.W;
            buttonDown = KeyCode.S;
            buttonRight = KeyCode.D;
            buttonLeft = KeyCode.A;

            dpadUp = KeyCode.UpArrow;
            dpadDown = KeyCode.DownArrow;
            dpadRight = KeyCode.RightArrow;
            dpadLeft = KeyCode.LeftArrow;
        }

        private void SetupPS4Controller()
        {

            hasGamepad = true;
            EventManager.Instance.SetControllerUI(2);


            dpadAxisX = "PS4_dpadAxisX";
            dpadAxisY = "PS4_dpadAxisY";

            buttonLeftTrigger = KeyCode.Joystick1Button4;
            buttonRightTrigger = KeyCode.Joystick1Button5;

            buttonRestart = KeyCode.Joystick1Button8;
            buttonMenu = KeyCode.Joystick1Button9;

            buttonUp = KeyCode.Joystick1Button3;
            buttonDown = KeyCode.Joystick1Button1;
            buttonRight = KeyCode.Joystick1Button2;
            buttonLeft = KeyCode.Joystick1Button0;

            dpadUp = KeyCode.None;
            dpadDown = KeyCode.None;
            dpadRight = KeyCode.None;
            dpadLeft = KeyCode.None;
        }

        private void SetupXBOX360Controller()
        {

            hasGamepad = true;
            EventManager.Instance.SetControllerUI(1);

            dpadAxisX = "XBOX360_dpadAxisX";
            dpadAxisY = "XBOX360_dpadAxisY";

            buttonLeftTrigger = KeyCode.Joystick1Button4;
            buttonRightTrigger = KeyCode.Joystick1Button5;

            buttonRestart = KeyCode.Joystick1Button6;
            buttonMenu = KeyCode.Joystick1Button7;

            buttonUp = KeyCode.Joystick1Button3;
            buttonDown = KeyCode.Joystick1Button0;
            buttonRight = KeyCode.Joystick1Button1;
            buttonLeft = KeyCode.Joystick1Button2;

            dpadUp = KeyCode.None;
            dpadDown = KeyCode.None;
            dpadRight = KeyCode.None;
            dpadLeft = KeyCode.None;
        }

        public void DpadInputCheck()
        {
            if (Input.GetAxisRaw(dpadAxisX) == -1)
            {
                if (!dpadAxisInUse)
                {
                    left = true;
                    currentDirection = Dpad.Left;
                    dpadAxisInUse = true;
                }
            }
            else if (Input.GetAxisRaw(dpadAxisX) == 1)
            {
                if (!dpadAxisInUse)
                {
                    right = true;
                    currentDirection = Dpad.Right;
                    dpadAxisInUse = true;
                }
            }
            else if (Input.GetAxisRaw(dpadAxisY) == 1)
            {
                if (!dpadAxisInUse)
                {
                    up = true;
                    currentDirection = Dpad.Up;
                    dpadAxisInUse = true;
                }
            }
            else if (Input.GetAxisRaw(dpadAxisY) == -1)
            {
                if (!dpadAxisInUse)
                {
                    down = true;
                    currentDirection = Dpad.Down;
                    dpadAxisInUse = true;
                }
            }
            else // Resets dpad input
            {
                dpadAxisInUse = false;
                left = false;
                up = false;
                right = false;
                down = false;
                currentDirection = Dpad.None;
            }
        }
    }
}