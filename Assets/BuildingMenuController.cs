using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuController : MonoBehaviour
{
    [Header("Indoor Air Quality Toggles")]
    public Toggle iaqPrivate;
    public Toggle iaqMeeting;
    public Toggle iaqCommon;
    public Toggle iaqAll;

    [Header("Motion Toggles")]
    public Toggle motionPrivate;
    public Toggle motionMeeting;
    public Toggle motionCommon;
    public Toggle motionAll;

    void Start()
    {
        // IAQ
        iaqAll.onValueChanged.AddListener((value) => SetAllIAQ(value));
        iaqPrivate.onValueChanged.AddListener((_) => UpdateIAQAll());
        iaqMeeting.onValueChanged.AddListener((_) => UpdateIAQAll());
        iaqCommon.onValueChanged.AddListener((_) => UpdateIAQAll());

        // Motion
        motionAll.onValueChanged.AddListener((value) => SetAllMotion(value));
        motionPrivate.onValueChanged.AddListener((_) => UpdateMotionAll());
        motionMeeting.onValueChanged.AddListener((_) => UpdateMotionAll());
        motionCommon.onValueChanged.AddListener((_) => UpdateMotionAll());
    }

    // ---------- IAQ ----------
    void SetAllIAQ(bool value)
    {
        iaqPrivate.isOn = value;
        iaqMeeting.isOn = value;
        iaqCommon.isOn = value;
    }

    void UpdateIAQAll()
    {
        iaqAll.isOn =
            iaqPrivate.isOn &&
            iaqMeeting.isOn &&
            iaqCommon.isOn;
    }

    // ---------- Motion ----------
    void SetAllMotion(bool value)
    {
        motionPrivate.isOn = value;
        motionMeeting.isOn = value;
        motionCommon.isOn = value;
    }

    void UpdateMotionAll()
    {
        motionAll.isOn =
            motionPrivate.isOn &&
            motionMeeting.isOn &&
            motionCommon.isOn;
    }
}
