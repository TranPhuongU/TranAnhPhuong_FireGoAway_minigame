using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        // PC
        if (Input.GetMouseButtonDown(0))
            HandleClick(Input.mousePosition);

        // Mobile
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //HandleClick(Input.GetTouch(0).position);
    }

    void HandleClick(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PipeTile tile = hit.collider.GetComponent<PipeTile>();

            if (tile != null) 
            {
                tile.OnTapped();
                SoundManager.Instance.PlayPipeClick();
            }

        }
    }
}