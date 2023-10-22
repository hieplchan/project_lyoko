// https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/#2.5

using UnityEngine;

public class CharacterMoveControl : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;

    private Vector3 _velocity = new Vector3();
    private Vector3 _displacement = new Vector3();

    private Vector2 _playerInput = new Vector2();
    private Vector3 _playerInput3D = new Vector3();

    private Vector3 _desiredVelocity = new Vector3();
    private float _maxSpeedChange = 0f;

    private void Awake()
    {
        _playerInput = Vector2.zero;
        _playerInput3D = Vector3.zero;

        _velocity = Vector3.zero;
        _displacement = Vector3.zero;

        _desiredVelocity = Vector3.zero;
    }

    private void Update()
    {
        _playerInput.x = Input.GetAxis("Horizontal");
        _playerInput.y = Input.GetAxis("Vertical");
        _playerInput = Vector2.ClampMagnitude(_playerInput, 1f);
        _playerInput3D.x = _playerInput.x;
        _playerInput3D.z = _playerInput.y;

        _desiredVelocity = _playerInput3D * maxSpeed;
        _maxSpeedChange = maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z, _maxSpeedChange);
        _displacement = _velocity * Time.deltaTime;

        transform.localPosition += _displacement;
    }
}
