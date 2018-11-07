using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum PowerUps { freeze, confuse, shield, weaken, blind, slippery, orbitalBeam, teleport };

public class PowerUpScript : NetworkBehaviour {

    [Header("Trickery Durations")]
    [SerializeField] float frozenDuration = 5.0f;
    [SerializeField] float confuseDuration = 5.0f;
    [SerializeField] float weakenDuration = 5.0f;
    [SerializeField] float blindDuration = 5.0f;
    [SerializeField] float slipperyDuration = 5.0f;

    [Header("Buff Duration")]
    [SerializeField] float shieldDuration = 5.0f;
    [SerializeField] float orbitalBeamDuration = 10.0f;

    [Header("Buff / Debuff Parameters")]
    [SerializeField] float weakenWalkSpeedReductionMultiplier = 2.0f;
    [SerializeField] float weakenJumpSpeedReductionMultiplier = 2.0f;
    [SerializeField] float slipperyMultiplier = 2.5f;

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

    #region randomizing power-ups
    public void RandomizedPower()
    {
        float calculateDistanceDifference = playerObject.transform.position.y - opponentObject.transform.position.y;

        if (calculateDistanceDifference > 100f)
        {
            Debug.Log("You are way ahead.");
            // Common trickery: 10%; Shield: 90%
            LeadingAndWayLeadingPowerUpRNG(2.5f, 5.0f, 7.5f, 10.0f, 100f);
        }
        else if (calculateDistanceDifference > 50f && calculateDistanceDifference <= 100f)
        {
            Debug.Log("You are leading.");
            // Common trickery: 50%; Shield: 50%
            LeadingAndWayLeadingPowerUpRNG(12.5f, 25.0f, 37.5f, 50.0f, 100f);
        }
        else if (calculateDistanceDifference > -50f && calculateDistanceDifference <= 50f)
        {
            Debug.Log("Deadlock state");
            // Freeze: 5%; Common trickery + Shield: 90%, Teleport: 5%
            DeadlockPowerUpRNG(5.0f, 23.0f, 41.0f, 59.0f, 77.0f, 95.0f, 100.0f);
        }
        else if (calculateDistanceDifference > -100f && calculateDistanceDifference <= -50f)
        {
            Debug.Log("You are falling behind");
            // Freeze: 35%; Common Trickery: 40%; Teleport: 25%
            FallingBehindPowerUpRNG(35.0f, 45.0f, 55.0f, 65.0f, 75.0f, 100f);
        }
        else if (calculateDistanceDifference <= -100f)
        {
            Debug.Log("You are falling way behind.");
            // Freeze: 60% (Or Less); OBeam: 10% (Or More); Teleport: 30%
            FallingWayBehindPowerUpRNG(60.0f, 70.0f, 100.0f, calculateDistanceDifference);
        }
        else
        {
            Debug.Log("Not registered distance");
        }

        thisPlayerPowerUpUI.SetUpPowerUpImageAndReadiness(currentPowerUp, true);
    }

    void LeadingAndWayLeadingPowerUpRNG(float confuseRange, float weakenRange, float blindRange, 
                                        float slipperyRange, float shieldRange)
    {
        float powerUpRNG = Random.Range(1f, 100f);
        Debug.Log("Leading PowerUpRNG: " + powerUpRNG);

        if (powerUpRNG <= confuseRange)
        {
            currentPowerUp = PowerUps.confuse;
        }
        else if (powerUpRNG > confuseRange && powerUpRNG <= weakenRange)
        {
            currentPowerUp = PowerUps.weaken;
        }
        else if (powerUpRNG > weakenRange && powerUpRNG <= blindRange)
        {
            currentPowerUp = PowerUps.blind;
        }
        else if (powerUpRNG > blindRange && powerUpRNG <= slipperyRange)
        {
            currentPowerUp = PowerUps.slippery;
        }
        else if (powerUpRNG > slipperyRange && powerUpRNG <= shieldRange)
        {
            currentPowerUp = PowerUps.shield;
        }
    }

    void DeadlockPowerUpRNG(float freezeRange, float confuseRange, float weakenRange, 
                            float blindRange, float slipperyRange, float shieldRange,
                            float teleportRange)
    {
        float powerUpRNG = Random.Range(1f, 100f);
        Debug.Log("Deadlock PowerUpRNG: " + powerUpRNG);

        if (powerUpRNG <= freezeRange)
        {
            currentPowerUp = PowerUps.freeze;
        }
        else if (powerUpRNG > freezeRange && powerUpRNG <= confuseRange)
        {
            currentPowerUp = PowerUps.confuse;
        }
        else if (powerUpRNG > confuseRange && powerUpRNG <= weakenRange)
        {
            currentPowerUp = PowerUps.weaken;
        }
        else if (powerUpRNG > weakenRange && powerUpRNG <= blindRange)
        {
            currentPowerUp = PowerUps.blind;
        }
        else if (powerUpRNG > blindRange && powerUpRNG <= slipperyRange)
        {
            currentPowerUp = PowerUps.slippery;
        }
        else if (powerUpRNG > slipperyRange && powerUpRNG <= shieldRange)
        {
            currentPowerUp = PowerUps.shield;
        }
        else if (powerUpRNG > shieldRange && powerUpRNG <= teleportRange)
        {
            currentPowerUp = PowerUps.teleport;
        }
    }

