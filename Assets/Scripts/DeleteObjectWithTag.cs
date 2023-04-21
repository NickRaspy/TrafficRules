using UnityEngine;

[RequireComponent(typeof(Collider))] // ������� ������� ���������� ������� �� �������
public class DeleteObjectWithTag : MonoBehaviour
{
    public string objectTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(objectTag))
        {
            Destroy(other.gameObject);
        }
    }
}
