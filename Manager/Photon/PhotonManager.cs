#if UNIRX_SUPPORT && PUN_2_SUPPORT && UNITASK_SUPPORT

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using Pun2Task;
using Cysharp.Threading.Tasks;
using System;

namespace JHS.Library.Manager.Photon
{
    /// <summary>
    /// Photon에 대한 기능 정의, Photon Callback들에 대한 UniRx 기반 Observable 제공
    /// </summary>
    public class PhotonManager : IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks, IWebRpcCallback, IErrorInfoCallback
    {
        #region Field

        [SerializeField] bool isOfflineMode = false;
        [SerializeField] bool isAutomaticallySyncScene = true;

        private PhotonView pvCache;

        Subject<Unit> onConnectedSubject = new();
        Subject<Unit> onConnectedToMasterSubject = new();
        Subject<DisconnectCause> onDisconnectedSubject = new();
        Subject<RegionHandler> onRegionListReceivedSubject = new();
        Subject<Dictionary<string, object>> onCustomAuthenticationResponseSubject = new();
        Subject<string> onCustomAuthenticationFailedSubject = new();
        Subject<List<FriendInfo>> onFriendListUpdateSubject = new();
        Subject<Unit> onCreatedRoomSubject = new();
        Subject<(short returnCode, string message)> onCreateRoomFailedSubject = new();
        Subject<Unit> onJoinedRoomSubject = new();
        Subject<(short returnCode, string message)> onJoinRoomFailedSubject = new();
        Subject<(short returnCode, string message)> onJoinRandomFailedSubject = new();
        Subject<Unit> onLeftRoomSubject = new();
        Subject<Player> onPlayerEnteredRoomSubject = new();
        Subject<Player> onPlayerLeftRoomSubject = new();
        Subject<Hashtable> onRoomPropertiesUpdateSubject = new();
        Subject<(Player targetPlayer, Hashtable changedProps)> onPlayerPropertiesUpdateSubject = new();
        Subject<Player> onMasterClientSwitchedSubject = new();
        Subject<Unit> onJoinedLobbySubject = new();
        Subject<Unit> onLeftLobbySubject = new();
        Subject<List<RoomInfo>> onRoomListUpdateSubject = new();
        Subject<List<TypedLobbyInfo>> onLobbyStatisticsUpdateSubject = new();
        Subject<OperationResponse> onWebRpcResponseSubject = new();
        Subject<ErrorInfo> onErrorInfoSubject = new();

        #endregion

        #region Property

