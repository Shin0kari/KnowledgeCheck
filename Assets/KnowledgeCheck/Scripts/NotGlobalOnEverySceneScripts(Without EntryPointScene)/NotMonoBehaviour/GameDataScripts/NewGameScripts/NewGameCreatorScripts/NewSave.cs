// using Cysharp.Threading.Tasks;
// using UnityEngine;
// using Zenject;
// using static SceneUtils;

// public class NewSave
// {
//     private IGetGameData _gameData;
//     private GameDataChanger _gameDataChanger;
//     private UpdateSaveFromInventory _saveUpdater;

//     private bool _isCurrentSaveAvailable;

//     [Inject]
//     private void Construct(
//         IGetGameData gameData,
//         GameDataChanger gameDataChanger,
//         UpdateSaveFromInventory saveUpdater
//     )
//     {
//         _gameData = gameData;
//         _gameDataChanger = gameDataChanger;
//         _saveUpdater = saveUpdater;
//     }

//     public void StartProcess()
//     {
//         StartAsyncProcess().Forget();
//     }

//     private async UniTask StartAsyncProcess()
//     {
//         _isCurrentSaveAvailable = _gameData.GetCurrentGameData().uuid == null;
//         _gameDataChanger.CreateSaveWithCurrentData();
//     }
// }
