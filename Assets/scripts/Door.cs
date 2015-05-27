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
    public Image progressBarPrefab;
    private Image progressBarClone;

    // Use this for initialization
    void Start()
    {
        progressBarClone = (Image)Instantiate(progressBarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        progressBarClone.rectTransform.parent = FindObjectOfType<Canvas>().transform;
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
            if (progressBarClone != null)
            {
                progressBarClone.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(transform.position); 
                progressBarClone.fillAmount = _doorTimer / 5;
            }
        }
        else
        {
            if (progressBarClone != null)
            {
                progressBarClone.fillAmount = 0;
            }
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
