using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private Camera camera;
    private Ray ray;
    float xRot = 0;
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");
        transform.Translate(h * Time.deltaTime * 5f, 0, v * Time.deltaTime * 5f);
        xRot -= mY;  xRot = Mathf.Clamp(xRot, -90, 90);
        camera.transform.localEulerAngles = new Vector3(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mX);

        RaycastHit hit;
        ray = new Ray(camera.transform.position, camera.transform.forward);
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 50f))
        {
            if (hit.transform.parent.CompareTag("Button"))
            {
                hit.transform.parent.GetComponent<Image>().color = Color.red;
                if(Input.GetMouseButtonDown(0))
                {
                    Debug.Log(hit.transform.parent.name);
                    hit.transform.parent.GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(ray);
    }
}
