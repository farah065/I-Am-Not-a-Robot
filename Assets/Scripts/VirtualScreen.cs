using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualScreen : GraphicRaycaster
{
    public Camera screenCamera; // Reference to the camera responsible for rendering the virtual screen's rendertexture

    public GraphicRaycaster screenCaster; // Reference to the GraphicRaycaster of the canvas displayed on the virtual screen

    // Called by Unity when a Raycaster should raycast because it extends BaseRaycaster.
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        Ray ray = eventCamera.ScreenPointToRay(eventData.position); // Mouse
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.transform == transform)
            {
                // Figure out where the pointer would be in the second camera based on texture position or RenderTexture.
                Vector3 virtualPos = new Vector3(hit.textureCoord.x, hit.textureCoord.y);
                virtualPos.x *= screenCamera.targetTexture.width;
                virtualPos.y *= screenCamera.targetTexture.height;

                eventData.position = virtualPos;

                screenCaster.Raycast(eventData, resultAppendList);
            }
        }
    }
}