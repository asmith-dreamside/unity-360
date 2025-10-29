using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

// Clase para representar una pregunta específica de objeto
[System.Serializable]
public class ObjectQuestion
{
    public string question;
    public List<string> options;
    public int answer;
    public string objectName; // Identificador del objeto al que pertenece esta pregunta
}

[System.Serializable]
public class ObjectQuizResult
{
    public int correctAnswers;
    public int totalQuestions;
    public List<QuestionResult> questionResults;
}

[System.Serializable]
public class ObjectQuizOption
{
    public Button optionButton;           // Botón de la opción
    public TextMeshProUGUI optionText;    // Texto completo de la opción
}

public class QuizManagerObject : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public List<QuizOption> quizOptions;  // Lista de opciones (botón + texto)
    // public TextMeshProUGUI feedbackText; // Comentado hasta implementar UI de feedback
    public string jsonFileName = "object_questions.json"; // Archivo JSON específico para preguntas de objetos
    public GameObject quizPanel;
    public Button toggleQuizButton;
    public ObjectInfoRaycast objectInfoRaycast; // Referencia al ObjectInfoRaycast

    private List<ObjectQuestion> allQuestions;
    private List<ObjectQuestion> currentObjectQuestions;
    private int currentQuestionIndex = 0;
    private ObjectQuizResult quizResult;
    private string resultsFileName;
    private string currentObjectName = "";

    private string GenerateResultsFileName()
    {
        return $"object_respuesta_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.json";
    }

    void Start()
    {
        // Verificar componentes UI
        if (questionText == null || quizOptions == null || quizOptions.Count == 0)
        {
            Debug.LogError("Faltan referencias a componentes UI en QuizManagerObject.");
            return;
        }

        // Verificar opciones
        foreach (var option in quizOptions)
        {
            if (option.optionButton == null || option.optionText == null)
            {
                Debug.LogError("Alguna opción no tiene todos sus componentes asignados en QuizManagerObject.");
                return;
            }
        }

        // Configurar botón toggle
        if (toggleQuizButton != null)
        {
            toggleQuizButton.onClick.AddListener(ToggleQuizPanel);
        }

        // Ocultar panel al inicio
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        // Inicializar resultados
        resultsFileName = GenerateResultsFileName();
        quizResult = new ObjectQuizResult
        {
            correctAnswers = 0,
            totalQuestions = 0,
            questionResults = new List<QuestionResult>()
        };

        LoadQuestions();
    }

    void LoadQuestions()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        Debug.Log($"Intentando cargar preguntas de objetos desde: {path}");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            try
            {
                allQuestions = JsonUtility.FromJson<ObjectQuestionList>(json).questions;
                Debug.Log($"Preguntas de objetos cargadas: {allQuestions?.Count ?? 0}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error al deserializar el JSON de preguntas de objetos: {ex.Message}");
                allQuestions = new List<ObjectQuestion>();
            }
        }
        else
        {
            Debug.LogError($"No se encontró el archivo de preguntas de objetos en: {path}");
            allQuestions = new List<ObjectQuestion>();
        }
    }

    void FilterQuestionsByObject(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            Debug.LogWarning("Nombre de objeto vacío, no se pueden filtrar las preguntas");
            currentObjectQuestions = new List<ObjectQuestion>();
            return;
        }

        currentObjectName = objectName;
        if (allQuestions != null)
        {
            currentObjectQuestions = allQuestions.FindAll(q => q.objectName == objectName);
            currentQuestionIndex = 0;
            Debug.Log($"Preguntas filtradas para objeto {objectName}: {currentObjectQuestions.Count}");
        }
        else
        {
            Debug.LogError("No hay preguntas de objetos cargadas");
            currentObjectQuestions = new List<ObjectQuestion>();
        }
    }

    void ToggleQuizPanel()
    {
        if (quizPanel != null)
        {
            if (!quizPanel.activeSelf)
            {
                // Verificar si hay un objeto siendo observado
                if (objectInfoRaycast != null && objectInfoRaycast.objectNameText != null)
                {
                    string currentObject = objectInfoRaycast.objectNameText.text;
                    FilterQuestionsByObject(currentObject);
                    if (currentObjectQuestions.Count > 0)
                    {
                        quizPanel.SetActive(true);
                        ShowQuestion();
                    }
                    else
                    {
                        Debug.Log($"No hay preguntas para el objeto: {currentObject}");
                    }
                }
            }
            else
            {
                quizPanel.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Actualizar preguntas si cambia el objeto observado
        if (objectInfoRaycast != null && objectInfoRaycast.objectNameText != null)
        {
            string newObjectName = objectInfoRaycast.objectNameText.text;
            if (newObjectName != currentObjectName && quizPanel != null && quizPanel.activeSelf)
            {
                FilterQuestionsByObject(newObjectName);
                ShowQuestion();
            }
        }
    }

    [System.Serializable]
    private class ObjectQuestionList
    {
        public List<ObjectQuestion> questions;
    }

    void ShowQuestion()
    {
        if (currentObjectQuestions == null || currentObjectQuestions.Count == 0 || currentQuestionIndex >= currentObjectQuestions.Count)
        {
            questionText.text = $"Quiz del objeto {currentObjectName} completado!\nRespuestas correctas: {quizResult.correctAnswers} de {quizResult.totalQuestions}";
            foreach (var option in quizOptions)
            {
                option.optionButton.gameObject.SetActive(false);
                option.optionText.text = "";
            }
            SaveQuizResults();
            return;
        }

        ObjectQuestion q = currentObjectQuestions[currentQuestionIndex];
        questionText.text = q.question;

        for (int i = 0; i < quizOptions.Count; i++)
        {
            if (i < q.options.Count)
            {
                quizOptions[i].optionButton.gameObject.SetActive(true);
                quizOptions[i].optionButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Opción {i + 1}";
                quizOptions[i].optionText.text = q.options[i];
                int idx = i;
                quizOptions[i].optionButton.onClick.RemoveAllListeners();
                quizOptions[i].optionButton.onClick.AddListener(() => OnOptionSelected(idx));
            }
            else
            {
                quizOptions[i].optionButton.gameObject.SetActive(false);
                quizOptions[i].optionText.text = "";
            }
        }
    }

    private int currentSelectedIndex = -1;

    void OnOptionSelected(int selectedIndex)
    {
        ObjectQuestion q = currentObjectQuestions[currentQuestionIndex];
        currentSelectedIndex = selectedIndex;
        
        // Desactivar otras opciones y resaltar la seleccionada
        for (int i = 0; i < quizOptions.Count; i++)
        {
            if (i == selectedIndex)
            {
                quizOptions[i].optionButton.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                quizOptions[i].optionButton.GetComponent<Image>().color = Color.white;
            }
        }

        Debug.Log($"Opción seleccionada {selectedIndex}: {q.options[selectedIndex]}");
        ValidateAnswer();
    }

    void ValidateAnswer()
    {
        if (currentSelectedIndex == -1) return;

        ObjectQuestion q = currentObjectQuestions[currentQuestionIndex];
        bool isCorrect = currentSelectedIndex == q.answer;
        
        // Registrar resultado
        QuestionResult result = new QuestionResult
        {
            question = q.question,
            selectedAnswer = q.options[currentSelectedIndex],
            correctAnswer = q.options[q.answer],
            wasCorrect = isCorrect
        };
        quizResult.questionResults.Add(result);
        quizResult.totalQuestions++;

        if (isCorrect)
        {
            quizResult.correctAnswers++;
            questionText.text = "¡Correcto!";
            quizOptions[currentSelectedIndex].optionButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            questionText.text = $"Incorrecto. La respuesta correcta era: {q.options[q.answer]}";
            quizOptions[currentSelectedIndex].optionButton.GetComponent<Image>().color = Color.red;
            quizOptions[q.answer].optionButton.GetComponent<Image>().color = Color.green;
        }
        
        Invoke("NextQuestion", 2.5f);
    }

    void NextQuestion()
    {
        currentQuestionIndex++;
        ShowQuestion();
    }

    void SaveQuizResults()
    {
        try
        {
            string dirPath = Path.Combine(Application.persistentDataPath, "ObjectQuizResults");
            Directory.CreateDirectory(dirPath);

            string resultJson = JsonUtility.ToJson(quizResult, true);
            string fullPath = Path.Combine(dirPath, resultsFileName);
            File.WriteAllText(fullPath, resultJson);
            
            Debug.Log($"Resultados del quiz de objeto guardados en: {fullPath}");
            Debug.Log($"Contenido del resultado:\n{resultJson}");
            
            questionText.text += $"\nResultados guardados en: {resultsFileName}";
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al guardar los resultados del quiz de objeto: {ex.Message}");
            questionText.text += "\nError al guardar los resultados.";
        }
    }
}