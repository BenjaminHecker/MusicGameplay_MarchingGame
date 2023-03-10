using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event musicEvent;
    private uint playingID;

    [SerializeField] private Transform player;
    [SerializeField] private float stepSize = 0.5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float moveSpeed = 10f;

    private Vector2 dir;
    private Vector2 currentPos;
    private Vector2 targetPos;

    private void Start()
    {
        playingID = musicEvent.Post(
            gameObject,
            (uint) (
                AkCallbackType.AK_MusicSyncAll
                | AkCallbackType.AK_EnableGetMusicPlayPosition
                | AkCallbackType.AK_MIDIEvent
            ),
            CallbackFunction
        );
    }

    private void Update()
    {
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (dir.x > 0) dir.x = 1;
        if (dir.x < 0) dir.x = -1;
        if (dir.y > 0) dir.y = 1;
        if (dir.y < 0) dir.y = -1;

        dir *= stepSize;

        if ((Vector2)player.up == -dir)
            dir += (Vector2) player.right * 0.01f;

        player.up = Vector3.Lerp(player.up, dir, rotateSpeed * Time.deltaTime);
        player.position = Vector3.Lerp(player.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private void CallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        AkMusicSyncCallbackInfo musicInfo;

        if (in_info is AkMusicSyncCallbackInfo)
        {
            musicInfo = (AkMusicSyncCallbackInfo)in_info;
            switch (in_type)
            {
                case AkCallbackType.AK_MusicSyncUserCue:
                    break;
                case AkCallbackType.AK_MusicSyncBeat:
                    Step();
                    break;
                case AkCallbackType.AK_MusicSyncBar:
                    break;
            }
        }
    }

    private void Step()
    {
        currentPos = targetPos;
        targetPos = currentPos += dir;
    }
}
