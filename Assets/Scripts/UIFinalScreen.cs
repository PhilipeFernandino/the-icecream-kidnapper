using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFinalScreen : MonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _finalStatus;
    [SerializeField] private Canvas _canvas;

    public void Setup()
    {
        _canvas.enabled = true;
        _finalStatus.text = _finalStatus.text.Replace("$", FindObjectOfType<StealController>().TotalStealed.ToString());
    }

    private void ToggleCanvas()
    {
        _canvas.enabled = !_canvas.enabled;
        if ( _canvas.enabled )
        {
            Setup();
        }
    }

    private void Awake()
    {
        _canvas.enabled = false;
        _restartButton.onClick.AddListener(RestartButtonClickedEventHandler);
        DaytimeSystem.Instance.DayEndReached += Setup;
    }

    private void RestartButtonClickedEventHandler()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCanvas();
        }
    }
}