        public PhotonView photonView
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || this.pvCache == null)
                {
                    this.pvCache = PhotonView.Get(this);
                }
#else
            if (this.pvCache == null)
            {
                this.pvCache = PhotonView.Get(this);
            }
#endif
                return this.pvCache;
            }
        }

        public IObservable<Unit> OnConnectedAsObservable => onConnectedSubject.Share();
        public IObservable<Unit> OnConnectedToMasterAsObservable => onConnectedToMasterSubject.Share();
        public IObservable<DisconnectCause> OnDisconnectedAsObservable => onDisconnectedSubject.Share();
        public IObservable<RegionHandler> OnRegionListReceivedAsObservable => onRegionListReceivedSubject.Share();
        public IObservable<Dictionary<string, object>> OnCustomAuthenticationResponseAsObservable => onCustomAuthenticationResponseSubject.Share();
        public IObservable<string> OnCustomAuthenticationFailedAsObservable => onCustomAuthenticationFailedSubject.Share();
        public IObservable<List<FriendInfo>> OnFriendListUpdateAsObservable => onFriendListUpdateSubject.Share();
        public IObservable<Unit> OnCreatedRoomAsObservable => onCreatedRoomSubject.Share();
        public IObservable<(short returnCode, string message)> OnCreateRoomFailedAsObservable => onCreateRoomFailedSubject.Share();
        public IObservable<Unit> OnJoinedRoomAsObservable => onJoinedRoomSubject.Share();
        public IObservable<(short returnCode, string message)> OnJoinRoomFailedAsObservable => onJoinRoomFailedSubject.Share();
        public IObservable<(short returnCode, string message)> OnJoinRandomFailedAsObservable => onJoinRandomFailedSubject.Share();
        public IObservable<Unit> OnLeftRoomAsObservable => onLeftRoomSubject.Share();
        public IObservable<Player> OnPlayerEnteredRoomAsObservable => onPlayerEnteredRoomSubject.Share();
        public IObservable<Player> OnPlayerLeftRoomAsObservable => onPlayerLeftRoomSubject.Share();
        public IObservable<Hashtable> OnRoomPropertiesUpdateAsObservable => onRoomPropertiesUpdateSubject.Share();
        public IObservable<(Player targetPlayer, Hashtable changedProps)> OnPlayerPropertiesUpdateAsObservable => onPlayerPropertiesUpdateSubject.Share();
        public IObservable<Player> OnMasterClientSwitchedAsObservable => onMasterClientSwitchedSubject.Share();
        public IObservable<Unit> OnJoinedLobbyAsObservable => onJoinedLobbySubject.Share();
        public IObservable<Unit> OnLeftLobbyAsObservable => onLeftLobbySubject.Share();
        public IObservable<List<RoomInfo>> OnRoomListUpdateAsObservable => onRoomListUpdateSubject.Share();
        public IObservable<List<TypedLobbyInfo>> OnLobbyStatisticsUpdateAsObservable => onLobbyStatisticsUpdateSubject.Share();
        public IObservable<OperationResponse> OnWebRpcResponseAsObservable => onWebRpcResponseSubject.Share();
        public IObservable<ErrorInfo> OnErrorInfoAsObservable => onErrorInfoSubject.Share();

        #endregion

        #region Unity Lifecycle

        public void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

        #region 외부 메소드

        public async UniTask ConnectionAsync(CancellationToken token)
        {
            try
            {
                //PhotonNetwork.OfflineMode = isOfflineMode;
                PhotonNetwork.AutomaticallySyncScene = isAutomaticallySyncScene;

                await Pun2TaskNetwork.ConnectUsingSettingsAsync(token);
                Debug.Log("서버 연결 완료");

                if (!PhotonNetwork.OfflineMode) await Pun2TaskNetwork.JoinLobbyAsync(token: token);
                if (!PhotonNetwork.OfflineMode) Util.Log("로비 입장 완료");
            }
            catch (Pun2TaskNetwork.ConnectionFailedException ex)
            {
                Debug.LogError(ex);
            }
        }

        #endregion

        #region 콜백 메소드

        void IConnectionCallbacks.OnConnected() => onConnectedSubject.OnNext(default);
        void IConnectionCallbacks.OnConnectedToMaster() => onConnectedToMasterSubject.OnNext(default);
        void IConnectionCallbacks.OnDisconnected(DisconnectCause cause) => onDisconnectedSubject.OnNext(cause);
        void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler) => onRegionListReceivedSubject.OnNext(regionHandler);
        void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data) => onCustomAuthenticationResponseSubject.OnNext(data);
        void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage) => onCustomAuthenticationFailedSubject.OnNext(debugMessage);
        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList) => onFriendListUpdateSubject.OnNext(friendList);
        void IMatchmakingCallbacks.OnCreatedRoom() => onCreatedRoomSubject.OnNext(default);
        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message) => onCreateRoomFailedSubject.OnNext((returnCode, message));
        void IMatchmakingCallbacks.OnJoinedRoom() => onJoinedRoomSubject.OnNext(default);
        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message) => onJoinRoomFailedSubject.OnNext((returnCode, message));
        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message) => onJoinRandomFailedSubject.OnNext((returnCode, message));
        void IMatchmakingCallbacks.OnLeftRoom() => onLeftRoomSubject.OnNext(default);
        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer) => onPlayerEnteredRoomSubject.OnNext(newPlayer);
        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer) => onPlayerLeftRoomSubject.OnNext(otherPlayer);
        void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) => onRoomPropertiesUpdateSubject.OnNext(propertiesThatChanged);
        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) => onPlayerPropertiesUpdateSubject.OnNext((targetPlayer, changedProps));
        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient) => onMasterClientSwitchedSubject.OnNext(newMasterClient);
        void ILobbyCallbacks.OnJoinedLobby() => onJoinedLobbySubject.OnNext(default);
        void ILobbyCallbacks.OnLeftLobby() => onLeftLobbySubject.OnNext(default);
        void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomList) => onRoomListUpdateSubject.OnNext(roomList);
        void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) => onLobbyStatisticsUpdateSubject.OnNext(lobbyStatistics);
        void IWebRpcCallback.OnWebRpcResponse(OperationResponse response) => onWebRpcResponseSubject.OnNext(response);
        void IErrorInfoCallback.OnErrorInfo(ErrorInfo errorInfo) => onErrorInfoSubject.OnNext(errorInfo);

        #endregion

        #region 디버그

        [ContextMenu("정보")]
        void Info()
        {
            if (PhotonNetwork.InRoom)
            {
                Debug.Log("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
                Debug.Log("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
                Debug.Log("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

                string playerStr = "방에 있는 플레이어 목록 : ";
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
                Debug.Log(playerStr);
            }
            else
            {
                Debug.Log("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
                Debug.Log("방 개수 : " + PhotonNetwork.CountOfRooms);
                Debug.Log("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
                Debug.Log("로비에 있는지? : " + PhotonNetwork.InLobby);
                Debug.Log("연결됐는지? : " + PhotonNetwork.IsConnected);
            }
        }

        [ContextMenu("테스트")]
        public void Test()
        {
            Debug.Log(PhotonNetwork.LevelLoadingProgress.ToString("%"));
        }

        #endregion
    }
}

#endif