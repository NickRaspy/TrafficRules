using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LeftHand : HandController
{
    [SerializeField] InputActionReference controllerMenuButton;

    protected override void OnEnable()
    {
        base.OnEnable();
        controllerMenuButton.action.performed += MenuPress;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        controllerMenuButton.action.performed -= MenuPress;
    }

    private void MenuPress(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(0);
    }
}
