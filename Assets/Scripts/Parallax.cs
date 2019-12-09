using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform background;
    public float parallaxScale;
    public float velocidade;

    public Transform cam;
    private Vector3 previewCamPosition;

    void Start()
    {
        cam = Camera.main.transform;
        previewCamPosition = cam.position;
    }

    void LateUpdate()
    {
        float ParallaxX = (previewCamPosition.x - cam.position.x) * parallaxScale;
        float bgTargetX = background.position.x + ParallaxX;

        Vector3 bgPos = new Vector3(bgTargetX, background.position.y, background.position.z);
        background.position = Vector3.Lerp(background.position, bgPos, velocidade * Time.deltaTime);

        previewCamPosition = cam.position;
    }
}