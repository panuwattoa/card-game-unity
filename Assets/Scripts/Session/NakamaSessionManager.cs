/**
 * Copyright 2019 The Knights Of Unity, created by Pawel Stolarczyk
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;
using System.Threading.Tasks;
using System.Linq;
using Scripts.Utils;
using WebGLSupport;
using AppleAuth;
using AppleAuth.Enums;
using System.Text;
using AppleAuth.Interfaces;
using AppleAuth.Extensions;
using AppleAuth.Native;

public enum LoginType
{
    None = 0,
    Guest = 1,
    Facebook = 2,
    Apple = 3
}
namespace Scripts.Session
{

    /// <summary>
    /// Manages Nakama server interaction and user session throughout the game.
    /// </summary>
    /// <remarks>
    /// Whenever a user tries to communicate with game server it ensures that their session hasn't expired. If the
    /// session is expired the user will have to reauthenticate the session and obtain a new session.
    /// </remarks>
    public class NakamaSessionManager : Singleton<NakamaSessionManager>, Subject
    {
        #region Variables

        /// <summary>
        /// IP Address of the server.
        /// For demonstration purposes, the value is set through Inspector.
        /// </summary>
        [SerializeField] private string _ipAddress = "localhost";

        /// <summary>
        /// Port behind which Nakama server can be found.
        /// The default value is 7350
        /// For demonstration purposes, the value is set through Inspector.
        /// </summary>
        [SerializeField] private int _port = 7350;

        [SerializeField] private bool allowAutoRegis = true;
        [SerializeField] public int GameVersion;

        /// <summary>
        /// Cached value of <see cref="SystemInfo.deviceUniqueIdentifier"/>.
        /// Used to authenticate this device on Nakama server.
        /// </summary>
        private string _deviceId;

        /// <summary>
        /// Used to establish connection between the client and the server.
        /// Contains a list of usefull methods required to communicate with Nakama server.
        /// Do not use this directly, use <see cref="Client"/> instead.
        /// </summary>
        private Client _client;

        /// <summary>
        /// Socket responsible for maintaining connection with Nakama server and exchanger realtime messages.
        /// Do not use this directly, use <see cref="Socket"/> instead.
        /// </summary>
        private ISocket _socket;
        private string _facebookToken;
        private string AppleUserIdKey = "AppleUserIdKey";
        private string AppleSessionKey = "AppleSessionKey";


        public IAppleAuthManager appleAuthManager { get; private set; }
        #region Debug

        [Header("Debug")]
        /// <summary>
        /// If true, stored session authentication token and device id will be erased on start
        /// </summary>
        [SerializeField] private bool _erasePlayerPrefsOnStart = false;

        /// <summary>
        /// Sufix added to <see cref="_deviceId"/> to generate new device id.
        /// </summary>
        [SerializeField] private string _sufix = string.Empty;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Used to communicate with Nakama server.
        /// For the user to send and receive messages from the server, <see cref="Session"/> must not be expired.
        /// Default expiration time is 60s, but for this demo we set it to 3 weeks (1 814 400 seconds).
        /// To initialize the session, call <see cref="AuthenticateDeviceIdAsync"/> or <see cref="AuthenticateFacebookAsync"/> methods.
        /// To reinitialize expired session, call <see cref="Reauthenticate"/> method.
        /// </summary>
        public ISession Session { get;  set; }

        /// <summary>
        /// Contains all the identifying data of a <see cref="Client"/>, like User Id, linked Device IDs,
        /// linked Facebook account, username, etc.
        /// </summary>
        public IApiAccount Account { get; private set; }

        /// <summary>
        /// Used to establish connection between the client and the server.
        /// Contains a list of usefull methods required to communicate with Nakama server.
        /// </summary>
        public Client Client
        {
            get
            {
                if (_client == null)
                {
                    // "defaultkey" should be changed when releasing the app
                    // see https://heroiclabs.com/docs/install-configuration/#socket
                    _client = new Client("http",_ipAddress, _port, "mynewkey",  UnityWebRequestAdapter.Instance);
                }
                return _client;
            }
        }

        /// <summary>
        /// Socket responsible for maintaining connection with Nakama server and exchange realtime messages.
        /// </summary>
        public ISocket Socket
        {
            get
            {
                if (_socket == null)
                {
                    // Initializing socket
                    _socket = _client.NewSocket();
                }
                return _socket;
            }
        }

        /// <summary>
        /// Returns true if <see cref="Session"/> between this device and Nakama server exists.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (Session == null || Session.HasExpired(DateTime.UtcNow) == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Invoked whenever client first authorizes using DeviceId.
        /// </summary>
        public event Action OnConnectionSuccess = delegate { Debug.Log(">> Connection Success"); };

        /// <summary>
        /// Invoked whenever client first authorizes using DeviceId.
        /// </summary>
        public event Action OnNewAccountCreated = delegate { Debug.Log(">> New Account Created"); };

        /// <summary>
        /// Invoked upon DeviceId authorisation failure.
        /// </summary>
        public event Action OnConnectionFailure = delegate { Debug.Log(">> Connection Error"); };

        /// <summary>
        /// Invoked after <see cref="DisconnectAsync"/> is called.
        /// </summary>
        public event Action OnDisconnected = delegate { Debug.Log(">> Disconnected"); };

        public event Action OnLoginFail = delegate { Debug.Log(">> login Error"); };

        #endregion

        #region Mono

        /// <summary>
        /// Creates new <see cref="Nakama.Client"/> object used to communicate with Nakama server.
        /// Authenticates this device using its <see cref="SystemInfo.deviceUniqueIdentifier"/> or
        /// using Facebook account, if <see cref="IsFacebookConnected"/> is true.
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            if (_erasePlayerPrefsOnStart == true)
            {
                PlayerPrefs.SetString("nakama.authToken", "");
                PlayerPrefs.SetString("nakama.deviceId", "");
            }

        }


        /// <summary>
        /// Closes Nakama session.
        /// </summary>
        protected override void OnDestroy()
        {
            _ = DisconnectAsync();
        }

        #endregion

        #region Authentication

        public void SetIp(string ip)
        {
            if (IsConnected == false)
            {
                _ipAddress = ip;
            }
        }

        /// <summary>
        /// Restores session or tries to establish a new one.
        /// Invokes <see cref="OnConnectionSuccess"/> or <see cref="OnConnectionFailure"/>.
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticationResponse> ConnectAsync()
        {
            AuthenticationResponse response = await RestoreTokenAsync();
            switch (response)
            {
                case AuthenticationResponse.Authenticated:
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.NewAccountCreated:
                    OnNewAccountCreated?.Invoke();
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.Error:
                    if  (PlayerPrefs.HasKey("logintype"))
                    {
                        int type = PlayerPrefs.GetInt("logintype");
                        if (type == (int)LoginType.Facebook)
                        {
                            await FacebookCallbackTokenAsync();
                            break;
                        }
                        else if (type == (int)LoginType.Guest)
                        {
                            await ConnectWithGuest();
                            break;
                        }else if (type == (int)LoginType.Apple)
                        {
                            SigninWithApple();
                            break;
                        }
                    }
                    OnLoginFail?.Invoke();
                    break;
                case AuthenticationResponse.ConnectionError:
                    OnConnectionFailure?.Invoke();
                    break;
                default:
                    Debug.LogError("Unhandled response received: " + response);
                    break;
            }
            return response;
        }

        public async Task<AuthenticationResponse> ConnectWithEmailAsync(string email, string password)
        {
            AuthenticationResponse response = await EmailAuthAsynce(email + "@m.com", password);
            switch (response)
            {
                case AuthenticationResponse.Authenticated:
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.NewAccountCreated:
                    OnNewAccountCreated?.Invoke();
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.Error:
                    OnLoginFail?.Invoke();
                    break;

                case AuthenticationResponse.ConnectionError:
                    OnConnectionFailure?.Invoke();
                    break;
                default:
                    Debug.LogError("Unhandled response received: " + response);
                    break;
            }
            return response;
        }

        public async Task<AuthenticationResponse> EmailAuthAsynce(string email, string password)
        {
            AuthenticationResponse response = await AuthenticateEmail(email, password);
            Debug.Log("AuthenticateEmail {0}" + response);

            if (response == AuthenticationResponse.Error)
            {

                return AuthenticationResponse.Error;
            }
            if (response == AuthenticationResponse.ConnectionError)
            {
                return AuthenticationResponse.ConnectionError;
            }

            Account = await GetAccountAsync();
            if (Account == null)
            {
                return AuthenticationResponse.Error;
            }

            bool socketConnected = await ConnectSocketAsync();
            if (socketConnected == false)
            {
                return AuthenticationResponse.ConnectionError;
            }

            StoreSessionToken();
            return response;
        }

        public async Task<string> SyncAccount()
        {
            Account = await GetAccountAsync();
            return Account.Wallet;
        }

        /// <summary>
        /// Restores saved Session Athentication Token if user has already authenticated with the server in the past.
        /// If it's the first time authenticating using this device id, a new account will be created.
        /// </summary>
        private async Task<AuthenticationResponse> RestoreTokenAsync()
        {
            // Restoring authentication token from player prefs
            string authToken = PlayerPrefs.GetString("nakama.authToken", null);
           // string authToken = WebGLWindowPlugin.WebGLWindowInjectLocalStorage();
            if (string.IsNullOrWhiteSpace(authToken) == true)
            {
                authToken = PlayerPrefs.GetString("nakama.authToken", null);
            }
             if (string.IsNullOrWhiteSpace(authToken) == true)
            {
                // Token not found
                return AuthenticationResponse.Error;
            }
            else
            {
                // Restoring previous session
                Session = Nakama.Session.Restore(authToken);
                if (Session.HasExpired(DateTime.UtcNow) == true)
                {
                    Debug.Log("session exp");
                    // Restored session has expired
                    // Authenticating new session
                    return AuthenticationResponse.Error;
                }
                else
                {
                    // Session restored
                    // Getting Account info
                    Account = await GetAccountAsync();
                    if (Account == null)
                    {
                        // Account not found
                        // Creating new account
                        return AuthenticationResponse.Error;
                    }

                    // Creating real-time communication socket
                    bool socketConnected = await ConnectSocketAsync();
                    if (socketConnected == false)
                    {
                        return AuthenticationResponse.ConnectionError;
                    }

                    Debug.Log("Session restored with token:" + Session.AuthToken);
                    return AuthenticationResponse.Authenticated;
                }
            }
        }

        public async Task<AuthenticationResponse> ConnectWithGuest()
        {

            GetDeviceId();
            AuthenticationResponse response = await AuthenticateAsync();
            switch (response)
            {
                case AuthenticationResponse.Authenticated:
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.NewAccountCreated:
                    OnNewAccountCreated?.Invoke();
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.Error:
                    OnLoginFail?.Invoke();
                    break;

                case AuthenticationResponse.ConnectionError:
                    OnConnectionFailure?.Invoke();
                    break;
                default:
                    Debug.LogError("Unhandled response received: " + response);
                    break;
            }
            return response;
        }

        /// <summary>
        /// This method authenticates this device using local <see cref="_deviceId"/> and initializes new session
        /// with Nakama server. If it's the first time user logs in using this device, a new account will be created
        /// (calling <see cref="OnDeviceIdAccountCreated"/>). Upon successfull authentication, Account data is retrieved
        /// and real-time communication socket is connected.
        /// </summary>
        /// <returns>Returns true if every server call was successful.</returns>
        private async Task<AuthenticationResponse> AuthenticateAsync()
        {
            AuthenticationResponse response = await AuthenticateDeviceIdAsync();
            if (response == AuthenticationResponse.Error)
            {
                return AuthenticationResponse.Error;
            }

            if (response == AuthenticationResponse.ConnectionError)
            {
                return AuthenticationResponse.ConnectionError;
            }

            Account = await GetAccountAsync();
            if (Account == null)
            {
                return AuthenticationResponse.Error;
            }

            bool socketConnected = await ConnectSocketAsync();
            if (socketConnected == false)
            {
                return AuthenticationResponse.ConnectionError;
            }

            StoreSessionToken();
            return response;
        }

        /// <summary>
        /// Authenticates a new session using DeviceId. If it's the first time authenticating using
        /// this device, new account is created.
        /// </summary>
        /// <returns>Returns true if every server call was successful.</returns>
        private async Task<AuthenticationResponse> AuthenticateDeviceIdAsync()
        {
            try
            {
                Session = await Client.AuthenticateDeviceAsync(_deviceId, null, allowAutoRegis);
                Debug.Log("Device authenticated with token:" + Session.AuthToken);
                return AuthenticationResponse.Authenticated;
            }
            catch (ApiResponseException e)
            {
                if (e.StatusCode == (long)System.Net.HttpStatusCode.NotFound)
                {
                    Debug.Log("Couldn't find DeviceId in database, creating new user; message: " + e);
                    return await CreateAccountAsync();
                }
                else
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    return AuthenticationResponse.Error;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Counldn't connect to Nakama server; message: " + e);
                return AuthenticationResponse.Error;
            }
        }


        private async Task<AuthenticationResponse> AuthenticateEmail(string email, string password)
        {
            try
            {
                Session = await Client.AuthenticateEmailAsync(email, password, email, allowAutoRegis);
                Debug.Log("Device authenticated with token:" + Session.AuthToken);
                return AuthenticationResponse.Authenticated;
            }
            catch (ApiResponseException e)
            {
                if (e.StatusCode == (long)System.Net.HttpStatusCode.NotFound)
                {
                    return AuthenticationResponse.Error;
                }
                else if (e.StatusCode == (long)System.Net.HttpStatusCode.BadRequest)
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    return AuthenticationResponse.Error;
                }
                else if (e.StatusCode == (long)System.Net.HttpStatusCode.BadRequest)
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    return AuthenticationResponse.Error;

                }
                else
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    return AuthenticationResponse.ConnectionError;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Counldn't connect to Nakama server; message: " + e);
                return AuthenticationResponse.ConnectionError;
            }
        }

        /// <summary>
        /// Creates new account on Nakama server using local <see cref="_deviceId"/>.
        /// </summary>
        /// <returns>Returns true if account was successfully created.</returns>
        private async Task<AuthenticationResponse> CreateAccountAsync()
        {
            try
            {
                Session = await Client.AuthenticateDeviceAsync(_deviceId, null, true);
                return AuthenticationResponse.NewAccountCreated;
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't create account using DeviceId; message: " + e);
                return AuthenticationResponse.Error;
            }
        }

        /// <summary>
        /// Connects <see cref="Socket"/> to Nakama server to enable real-time communication.
        /// </summary>
        /// <returns>Returns true if socket has connected successfully.</returns>
        private async Task<bool> ConnectSocketAsync()
        {
            try
            {
                if (_socket != null)
                {
                    await _socket.CloseAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Couldn't disconnect the socket: " + e);
            }

            try
            {
                await Socket.ConnectAsync(Session);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("An error has occured while connecting socket: " + e);
                return false;
            }
        }

        /// <summary>
        /// Removes session and account from cache, logs out of Facebook and invokes <see cref="OnDisconnected"/>.
        /// </summary>
        public async Task DisconnectAsync()
        {

            if (Session == null)
            {
                return;
            }
            else
            {
                Session = null;
                Account = null;
                try
                {
                    if (_socket != null)
                    {
                        await _socket.CloseAsync();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Couldn't disconnect the socket: " + e);
                }
                Debug.Log("Disconnected from Nakama");
                OnDisconnected.Invoke();
            }
        }

        public async Task DisconnectWithOutPopupAsync()
        {

            if (Session == null)
            {
                return;
            }
            else
            {
                Session = null;
                Account = null;
                try
                {
                    if (_socket != null)
                    {
                        await _socket.CloseAsync();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Couldn't disconnect the socket: " + e);
                }
                Debug.Log("Disconnected from Nakama");
            }
        }

        #endregion

        #region UserInfo

        /// <summary>
        /// Receives currently logged in user's <see cref="IApiAccount"/> from server.
        /// </summary>
        public async Task<IApiAccount> GetAccountAsync()
        {
            try
            {
                IApiAccount results = await Client.GetAccountAsync(Session);
                return results;
            }
            catch (Exception e)
            {
                Debug.LogWarning("An error has occured while retrieving account: " + e);
                return null;
            }
        }

        /// <summary>
        /// Receives <see cref="IApiUser"/> info from server using user id or username.
        /// Either <paramref name="userId"/> or <paramref name="username"/> must not be null.
        /// </summary>
        public async Task<IApiUser> GetUserInfoAsync(string userId, string username)
        {
            try
            {
                IApiUsers results = await Client.GetUsersAsync(Session, new string[] { userId }, new string[] { username });
                if (results.Users.Count() != 0)
                {
                    return results.Users.ElementAt(0);
                }
                else
                {
                    Debug.LogWarning("Couldn't find user with id: " + userId);
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("An error has occured while retrieving user info: " + e);
                return null;
            }
        }

        /// <summary>
        /// Async method used to update user's username and avatar url.
        /// </summary>
        public async Task<AuthenticationResponse> UpdateUserInfoAsync(string username, string avatarUrl)
        {
            try
            {
                await Client.UpdateAccountAsync(Session, username, null, avatarUrl);
                return AuthenticationResponse.UserInfoUpdated;
            }
            catch (ApiResponseException e)
            {
                Debug.LogError("Couldn't update user info with code " + e.StatusCode + ": " + e);
                return AuthenticationResponse.Error;
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't update user info: " + e);
                return AuthenticationResponse.Error;
            }
        }

        /// <summary>
        /// Retrieves device id from player prefs. If it's the first time running this app
        /// on this device, <see cref="_deviceId"/> is filled with <see cref="SystemInfo.deviceUniqueIdentifier"/>.
        /// </summary>
        private void GetDeviceId()
        {
            if (string.IsNullOrEmpty(_deviceId) == true)
            {
                _deviceId = PlayerPrefs.GetString("nakama.deviceId");
                if (string.IsNullOrWhiteSpace(_deviceId) == true)
                {
                    // SystemInfo.deviceUniqueIdentifier is not supported in WebGL,
                    // we generate a random one instead via System.Guid
#if UNITY_WEBGL && !UNITY_EDITOR
                    _deviceId = System.Guid.NewGuid().ToString();
#else
                    _deviceId = SystemInfo.deviceUniqueIdentifier;
#endif                    
                    PlayerPrefs.SetString("nakama.deviceId", _deviceId);
                }
                _deviceId += _sufix;
            }
        }

        /// <summary>
        /// Stores Nakama session authentication token in player prefs
        /// </summary>
        private void StoreSessionToken()
        {
            if (Session == null)
            {
                Debug.LogWarning("Session is null; cannot store in player prefs");
            }
            else
            {
                PlayerPrefs.SetString("nakama.authToken", Session.AuthToken);
            }
        }

        #endregion





        /// <summary>
        /// Transfers this Device Id to an already existing user account linked with Facebook.
        /// This will leave current account floating, with no real device linked to it.
        /// </summary>
        public async Task<bool> MigrateDeviceIdAsync(string facebookToken)
        {
            try
            {
                Debug.Log("Starting account migration");
                string dummyGuid = _deviceId + "-";

                await Client.LinkDeviceAsync(Session, dummyGuid);
                Debug.Log("Dummy id linked");

                ISession activatedSession = await Client.AuthenticateFacebookAsync(facebookToken, null, false);
                Debug.Log("Facebook authenticated");

                await Client.UnlinkDeviceAsync(Session, _deviceId);
                Debug.Log("Local id unlinked");

                await Client.LinkDeviceAsync(activatedSession, _deviceId);
                Debug.Log("Local id linked. Migration successfull");

                Session = activatedSession;
                StoreSessionToken();

                Account = await GetAccountAsync();
                if (Account == null)
                {
                    throw new Exception("Couldn't retrieve linked account data");
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("An error has occured while linking dummy guid to local account: " + e);
                return false;
            }
        }


        public void LinkFacebook()
        {
            List<string> permissions = new List<string>();
            permissions.Add("public_profile");
            permissions.Add("email");
        }

        public void SigninWithApple()
        {
#if UNITY_IOS
            
            if (PlayerPrefs.HasKey(AppleUserIdKey))
            {
                this.appleAuthManager.GetCredentialState(
                PlayerPrefs.GetString(AppleUserIdKey),
                state =>
                {
                    switch (state)
                    {
                        case CredentialState.Authorized:
                            // User ID is still valid. Login the user.
                           _ = AppleTokenAsync("");
                           break;

                        case CredentialState.Revoked:
                            QuickLoginApple();
                            break;

                        case CredentialState.NotFound:
                            QuickLoginApple();
                            break;
                    }
                },
                error =>
                {
                    Debug.LogErrorFormat("error", error);
                });
            }

            QuickLoginApple();
#endif
        }


        private void AuthLoginApple()
        {
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            this.appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // Obtained credential, cast it to IAppleIDCredential
                    var appleIdCredential = credential as IAppleIDCredential;
                    if (appleIdCredential != null)
                    {
                        // Identity token
                        var identityToken = Encoding.UTF8.GetString(
                                        appleIdCredential.IdentityToken,
                                        0,
                                        appleIdCredential.IdentityToken.Length);
                        PlayerPrefs.SetString(AppleSessionKey, identityToken);

                        // You should save the user ID somewhere in the device
                        var userId = appleIdCredential.User;
                        PlayerPrefs.SetString(AppleUserIdKey, userId);
                        _ = AppleTokenAsync(identityToken);

                        // Authorization code
                        var authorizationCode = Encoding.UTF8.GetString(
                                        appleIdCredential.AuthorizationCode,
                                        0,
                                        appleIdCredential.AuthorizationCode.Length);

                         // And now you have all the information to create/login a user in your system
                      }
                 },error =>
                 {
                    // Something went wrong
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                     OnLoginFail?.Invoke();

                 });
        }

        private void QuickLoginApple()
        {
            var quickLoginArgs = new AppleAuthQuickLoginArgs();
            this.appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    // Received a valid credential!
                    // Try casting to IAppleIDCredential or IPasswordCredential

                    // Previous Apple sign in credential
                    var appleIdCredential = credential as IAppleIDCredential;

                    // Saved Keychain credential (read about Keychain Items)
                    var passwordCredential = credential as IPasswordCredential;
                    // Identity token
                    var identityToken = Encoding.UTF8.GetString(
                                    appleIdCredential.IdentityToken,
                                    0,
                                    appleIdCredential.IdentityToken.Length);
                    PlayerPrefs.SetString(AppleSessionKey, identityToken);
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;
                    PlayerPrefs.SetString(AppleUserIdKey, userId);
                    _ = AppleTokenAsync(identityToken);
                },
                error =>
                {
                    // Quick login failed. The user has never used Sign in With Apple on your app. Go to login screen
                    AuthLoginApple();
                });
        }

        private async Task FacebookCallbackTokenAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_facebookToken))
                {
                    if (PlayerPrefs.HasKey("facebook_pokdeng_token"))
                    {
                        _facebookToken = PlayerPrefs.GetString("facebook_pokdeng_token");
                    }
                }
                Session = await Client.AuthenticateFacebookAsync(_facebookToken);
                Debug.Log("Device authenticated with token:" + Session.AuthToken);

                Account = await GetAccountAsync();
                if (Account == null)
                {
                    OnLoginFail?.Invoke();
                }

                bool socketConnected = await ConnectSocketAsync();
                if (socketConnected == false)
                {
                    OnConnectionFailure?.Invoke();
                }

                StoreSessionToken();
                // success
                OnConnectionSuccess?.Invoke();
            }
            catch (ApiResponseException e)
            {
                if (e.StatusCode == (long)System.Net.HttpStatusCode.NotFound)
                {
                    OnLoginFail?.Invoke();
                }
                else if (e.StatusCode == (long)System.Net.HttpStatusCode.BadRequest)
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    OnLoginFail?.Invoke();
                }
                else if (e.StatusCode == (long)System.Net.HttpStatusCode.BadRequest)
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    OnLoginFail?.Invoke();
                }
                else
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    OnConnectionFailure?.Invoke();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Counldn't connect to Nakama server; message: " + e);
                OnConnectionFailure?.Invoke();
            }
        }

        private async Task AppleTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    if (PlayerPrefs.HasKey(AppleSessionKey))
                    {
                        token = PlayerPrefs.GetString(AppleSessionKey);
                    }
                }
                Session = await Client.AuthenticateAppleAsync(token);
                Debug.Log("apple authenticated with token:" + Session.AuthToken);

                Account = await GetAccountAsync();
                if (Account == null)
                {
                    OnLoginFail?.Invoke();
                }

                bool socketConnected = await ConnectSocketAsync();
                if (socketConnected == false)
                {
                    OnConnectionFailure?.Invoke();
                }

                StoreSessionToken();
                // success
                OnConnectionSuccess?.Invoke();
            }
            catch (ApiResponseException e)
            {
                if (e.StatusCode == (long)System.Net.HttpStatusCode.NotFound)
                {
                    OnLoginFail?.Invoke();
                }
                else if (e.StatusCode == (long)System.Net.HttpStatusCode.BadRequest)
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    OnLoginFail?.Invoke();
                }
                else if (e.StatusCode == (long)System.Net.HttpStatusCode.BadRequest)
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    OnLoginFail?.Invoke();
                }
                else
                {
                    Debug.LogError("An error has occured reaching Nakama server; message: " + e);
                    OnConnectionFailure?.Invoke();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Counldn't connect to Nakama server; message: " + e);
                OnConnectionFailure?.Invoke();
            }
        }


        public void SetAppleAuth()
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            appleAuthManager = new AppleAuthManager(deserializer);
        }
        ////////////////////////////////////////////////////////////////////////////////
        //  Interface
        ////////////////////////////////////////////////////////////////////////////////
        private List<Observer> m_observers = new List<Observer>();

        /// <summary>
        /// Add observer to be notified.
        /// </summary>
        /// <param name="o">O.</param>
        public void AddObserver(Observer o)
        {
            m_observers.Add(o);
        }
        /// <summary>
        /// Remove observer from notified list.
        /// </summary>
        /// <param name="o">O.</param>
        public void RemoveObserver(Observer o)
        {
            //int index = m_observers.Find (o); 
            //m_observers.RemoveAt(index);
            m_observers.Remove(o);
        }
        /// <summary>
        /// Notify all added observers.
        /// </summary>
        public void Notify()
        {
            foreach (Observer o in m_observers)
            {
                o.Notify(this);
            }
        }

    }

}
