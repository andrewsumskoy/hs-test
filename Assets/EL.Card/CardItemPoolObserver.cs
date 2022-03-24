using EL.Common.Pool;
using EL.Res;
using UnityEngine;

namespace EL.Card
{
    public class CardItemPoolObserver : IPoolItemObserver<Card, CardModel>
    {
        private readonly Camera _cam;
        private readonly GamePrefabs _gamePrefabs;
        private readonly IGameResources _gameResources;

        private readonly Transform _root;
        private readonly ITranslate _translate;

        public CardItemPoolObserver(
            GamePrefabs gamePrefabs,
            IGameResources gameResources,
            ITranslate translate,
            Camera cam)
        {
            _gameResources = gameResources;
            _gamePrefabs = gamePrefabs;
            _translate = translate;
            _cam = cam;

            var go = new GameObject("[Card.bundle]");
            _root = go.transform;
        }

        public Card Create(CardModel source)
        {
            var instance = _gamePrefabs.Get<CardView>(GamePrefabs.ItemType.Card);
            instance.gameObject.SetActive(false);
            instance.gameObject.transform.SetParent(_root, false);
            instance.transform.position = new Vector3(-1000, -1000, -1000);
            return new Card(instance, source, _gameResources, _translate, _cam);
        }

        public void AfterTake(Card item)
        {
            item.View.gameObject.SetActive(true);
        }

        public void AfterReturn(Card item)
        {
            item.View.DetachView();
            item.View.transform.position = new Vector3(-1000, -1000, -1000);
            item.View.gameObject.transform.SetParent(_root, false);
            item.View.gameObject.SetActive(false);
        }
    }
}