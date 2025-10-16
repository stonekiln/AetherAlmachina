using UnityEngine;

public class CardController : MonoBehaviour
{
    public int[] playerDeck;
    public int[] enemyDeck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        CardMaker cardMaker = new();
        playerDeck = cardMaker.playerDeck;
        enemyDeck = cardMaker.enemyDeck;
    }
}

public class CardMaker
    {
        int deckNumber = 27;
        public int[] playerDeck { get; private set; }
        public int[] enemyDeck { get; private set; }

        public CardMaker()
        {
            int[] deck = new int[deckNumber];

            //14はjoker
            for (int i = 0; i < deck.Length; i++)
            {
                deck[i] = (int)Mathf.Ceil((i + 1) / 2);
            }

            playerDeck = FisherYates(deck);
            enemyDeck = FisherYates(deck);
        }

        int[] FisherYates(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int RandomNumber = Random.Range(0, i);
                (array[i], array[RandomNumber]) = (array[RandomNumber], array[i]);
            }
            return array;
        }
    }