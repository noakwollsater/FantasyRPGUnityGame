using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class OpenNews : MonoBehaviour
    {
        public string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

        public void OpenSite()
        {
            Application.OpenURL(url);
        }
    }
}
