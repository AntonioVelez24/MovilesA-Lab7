using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameTMP;
    [SerializeField] private TMP_Text levelTMP;
    [SerializeField] private TMP_Text experienceTMP;
    [SerializeField] private TMP_Text skillPointsTMP;

    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text agilityText;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private Image experienceBar;

    [SerializeField] private Button updateNameButton;
    [SerializeField] private Button openNamePanelButton;
    [SerializeField] private Button closeNamePanelButton;

    [SerializeField] private Button addStrengthButton;
    [SerializeField] private Button addDefenseButton;
    [SerializeField] private Button addAgilityButton;

    private int playerLevel;
    private int experience;
    private int skillPoints;

    private int strength;
    private int defense;
    private int agility;
    private async void Start()
    {
        await LoadPlayerData();

        levelPanel.SetActive(true);
        namePanel.SetActive(false);

        updateNameButton.onClick.AddListener(UpdateName);
        openNamePanelButton.onClick.AddListener(OpenNamePaneL);
        closeNamePanelButton.onClick.AddListener(CloseNamePanel);

        addStrengthButton.onClick.AddListener(() => AddSkillPoint("strength"));
        addDefenseButton.onClick.AddListener(() => AddSkillPoint("defense"));
        addAgilityButton.onClick.AddListener(() => AddSkillPoint("agility"));
    }
    private void UpdateUI()
    {
        playerNameTMP.text = AuthenticationService.Instance.PlayerName;
        levelTMP.text = playerLevel.ToString();
        experienceTMP.text = "Exp: " + experience + "/100";
        skillPointsTMP.text = "Skill Points: " + skillPoints;

        strengthText.text = strength.ToString();
        defenseText.text = defense.ToString();
        agilityText.text = agility.ToString();

        if (experienceBar != null)
        {
            experienceBar.fillAmount = (float)experience / 100f; 
        }
    }

    private async Task LoadPlayerData()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
            new HashSet<string> { "playerLevel", "experience", "skillPoints", "strength", "defense", "agility" });

        if (playerData.TryGetValue("playerLevel", out var level))
            playerLevel = level.Value.GetAs<int>();
        if (playerData.TryGetValue("experience", out var exp))
            experience = exp.Value.GetAs<int>();
        if (playerData.TryGetValue("skillPoints", out var points))
            skillPoints = points.Value.GetAs<int>();
        if (playerData.TryGetValue("strength", out var str))
            strength = str.Value.GetAs<int>();
        if (playerData.TryGetValue("defense", out var def))
            defense = def.Value.GetAs<int>();
        if (playerData.TryGetValue("agility", out var agi))
            agility = agi.Value.GetAs<int>();

        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            GainExperience(1); 
        }
    }

    private void GainExperience(int amount)
    {
        experience += amount;

        SaveData("experience", experience.ToString());

        if (experience >= 100)
        {
            LevelUp();
        }

        UpdateUI(); 
    }
    private void LevelUp()
    {
        playerLevel++;

        skillPoints++;

        experience = 0;

        SaveData("playerLevel", playerLevel.ToString());
        SaveData("experience", experience.ToString());
        SaveData("skillPoints", skillPoints.ToString());

        UpdateUI();
    }


    private async void UpdateName()
    {
        string newName = nameInputField.text;

        await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
        var name = await AuthenticationService.Instance.GetPlayerNameAsync();

        playerNameTMP.text = name;

        CloseNamePanel();
    }

    private void OpenNamePaneL()
    {
        namePanel.SetActive(true);
        levelPanel.SetActive(false);
    }

    private void CloseNamePanel()
    {
        namePanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    private async void AddSkillPoint(string stat)
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            if (stat == "strength")
                strength++;
            if (stat == "defense")
                defense++;
            if (stat == "agility")
                agility++;

            SaveData("strength", strength.ToString());
            SaveData("defense", defense.ToString());
            SaveData("agility", agility.ToString());
            SaveData("skillPoints", skillPoints.ToString());

            UpdateUI();
        }
        else
        {
            Debug.Log("No hay puntos suficientes");
        }
    }
    public async void SaveData(string key, string value)
    {
        var playerData = new Dictionary<string, object>()
        {
            {key, value}
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
    }
}
