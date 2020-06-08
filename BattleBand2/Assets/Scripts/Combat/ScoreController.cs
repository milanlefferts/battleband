using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreType
{
	Beat,
    Jam,
    Miss,
    NoteHit,
    AbilityUse,
    Combo,
    Chain
}

public class ScoreController : MonoBehaviour {

	public int Score {
		get
        {
			return scoreNumber; 
		}
	}

    public int ScoreLevel
    {
        get
        {
            return scoreLevel;
        }
    }

    private int totalScore;

    [SerializeField]
    private int scoreNumber;
    [SerializeField]
    private int scoreLevel;
    //private string[] scoreText;
    private int chain, chainPeak;

    private List<Ability> abilitiesUsed = new List<Ability>();
    private Ability previousAbility;

    private void OnEnable()
    {
        EventManager.BeatEvent += ScoreBeat;
        EventManager.BeaterHitEvent += ScoreJam;
        EventManager.BeaterMissEvent += ScoreMiss;

        EventManager.NoteHitEvent += ScoreNoteHit;

        EventManager.AbilityUseEvent += ComboCheck;
        
        EventManager.EndGameStateEvent += SendEndGameStats;
    }

    private void OnDisable()
    {
        EventManager.BeatEvent -= ScoreBeat;
        EventManager.BeaterHitEvent -= ScoreJam;
        EventManager.BeaterMissEvent -= ScoreMiss;

        EventManager.NoteHitEvent -= ScoreNoteHit;

        EventManager.AbilityUseEvent -= ComboCheck;

        EventManager.EndGameStateEvent -= SendEndGameStats;

    }

    private void Start () {
        /*
        scoreText = new string[] {
            "",
            "Fresh!",
            "Excellent!",
            "Dashing!",
            "Crazy!",
            "Blazing!",
            "Amazing!",
            "Snazzy!",
            "SSuper!",
            "SSSick!"
        };*/
        chain = 0;
        totalScore = 0;
        scoreNumber = 0;
        scoreLevel = 0;
        EventManager.Instance.UpdateChain(chain);
        EventManager.Instance.UpdateScore(totalScore);

        UpdateStarMeter();
    }

    private void AddScore(int val)
    {
		scoreNumber += (val * (scoreLevel + 1)); // Add to Total Score

        if (val > 0)
        {
            totalScore += val;
            EventManager.Instance.UpdateScore(totalScore);

            chain++;
            EventManager.Instance.UpdateChain(chain);
            UpdateChainPeak();
        }
		if (scoreNumber < 0)
        {
            DetractScoreLevel();
        }
        else if (scoreNumber >= 100 && scoreLevel < 5)
        {
            AddScoreLevel();
        }
        if (scoreNumber > 100)
        {
            scoreNumber = 100;
        }
        UpdateStarMeter();
    }

    private void DetractScoreLevel()
    {
        int tempScore = scoreNumber;
        if (scoreLevel != 0) // Go down a score level
        {
            scoreNumber = 100 + tempScore;
            scoreLevel--;
        } else // Reset to 0
        {
            scoreNumber = 0;
        }
    
        if (scoreLevel < 0)
            scoreLevel = 0;
    }

    private void AddScoreLevel()
    {
        scoreNumber = 0;
        scoreLevel++;
        if (scoreLevel > 5)
            scoreLevel = 5;

        StartCoroutine(PauseScoreDrain());
    }
    private bool scoreDraining = true;

    private IEnumerator PauseScoreDrain()
    {
        scoreDraining = false;
        yield return new WaitForSeconds(1.5f);
        scoreDraining = true;

    }

    private void UseAbility(Ability ability, Character user, Character target)
	{
		if (ability == previousAbility)
		{
            ScoreCheck(ScoreType.AbilityUse);
            abilitiesUsed.Clear();
            abilitiesUsed.Add(ability);
            previousAbility = ability;
        }
        else
        {
            abilitiesUsed.Add(ability);
            ComboCheck(ability, user, target);
        }
    }

    private void ComboCheck (Ability ability, Character user, Character target)
    {
        bool combo = false;
        switch (ability.type)
        {
            case AbilityType.AttackSingleTarget: // Increased Damage
                if (user.status == StatusEffect.AttackUp || target.status == StatusEffect.DefenseDown)
                {
                    ScoreCheck(ScoreType.Combo);
                    combo = true;
                }
                break;
            case AbilityType.Heal: // Increased Heal
                if (user.status == StatusEffect.AttackUp)
                {
                    ScoreCheck(ScoreType.Combo);
                    combo = true;
                }
                break;
            case AbilityType.Buff: // Dispell
                if (target.status == StatusEffect.DefenseDown || target.status == StatusEffect.AttackDown)
                {
                    ScoreCheck(ScoreType.Combo);
                    combo = true;
                }
                break;
            case AbilityType.Debuff: // Dispell
                if (target.status == StatusEffect.DefenseUp || target.status == StatusEffect.AttackUp)
                {
                    ScoreCheck(ScoreType.Combo);
                    combo = true;
                }
                break;
            default:
                break;
        }
        if (!combo)
            ScoreCheck(ScoreType.Chain);
    }

    private void ScoreCheck(ScoreType type)
	{
		switch (type) {
            case ScoreType.Beat:
                if (scoreDraining)
                    AddScore(-3);
                break;

            case ScoreType.Miss:
                AddScore(-20);
                chain = 0;
                EventManager.Instance.UpdateChain(chain);
                break;

            case ScoreType.Jam:
                AddScore(7);
                break;

            case ScoreType.NoteHit:
                AddScore(10);
                break;

            case ScoreType.AbilityUse:
                AddScore(12);
                break;

            case ScoreType.Chain:
                AddScore(15);
                break;

            case ScoreType.Combo:
                AddScore(20);
                break;

            default:
			    break;
		}
	}

    private void ScoreBeat()
    {
        ScoreCheck(ScoreType.Beat);
    }

    private void ScoreNoteHit()
    {
        ScoreCheck(ScoreType.NoteHit);
    }

    private void ScoreJam()
    {
        ScoreCheck(ScoreType.Jam);
    }

    private void ScoreMiss()
    {
        ScoreCheck(ScoreType.Miss);
    }
    
    private void UpdateStarMeter()
    {
        float scoreFrac = scoreNumber / 100f;
        EventManager.Instance.UpdateStarMeter(scoreFrac, scoreLevel); 
    }

    private void UpdateChainPeak()
    {
        if (chainPeak < chain)
            chainPeak = chain;
    }

    private void SendEndGameStats()
    {
        PartyController.Instance.AddFameScore(totalScore);
        EventManager.Instance.SetEndCombo(chainPeak);
        EventManager.Instance.SetEndScore(totalScore);
    }
    
}