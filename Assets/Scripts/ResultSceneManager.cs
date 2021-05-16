using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ResultSceneManager : PresenterMonoBehaviour
{
    private GameResult _result = GameResult.Draw;
    private IEnumerator Start()
    {
        Disposables.Add(GroupResult.Instance.OnClickTitle.Subscribe(_ =>
        {
            SoundPlayer.Instance.PlaySound(1);
            TitleMusicPlayer.Instance.StartMusic();
            SceneController.Instance.ChangeScene("ResultScene", "TitleScene");
        }));
        
        Disposables.Add(GroupResult.Instance.OnClickPrepare.Subscribe(_ =>
        {
            SoundPlayer.Instance.PlaySound(1);
            TitleMusicPlayer.Instance.StartMusic();
            SceneController.Instance.ChangeScene("ResultScene", "PrepareScene");
        }));
        
        Disposables.Add(GroupResult.Instance.OnClickRetry.Subscribe(_ =>
        {
            SoundPlayer.Instance.PlaySound(1);
            TitleMusicPlayer.Instance.StartMusic();
            SceneController.Instance.ChangeScene("ResultScene", "MainScene");
        }));
        
        Disposables.Add(ResultModel.Instance.Result.Subscribe(result =>
        {
            _result = result;
            GroupResult.Instance.DrawResult(result);
        }));
        
        Disposables.Add(AvatarModel.Instance.AvatarId.Subscribe(avatarId =>
        {
            GroupResult.Instance.DrawAvatar(avatarId);
        }));
        
        yield return new WaitForSeconds(1.0f);
        
        GroupFadeEffect.Instance.FadeOut();
        
        switch (_result)
        {
            case GameResult.Win:
                ResultMusicPlayer.Instance.StartMusicWin();
                break;
            case GameResult.Lose:
                ResultMusicPlayer.Instance.StartMusicLose();
                break;
        }
        
        yield return new WaitForSeconds(2.0f);
        
        GroupResult.Instance.ShowButtons();
    }
}
