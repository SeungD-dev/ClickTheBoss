using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ScottGarland;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

public class Game : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public float currentScore;
        public float currentAtkStat;
        public float currentMoney;
        public float currentMoneyRatio;
        public bool isAutoClicking;
        public float atkUpgradeCost;
        public float moneyUpgradeCost;
        public float autoClickUpgradeCost;
        public float autoClickSpeedUpCost;
        public float autoClickSpeed;
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.currentScore = currentScore;
        data.currentMoney = currentMoney;
        data.currentMoneyRatio = currentMoneyRatio;
        data.currentAtkStat = currentAtkStat;
        data.isAutoClicking = isAutoClicking;
        data.atkUpgradeCost = atkUpgradeCost;
        data.moneyUpgradeCost = moneyUpgradeCost;
        data.autoClickUpgradeCost = autoClickUpgradeCost;
        data.autoClickSpeedUpCost = autoClickSpeedUpCost;
        data.autoClickSpeed = autoClickSpeed;
       
        

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SaveData));
        MemoryStream memoryStream = new MemoryStream();
        serializer.WriteObject(memoryStream, data);
        string json = Encoding.Default.GetString(memoryStream.ToArray());

        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/save.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/save.json");

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SaveData));
            MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(json));
            SaveData data = (SaveData)serializer.ReadObject(memoryStream);

            currentScore = data.currentScore;
            currentMoney = data.currentMoney;
            
        }
    }

    [Header("Manage Score")]
    [SerializeField] Text scoreTxt;
    public float currentScore;
    public float scoreIncreasePerSecond;
    public float x;

    [Header("Manage Shop")]
    [SerializeField] GameObject ShopPanel;
    [SerializeField] Text currentAtkTxt;
    [SerializeField] Text currentMoneyTxt;
    [SerializeField] Text currentMoneyRatioTxt;
    [SerializeField] Text currentAutoClickStatusTxt;
    [SerializeField] Text currentAtkUpgradeCostTxt;
    [SerializeField] Text currentMoneyUpgradeCostTxt;
    [SerializeField] Text autoClickUpgradeCostTxt;
    [SerializeField] Text autoClickSpeedUpCostTxt;
    [SerializeField] Text currentAutoClickSpeedTxt;
    public float currentAtkStat;
    public float currentMoney;
    public float currentMoneyRatio;
    public bool isAutoClicking;
    public float atkUpgradeCost;
    public float moneyUpgradeCost;
    public float autoClickUpgradeCost;
    public float autoClickSpeedUpCost;
    public float autoClickSpeed;

    [Header("Manage VFX")]
    [SerializeField] GameObject floatingDamageTxt;
    [SerializeField] ParticleSystem effectParticle;

    public Player player;


    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/save.json"))
        {
            LoadGame();
        }
        else
        {
            currentScore = 0f;
            currentAtkStat = 100f;
            scoreIncreasePerSecond = 1f;
            x = 0f;
            currentMoney = 1000f;
            currentMoneyRatio = 1f;
            autoClickUpgradeCost = 5000f;
            atkUpgradeCost = 5000;
            moneyUpgradeCost = 5000f;
            autoClickSpeedUpCost = 5000f;
            autoClickSpeed = 1f;
            ShopPanel.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    void Update()
    {     
        UpdateTexts();
        scoreIncreasePerSecond = x * Time.deltaTime;
        currentScore = currentScore + scoreIncreasePerSecond;
    }

    public void UpdateTexts()
    {
        scoreTxt.text = "Score : " + GetNumberText((int)currentScore);
        currentMoneyTxt.text = "���� ������ : " + GetNumberText((int)currentMoney);
        currentMoneyRatioTxt.text = "���� ���� : " + currentMoneyRatio;
        currentAtkTxt.text = "���ݷ� : " + GetNumberText((int)currentAtkStat);
        currentAutoClickStatusTxt.text = "�ڵ� Ŭ����� : " + isAutoClicking;
        currentAtkUpgradeCostTxt.text = "���ݷ� ��ȭ ��� : " + GetNumberText((int)atkUpgradeCost);
        currentMoneyUpgradeCostTxt.text = "���� ���� ��ȭ ��� : " + GetNumberText((int)moneyUpgradeCost);
        autoClickUpgradeCostTxt.text = "�ڵ� Ŭ�� ��� ��ȭ ��� : " + autoClickUpgradeCost;
        autoClickSpeedUpCostTxt.text = "�ڵ� Ŭ�� �ӵ� ���� ��ȭ ��� : " + autoClickSpeedUpCost;
        currentAutoClickSpeedTxt.text = "���� �ڵ� Ŭ�� �ӵ� : " + autoClickSpeed;

    }

    public void Hit()
    {
        currentScore += currentAtkStat;
        currentMoney += currentAtkStat * currentMoneyRatio;
        StartCoroutine(FloatingDamage());
        player.BossClicked();


        Vector3 effectPos = new Vector3(-0.67f, -1.15f);
        
        ParticleSystem createdEffect = Instantiate(effectParticle, effectPos, Quaternion.identity);

        // ��ƼŬ �ý����� ������ ������ �ı�
        StartCoroutine(DestroyAfterSeconds(createdEffect, createdEffect.main.duration));
    }

    private IEnumerator DestroyAfterSeconds(ParticleSystem ps, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(ps.gameObject);
    }

    public void OpenShop()
    {
        ShopPanel.SetActive(true);
       
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
       
    }

    public void UpgradeAtk()
    {
        if(currentMoney >= atkUpgradeCost)
        {
            currentMoney -= atkUpgradeCost;
            currentAtkStat *= 2;
            atkUpgradeCost *= 2;
        }
        else
        {
            return;
        }
    }

    public void UpgradeMoney()
    {
        if (currentMoney >= moneyUpgradeCost)
        {
            currentMoney -= moneyUpgradeCost;
            currentMoneyRatio *= 2;
            moneyUpgradeCost *= 2;
        }
        else
        {
            return;
        }
    }

    public void EnableAutoClick()
    {
        if(currentMoney >= autoClickUpgradeCost)
        {
            currentMoney -= autoClickUpgradeCost;
            isAutoClicking = true;
            StartCoroutine(AutoClick());
        }
        else
        {
            return;
        }
    }

    public void UpgradeAutoClickSpeed()
    {
        if(currentMoney >= autoClickUpgradeCost)
        {
            currentMoney -= autoClickUpgradeCost;
            autoClickSpeed -= 0.1f;
            autoClickSpeedUpCost *= 2;
        }
        else {  return;}
    }

    private IEnumerator AutoClick()
    {
        while(isAutoClicking)
        {
            Hit();
            yield return new WaitForSeconds(autoClickSpeed);
        }
    }

    private IEnumerator FloatingDamage()
    {
        Vector3[] positions = new Vector3[]
        {
        new Vector3(-1.16f, 0.31f),
        new Vector3(-1.16f, 1.77f),
        new Vector3(-0.15f, 1.77f),
        new Vector3(0.91f, 1.77f),
        new Vector3(1.67f, 1.77f),
        new Vector3(1.67f, 0.37f)
        };

        Vector3 randomPos = positions[Random.Range(0, positions.Length)];
        GameObject damageTextInstance = Instantiate(floatingDamageTxt, randomPos, Quaternion.identity) as GameObject;
        damageTextInstance.transform.GetChild(0).GetComponent<TextMesh>().text = " " + GetNumberText((int)currentAtkStat);
        StartCoroutine(MoveUp(damageTextInstance.transform));
        yield return new WaitForSeconds(1);
        Destroy(damageTextInstance);
    }

    private IEnumerator MoveUp(Transform transform)
    {
        float speed = 1f; 
        float duration = 1f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private string[] numberUnitArr = new string[] { "", "K", "M","B", "T" }; //K = õ, M = �鸸, B = �ʾ� , T = ��

    private string GetNumberText(BigInteger value)
    {
        int placeN = 0;
        while (value >= 1000 && placeN < numberUnitArr.Length - 1)
        {
            value /= 1000;
            placeN++;
        }
        return value.ToString() + numberUnitArr[placeN];
    }


}

