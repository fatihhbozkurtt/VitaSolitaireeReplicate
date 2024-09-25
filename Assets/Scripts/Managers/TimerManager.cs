using TMPro;
using UnityEngine;

namespace Managers
{
    public class TimerManager : MonoSingleton<TimerManager>
    {
        [Header("References")] [SerializeField]
        private TextMeshProUGUI timerText;

        private float totalTime; // Starting time (starts at 0)

        private bool isTimerRunning = false;

        void Start()
        {
            GameManager.instance.LevelStartedEvent += StartTimer;
        }

        private void Update()
        {
            if (!GameManager.instance.isLevelActive) return;
            if (!isTimerRunning) return;

            totalTime += Time.deltaTime; // Increment the time
            UpdateTimerDisplay(totalTime);
        }

        // Starts the timer
        private void StartTimer()
        {
            isTimerRunning = true;
        }

        public void StopTimer()
        {
            isTimerRunning = false;
        }

        void UpdateTimerDisplay(float timeToDisplay)
        {
            int minutes = Mathf.FloorToInt(timeToDisplay / 60); // Get the minutes
            int seconds = Mathf.FloorToInt(timeToDisplay % 60); // Get the seconds

            // Update the UI text in minutes:seconds format
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}