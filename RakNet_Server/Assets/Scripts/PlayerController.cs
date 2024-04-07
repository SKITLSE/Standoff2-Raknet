using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RiseRakNet
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private CharacterController _controller;
        public bool IsLocal { get; set; } = false;

        public void Move(Vector3 move, Vector3 from)
        {
            if (transform.position.normalized != from.normalized && IsLocal)
            {
                Debug.Log("desync player. sync with server");
                transform.position = from;
                return;
            }
            transform.position = from;
            _controller.Move(move);
        }

        private void FixedUpdate()
        {
            if (!IsLocal) return;
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            if (horizontal == 0 && vertical == 0) return;
            var move = (transform.right * horizontal + transform.forward * vertical).normalized * _speed * Time.deltaTime;

            _controller.Move(move);

            using var bsOut = PooledBitStream.GetBitStream();
            bsOut.Write((byte)RisePacketInfo.PLAYER_MOVE_REQUEST);
            bsOut.Write(horizontal);
            bsOut.Write(vertical);
            RakClient.Send(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
        }
    }
}
