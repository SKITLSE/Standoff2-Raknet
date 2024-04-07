using System;
using RiseRakNet.RakNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace RiseRakNet
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
        public Quaternion Rotation = Quaternion.identity;

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
    public class PlayerServerController : MonoBehaviour
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once ConvertToConstant.Local
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private CharacterController _controller;
        private float _timer;
        private int _currentTick;
        private float _minTimeBetweenTicks;
        private float _serverTickRate = LazySingleton<Server>.Instance.Info.TickRate;
        private const int BufferSize = 1024;
        private const int MaxPacketsPerTick = 8; // Максимальное количество пакетов на тик для обработки

        private readonly StatePayload[] _stateBuffer = new StatePayload[BufferSize];
        private readonly Queue<InputPayload> _inputQueue = new();
        public ulong PlayerId { get; set; }
        public int PlayerIndex { get; set; }

        [Obsolete("no need")]
        public void Move(float horizontal, float vertical, float deltaTime)
        {
            throw new Exception("Not Used Any More");
            // Создаем вектор движения
            var prevPos = transform.position;
            var inputVector = new Vector3(horizontal, 0, vertical);
            if (inputVector.magnitude > 1f)
            {
                inputVector.Normalize();
            }

            if (inputVector == Vector3.zero) return;

            var move = (transform.right * inputVector.x + transform.forward * inputVector.z) * _speed * deltaTime;

            // Двигаем персонажа
            _controller.Move(move);

            using var bsOut = PooledBitStream.GetBitStream();
            bsOut.Write((byte)RisePacketInfo.PLAYER_MOVED);
            bsOut.Write(PlayerId);
            bsOut.Write(prevPos);
            bsOut.Write(move);
            bsOut.Write(transform.position);
            RakServer.SendToAll(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
        }

        public void OnClientInput(InputPayload inputPayload)
        {
            _inputQueue.Enqueue(inputPayload);
        }
        private void Awake()
        {
            _serverTickRate = LazySingleton<Server>.Instance.Info.TickRate;
            _minTimeBetweenTicks = 1f / _serverTickRate;
        }
        private void Update()
        {
            _timer += Time.deltaTime;

            while (_timer >= _minTimeBetweenTicks)
            {
                _timer -= _minTimeBetweenTicks;
                HandleTick();
                _currentTick++;
            }
        }

        private void HandleTick()
        {
            // Process the input queue
            var bufferIndex = -1;
            for (var i = 0; i < MaxPacketsPerTick && _inputQueue.Count > 0; i++)
            {
                var inputPayload = _inputQueue.Dequeue();
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (inputPayload.MinTimeBetweenTicks != _minTimeBetweenTicks)
                {
                    Debug.Log($"player ticks = {inputPayload.MinTimeBetweenTicks} server ticks = {_minTimeBetweenTicks}");
                    RakServer.CloseConnection(PlayerId, disconnect_message: "Incorrect tick-rate");

                }
                var stats = new RakNetStatistics();
                if (!RakServer.GetStatistics((uint)PlayerIndex, ref stats) && (stats.ConnectionTime() - inputPayload.Time) > 125) continue;
                bufferIndex = inputPayload.Tick % BufferSize;
                var statePayload = ProcessMovement(inputPayload);
                _stateBuffer[bufferIndex] = statePayload;
            }
            _inputQueue.Clear();
            if (bufferIndex != -1)
            {
                SendToClient(_stateBuffer[bufferIndex], RisePacketInfo.PLAYER_MOVED);
            }
        }

        private StatePayload ProcessMovement(InputPayload input)
        {
            // Should always be in sync with same function on Client
            transform.rotation = input.Rotation;
            var forwardMovement = transform.forward * input.InputVector.z;
            var rightMovement = transform.right * input.InputVector.x;
            // Суммируем два вектора, чтобы получить конечное направление движения
            var moveDirection = _speed * _minTimeBetweenTicks * (forwardMovement + rightMovement).normalized;

            _controller.Move(moveDirection);

            return new StatePayload
            {
                Tick = input.Tick,
                Position = transform.position,
                Rotation = input.Rotation
            };
        }
        private void SendToClient(MessageBase inputPayload, RisePacketInfo packet)
        {
            using var bsOut = PooledBitStream.GetBitStream();
            bsOut.Write((byte)packet);
            bsOut.Write(PlayerId);
            inputPayload.Serialize(bsOut);
            RakServer.SendToAll(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
        }
    }
}
