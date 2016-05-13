using System.Collections;
using Destruction.Common;
using UnityEngine;

namespace Destruction
{
    public class Cannon : MonoBehaviour
    {
        public GameObject explosionPrefab;
        public Rigidbody firePrefab;

        public Transform target;
        public float distance = 10.0f;

        public float impactSize = 2;

        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        private float x;
        private float y;

        [Range(0, 50000)]
        public float force = 10000;

        private void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            UpdatePosition();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                FireCannon();
            }

            if (target && Input.GetKey(KeyCode.Mouse1))
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            var rotation = Quaternion.Euler(y, x, 0);
            var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;

        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }

            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        private void FireCannon()
        {
            if(firePrefab == null)
            {
                StartCoroutine(FireRayCannon());
            }
            else
            {
                FireAmmoCannon();
            }
        }

        private IEnumerator FireRayCannon()
        {
            RaycastHit hit;
            Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out hit, 100))
            {
                var collidersNearby = Physics.OverlapSphere(hit.point, 0.4f);
                if (explosionPrefab == null)
                {
                    foreach (Collider c in collidersNearby)
                    {
                        if (c == null) continue;

                        Rigidbody targetBody = c.attachedRigidbody;

                        if (targetBody != null)
                        {
                            targetBody.AddForceAtPosition(force*mouseRay.direction, hit.point);
                        }
                    }

                    yield return new WaitForSeconds(0.01f);

                    foreach (Collider c in collidersNearby)
                    {
                        if (c == null) continue;

                        Component hitDestructable = c.GetComponent(typeof (IDestructable));

                        if (hitDestructable is IDestructable)
                        {
                            (hitDestructable as IDestructable).Destroy(hit.point, impactSize);
                        }
                    }
                }
                else
                {
                    Instantiate(explosionPrefab, hit.point, Quaternion.LookRotation(Vector3.Cross(hit.normal, transform.right), hit.normal));
                }
            }
        }

        private void FireAmmoCannon()
        {
            Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            var ammo = Instantiate(firePrefab, mouseRay.origin, transform.rotation) as Rigidbody;
            ammo.AddForce(mouseRay.direction * force);
        }
    }
}

