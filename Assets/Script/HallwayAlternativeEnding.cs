using System.Collections;
using UnityEngine;

public class HallwayAlternativeEnding : MonoBehaviour
{
    [SerializeField] int TriggerAmountToUnlock = 10;
    [SerializeField] UI_Fade fade;

    public void SetFrontTrigger(TriggerEndingAlternative frontCollider) 
    {
        frontCollider.OnTriggerEndAlternativeEnter += UpdateEndingProgress;
    }

    public void SetBackTrigger(TriggerEndingAlternative backCollider) 
    {
        backCollider.OnTriggerEndAlternativeEnter += UpdateEndingProgress;
    }

    private void UpdateEndingProgress(TriggerEndingAlternative trigger, TriggerEndingAlternative otherTrigger) 
    {
        // Reduce the trigger amount
        TriggerAmountToUnlock--;

        // Inverts triggers
        otherTrigger.gameObject.SetActive(true);
        trigger.gameObject.SetActive(false);

        if (TriggerAmountToUnlock <= 0)
        {
            // Enable the ending exit
            otherTrigger.ActivateAlternativeEndingExit();
            // Add alternative end game event
            otherTrigger.OnTriggerEndAlternativeEnter += EndGameAlternative;
            // Remove this event
            otherTrigger.OnTriggerEndAlternativeEnter -= UpdateEndingProgress;
            Debug.Log("Alternative ending Unlocked");
            return;
        }

        Debug.Log("Amount of triggers left: " + TriggerAmountToUnlock);
    }

    private void EndGameAlternative(TriggerEndingAlternative trigger, TriggerEndingAlternative otherTrigger)
    {
        StartCoroutine(EndGameCoroutine());
    }

    private IEnumerator EndGameCoroutine() 
    {
        UI_Fade fade = Instantiate(this.fade, Vector3.zero, Quaternion.identity, null);

        while (true)
        {
            if (fade.GetFadeValue() <= 1.0f)
            {
                yield return null;
            }

            Debug.Log("Game ended (Alternative)");
            Application.Quit();

            yield break;
        }
    }
}
