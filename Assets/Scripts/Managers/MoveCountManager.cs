using TMPro;
using UnityEngine;

namespace Managers
{
    public class MoveCountManager : MonoSingleton<MoveCountManager>
    {
        [Header("References")] [SerializeField]
        private TextMeshProUGUI moveCountText; 

        private int moveCount;

        private void Start()
        {
            UpdateMoveCountUI();
            UndoManager.instance.AnyKindOfMovementPerformedEvent += IncrementMoveCount;
        }

        public void IncrementMoveCount()
        {
            moveCount++;
            UpdateMoveCountUI();
        }
        private void UpdateMoveCountUI()
        {
            moveCountText.text = "Moves: " + moveCount;
        }
    }
}