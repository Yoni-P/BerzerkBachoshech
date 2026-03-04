using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Can be used for pixel perfect movement
 */
public class PixelatedMovement : MonoBehaviour
{
    [SerializeField]private SpriteRenderer _spriteRenderer;
    
    private Vector2 _velocity;
    private Rigidbody2D _rigidbody2D;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        MoveToPixelGrid();
    }
    
    /**
     * Sets the velocity
     */
    public void SetVelocity (Vector2 newVelocity)
    {
        _velocity = newVelocity;
    }
    
    private Vector2 PixelPerfectClamp(Vector2 moveDirection, float ppu)
    {
        Vector2 vectorInPixels = new Vector2(
            Mathf.RoundToInt(moveDirection.x * ppu),
            Mathf.RoundToInt(moveDirection.y * ppu));

        return vectorInPixels / ppu;
    }
    private void FixedUpdate()
    {
        _rigidbody2D.MovePosition((Vector2) transform.position +
                                  PixelPerfectClamp(_velocity, _spriteRenderer.sprite.pixelsPerUnit) * Time.deltaTime);
    }
    
    /**
     * Moves the game object to the pixel grid
     */
    public void MoveToPixelGrid()
    {
        Vector2 curPosition = transform.position;
        float ppu = _spriteRenderer.sprite.pixelsPerUnit;
        float newX = Mathf.RoundToInt(curPosition.x * ppu) / ppu;
        float newY = Mathf.RoundToInt(curPosition.y * ppu) / ppu;
        transform.position = new Vector3(newX, newY);
    }
}
