using System.Collections;
using UnityEngine;

public class Muzzleflash : MonoBehaviour
{
    [SerializeField] float aliveTime = 0.06f;

    public IEnumerator Activate()
    {
        // rotate muzzleflash on x-axis randomly
        Vector3 muzzleRotation = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(Random.Range(0, 360), muzzleRotation.y, muzzleRotation.z);
        // show muzzle
        gameObject.SetActive(true);
        // wait x seconds before hiding muzzle
        yield return new WaitForSeconds(aliveTime);
        gameObject.SetActive(false);
    }
}
