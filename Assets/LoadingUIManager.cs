using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour {

    [SerializeField] List<GameObject> instructions;
    [SerializeField] Button nextButton;
    [SerializeField] Button backButton;

    int currentPage = 0;

    private void Start()
    {
        CheckButtonsAvailability();
    }

    public void OnClickNextPage()
    {
        instructions[currentPage].SetActive(false);
        currentPage++;
        instructions[currentPage].SetActive(true);
        CheckButtonsAvailability();
    }

    public void OnClickBackPage()
    {
        instructions[currentPage].SetActive(false);
        currentPage--;
        instructions[currentPage].SetActive(true);
        CheckButtonsAvailability();
    }

    private void CheckButtonsAvailability()
    {
        if (currentPage >= instructions.Count - 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }

        if (currentPage <= 0)
        {
            backButton.interactable = false;
        }
        else
        {
            backButton.interactable = true;
        }
    }
}
