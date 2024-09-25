using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Managers
{
    public class UndoManager : MonoSingleton<UndoManager>
    {
        public event System.Action AnyKindOfMovementPerformedEvent;
        [Header("Debug")] [SerializeField] private List<MovePathData> movePathData;

        private void AddNewMove(MovePathData newMovePath)
        {
            if (movePathData.Contains(newMovePath)) return;

            movePathData.Add(newMovePath);
            AnyKindOfMovementPerformedEvent?.Invoke();
        }

        private void Start()
        {
            DeckManager.instance.NewMovementPerformedEvent += (OnNewMovementPerformed);
        }

        private void OnNewMovementPerformed(MovePathData newPathData)
        {
            AddNewMove(newPathData);
        }

        public void PerformUndo()
        {
            if (movePathData.Count == 0) return;

            AnyKindOfMovementPerformedEvent?.Invoke();
            MovePathData data = movePathData[^1];


            // check if multiple cards moved
            data.card.MoveForUndo(data.from);
            movePathData.Remove(data);
        }
 
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformUndo();
            }
        }
    }
}