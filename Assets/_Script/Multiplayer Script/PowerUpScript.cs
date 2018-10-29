using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum PowerUps { freeze, confuse, shield, weaken };

public class PowerUpScript : NetworkBehaviour {

    [Header("Buff / Debuff Durations")]
    [SerializeField] float frozenDuration = 5.0f;
    [SerializeField] float confuseDuration = 5.0f;
    [SerializeField] float shieldDuration = 5.0f;
    [SerializeField] float weakenDuration = 5.0f;

    [Header("Buff / Debuff Parameters")]
    [SerializeField] float weakenWalkSpeedReductionMultiplier = 2.0f;
    [SerializeField] float weakenJumpSpeedReductionMultiplier = 2.0f;

    [SerializeField] PowerUps currentPowerUp;
    GameObject playerObject;
    GameObject opponentObject;
    NetworkPowerUpUI thisPlayerPowerUpUI;


    #region Initialization
    // Use this for initialization
    void Start()
    {
        if (!hasAuthority) { return; }

        StartSettingUpPlayersGameObject();
    }

    // PLAYER SETUP
    void StartSettingUpPlayersGameObject()
    {
        playerObject = this.gameObject;

        if (playerObject == null)
        {
            StartCoroutine(SearchPlayerObjectAgain());
            return;
        }

        LinkingPlayerPowerUpUI();
        SetupOpponentGameObject();
    }

    IEnumerator SearchPlayerObjectAgain()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        StartSettingUpPlayersGameObject();
    }

    void LinkingPlayerPowerUpUI()
    {
        NetworkPlayer thisPlayerScript = playerObject.GetComponent<NetworkPlayer>();

        if (thisPlayerScript.GetThisPlayerPowerUpUI() != null)
        {
            thisPlayerPowerUpUI = thisPlayerScript.GetThisPlayerPowerUpUI();
        }
        else
        {
            StartCoroutine(LinkingPlayerPowerUpUIAgain());
        }
    }

    IEnumerator LinkingPlayerPowerUpUIAgain()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        LinkingPlayerPowerUpUI();
    }

    // OPPONENT SETUP
    void SetupOpponentGameObject()
    {
        var inGamePlayerUnits = FindObjectsOfType<NetworkPlayer>();
        foreach (NetworkPlayer player in inGamePlayerUnits)
        {
            if (player.gameObject != gameObject)
            {
                opponentObject = player.gameObject;
            }
        }

        if (opponentObject == null)
        {
            StartCoroutine(SearchOpponentObjectAgain());
        }
    }

    IEnumerator SearchOpponentObjectAgain()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SetupOpponentGameObject();
    }

    #endregion

    public void RandomizedPower()
    {
        float calculateDistanceDifference = playerObject.transform.position.y - opponentObject.transform.position.y;

        if (calculateDistanceDifference < 0)
        {
            Debug.Log("You are falling behind!");
            RandomizedResultingPowerUp(25, 60, 95);
        }
        else if (calculateDistanceDifference > 0)
        {
            Debug.Log("You are leading!");
            RandomizedResultingPowerUp(5, 25, 45);
        }
        else
        {
            RandomizedResultingPowerUp(25, 50, 75);
        }

        thisPlayerPowerUpUI.SetUpPowerUpImageAndReadiness(currentPowerUp, true);
    }

    void RandomizedResultingPowerUp(int firstMax, int secondMax, int thirdMax)
    {
        int powerUpRNG = Random.Range(1, 100);
        Debug.Log("PowerUpRNG: " + powerUpRNG);

        if (powerUpRNG <= firstMax)
        {
            currentPowerUp = PowerUps.freeze;
        }
        else if (powerUpRNG > firstMax && powerUpRNG <= secondMax)
        {
            currentPowerUp = PowerUps.confuse;
        }
        else if (powerUpRNG > secondMax && powerUpRNG <= thirdMax)
        {
            currentPowerUp = PowerUps.weaken;
        }
        else if (powerUpRNG > thirdMax)
        {
            currentPowerUp = PowerUps.shield;
        }
    }


    public void ExecutePower(PowerUps currentPowerUp)
    {
        switch (currentPowerUp)
        {
            case PowerUps.freeze:
                CmdFreezeOpponentPlayer(opponentObject, frozenDuration);
                SendCasterResultText(currentPowerUp, opponentObject);
                break;
            case PowerUps.confuse:
                CmdConfuseOpponentPlayer(opponentObject, confuseDuration);
                SendCasterResultText(currentPowerUp, opponentObject);
                break;
            case PowerUps.shield:
                CmdShieldCurrentPlayer(playerObject, shieldDuration);
                break;
            case PowerUps.weaken:
                CmdWeakenOpponentPlayer(opponentObject, weakenDuration);
                SendCasterResultText(currentPowerUp, opponentObject);
                break;
            default:
                Debug.LogError("No Execution for this Power Up!");
                break;
        }
    }

    void SendCasterResultText(PowerUps usedPowerUp, GameObject target)
    {
        // If opponent is NOT shielded
        if (target.GetComponent<NetworkPlayer>().GetPlayerIsShielded() == false)
        {
            thisPlayerPowerUpUI.UserPowerUpSuccessfulInflictionText(usedPowerUp);
        }
        else
        {
            thisPlayerPowerUpUI.UserPowerUpNegatedText();
        }
    }

    #region Getter and Setter
    public float GetWeakenWalkSpeedReductionMultiplier()
    {
        return weakenWalkSpeedReductionMultiplier;
    }

    public float GetWeakenJumpSpeedReductionMultiplier()
    {
        return weakenJumpSpeedReductionMultiplier;
    }
    #endregion


    [Command]
    void CmdFreezeOpponentPlayer(GameObject player, float duration)
    {
        RpcFreezeOpponentPlayer(player, duration);
    }

    [Command]
    void CmdConfuseOpponentPlayer(GameObject player, float duration)
    {
        RpcConfuseOpponentPlayer(player, duration);
    }

    [Command]
    void CmdShieldCurrentPlayer(GameObject player, float duration)
    {
        RpcShieldCurrentPlayer(player, duration);
    }

    [Command]
    void CmdWeakenOpponentPlayer(GameObject player, float duration)
    {
        RpcWeakenOpponentPlayer(player, duration);
    }


    [ClientRpc]
    void RpcFreezeOpponentPlayer(GameObject player, float duration)
    {
        player.GetComponent<NetworkPlayer>().FreezePlayer(duration);
    }

    [ClientRpc]
    void RpcConfuseOpponentPlayer(GameObject player, float duration)
    {
        player.GetComponent<NetworkPlayer>().ConfusePlayer(duration);
    }

    [ClientRpc]
    void RpcShieldCurrentPlayer(GameObject player, float duration)
    {
        player.GetComponent<NetworkPlayer>().ShieldPlayer(duration);
    }

    [ClientRpc]
    void RpcWeakenOpponentPlayer(GameObject player, float duration)
    {
        player.GetComponent<NetworkPlayer>().WeakenPlayer(duration);
    }

}
