using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EL.Common.Pool;
using EL.Res;
using UnityEngine;

namespace EL.Popups
{
    public class SelectCardsController
    {
        private readonly Camera _camera;
        private readonly IPool<Card.Card> _cardPool;
        private readonly GamePrefabs _gamePrefabs;
        private List<Card.Card> _cards;
        private bool[] _selectedChecks;

        public SelectCardsController(GamePrefabs gamePrefabs, IObjectPool pool, Camera camera)
        {
            _gamePrefabs = gamePrefabs;
            _cardPool = pool.GetPoll<Card.Card>();
            _camera = camera;
        }

        public async Task<Card.Card[]> SelectCards(int minCount, int maxCount)
        {
            var view = _gamePrefabs.Get<SelectCardsView>(GamePrefabs.ItemType.SelectHandDialog);
            view.Show(_camera);

            CollectCards(Random.Range(minCount, maxCount + 1));

            await view.SetViews(_cards);
            //while (!view.IsComplete)
            await UniTask.Delay(1500);

            var toReturn = new List<Card.Card>(_cards.Count);
            var selected = new List<Card.Card>(_cards.Count);

            for (var i = 0; i < _cards.Count; i++)
            {
                _cards[i].View.OnClick -= OnCardClick;
                if (_selectedChecks[i])
                    selected.Add(_cards[i]);
                else
                    toReturn.Add(_cards[i]);
            }

            view.Reset();
            foreach (var item in toReturn)
                _cardPool.Release(item);
            _gamePrefabs.Return(view.gameObject);

            return selected.ToArray();
        }

        private void CollectCards(int count)
        {
            _cards = new List<Card.Card>(count);
            for (var i = 0; i < count; i++)
            {
                if (!_cardPool.CanTake())
                    break;
                _cards.Add(_cardPool.Take());
            }

            _selectedChecks = new bool[_cards.Count];

            for (var i = 0; i < _cards.Count; i++)
            {
                _selectedChecks[i] = true;
                _cards[i].View.OnClick += OnCardClick;
                _cards[i].View.IsSelected = _selectedChecks[i];
            }
        }

        private void OnCardClick(Card.Card card)
        {
            var index = _cards.IndexOf(card);
            card.View.IsSelected = _selectedChecks[index] = !_selectedChecks[index];
        }
    }
}