using System.Threading.Tasks;
using UnityEngine;

namespace EL.Res
{
    public interface IGameResources
    {
        Task<CardDesign[]> LoadAllCardDesign();
        Sprite GetCardSprite(string designId);
    }
}