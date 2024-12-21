using System.Collections;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;
    #region variables
    [Header("Login")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;
    [SerializeField] Button loginButton;

    [Header("Sign up")]
    public TMP_InputField signupEmail;
    public TMP_InputField userName;
    public TMP_InputField signupPassword;
    public TMP_InputField signupPasswordConfirm;

    public TextMeshProUGUI signupPlaceholder;
    public TextMeshProUGUI usernamePlaceholder;
    public TextMeshProUGUI signupPasswordPlaceholder;
    public TextMeshProUGUI signupPasswordConfirmPlaceholder;
    [SerializeField] Button signUpButton;


    [Header("Extra")]
    public GameObject loadingScreen;
    public TextMeshProUGUI logTxt;

    public GameObject loginUi, signupUi, SuccessUi, loginOptionsPanel;
    public Button emailLoginButton;
    public Button guestLoginButton;
    public GameObject loginCanvas;
    [SerializeField] TextMeshProUGUI id;
    #endregion

    //[SerializeField] GameObject PlayerInfoUI;

    const string MAILKEY = "mail";
    const string PASSKEY = "pass";

    private void Awake()
    {
        if (Instance != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start() {
        DontDestroyOnLoad(this);
        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(SignUp);

        //PlayerInfoUI.SetActive(false);

        // show mail and pass if PlayerPrefs having key "mail" "pass"
        if(PlayerPrefs.HasKey(MAILKEY) && PlayerPrefs.HasKey(PASSKEY)) {
            loginEmail.text = PlayerPrefs.GetString(MAILKEY);
            loginPassword.text = PlayerPrefs.GetString(PASSKEY);
        }
    }

    #region signup

    void SignUp()
    {
        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = signupEmail.text;
        string useName = userName.text;
        string password = signupPassword.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                StartCoroutine(ResetLoadingScreenCo());
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                ShowLogMsg_SingUP("Sign up unsuccessfully");
                StartCoroutine(ResetLoadingScreenCo());
                return;
            }
            // Firebase user has been created.

            StartCoroutine(ResetLoadingScreenCo());
            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            signupEmail.text = "";
            userName.text = "";
            signupPassword.text = "";
            signupPasswordConfirm.text = "";

            signupPlaceholder.text = "Enter Email...";
            usernamePlaceholder.text = "Enter User Name...";
            signupPasswordPlaceholder.text = "Enter Password...";
            signupPasswordConfirmPlaceholder.text = "Confirm Password...";

            if (result.User.IsEmailVerified)
            {
                ShowLogMsg_SingUP("Sign up Successfully");
                
            }
            else {
                ShowLogMsg_SingUP("Please verify your email!!");
                SendEmailVerification();
            }

            StartCoroutine(TextFadeOut_SignUp(2f));
            // save after having email and password
            DataSaver.Instance.SaveToSignup(useName, result.User.UserId);
        });
    }
    IEnumerator ResetLoadingScreenCo() {
        yield return new WaitForSeconds(1f);
        loadingScreen.SetActive(false);
    }

    void SendEmailVerification() {
        StartCoroutine(SendEmailForVerificationAsync());
    }

    IEnumerator SendEmailForVerificationAsync() {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user!=null)
        {
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if (sendEmailTask.Exception != null)
            {
                print("Email send error");
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError)firebaseException.ErrorCode;

                switch (error)
                {
                    case AuthError.None:
                        break;
                    case AuthError.Unimplemented:
                        break;
                    case AuthError.Failure:
                        break;
                    case AuthError.InvalidCustomToken:
                        break;
                    case AuthError.CustomTokenMismatch:
                        break;
                    case AuthError.InvalidCredential:
                        break;
                    case AuthError.UserDisabled:
                        break;
                    case AuthError.AccountExistsWithDifferentCredentials:
                        break;
                    case AuthError.OperationNotAllowed:
                        break;
                    case AuthError.EmailAlreadyInUse:
                        break;
                    case AuthError.RequiresRecentLogin:
                        break;
                    case AuthError.CredentialAlreadyInUse:
                        break;
                    case AuthError.InvalidEmail:
                        break;
                    case AuthError.WrongPassword:
                        break;
                    case AuthError.TooManyRequests:
                        break;
                    case AuthError.UserNotFound:
                        break;
                    case AuthError.ProviderAlreadyLinked:
                        break;
                    case AuthError.NoSuchProvider:
                        break;
                    case AuthError.InvalidUserToken:
                        break;
                    case AuthError.UserTokenExpired:
                        break;
                    case AuthError.NetworkRequestFailed:
                        break;
                    case AuthError.InvalidApiKey:
                        break;
                    case AuthError.AppNotAuthorized:
                        break;
                    case AuthError.UserMismatch:
                        break;
                    case AuthError.WeakPassword:
                        break;
                    case AuthError.NoSignedInUser:
                        break;
                    case AuthError.ApiNotAvailable:
                        break;
                    case AuthError.ExpiredActionCode:
                        break;
                    case AuthError.InvalidActionCode:
                        break;
                    case AuthError.InvalidMessagePayload:
                        break;
                    case AuthError.InvalidPhoneNumber:
                        break;
                    case AuthError.MissingPhoneNumber:
                        break;
                    case AuthError.InvalidRecipientEmail:
                        break;
                    case AuthError.InvalidSender:
                        break;
                    case AuthError.InvalidVerificationCode:
                        break;
                    case AuthError.InvalidVerificationId:
                        break;
                    case AuthError.MissingVerificationCode:
                        break;
                    case AuthError.MissingVerificationId:
                        break;
                    case AuthError.MissingEmail:
                        break;
                    case AuthError.MissingPassword:
                        break;
                    case AuthError.QuotaExceeded:
                        break;
                    case AuthError.RetryPhoneAuth:
                        break;
                    case AuthError.SessionExpired:
                        break;
                    case AuthError.AppNotVerified:
                        break;
                    case AuthError.AppVerificationFailed:
                        break;
                    case AuthError.CaptchaCheckFailed:
                        break;
                    case AuthError.InvalidAppCredential:
                        break;
                    case AuthError.MissingAppCredential:
                        break;
                    case AuthError.InvalidClientId:
                        break;
                    case AuthError.InvalidContinueUri:
                        break;
                    case AuthError.MissingContinueUri:
                        break;
                    case AuthError.KeychainError:
                        break;
                    case AuthError.MissingAppToken:
                        break;
                    case AuthError.MissingIosBundleId:
                        break;
                    case AuthError.NotificationNotForwarded:
                        break;
                    case AuthError.UnauthorizedDomain:
                        break;
                    case AuthError.WebContextAlreadyPresented:
                        break;
                    case AuthError.WebContextCancelled:
                        break;
                    case AuthError.DynamicLinkNotActivated:
                        break;
                    case AuthError.Cancelled:
                        break;
                    case AuthError.InvalidProviderId:
                        break;
                    case AuthError.WebInternalError:
                        break;
                    case AuthError.WebStorateUnsupported:
                        break;
                    case AuthError.TenantIdMismatch:
                        break;
                    case AuthError.UnsupportedTenantOperation:
                        break;
                    case AuthError.InvalidLinkDomain:
                        break;
                    case AuthError.RejectedCredential:
                        break;
                    case AuthError.PhoneNumberNotFound:
                        break;
                    case AuthError.InvalidTenantId:
                        break;
                    case AuthError.MissingClientIdentifier:
                        break;
                    case AuthError.MissingMultiFactorSession:
                        break;
                    case AuthError.MissingMultiFactorInfo:
                        break;
                    case AuthError.InvalidMultiFactorSession:
                        break;
                    case AuthError.MultiFactorInfoNotFound:
                        break;
                    case AuthError.AdminRestrictedOperation:
                        break;
                    case AuthError.UnverifiedEmail:
                        break;
                    case AuthError.SecondFactorAlreadyEnrolled:
                        break;
                    case AuthError.MaximumSecondFactorCountExceeded:
                        break;
                    case AuthError.UnsupportedFirstFactor:
                        break;
                    case AuthError.EmailChangeNeedsVerification:
                        break;
                    default:
                        break;
                }
            }
            else {
                print("Email successfully send");
            }
        }
    }

    #endregion

    #region Login
    public void Login() {
        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = loginEmail.text;
        string password = loginPassword.text;

        Credential credential =
        EmailAuthProvider.GetCredential(email, password);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                StartCoroutine(ResetLoadingScreenCo());
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                ShowLogMsg("Log in fail");
                StartCoroutine(ResetLoadingScreenCo());
                return;
            }
            
            // loadingScreen.SetActive(false);
            StartCoroutine(ResetLoadingScreenCo());
            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            if (result.User.IsEmailVerified)
            {
                ShowLogMsg("Login Successful");

                loginUi.SetActive(false);
                SuccessUi.SetActive(true);
                /* SuccessUi.transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = "Id: " + result.User.UserId; */
                id.text = $"ID: {result.User.UserId}";

                //? gan userId cho saveLoadHander Firebase | FireStore
                DataSaver.Instance.userId = result.User.UserId;

                /* DataSaveLoadHander.Instance.userId = result.User.UserId; */

                /* SceneManager.LoadSceneAsync("MainLobby").completed += (operation) =>
                {
                    loginCanvas.SetActive(false);
                    FindObjectOfType<ShowPlayerInfo>().currentRank = DataSaver.Instance.dataToSave.rank;
                }; */
                //StartCoroutine(LoadMainMenuLobby());
                LoadingScene.Instance.LoadScene("MainLobby");
                loginCanvas.SetActive(false);
                FindObjectOfType<ShowPlayerInfo>().currentRank = DataSaver.Instance.dataToSave.rank;
            }
            else {
                ShowLogMsg("Please verify email!!");
            }

            //Load data
            DataSaver.Instance.LoadData();
            SetPlayerPref(email, password);
        });
    }

    IEnumerator LoadMainMenuLobby() {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("MainLobby").completed += (operation) =>
        {
            loginCanvas.SetActive(false);
            FindObjectOfType<ShowPlayerInfo>().currentRank = DataSaver.Instance.dataToSave.rank;
        };
    }

    public void LoginQuest()
    {
        Debug.Log("Login quest");
        loadingScreen.SetActive(true);
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignIn Guest was canceled.");
                //loadingScreen.SetActive(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignIn Guest encountered an error: " + task.Exception);
                loadingScreen.SetActive(false);
                return;
            }

            loadingScreen.SetActive(false);
            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);


            ShowLogMsg("Log in Successful");
            loginUi.SetActive(false);
            SuccessUi.SetActive(true);
            id.text = $"ID: {result.User.UserId}";

            //? gan userId cho saveLoadHander Firebase | FireStore
            DataSaver.Instance.userId = result.User.UserId;
            DataSaver.Instance.dataToSave.userName = "Quest";
            //SceneManager.LoadSceneAsync("MainLobby").completed += (operation) =>
            //{
            //    loginCanvas.SetActive(false);
            //    FindObjectOfType<ShowPlayerInfo>().currentRank = DataSaver.Instance.dataToSave.rank;
            //};
            LoadingScene.Instance.LoadScene("MainLobby");
            loginCanvas.SetActive(false);
            FindObjectOfType<ShowPlayerInfo>().currentRank = DataSaver.Instance.dataToSave.rank;
        });       
    }

    public void LoginTest()
    {
        Debug.Log("Login quest");

    }
    public void SignOut()
    {
        // Show loading screen while signing out
        //loadingScreen.SetActive(true);

        // Firebase sign-out
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();

        //Debug.Log("User signed out successfully.");
        DataSaver.Instance.ResetData();

        loginCanvas.SetActive(true);
        loginOptionsPanel.SetActive(true);
        FindObjectOfType<ShowPlayerInfo>().ResetRank();
        // Load the Login scene
        SceneManager.LoadSceneAsync("Login");
    }
    #endregion

    #region extra
    void ShowLogMsg(string msg)
    {
        logTxt.text = msg;
        //logTxt.GetComponent<Animation>().Play("FadeOutAnimation");
        StartCoroutine(TextFadeOut(1.5f));
    }
    IEnumerator TextFadeOut(float time) {
        yield return new WaitForSeconds(time);
        logTxt.text = "";

        if (SuccessUi != null)
            SuccessUi.SetActive(false);
        //if (PlayerInfoUI != null)
        //    PlayerInfoUI.SetActive(true);
    }

    void ShowLogMsg_SingUP(string msg)
    {
        logTxt.text = msg;
        StartCoroutine(TextFadeOut_SignUp(3f));
    }
    IEnumerator TextFadeOut_SignUp(float time) {
        yield return new WaitForSeconds(time);
        logTxt.text = "";

        SuccessUi.SetActive(false);

        yield return new WaitForSeconds(time);
        signupUi.gameObject.SetActive(false);
        loginUi.gameObject.SetActive(true);
    }

    void SetPlayerPref(string mail, string password) {
        PlayerPrefs.SetString(MAILKEY, mail);
        PlayerPrefs.SetString(PASSKEY, password);
        PlayerPrefs.Save();
    }

    public void QuickBattle()
    {
        SceneManager.LoadScene("Quang_Scene");
    }
    #endregion


}