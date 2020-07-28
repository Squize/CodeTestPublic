using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class BootScript : MonoBehaviour {

//---------------------------------------------------------------------------------------
		private void Start() {
			SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
		}
	}
}
