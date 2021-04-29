using UniRx;

public class TitleSceneManager : PresenterMonoBehaviour
{
    private void Start()
    {
        Disposables.Add(GroupTitle.Instance.OnClickStart.Subscribe(_ =>
        {
            SceneController.Instance.ChangeScene("TitleScene", "PrepareScene");
        }));

        Disposables.Add(GroupTitle.Instance.OnClickSettings.Subscribe(_ =>
        {
            SceneController.Instance.ChangeScene("TitleScene", "SettingsScene");
        }));
    }
}
