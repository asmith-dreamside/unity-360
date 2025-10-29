using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

// Clase para representar una pregunta
[System.Serializable]
public class Question
{
    public string question;
    public List<string> options;
    public int answer;
    public string sala; // Identificador de la sala donde se muestra esta pregunta
}// Clase para registrar las respuestas del usuario
[System.Serializable]
public class QuizResult
{
    public int correctAnswers;
    public int totalQuestions;
    public List<QuestionResult> questionResults;
}

[System.Serializable]
public class QuestionResult
{
    public string question;
    public string selectedAnswer;
    public string correctAnswer;
    public bool wasCorrect;
}

[System.Serializable]
public class QuizOption
{
    public Button optionButton;           // Botón de la opción
    public TextMeshProUGUI optionText;    // Texto completo de la opción
}

public class QuizManager : MonoBehaviour
{
	public TextMeshProUGUI questionText;
	public List<QuizOption> quizOptions;  // Lista de opciones (botón + texto)
	public TextMeshProUGUI feedbackText;
	public string jsonFileName = "questions.json";
	public GameObject quizPanel; // Panel principal del quiz
	public Button toggleQuizButton; // Botón para mostrar/ocultar el quiz
	public SalaMinimapHighlighter salaHighlighter; // Referencia al SalaMinimapHighlighter

	private List<Question> allQuestions; // Todas las preguntas del JSON
	private List<Question> currentSalaQuestions; // Preguntas filtradas para la sala actual
	private int currentQuestionIndex = 0;
	private QuizResult quizResult;
	private string resultsFileName;
	private string currentSala = "";

	private string GenerateResultsFileName()
	{
		// Formato: respuesta_YYYY-MM-DD_HH-mm-ss.json
		return $"respuesta_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.json";
	}

