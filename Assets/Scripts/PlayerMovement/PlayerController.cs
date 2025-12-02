using System;
using System.Net.Security;
using System.Reflection.Emit;
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

        public struct CollisionInfo
        {
            public bool Left;
            public bool Right;
            public bool Top;
            public bool Bottom;
            public bool ClimbingSlope;
            public bool DescendingSlope;

            public float SlopeAngle;
            public float PreviousSlopeAngle;
            public Vector2 PreviousVelocity;

            public void ResetCollisions()
            {
                Left = false;
                Right = false;
                Top = false;
                Bottom = false;
                ClimbingSlope = false;
                DescendingSlope = false;

                PreviousSlopeAngle = SlopeAngle;
                SlopeAngle = 0;
            }
        }

        [SerializeField] private int maxSlopeAngle = 75;
        [SerializeField] private int maxDescendAngle = 75;
        [SerializeField] private int horizontalRayCount = 4;
        [SerializeField] private int verticalRayCount = 4;
        [SerializeField] private LayerMask collisionLayer;
        
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        
        private const float ColliderSkinWidth = 0.1f;
        
        private BoxCollider2D _playerBoxCollider;
        private RaycastOrigins _raycastOrigins;
        private CollisionInfo _collisionInfo;
        
        public CollisionInfo GetCollisionInfo => _collisionInfo;

        private void Start()
        {            
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
        }

        public void Displacement(Vector2 velocity)
        {
            UpdateRaycastOrigins();
            _collisionInfo.ResetCollisions();
            _collisionInfo.PreviousVelocity = velocity;

            if (velocity.y < 0)
            {
                DescendSlope(ref velocity);
            }

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

                    if (_collisionInfo.ClimbingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(_collisionInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }
                    
                    _collisionInfo.Bottom = dirY == -1;
                    _collisionInfo.Top = dirY == 1;
                }
            }
            
            if (_collisionInfo.ClimbingSlope)
            {
                float dirX = Mathf.Sign(velocity.x);
                rayLength  = Mathf.Abs(velocity.x) + ColliderSkinWidth;
                Vector2 rayOrigin = ((dirX == -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionLayer);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (!Mathf.Approximately(slopeAngle, _collisionInfo.SlopeAngle))
                    {
                        velocity.x = (hit.distance - ColliderSkinWidth) * dirX;
                        _collisionInfo.SlopeAngle = slopeAngle;
                    }
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

                // We hit a wall 
                if (hit)
                {      
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (i == 0 && slopeAngle <= maxSlopeAngle)
                    {
                        if (_collisionInfo.DescendingSlope)
                        {
                            _collisionInfo.DescendingSlope = false;
                            velocity = _collisionInfo.PreviousVelocity;
                        }
                        
                        float distanceToSlope = 0f;
                        if (!Mathf.Approximately(slopeAngle, _collisionInfo.PreviousSlopeAngle))
                        {
                            distanceToSlope = hit.distance - ColliderSkinWidth;
                            velocity.x -= distanceToSlope * dirX;
                        }
                        ClimbSlope(ref velocity, slopeAngle);
                        velocity.x += distanceToSlope * dirX;
                    }

                    //We hit a wall while climbing slope
                    if (!_collisionInfo.ClimbingSlope || slopeAngle > maxSlopeAngle)
                    {
                        velocity.x = (hit.distance - ColliderSkinWidth) * dirX;
                        rayLength = hit.distance;

                        if (_collisionInfo.ClimbingSlope)
                        {
                            velocity.y = Mathf.Tan(_collisionInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                        }

                        _collisionInfo.Left = dirX == -1;
                        _collisionInfo.Right = dirX == 1;
                    }
                }
            }
        }

        private void ClimbSlope(ref Vector2 velocity, float slopeAngle)
        {
            float climbDistance = Mathf.Abs(velocity.x);
            float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * climbDistance;

            if (velocity.y <= climbVelocityY)
            {
                velocity.y = climbVelocityY;
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * climbDistance * Mathf.Sign(velocity.x);
                
                _collisionInfo.Bottom = true;
                _collisionInfo.ClimbingSlope = true;
                _collisionInfo.SlopeAngle = slopeAngle;
            }
        }

        private void DescendSlope(ref Vector2 velocity)
        {
            float dirX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = dirX == -1 ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionLayer);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == dirX)
                    {
                        if (hit.distance - ColliderSkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                        {
                            float descendDistance = Mathf.Abs(velocity.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * descendDistance;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * descendDistance * Mathf.Sign(velocity.x);
                            velocity.y -= descendVelocityY;
                            
                            _collisionInfo.SlopeAngle = slopeAngle;
                            _collisionInfo.DescendingSlope = true;
                            _collisionInfo.Bottom = true;
                        }
                    }
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


