using UnityEngine;

public class TriggerEndingAlternative : MonoBehaviour
{
    public delegate void OnTriggerEndAlternative(TriggerEndingAlternative trigger, TriggerEndingAlternative otherTrigger);
    public event OnTriggerEndAlternative OnTriggerEndAlternativeEnter;

    TriggerEndingAlternative otherTrigger;

    public void SetOtherTrigger(TriggerEndingAlternative otherTrigger) 
    {
        this.otherTrigger = otherTrigger;
    }

    public void ActivateAlternativeEndingExit()
    {
        transform.Find("Alternative_Ending_Exit").gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEndAlternativeEnter?.Invoke(this, otherTrigger);
    }

}