    void Start()
    {
        // Verificar que todos los componentes UI estén asignados
        if (questionText == null || feedbackText == null || quizOptions == null || quizOptions.Count == 0)
        {
            Debug.LogError("Faltan referencias a componentes UI. Por favor, verifica en el Inspector.");
            return;
        }

        // Verificar que cada opción tenga sus componentes
        foreach (var option in quizOptions)
        {
            if (option.optionButton == null || option.optionText == null)
            {
                Debug.LogError("Alguna opción no tiene todos sus componentes asignados (botón o texto).");
                return;
            }
        }

        // Configurar el botón de toggle
        if (toggleQuizButton != null)
        {
            toggleQuizButton.onClick.AddListener(ToggleQuizPanel);
        }

        // Ocultar el panel de quiz al inicio
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        // Generar nombre del archivo de resultados
        resultsFileName = GenerateResultsFileName();
        Debug.Log($"Se guardarán los resultados en: {resultsFileName}");

        // Inicializar el registro de resultados
        quizResult = new QuizResult
        {
            correctAnswers = 0,
            totalQuestions = 0,
            questionResults = new List<QuestionResult>()
        };

        LoadQuestions();
    }    void LoadQuestions()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        Debug.Log($"Intentando cargar preguntas desde: {path}");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"Contenido JSON leído: {json}");
            try
            {
                allQuestions = JsonUtility.FromJson<QuestionList>(json).questions;
                Debug.Log($"Preguntas totales cargadas: {allQuestions?.Count ?? 0}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error al deserializar el JSON: {ex.Message}");
                allQuestions = new List<Question>();
            }
        }
        else
        {
            Debug.LogError($"No se encontró el archivo de preguntas en: {path}");
            allQuestions = new List<Question>();
        }
    }

    void FilterQuestionsBySala(string salaName)
    {
        if (string.IsNullOrEmpty(salaName))
        {
            Debug.LogWarning("Nombre de sala vacío, no se pueden filtrar las preguntas");
            currentSalaQuestions = new List<Question>();
            return;
        }

        currentSala = salaName;
        if (allQuestions != null)
        {
            currentSalaQuestions = allQuestions.FindAll(q => q.sala == salaName);
            currentQuestionIndex = 0;
            Debug.Log($"Preguntas filtradas para sala {salaName}: {currentSalaQuestions.Count}");
        }
        else
        {
            Debug.LogError("No hay preguntas cargadas en allQuestions");
            currentSalaQuestions = new List<Question>();
        }
    }

    void ToggleQuizPanel()
    {
        if (quizPanel != null)
        {
            if (!quizPanel.activeSelf)
            {
                // Al mostrar el panel, actualizar las preguntas según la sala actual
                if (salaHighlighter != null && salaHighlighter.locationText != null)
                {
                    string currentSala = salaHighlighter.locationText.text;
                    FilterQuestionsBySala(currentSala);
                    if (currentSalaQuestions.Count > 0)
                    {
                        quizPanel.SetActive(true);
                        ShowQuestion();
                    }
                    else
                    {
                        Debug.Log($"No hay preguntas para la sala: {currentSala}");
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
        // Actualizar las preguntas si cambia la sala
        if (salaHighlighter != null && salaHighlighter.locationText != null)
        {
            string newSala = salaHighlighter.locationText.text;
            if (newSala != currentSala && quizPanel != null && quizPanel.activeSelf)
            {
                FilterQuestionsBySala(newSala);
                ShowQuestion();
            }
        }
    }



	[System.Serializable]
	private class QuestionList
	{
		public List<Question> questions;
	}

	void ShowQuestion()
	{
		if (currentSalaQuestions == null || currentSalaQuestions.Count == 0 || currentQuestionIndex >= currentSalaQuestions.Count)
		{
			questionText.text = $"Quiz de {currentSala} completado!";
			foreach (var option in quizOptions)
			{
				option.optionButton.gameObject.SetActive(false);
				option.optionText.text = "";
			}
			SaveQuizResults();
			feedbackText.text = $"Respuestas correctas: {quizResult.correctAnswers} de {quizResult.totalQuestions}";
			return;
		}

		Question q = currentSalaQuestions[currentQuestionIndex];
		questionText.text = q.question;
		feedbackText.text = "";

		for (int i = 0; i < quizOptions.Count; i++)
		{
			if (i < q.options.Count)
			{
				quizOptions[i].optionButton.gameObject.SetActive(true);
				// Mostrar un resumen en el botón
				quizOptions[i].optionButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Opción {i + 1}";
				// Mostrar el texto completo en el texto asociado
				quizOptions[i].optionText.text = q.options[i];
				int idx = i; // Necesario para el closure
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
		Question q = currentSalaQuestions[currentQuestionIndex];
		currentSelectedIndex = selectedIndex;
		
		// Desactivar las demás opciones y resaltar la seleccionada
		for (int i = 0; i < quizOptions.Count; i++)
		{
			if (i == selectedIndex)
			{
				quizOptions[i].optionButton.GetComponent<Image>().color = Color.yellow; // Resaltar seleccionada
			}
			else
			{
				quizOptions[i].optionButton.GetComponent<Image>().color = Color.white;
			}
		}

		Debug.Log($"Opción seleccionada {selectedIndex}: {q.options[selectedIndex]}");
		
		// Proceder directamente con la validación
		ValidateAnswer();
	}

	void ValidateAnswer()
	{
		if (currentSelectedIndex == -1) return;

		Question q = currentSalaQuestions[currentQuestionIndex];
		Debug.Log($"Validando opción {currentSelectedIndex}: {q.options[currentSelectedIndex]}");
		Debug.Log($"Respuesta correcta es {q.answer}: {q.options[q.answer]}");

		bool isCorrect = currentSelectedIndex == q.answer;
		
		// Registrar el resultado de esta pregunta
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
			feedbackText.text = "¡Correcto!";
			quizOptions[currentSelectedIndex].optionButton.GetComponent<Image>().color = Color.green;
		}
		else
		{
			feedbackText.text = $"Incorrecto. La respuesta correcta era: {q.options[q.answer]}";
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
			// Crear el directorio si no existe
			string dirPath = Path.Combine(Application.persistentDataPath, "QuizResults");
			Directory.CreateDirectory(dirPath);

			// Generar el JSON con formato pretty-print
			string resultJson = JsonUtility.ToJson(quizResult, true);
			
			// Guardar en el subdirectorio QuizResults
			string fullPath = Path.Combine(dirPath, resultsFileName);
			File.WriteAllText(fullPath, resultJson);
			
			Debug.Log($"Resultados del quiz guardados en: {fullPath}");
			Debug.Log($"Contenido del resultado:\n{resultJson}");
			
			// Mostrar mensaje en el feedbackText
			feedbackText.text += $"\nResultados guardados en: {resultsFileName}";
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Error al guardar los resultados: {ex.Message}");
			feedbackText.text += "\nError al guardar los resultados.";
		}
	}
}
