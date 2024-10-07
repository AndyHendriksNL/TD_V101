using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Voor gebruik van layouts

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI instance;

    public TextMeshProUGUI notificationPrefab; // Prefab van de notificatietekst
    public Transform notificationContainer; // Container (bijv. een Panel) waarin de meldingen komen
    public float fadeDuration = 2f;  // Duur van de fade
    public float displayDuration = 3f;  // Hoe lang de melding zichtbaar blijft

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        notificationPrefab.enabled = false;
    }

    public void SetNotificationMessage(string message)
    {
        // Maak een nieuwe notificatie door de prefab te klonen
        TextMeshProUGUI newNotification = Instantiate(notificationPrefab, notificationContainer);
        newNotification.text = message;
        newNotification.alpha = 1f;
        newNotification.enabled = true;

        // Start de coroutine voor het automatisch laten wegfaden van de melding
        StartCoroutine(FadeOutAndDestroy(newNotification));
    }

    private IEnumerator FadeOutAndDestroy(TextMeshProUGUI notificationText)
    {
        // Wacht voor de aangegeven tijd voordat de fade begint
        yield return new WaitForSeconds(displayDuration);

        // Voer de fade uit over een aantal seconden
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // Bereken de nieuwe alpha-waarde op basis van hoe ver de fade is gevorderd
            notificationText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null; // Wacht tot de volgende frame
        }

        // Verwijder de notificatie nadat de fade is voltooid
        Destroy(notificationText.gameObject);
    }
}
