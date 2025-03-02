using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Pause Menu")]
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseMenu : MonoBehaviour
    {
        public bool freezOnPause = true;
        public GameObject UI;

        public UnityEvent OnPause;
        public UnityEvent OnResume;

        public static PauseMenu Instance;
        private CanvasGroup canvasGroup;

        public bool paused { get; set; }
        public Controls controls;

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);

            if (!LoadingScreen.Instance)
                SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            Time.timeScale = 1; // Đảm bảo game chạy bình thường khi bắt đầu

        }

        private void Start()
        {
            paused = false; // Đảm bảo game bắt đầu không bị pause
            UI.SetActive(false); // Ẩn UI khi bắt đầu game

            controls = new Controls();
            controls.UI.Enable();

            controls.UI.Pause.performed += context =>
            {
                paused = !paused;

                if (paused) OnPause?.Invoke();
                if (!paused) OnResume?.Invoke();
            };

            canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }


        void Update()
        {
            // Nếu nhấn Ctrl => Pause game nhưng KHÔNG mở UI
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                paused = !paused; // Đảo trạng thái Pause
            }

            // Nếu nhấn Esc => Pause game và mở UI
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = true; // Luôn Pause khi nhấn Esc
                UI.SetActive(true); // Hiện UI menu
            }

            // Nếu đang Pause nhưng nhấn Ctrl thì ẨN UI
            if (paused && Input.GetKeyDown(KeyCode.LeftControl))
            {
                UI.SetActive(false);
            }

            // Cập nhật trạng thái chuột
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;

            // Cập nhật Time.timeScale để dừng game
            if (freezOnPause)
                Time.timeScale = paused ? 0 : 1;
        }


        public void Resume()
        {
            paused = false;  // Hủy trạng thái pause
            UI.SetActive(false);  // Ẩn menu setting
            Cursor.lockState = CursorLockMode.Locked; // Ẩn chuột
            Cursor.visible = false;
            Time.timeScale = 1;  // Tiếp tục game
            OnResume?.Invoke();  // Gọi sự kiện nếu có
        }



        public void Pause()
        {
            paused = true;
            OnPause?.Invoke();
        }
        public void Quit() => Application.Quit();
    }
}