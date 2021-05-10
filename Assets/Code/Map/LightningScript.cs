using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightningScript : MonoBehaviour
{
    GameObject lightning1;
    public GameObject Lightning, marker1, marker2;
    float x;    
    private void Start()
    {

        //lightning1 = this.gameObject;
        StartCoroutine(LightningCoroutine());
    }

    IEnumerator LightningCoroutine()
    {
        x = Random.Range(marker1.transform.position.x, marker2.transform.position.x);
        if (lightning1 != null)
            Destroy(lightning1);
        lightning1 = Instantiate(Lightning, new Vector3(x, marker1.transform.position.y, marker1.transform.position.z), Quaternion.Euler(90,0,0), this.gameObject.transform);
        yield return new WaitForSeconds(Random.Range(6,10));
        StartCoroutine(LightningCoroutine());
    }
    
    // Start is called before the first frame update
    void Update()
    {
        
    }    
   
}
