using UnityEngine.SceneManagement;

namespace HereticalSolutions.FSM.Unity
{
    public class SceneState
        : IState
    {
        public Scene Scene { get; protected set; }
        
        public string SceneID { get; protected set; }

        public bool SetAsActiveOnLoad { get; protected set; }

        public SceneState(
            string sceneID,
            bool setAsActiveOnLoad = true)
        {
            SceneID = sceneID;

            Scene = SceneManager.GetSceneByName(sceneID);

            SetAsActiveOnLoad = setAsActiveOnLoad;
        }

        #region IState

        public void EnterState()
        {
            if (!Scene.isLoaded)
            {
                SceneManager.LoadScene(
                    SceneID,
                    LoadSceneMode.Additive);
            }

            if (SetAsActiveOnLoad)
            {
                SceneManager.SetActiveScene(
                    Scene);
            }
        }

        public void EnterState(
            ITransitionRequest transitionRequest)
        {
            EnterState();
        }

        public void ExitState()
        {
            if (Scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(
                    SceneID);
            }
        }

        public void ExitState(
            ITransitionRequest transitionRequest)
        {
            ExitState();
        }

        #endregion
    }
}