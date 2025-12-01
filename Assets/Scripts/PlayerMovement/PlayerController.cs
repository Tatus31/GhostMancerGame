using UnityEngine;

namespace PlayerMovement
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        private struct RaycastOrigins
        {
            public Vector2 BottomLeft;
            public Vector2 BottomRight;
            public Vector2 TopLeft;
            public Vector2 TopRight;
        }
        
        [SerializeField] private int horizontalRayCount = 4;
        [SerializeField] private int verticalRayCount = 4;
        [SerializeField] private LayerMask collisionLayer;
        
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        
        private const float ColliderSkinWidth = 0.1f;
        
        private BoxCollider2D _playerBoxCollider;
        private RaycastOrigins _raycastOrigins;

        private void Start()
        {            
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
        }

        public void Displacement(Vector2 velocity)
        {
            UpdateRaycastOrigins();

            if (velocity.x != 0)
            {
                CalculateHorizontalCollisions(ref velocity);
            }

            if (velocity.y != 0)
            {
                CalculateVerticalCollisions(ref velocity);
            }
            
            transform.Translate(velocity);
        }
        
        private void UpdateRaycastOrigins()
        {
            Bounds bounds = _playerBoxCollider.bounds;
            bounds.Expand(ColliderSkinWidth * -2);
            
            _raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            _raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        private void CalculateVerticalCollisions(ref Vector2 velocity)
        {
            float dirY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + ColliderSkinWidth;
            
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (dirY == -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i);
                
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up  * dirY, rayLength, collisionLayer);

                if (hit)
                {
                    velocity.y = (hit.distance - ColliderSkinWidth) * dirY;
                    rayLength = hit.distance;
                }
            }
        }
        
        private void CalculateHorizontalCollisions(ref Vector2 velocity)
        {
            float dirX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) + ColliderSkinWidth;
            
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (dirX == -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i) ;
                
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionLayer);

                if (hit)
                {
                    velocity.x = (hit.distance - ColliderSkinWidth) * dirX;
                    rayLength = hit.distance;
                }
            }
        }

        private void CalculateRaySpacing()
        {
            Bounds bounds = _playerBoxCollider.bounds;
            bounds.Expand(ColliderSkinWidth * -2);
            
            //At least 2 rays
            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
            
            //N rays (N - 1) gaps 
            _horizontalRaySpacing  = bounds.size.y / (horizontalRayCount - 1);
            _verticalRaySpacing  = bounds.size.x / (verticalRayCount - 1);
        }

        private void OnDrawGizmos()
        {
            if(!_playerBoxCollider)
                return;
            
            UpdateRaycastOrigins();
            CalculateRaySpacing();

            //Debug.Log($"{_playerBoxCollider} drawing rays with spacing {_verticalRaySpacing} amount of rays {verticalRayCount}");
            for (int i = 0; i < verticalRayCount; i++)
            {
                Debug.DrawRay(_raycastOrigins.BottomLeft + Vector2.right * _verticalRaySpacing * i, Vector3.down * 2,  Color.red); 
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Debug.DrawRay(_raycastOrigins.BottomRight + Vector2.up * _horizontalRaySpacing * i, Vector3.right * 2,  Color.red); 
            }
        }
    }
} 


