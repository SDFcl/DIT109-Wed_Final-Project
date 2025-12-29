using UnityEngine;

public class ApiMangerInScene : MonoBehaviour
{
    private void Start()
    {
        ApiManager.instance.CRlevel_id = ApiManager.instance.CRsceneName();
    }
}
