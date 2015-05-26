using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Door : MonoBehaviour
{
    Animator _doorAnimation;
    private int _isOpenHash = Animator.StringToHash("isOpen");
    public bool IsOpen;

    float _doorTimer = 5.0f;
    public bool UnitUsingDoor = false;
    public Image ProgressBar;

    // Use this for initialization
    void Start()
    {
        _doorAnimation = GetComponent<Animator>();
        IsOpen = false;
    }

    void Update()
    {
        if (UnitUsingDoor)
        {
            _doorTimer -= Time.deltaTime;
            if (_doorTimer < 0 && !IsOpen)
            {
                StartDoorOpen();
                _doorTimer = 5.0f;
                UnitUsingDoor = false;
            }
            else if (_doorTimer < 0 && IsOpen)
            {
                StartDoorClose();
                _doorTimer = 5.0f;
                UnitUsingDoor = false;
            }
            ProgressBar.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(transform.position); 
            ProgressBar.fillAmount = _doorTimer / 5;
        }
        else
        {
            ProgressBar.fillAmount = 0;
        }
        
        
    }
    public void StartDoorOpen()
    {
        _doorAnimation.SetBool(_isOpenHash, true);
        IsOpen = true;
    }

    public void StartDoorClose()
    {
        _doorAnimation.SetBool(_isOpenHash, false);
        IsOpen = false;
    }

    public bool IsDoorOpen()
    {
        return IsOpen;
    }
}
