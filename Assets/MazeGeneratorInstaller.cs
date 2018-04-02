using UnityEngine;
using Zenject;

public class MazeGeneratorInstaller : MonoInstaller<MazeGeneratorInstaller> {
    public override void InstallBindings() {
        Container.Bind<MazeGenerator>().To<MazeGenerator>().AsSingle();
    }
}