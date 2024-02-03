using StartledSeal.Utils.Extension;
using UnityEngine;

namespace StartledSeal
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Score { get; private set; }

        public void AddScore(int score)
        {
            Score += score;
        }
    }
}