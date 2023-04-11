using System.Collections;
using UnityEngine;
using Utility.Random;
using Utility.EventCommunication;

namespace Utility.SceneCamera.Camera2D
{
    public class CameraShake : MonoBehaviour
    {
        Coroutine routine;

        Vector3 seed;

        Vector2 offSet;
        [SerializeField] Vector2 offSetMultiplier;

        float zAngle;
        [SerializeField] float angleMultiplier;

        float growth;
        [SerializeField] float decay;
        [SerializeField] float speed;

        void Awake()
        {
            seed = RandomStream.NextVector3(-1000, 1000);
            EventHub.Subscribe(EventList.ShakeCamera, Shake);
        }

        void OnDestroy()
        {
            EventHub.UnSubscribe(EventList.ShakeCamera, Shake);
        }

        void Shake(EventData data)
        {
            float intensity = (float)data.eventInformation;
            Shake(intensity);
        }

        public void Shake(float intensity)
        {
            if(routine != null)
            { StopCoroutine(routine); }
            routine = StartCoroutine(ShakeCycle(intensity));
        }

        IEnumerator ShakeCycle(float intensity)
        {
            while(intensity > 0.0f)
            {
                zAngle = intensity * angleMultiplier * ShakeFunction(seed.z, Time.time * speed);
                offSet.x = intensity * offSetMultiplier.x * ShakeFunction(seed.x, Time.time * speed);
                offSet.y = intensity * offSetMultiplier.y * ShakeFunction(seed.y, Time.time * speed);
                transform.rotation = Quaternion.Euler(0, 0, zAngle);
                transform.position += new Vector3(offSet.x, offSet.y);
                intensity -= decay * Time.deltaTime;
                yield return null;
            }
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        float ShakeFunction(float seed, float time)
        {
            return (1 - 2 * Mathf.PerlinNoise(seed + time, seed + time));
        }
    }
}