    void FallingBehindPowerUpRNG(float freezeRange, float confuseRange, float weakenRange,
                                 float blindRange, float slipperyRange, float teleportRange)
    {
        float powerUpRNG = Random.Range(1f, 100f);
        Debug.Log("Falling behind PowerUpRNG: " + powerUpRNG);

        if (powerUpRNG <= freezeRange)
        {
            currentPowerUp = PowerUps.freeze;
        }
        else if (powerUpRNG > freezeRange && powerUpRNG <= confuseRange)
        {
            currentPowerUp = PowerUps.confuse;
        }
        else if (powerUpRNG > confuseRange && powerUpRNG <= weakenRange)
        {
            currentPowerUp = PowerUps.weaken;
        }
        else if (powerUpRNG > weakenRange && powerUpRNG <= blindRange)
        {
            currentPowerUp = PowerUps.blind;
        }
        else if (powerUpRNG > blindRange && powerUpRNG <= slipperyRange)
        {
            currentPowerUp = PowerUps.slippery;
        }
        else if (powerUpRNG > slipperyRange && powerUpRNG <= teleportRange)
        {
            currentPowerUp = PowerUps.teleport;
        }
    }

    void FallingWayBehindPowerUpRNG(float freezeRange, float oBeamRange, float teleportRange,
                                    float differenceDist)
    {
        float powerUpRNG = Random.Range(1f, 100f);
        Debug.Log("Falling Way Behind PowerUpRNG: " + powerUpRNG);
        float absDifference = Mathf.Abs(differenceDist);

        // Max Freeze Range reduction is to 30
        if (absDifference <= 300f)
        {
            freezeRange -= (absDifference - 100f) * 0.2f;
        }
        else
        {
            freezeRange = 20.0f;
        }

        if (powerUpRNG <= freezeRange)
        {
            currentPowerUp = PowerUps.freeze;
        }
        else if (powerUpRNG > freezeRange && powerUpRNG <= oBeamRange)
        {
            currentPowerUp = PowerUps.orbitalBeam;
        }
        else if (powerUpRNG > oBeamRange && powerUpRNG <= teleportRange)
        {
            currentPowerUp = PowerUps.teleport;
        }
    }
    #endregion

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
            case PowerUps.weaken:
                CmdWeakenOpponentPlayer(opponentObject, weakenDuration);
                SendCasterResultText(currentPowerUp, opponentObject);
                break;
            case PowerUps.blind:
                CmdBlindOpponentPlayer(opponentObject, blindDuration);
                SendCasterResultText(currentPowerUp, opponentObject);
                break;
            case PowerUps.slippery:
                CmdSlipperyOpponentPlayer(opponentObject, slipperyDuration);
                SendCasterResultText(currentPowerUp, opponentObject);
                break;
            case PowerUps.shield:
                CmdShieldCurrentPlayer(playerObject, shieldDuration);
                SendCasterResultText(currentPowerUp, playerObject);
                break;
            case PowerUps.orbitalBeam:
                CmdOrbitalBeamCurrentPlayer(playerObject, orbitalBeamDuration);
                SendCasterResultText(currentPowerUp, playerObject);
                CmdOrbitalBeamOpponentPlayer(opponentObject);         // Notify and Play Sound
                break;
            case PowerUps.teleport:
                CmdTeleportCurrentPlayer(playerObject);
                SendCasterResultText(currentPowerUp, playerObject);
                break;
            default:
                Debug.LogError("No Execution for this Power Up!");
                break;
        }
    }

    void SendCasterResultText(PowerUps usedPowerUp, GameObject target)
    {
        // If this is a buff
        if (target == playerObject)
        {
            thisPlayerPowerUpUI.UserPowerUpSuccessfulInflictionText(usedPowerUp);
            return;
        }

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

    public float GetSlipperyMultiplier()
    {
        return slipperyMultiplier;
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

    [Command]
    void CmdBlindOpponentPlayer(GameObject player, float duration)
    {
        RpcBlindOpponentPlayer(player, duration);
    }

    [Command]
    void CmdSlipperyOpponentPlayer(GameObject player, float duration)
    {
        RpcSlipperyOpponentPlayer(player, duration);
    }

    [Command]
    void CmdOrbitalBeamCurrentPlayer(GameObject player, float duration)
    {
        RpcOrbitalBeamCurrentPlayer(player, duration);
    }

    [Command]
    void CmdOrbitalBeamOpponentPlayer(GameObject player)
    {
        RpcOrbitalBeamOpponentPlayer(player);
    }

    [Command]
    void CmdTeleportCurrentPlayer(GameObject player)
    {
        RpcTeleportCurrentPlayer(player);
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

    [ClientRpc]
    void RpcBlindOpponentPlayer(GameObject player, float duration)
    {
        player.GetComponent<NetworkPlayer>().BlindPlayer(duration);
    }

    [ClientRpc]
    void RpcSlipperyOpponentPlayer(GameObject player, float duration)
    {
        player.GetComponent<NetworkPlayer>().SlipperyPlayer(duration);
    }

    [ClientRpc]
    void RpcOrbitalBeamCurrentPlayer(GameObject player, float duration)
    {
        player.GetComponent<NetworkPlayer>().OrbitalBeamPlayer(duration);
    }

    [ClientRpc]
    void RpcOrbitalBeamOpponentPlayer(GameObject player)
    {
        player.GetComponent<NetworkPlayer>().OrbitalBeamWarning();
    }

    [ClientRpc]
    void RpcTeleportCurrentPlayer(GameObject player)
    {
        player.GetComponent<NetworkPlayer>().TeleportPlayer();
    }
}
