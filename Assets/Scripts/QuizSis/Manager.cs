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

public class Manager : MonoBehaviour
{
	public TextMeshProUGUI questionText;
	public List<Button> optionButtons;
	public TextMeshProUGUI feedbackText;
	public string jsonFileName = "questions.json";

	private List<Question> questions;
	private int currentQuestionIndex = 0;
	private QuizResult quizResult;
	private string resultsFileName;

	private string GenerateResultsFileName()
	{
		// Formato: respuesta_YYYY-MM-DD_HH-mm-ss.json
		return $"respuesta_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.json";
	}

    void Start()
    {
        // Verificar que todos los componentes UI estén asignados
        if (questionText == null || feedbackText == null || optionButtons == null || optionButtons.Count == 0)
        {
            Debug.LogError("Faltan referencias a componentes UI. Por favor, verifica en el Inspector.");
            return;
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
        ShowQuestion();
    }	void LoadQuestions()
	{
		string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
		Debug.Log($"Intentando cargar preguntas desde: {path}");
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			Debug.Log($"Contenido JSON leído: {json}");
			try
			{
				questions = JsonUtility.FromJson<QuestionList>(json).questions;
				Debug.Log($"Preguntas cargadas: {questions?.Count ?? 0}");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error al deserializar el JSON: {ex.Message}");
				questions = new List<Question>();
			}
		}
		else
		{
			Debug.LogError($"No se encontró el archivo de preguntas en: {path}");
			questions = new List<Question>();
		}
	}



	[System.Serializable]
	private class QuestionList
	{
		public List<Question> questions;
	}

	void ShowQuestion()
	{
		if (questions == null || questions.Count == 0 || currentQuestionIndex >= questions.Count)
		{
			questionText.text = "Quiz completado!";
			foreach (var btn in optionButtons) btn.gameObject.SetActive(false);
			SaveQuizResults();
			feedbackText.text = $"Respuestas correctas: {quizResult.correctAnswers} de {quizResult.totalQuestions}";
			return;
		}

		Question q = questions[currentQuestionIndex];
		questionText.text = q.question;
		feedbackText.text = "";
		for (int i = 0; i < optionButtons.Count; i++)
		{
			if (i < q.options.Count)
			{
				optionButtons[i].gameObject.SetActive(true);
				optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
				int idx = i; // Necesario para el closure
				optionButtons[i].onClick.RemoveAllListeners();
				optionButtons[i].onClick.AddListener(() => OnOptionSelected(idx));
			}
			else
			{
				optionButtons[i].gameObject.SetActive(false);
			}
		}
	}

	void OnOptionSelected(int selectedIndex)
	{
		Question q = questions[currentQuestionIndex];
		Debug.Log($"Seleccionada opción {selectedIndex}: {q.options[selectedIndex]}");
		Debug.Log($"Respuesta correcta es {q.answer}: {q.options[q.answer]}");

		bool isCorrect = selectedIndex == q.answer;
		
		// Registrar el resultado de esta pregunta
		QuestionResult result = new QuestionResult
		{
			question = q.question,
			selectedAnswer = q.options[selectedIndex],
			correctAnswer = q.options[q.answer],
			wasCorrect = isCorrect
		};
		quizResult.questionResults.Add(result);
		quizResult.totalQuestions++;
		if (isCorrect)
		{
			quizResult.correctAnswers++;
			feedbackText.text = "¡Correcto!";
		}
		else
		{
			feedbackText.text = $"Incorrecto. La respuesta correcta era: {q.options[q.answer]}";
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
