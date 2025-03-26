using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class BackgroundCreator : CharacterCreation
{
    [System.Serializable]
    public class QuestionData
    {
        public string id;               // internal key
        public string questionText;     // display text
        public List<string> answers;    // list of 6 options
    }

    [Header("UI References")]
    public TMP_Text questionText;
    public List<Button> answerButtons;
    public TMP_Text textSummary;
    public List<GameObject> questionIndicators; // one for each question
    public GameObject summaryPanel; // Drag this in via Inspector

    private List<QuestionData> questions = new List<QuestionData>();
    private Dictionary<string, string> selectedValues = new Dictionary<string, string>();
    private int currentQuestionIndex = 0;

    void Start()
    {
        InitializeQuestions();
        ShowQuestion();
    }

    void InitializeQuestions()
    {
        questions.Clear();

        questions.Add(new QuestionData
        {
            id = "Environment",
            questionText = "Where were you born?",
            answers = new List<string> {
                "a poor district", "a remote mountain village", "a nomadic tribe",
                "a noble estate", "a forgotten ruin city", "a guildhall in a bustling city"
            }
        });

        questions.Add(new QuestionData
        {
            id = "People",
            questionText = "Who did you grow up around?",
            answers = new List<string> {
                "thieves and sneaks", "learned mages", "war-hardened survivors",
                "monks and wise elders", "joyful farming families", "fanatical cultists"
            }
        });

        questions.Add(new QuestionData
        {
            id = "Skill",
            questionText = "What did you learn early in life?",
            answers = new List<string> {
                "how to fight with a sword", "how to manipulate others", "how to survive in the wild",
                "how to move unseen", "how to read magical texts", "how to heal wounds"
            }
        });

        questions.Add(new QuestionData
        {
            id = "Mentor",
            questionText = "Who trained or guided you?",
            answers = new List<string> {
                "an old assassin", "a secret order", "your uncle, a former adventurer",
                "temple priests", "a disguised dragon", "the warrior academy in the stone fortress"
            }
        });

        questions.Add(new QuestionData
        {
            id = "Attitude",
            questionText = "What kind of person did you become?",
            answers = new List<string> {
                "a cynical loner", "a driven avenger", "a protective leader",
                "a curious explorer", "a hidden hero", "a hesitant but loyal friend"
            }
        });

        questions.Add(new QuestionData
        {
            id = "Motivation",
            questionText = "What motivates you?",
            answers = new List<string> {
                "justice", "revenge", "truth", "power", "freedom", "redemption"
            }
        });

        questions.Add(new QuestionData
        {
            id = "Fear",
            questionText = "What do you fear the most?",
            answers = new List<string> {
                "failure", "being forgotten", "losing control",
                "darkness", "betrayal", "their true self being revealed"
            }
        });

        questions.Add(new QuestionData
        {
            id = "Flaw",
            questionText = "What is your greatest flaw?",
            answers = new List<string> {
                "impulsiveness", "pride", "distrust of others",
                "vengeful nature", "naïveté", "self-doubt"
            }
        });

        questions.Add(new QuestionData
        {
            id = "Desire",
            questionText = "What do you secretly desire?",
            answers = new List<string> {
                "to be remembered", "to find lost knowledge", "to earn forgiveness",
                "to prove their worth", "to rule one day", "to escape fate"
            }
        });

        // Make sure indicators are off at the beginning
        foreach (var indicator in questionIndicators)
        {
            indicator.SetActive(false);
        }
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            GenerateBackground();
            return;
        }

        var question = questions[currentQuestionIndex];
        questionText.text = question.questionText;

        // 🔄 Reset button visuals before assigning new answers
        EventSystem.current.SetSelectedGameObject(null); // Deselect anything
        foreach (var button in answerButtons)
        {
            var colors = button.colors;
            button.colors = colors; // Forces refresh
        }

        for (int i = 0; i < answerButtons.Count; i++)
        {
            int index = i;
            var btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
            btnText.text = question.answers[i];

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(index));
        }
    }

    public void SelectAnswer(int answerIndex)
    {
        var question = questions[currentQuestionIndex];
        string selectedAnswer = question.answers[answerIndex];
        selectedValues[question.id] = selectedAnswer;

        // ✅ Activate corresponding indicator
        if (currentQuestionIndex < questionIndicators.Count)
        {
            questionIndicators[currentQuestionIndex].SetActive(true);
        }

        currentQuestionIndex++;
        ShowQuestion();
    }

    public void GenerateBackground()
    {
        // Existing code for grabbing selected values...
        string env = selectedValues["Environment"];
        string people = selectedValues["People"];
        string skill = selectedValues["Skill"];
        string mentor = selectedValues["Mentor"];
        string attitude = selectedValues["Attitude"];
        string motivation = selectedValues["Motivation"];
        string fear = selectedValues["Fear"];
        string flaw = selectedValues["Flaw"];
        string desire = selectedValues["Desire"];

        _dictionaryLibrary.backgroundSkills.Clear();

        void AddSkills(string key)
        {
            if (_dictionaryLibrary.skillMap.ContainsKey(key))
            {
                foreach (var s in _dictionaryLibrary.skillMap[key])
                {
                    if (!_dictionaryLibrary.backgroundSkills.Contains(s))
                        _dictionaryLibrary.backgroundSkills.Add(s);
                }
            }
        }

        AddSkills(env);
        AddSkills(attitude);
        AddSkills(mentor);

        string skillsSummary = string.Join(", ", _dictionaryLibrary.backgroundSkills);

        string summary = $"You were born in {env}, surrounded by {people}. " +
                         $"From an early age, life taught you {skill}, under the guidance of {mentor}. " +
                         $"Over time, these experiences molded you into {attitude}.\n\n" +
                         $"Your journey is driven by a longing for {motivation}, but beneath your resolve lies a fear of {fear}. " +
                         $"You often struggle with your {flaw}, which has both hindered and defined your path. " +
                         $"And though few know it, your heart secretly desires {desire}.\n\n" +
                         $"<b>Skill Proficiencies:</b> {skillsSummary}\n\n" +
                         $"This is who you are. And yet, your story is only just beginning...";

        textSummary.text = summary;

        // ✅ Show the summary panel!
        if (summaryPanel != null)
            summaryPanel.SetActive(true);
    }

    public void ResetBackgroundCreator()
    {
        // Reset data
        selectedValues.Clear();
        currentQuestionIndex = 0;

        // Hide summary
        if (summaryPanel != null)
            summaryPanel.SetActive(false);

        // Show question UI again (if you had hidden it)
        // if (questionUI != null)
        //     questionUI.SetActive(true);

        // Hide all indicators
        foreach (var indicator in questionIndicators)
        {
            if (indicator != null)
                indicator.SetActive(false);
        }

        // Show first question again
        ShowQuestion();
    }

}
