using System;
using JetBrains.Annotations;
using RiseRakNet.Misc;
using RiseRakNet.RakNet;
using UnityEngine;

namespace RiseRakNet.Player
{
    public class InputPayload : MessageBase
    {
        public ulong Time;
        public int Tick;
        public Vector3 InputVector;
        public Quaternion Rotation;
        public float MinTimeBetweenTicks;

        public override void Deserialize(BitStream stream)
        {
            Time = stream.ReadULong();
            Tick = stream.ReadInt();
            InputVector = stream.ReadVector3();
            Rotation = stream.ReadQuaternion();
            MinTimeBetweenTicks = stream.ReadFloat();
        }

        public override void Serialize(BitStream stream)
        {
            stream.Write(Time);
            stream.Write(Tick);
            stream.Write(InputVector);
            stream.Write(Rotation);
            stream.Write(MinTimeBetweenTicks);
        }
    }

    public class StatePayload : MessageBase
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;

        public override void Deserialize(BitStream stream)
        {
            Tick = stream.ReadInt();
            Position = stream.ReadVector3();
            Rotation = stream.ReadQuaternion();
        }

        public override void Serialize(BitStream stream)
        {
            stream.Write(Tick);
            stream.Write(Position);
            stream.Write(Rotation);
        }
    }
    public class PlayerController : MonoBehaviour
    {
        // ReSharper disable once ConvertToConstant.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private float _speed = 5.0f;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        [SerializeField] private CharacterController _controller;
        [SerializeField] private GameObject _serverModel;
        [SerializeField] private GameObject _clientModel;
        [SerializeField] private Transform _mainCameraHolder;
        [SerializeField] private Transform _fpsDirective;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
        [SerializeField] private GameObject _fpsCameraHolder;
        // ReSharper disable once ConvertToConstant.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private float _minimumX = -70;
        // ReSharper disable once ConvertToConstant.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private float _maximumX = 70;
        // ReSharper disable once ConvertToConstant.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private float _sensitivity = 5f;
        [SerializeField] private float _serverTickRate = 64;
        private float _timer;
        private int _currentTick;
        private float MinTimeBetweenTicks => 1f / _serverTickRate;
        private const int BufferSize = 1024;
        private readonly StatePayload[] _stateBuffer = new StatePayload[BufferSize];
        private readonly InputPayload[] _inputBuffer = new InputPayload[BufferSize];
        private StatePayload _latestServerState = default;
        private StatePayload _lastProcessedState = default;
        private float _horizontalInput;
        private float _verticalInput;
        private PlayerMainCamera _mainCamera;
        private Vector2 _deltaAimAngles = Vector2.zero;
        private Vector3 _curAimAngle = Vector3.zero;
        private Vector3 _curEulerAngles = Vector3.zero;
        public bool IsLocal { get; set; } = false;

        public void SetMainCamera([NotNull] Camera mainCamera)
        {
            if (mainCamera == null) throw new ArgumentNullException(nameof(mainCamera));
            _mainCamera = mainCamera.GetComponent<PlayerMainCamera>() ?? mainCamera.gameObject.AddComponent<PlayerMainCamera>();
            _mainCamera.AttachToPlayer(this, _mainCameraHolder.transform);
        }
        public void ResetMainCamera()
        {
            _mainCamera = null;
        }
        public void OnServerMovementState(StatePayload serverState)
        {
            if (!IsLocal)
            {
                transform.SetPositionAndRotation(serverState.Position, serverState.Rotation);
                _clientModel.SetActive(false);
                return;
            }

            _serverModel.transform.SetPositionAndRotation(new Vector3(serverState.Position.x, serverState.Position.y + 0.88f, serverState.Position.z), serverState.Rotation);
            _serverModel.transform.SetParent(null);
            _latestServerState = serverState;
        }

        private void Awake()
        {
            _serverTickRate = LazySingleton<Client>.Instance.NetworkInfo.TickRate;
        }

        private void RotationControl()
        {
            var num = _curEulerAngles.y + _deltaAimAngles.y;
            var num2 = -_curAimAngle.x;
            if (num < -180f) num += 360f;
            num2 += _deltaAimAngles.x;
            if (num2 < -180f) num2 += 360f;
            num2 = Mathf.Clamp(num2, _minimumX,_maximumX);
            _curEulerAngles = new Vector3(0f, num, 0f);
            _curAimAngle = new Vector3(-num2, num, 0f);
            ApplyRotation();
        }
        private void ApplyRotation()
        {
            _fpsDirective.localEulerAngles = new Vector3(_curAimAngle.x, 0f, 0f);
            transform.eulerAngles = _curEulerAngles;
        }
        private void HandleRotations()
        {
            _deltaAimAngles = new Vector2(Input.GetAxis("Mouse Y") * _sensitivity, Input.GetAxis("Mouse X") * _sensitivity);
            RotationControl();
        }

        private void OnDestroy()
        {
            Destroy(_serverModel);
        }

        private void Update()
        {
            if (!IsLocal) return;

            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            _timer += Time.deltaTime;

            while (_timer >= MinTimeBetweenTicks)
            {
                _timer -= MinTimeBetweenTicks;
                HandleTick();
                _currentTick++;
            }
        }

        private void HandleTick()
        {
            var bufferIndex = _currentTick % BufferSize;
            // Add payload to inputBuffer
            HandleRotations();
            var inputPayload = new InputPayload
            {
                Time = RakClient.Statistics.ConnectionTime(),
                Tick = _currentTick,
                InputVector = new Vector3(_horizontalInput, 0, _verticalInput),
                Rotation = transform.rotation,
                MinTimeBetweenTicks = MinTimeBetweenTicks
            };
            _inputBuffer[bufferIndex] = inputPayload;

            // Add payload to stateBuffer
            _stateBuffer[bufferIndex] = ProcessMovement(inputPayload); // тут по какой-то причине позиция ставится другой

            if (_latestServerState != null &&
                (_lastProcessedState == null ||
                 !_latestServerState.Equals(_lastProcessedState)))
            {
                HandleServerReconciliation(); // Тут происходит смена позиции если она не совпадает с серверной
            }
            // Send input to server
            SendToServer(inputPayload, RisePacketInfo.PLAYER_MOVE_REQUEST);
        }

        private static void SendToServer(MessageBase inputPayload, RisePacketInfo packet)
        {
            using var bsOut = PooledBitStream.GetBitStream();
            bsOut.Write((byte)packet);
            inputPayload.Serialize(bsOut);
            RakClient.Send(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
        }

        private StatePayload ProcessMovement([NotNull] InputPayload input)
        {
            // Should always be in sync with same function on Server
            var forwardMovement = transform.forward * input.InputVector.z;
            var rightMovement = transform.right * input.InputVector.x;

            // Суммируем два вектора, чтобы получить конечное направление движения
            var moveDirection = _speed * MinTimeBetweenTicks * (forwardMovement + rightMovement).normalized;
            _controller.Move(moveDirection);

            return new StatePayload
            {
                Tick = input.Tick,
                Position = transform.position,
            };
        }

        private StatePayload ProcessMovement2([NotNull] InputPayload input)
        {
            // Should always be in sync with same function on Server
            transform.position += input.InputVector * _speed * MinTimeBetweenTicks;

            return new StatePayload
            {
                Tick = input.Tick,
                Position = transform.position,
            };
        }

        private void HandleServerReconciliation()
        {
            _lastProcessedState = _latestServerState;

            var serverStateBufferIndex = _latestServerState.Tick % BufferSize;
            var positionError = Vector3.Distance(_latestServerState.Position, _stateBuffer[serverStateBufferIndex].Position);

            if (!(positionError > 0.001f)) return;
            Debug.Log($"We have to reconcile bro {transform.position} {_latestServerState.Position}");
            // Rewind & Replay
            transform.position = _latestServerState.Position;
            // Update buffer at index of latest server state
            _stateBuffer[serverStateBufferIndex] = _latestServerState;

            // Now re-simulate the rest of the ticks up to the current tick on the client
            var tickToProcess = _latestServerState.Tick + 1;

            while (tickToProcess < _currentTick)
            {
                var bufferIndex = tickToProcess % BufferSize;

                // Process new movement with reconciled state
                var statePayload = ProcessMovement2(_inputBuffer[bufferIndex]);

                // Update buffer with recalculated state
                _stateBuffer[bufferIndex] = statePayload;

                tickToProcess++;
            }
        }
    }
}
