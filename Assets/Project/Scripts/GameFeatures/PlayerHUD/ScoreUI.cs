using System;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace StartledSeal.Project.Scripts.GameFeatures.PlayerHUD
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtScore;

        private void Start()
        {
            UpdateScore();
        }

        public async void UpdateScore()
        {
            await UniTask.WaitForEndOfFrame(this); // wait for game logic to run
            _txtScore.text = GameManager.Instance.Score.ToString();        
        }
    }
}