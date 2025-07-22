using System.Collections;
using UnityEngine;

public abstract class CityBuilding : MonoBehaviour
{
    [SerializeField] private GameObject abandonedPrefab; 
    private GameObject model;
    private float height;
    
    private void Start()
    {
        height = getHeight();
        model = transform.GetChild(0).gameObject;
        model.transform.localPosition = new Vector3(0, -height, 0);
        model.SetActive(false);
        StartCoroutine(createBuilding());
    }

    IEnumerator createBuilding()
    {
        yield return new WaitForSeconds(7);
        model.SetActive(true);
        float t = 0;
        while (t < 10)
        {
            model.transform.localPosition = Vector3.Lerp(new Vector3(0, -height, 0), new Vector3(0, 0, 0), (t/10));
            t += Time.deltaTime;
            yield return null;
        }
        model.transform.localPosition = new Vector3(0, 0, 0);
        yield return null;
    }

    protected virtual float getHeight()
    {
        return 1;
    }
    
    public void Abandon()
    {
        StopAllCoroutines();
        var instance = Instantiate(abandonedPrefab, transform.position, transform.rotation, null);
        instance.transform.localScale = transform.parent.localScale;
    }
}
