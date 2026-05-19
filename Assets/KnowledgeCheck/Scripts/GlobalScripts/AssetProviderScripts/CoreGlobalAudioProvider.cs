using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class CoreGlobalAudioProvider : AbstractDataSOProvider
{
    protected override Type DataType => typeof(GlobalAudioSO);

    [Inject]
    protected override void Construct(CoreContextProvider coreContextProvider)
    {
        _coreContextProvider = coreContextProvider;
    }
}