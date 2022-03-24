using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EL.Card;
using EL.Common.Exts;
using EL.Common.Pool;
using EL.Desk;
using EL.Popups;
using EL.Res;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EL.GameCore
{
    public class BattleEndpoint : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private GamePrefabs gamePrefabs;
        [SerializeField] private LoadingUI loadingUI;
        [SerializeField] private GamePlayUI gameplayUI;
        [SerializeField] private HandContainer hand;
        [SerializeField] private TableContainer table;

        private IGameResources _gameResources;
        private IObjectPool _objectPool;
        private SelectCardsController _selectCardsController;
        private Coroutine _testCoroutine;
        private ITranslate _translate;

        private async void Start()
        {
            Random.InitState((int) DateTime.Now.Ticks);
            gameplayUI.OnTestClick += HandleTestClick;
            gameplayUI.Hide();
            loadingUI.Show();
            try
            {
                _gameResources = new LocalGameResources();
                _objectPool = new ObjectPool();
                _translate = new NoneTranslate();

                var heroes = await _gameResources.LoadAllCardDesign();
                var cards = heroes.SelectMany(el => new[]
                {
                    new CardModel
                    {
                        id = Guid.NewGuid(),
                        design = el
                    },
                    new CardModel
                    {
                        id = Guid.NewGuid(),
                        design = el
                    }
                }).ToArray();

                var cardsPool = new FixedPool<Card.Card, CardModel>("Cards",
                    new CardItemPoolObserver(gamePrefabs, _gameResources, _translate, cam));
                _objectPool.Register(cardsPool);
                cards.Shuffle();
                await cardsPool.AddRange(cards);

                _selectCardsController = new SelectCardsController(gamePrefabs, _objectPool, cam);
                loadingUI.Hide();

                var localHand = await _selectCardsController.SelectCards(4, 6);
                hand.AddCard(localHand);
                gameplayUI.Show();
            }
            catch (Exception e)
            {
                loadingUI.SetException(e);
            }
        }

        private void HandleTestClick()
        {
            if (_testCoroutine != null)
                StopCoroutine(_testCoroutine);
            else
                _testCoroutine = StartCoroutine(Test_Coroutine());
        }

        private IEnumerator Test_Coroutine()
        {
            while (true)
            {
                var cards = hand.AttachedCards;
                for (var i = cards.Length - 1; i >= 0; i--)
                {
                    var card = cards[i];
                    var value = Random.Range(-2, 10);
                    var model = card.Model;
                    model.attackModificator = value - model.design.attack;
                    model.healthModificator = value - model.design.health;
                    model.costModificator = value - model.design.cost;
                    yield return card.SetModel(model).AsUniTask().ToCoroutine();
                    if (card.Model.Health <= 0)
                    {
                        hand.RemoveCard(card);
                        card.View.OrderLayout = 10;
                        card.View.transform.DOMoveZ(-10f, .3f).onComplete += () => { _objectPool.Release(card); };
                    }

                    yield return new WaitForSeconds(1f);
                }

                if (cards.Length == 0)
                    break;
            }

            _testCoroutine = null;
        }
    }
}