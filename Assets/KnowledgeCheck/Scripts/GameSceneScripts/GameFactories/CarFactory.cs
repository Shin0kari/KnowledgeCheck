using UnityEngine;
using Zenject;

public class CarFactory : AbstractFactoryStarter, IInitializable
{
    private Car _playerCar;
    readonly Car.Factory _carFactory;
    private readonly IGetGameData _gameData;

    private GameObject _playerCarPrefab;

    public CarFactory(
        Car.Factory carFactory,
        IGetGameData gameData,
        GameObject playerCarPrefab
    )
    {
        _carFactory = carFactory;
        _gameData = gameData;
        _playerCarPrefab = playerCarPrefab;
    }

    // Спавн машины
    public void Initialize()
    {
        if (!_isFactoryActive)
        {
            return;
        }
        if (_playerCar != null)
        {
            return;
        }

        // добавить в saveData данные о playerCar
        var saveData = _gameData.GetCurrentGameData().Item2;
        _playerCar = _carFactory.Create(_playerCarPrefab);

        // заполнение данных из saveData
    }
}
