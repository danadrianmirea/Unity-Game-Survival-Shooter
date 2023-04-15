using UnityEngine;
using UnityEngine.SceneManagement;

public class Temple : MonoBehaviour
{
    [SerializeField]
    private HUD Hud;

    [SerializeField]
    private EnemyManager enemyManager;

    private TimerManager _timer;
    public TimerManager timer
    {
        get
        {
            if (_timer == null)
            {
                _timer = FindObjectOfType<TimerManager>();
            }

            return _timer;
        }
    }

    private readonly QuestType[] stepQuests = { QuestType.FirstQuest, QuestType.SecondQuest, QuestType.ThirdQuest, QuestType.FinalQuest };
    private int idxCurrentQuest = 0;
    private bool playerOnRange = false;
    private bool onQuest = false;
    private QuestNumberEnemy questNumberEnemy = null;

    public int IdxCurrentQuest
    {
        get => idxCurrentQuest;
        set
        {
            idxCurrentQuest = value;
        }
    }

    public bool OnQuest { get => onQuest; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerOnRange && Input.GetKeyDown(KeyCode.G))
        {
            ToastManager.Instance.ShowToast("ENTERING QUEST",1);
            EnteringQuest();
        }
    }

    private void EnteringQuest()
    {
        if (onQuest) return;

        onQuest = true;
        questNumberEnemy = QuestConfig.GetNumberEnemy(stepQuests[idxCurrentQuest]).Clone();
        
        enemyManager.gameObject.SetActive(true);
        timer.StartTimer();
    }

    private void ExitingQuest()
    {
        timer.StopTimer();
        onQuest = false;
        questNumberEnemy = null;
        enemyManager.gameObject.SetActive(false);
        var enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            Debug.Log("Killing"+ enemy);
            enemy.Death();
            //Destroy(enemy.gameObject);
        }

        idxCurrentQuest++;
        ToastManager.Instance.ShowToast("Quest " + idxCurrentQuest + " is Completed!", 1);

        // retrieve the time
        // add it to the global time
        // remove the timer
        var questTime = timer.TakeTime();
        timer.gameObject.SetActive(false);
        ToastManager.Instance.ShowToast("Your total time now: " 
            + System.TimeSpan.FromSeconds(GlobalManager.Instance.TotalTime).ToString("ss")
            + " + " + System.TimeSpan.FromSeconds(questTime).ToString("ss") + " Seconds", 2);
        GlobalManager.Instance.TotalTime += questTime;
        ToastManager.Instance.ShowToast(System.TimeSpan.FromSeconds(GlobalManager.Instance.TotalTime).ToString("ss") + " Seconds", 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (onQuest)
            {
                ToastManager.Instance.ShowToast("- Good Luck with your Quest -",2);
            }
            else
            {
                ToastManager.Instance.ShowToast("- Press G to Enter Quest " +
                    (idxCurrentQuest+1) + " - ",2);
            }
            playerOnRange = true;
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Hud.CloseMessagePanel();
            playerOnRange = false;
        }
    }

    private int GetIdxQuestType(QuestType type)
    {
        switch (type)
        {
            case QuestType.FirstQuest: return 0;
            case QuestType.SecondQuest: return 1;
            case QuestType.ThirdQuest: return 2;
            case QuestType.FinalQuest: return 3;
            default: return -1;
        }
    }

    public void OnDeathEnemy(EnemyType enemyType)
    {
        if (!onQuest) return;

        Debug.Log("OnDeathEnemy");
        questNumberEnemy.Decrement(enemyType);
        Debug.Log("enemyType: " + enemyType + " > " + questNumberEnemy.Get(enemyType));
        Debug.Log("isEmpty: " + questNumberEnemy.IsEmpty());
        if (questNumberEnemy.IsEmpty())
        {
            ExitingQuest();
            if (enemyType.Equals(EnemyType.FinalBoss))
            {
                // masukin ke leaderboard
                ScoreBoardScoreManager.Instance.AddScore(new Score(GlobalManager.Instance.PlayerName, (float) GlobalManager.Instance.TotalTime));
                SceneManager.LoadSceneAsync("CutsceneEnding");
                return;
            }
        }
        else
        {
            ToastManager.Instance.ShowToast("Quest Enemies Left:\n" + questNumberEnemy.Stats(), 3);
        }
    }
}
