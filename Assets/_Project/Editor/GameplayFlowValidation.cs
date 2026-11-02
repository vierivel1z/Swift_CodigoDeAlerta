using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Swift.EditorTools
{
    public static class GameplayFlowValidation
    {
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/BancoCorresponsal.unity",OpenSceneMode.Single);
            new GameObject("Gameplay flow validation").AddComponent<Swift.Testing.GameplayFlowProbe>();
            EditorApplication.EnterPlaymode();
        }
    }
}
