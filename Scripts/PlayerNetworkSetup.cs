using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject localXRRigGameobject;
    public GameObject mainAvatarGameobject;

    public GameObject avatarHeadGameobject;
    public GameObject avatarBodyGameobject;

    public GameObject[] avatarModelPrefabs;

    public TextMeshProUGUI playerName_Text;

    void Start()
    {
        if(photonView.IsMine)
        {
            //The player is local
            localXRRigGameobject.SetActive(true);

            //Getting  the avatar selection data so that the correct avatar model can be instantiated.
            object avatarSelectionNumber;
            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                Debug.Log("Avatar selection number: " + (int)avatarSelectionNumber);
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            }


            SetLayerRecursively(avatarHeadGameobject, 6);
            SetLayerRecursively(avatarBodyGameobject, 7);

            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if(teleportationAreas.Length > 0)
            {
                Debug.Log("Found " + teleportationAreas.Length + " teleportation area. ");
                foreach (var item in teleportationAreas)
                {
                    item.teleportationProvider = localXRRigGameobject.GetComponent<TeleportationProvider>();
                }
            }
            mainAvatarGameobject.AddComponent<AudioListener>();
        }
        else
        {
            //the player is remote
            localXRRigGameobject.SetActive(false);

            SetLayerRecursively(avatarHeadGameobject, 0);
            SetLayerRecursively(avatarBodyGameobject, 0);
        }

        if(playerName_Text != null)
        {
            playerName_Text.text = photonView.Owner.NickName;
        }
    }

    void Update()
    {
        
    }

    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    [PunRPC]
    public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    {
        GameObject selectedAvatarGameobject = Instantiate(avatarModelPrefabs[avatarSelectionNumber], localXRRigGameobject.transform);

        AvatarInputConverter avatarInputConverter = localXRRigGameobject.GetComponent<AvatarInputConverter>();
        AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
        SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
        SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
        SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
        SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }
}
