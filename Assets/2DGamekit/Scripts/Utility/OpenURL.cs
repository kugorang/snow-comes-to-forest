#region

using UnityEngine;

#endregion

public class OpenURL : MonoBehaviour
{
    public string websiteAddress;

    public void OpenURLOnClick()
    {
        Application.OpenURL(websiteAddress);
    }
}