using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NetCode
{
    public class NetCodePlayer : NetworkBehaviour
    {
        public NetworkVariable<int> Color = new NetworkVariable<int>();
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public List<int> materialsUsed = new List<int>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Move();
                ChangeColor();
            }
        }

        public void Move() {
            SubmitPositionRequestServerRpc();
            
        }

        public void ChangeColor()
        {
            SubmitColorRequestServerRpc();
        }

        [Rpc(SendTo.Server)]
        void SubmitPositionRequestServerRpc(RpcParams rpcParams = default) {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }
        
        [Rpc(SendTo.Server)]
        void SubmitColorRequestServerRpc(RpcParams rpcParams = default)
        {
            materialsUsed.Remove(Color.Value);
            int tamano = NetCodeManager.instance.materials.Count;
            bool used = true;
            int rand = 0;
            while (used) {
                rand = Random.Range(0, tamano);
                used = false;
                foreach(int materialUsed in materialsUsed) {
                    if (rand == materialUsed) {
                        used = true;
                    }
                }
            }
            var randomMaterial = NetCodeManager.instance.materials[rand];
            GetComponent<MeshRenderer>().materials[0].color = randomMaterial.color;
            Color.Value = rand;
        }

        void Update()
        {
            transform.position = Position.Value;
            GetComponent<MeshRenderer>().materials[0].color = NetCodeManager.instance.materials[Color.Value].color;
        }
    }
}