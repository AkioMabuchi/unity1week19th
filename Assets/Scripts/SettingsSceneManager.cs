using UniRx;

public class SettingsSceneManager : PresenterMonoBehaviour
{
    private void Start()
    {
        Disposables.Add(GroupSettings.Instance.OnValueChangedMusic.Subscribe(value =>
        {
            SoundVolumeModel.Instance.SetMusicVolume(value);
        }));
        
        Disposables.Add(GroupSettings.Instance.OnValueChangedSound.Subscribe(value =>
        {
            SoundVolumeModel.Instance.SetSoundVolume(value);
        }));
        
        Disposables.Add(GroupSettings.Instance.OnPointerUpSound.Subscribe(_ =>
        {
            // 効果音調整用のスライダーが離されると、テスト効果音を鳴らす。
            SoundPlayer.Instance.PlaySound(0);
        }));
        
        Disposables.Add(GroupSettings.Instance.OnClickFinish.Subscribe(_ =>
        {
            SceneController.Instance.ChangeScene("SettingsScene", "TitleScene");
        }));
        
        Disposables.Add(SoundVolumeModel.Instance.MusicVolume.Subscribe(value =>
        {
            GroupSettings.Instance.SetMusicVolume(value);
        }));

        Disposables.Add(SoundVolumeModel.Instance.SoundVolume.Subscribe(value =>
        {
            GroupSettings.Instance.SetSoundVolume(value);
        }));
    }
}
