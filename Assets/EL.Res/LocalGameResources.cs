using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EL.Res
{
    public class LocalGameResources : IGameResources
    {
        private static readonly string CARDS_LOCATION = "cards/";

        private readonly Dictionary<string, CardDesignLocalStored> _loadedDesignData =
            new Dictionary<string, CardDesignLocalStored>();

        public async Task<CardDesign[]> LoadAllCardDesign()
        {
            var listAsset = (TextAsset) await Resources.LoadAsync<TextAsset>($"{CARDS_LOCATION}cards_list");
            var toLoad =
                listAsset.text
                    .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => Resources.LoadAsync<CardDesignLocalStored>(CARDS_LOCATION + path).ToUniTask());
            var loadedObjects = await UniTask.WhenAll(toLoad);
            var loaded = loadedObjects.OfType<CardDesignLocalStored>().ToArray();

            var forSpriteLoad = new List<string>();
            foreach (var heroesDesignLocalStored in loaded)
            {
                if (heroesDesignLocalStored.mainImage == null)
                    forSpriteLoad.Add(heroesDesignLocalStored.data.id);
                _loadedDesignData[heroesDesignLocalStored.data.id] = heroesDesignLocalStored;
            }

            if (forSpriteLoad.Count > 0)
            {
                var sprites = await LoadRemoteSprites(forSpriteLoad);
                foreach (var pair in sprites)
                    loaded.First(el => el.data.id == pair.Key).mainImage = pair.Value;
            }

            return loaded.Select(el => el.data).ToArray();
        }

        public Sprite GetCardSprite(string designId)
        {
            return _loadedDesignData[designId].mainImage;
        }

        private async Task<Dictionary<string, Sprite>> LoadRemoteSprites(IEnumerable<string> heroesId)
        {
            var downloaded = await heroesId.Select(async id =>
            {
                using var uwr = new UnityWebRequest("https://picsum.photos/512/512.jpg", UnityWebRequest.kHttpVerbGET);
                uwr.downloadHandler = new DownloadHandlerTexture();
                await uwr.SendWebRequest();
                return (id, DownloadHandlerTexture.GetContent(uwr));
            }).ToArray();

            var ret = downloaded.ToDictionary(
                el => el.id,
                el => CreateSpriteForCard(el.Item2));
            return ret;
        }

        private Sprite CreateSpriteForCard(Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                192.0f);
        }
    }
}