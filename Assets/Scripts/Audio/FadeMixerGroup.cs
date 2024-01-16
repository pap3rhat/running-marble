/* 
 * TAKEN FROM : https://johnleonardfrench.com/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/ 
 * Slightly modified.
*/

using System.Collections;
using UnityEngine.Audio;
using UnityEngine;
public static class FadeMixerGroup
{
    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;

            var newVol = Mathf.Lerp(currentVol, targetVolume, currentTime / duration);

            audioMixer.SetFloat(exposedParam, newVol);
            yield return null;
        }
        yield break;
    }
}