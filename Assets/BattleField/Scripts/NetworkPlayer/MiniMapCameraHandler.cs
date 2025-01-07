
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniMapCameraHandler : MonoBehaviour
{
    public Camera miniMapCamera;
    [SerializeField] Transform minimapAnchorPoint;

    bool isMiniMapShowed = false;
    [SerializeField] PlayerRoomController playerRoomController;
    private void Awake() {
        if(SceneManager.GetActiveScene().name == "MainLobby") return;
        transform.position = minimapAnchorPoint.position + new Vector3(0, 5, 0);
    }

    private void Update() {
        if(SceneManager.GetActiveScene().name == "MainLobby") return;

        //! check neu choi o che do team && ready battle && roi xuong dat
        if(isMiniMapShowed == false && (MatchmakingTeam.Instance.IsDone || Matchmaking.Instance.IsDone)) {
            isMiniMapShowed = true;
            ShowMinimapTeamMateIcon();
        }
    }

    void LateUpdate()
    {
        transform.position = minimapAnchorPoint.position + new Vector3(0, 5, 0);

        transform.rotation = Quaternion.Euler(90f, minimapAnchorPoint.eulerAngles.y, 0f);
    }

    void ShowMinimapTeamMateIcon() {
        
        string playerTeamID = playerRoomController.TeamID.ToString();

        var playerRoomControllerArr = FindObjectsOfType<PlayerRoomController>();
        Debug.Log($"_____check playerControllerArr = {playerRoomControllerArr.Length}");

        foreach (var item in playerRoomControllerArr) {
            if(item == playerRoomController) continue;
            string targetTeamID = item.GetComponent<PlayerRoomController>().TeamID.ToString();
            if(playerTeamID == targetTeamID) item.miniMapTeamMateImage.SetActive(true);
            
        }
    }

}
