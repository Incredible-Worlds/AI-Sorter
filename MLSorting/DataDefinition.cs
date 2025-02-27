using Microsoft.ML;
using Microsoft.ML.Data;
using System.Diagnostics;

string _trainingFilePath = "/app/Models/subjectmodel.tsv";
if (!File.Exists(_trainingFilePath))
{
	Console.WriteLine($"File don't found: {_trainingFilePath}");
}
else
{
	Console.WriteLine("File loaded");
}
string _modelFilePath = "/app/Models/model.zip";

MLContext _mlContext;
IDataView _trainingDataView;
ITransformer _model;

PredictionEngine<DataDefinition, PartsPredictions> _predictionEngine;

_mlContext = new MLContext(seed: 0);

if (File.Exists(_modelFilePath))
{
	Console.WriteLine("Loading pre-trained model...");
	_model = _mlContext.Model.Load(_modelFilePath, out var modelInputSchema);
	_predictionEngine = _mlContext.Model.CreatePredictionEngine<DataDefinition, PartsPredictions>(_model);
}
else
{
	Console.WriteLine("No pre-trained model found. Training a new model...");
	_trainingDataView = _mlContext.Data.LoadFromTextFile<DataDefinition>(_trainingFilePath, hasHeader: true);
	var pipeline = ProcessData();

	var transformedData = pipeline.Fit(_trainingDataView).Transform(_trainingDataView);
	Console.WriteLine("Transformed columns:");
	foreach (var column in transformedData.Schema)
	{
		Console.WriteLine($"- {column.Name} ({column.Type})");
	}

	var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);
	SaveModelAsFile();
	_predictionEngine = _mlContext.Model.CreatePredictionEngine<DataDefinition, PartsPredictions>(_model);
}

//
// СЮДЫ ПИХАТЬ ДАННЫЕ НА ПРОВЕРКУ (Я сделаю сервис из этого, обещаю)
//

var preditionTest = "Прокладка впускного коллектора  WP6G";
Console.WriteLine($"PartsPredictions for {preditionTest}");
Console.WriteLine(PredictPartForSubjectline(preditionTest));

preditionTest = "Штуцер соединения РВД 32/27, МТЗ-32/27";
Console.WriteLine($"PartsPredictions for {preditionTest}");
Console.WriteLine(PredictPartForSubjectline(preditionTest));

preditionTest = "Блок управления ДВС с прошивкой (ЕСМ) C4995445 ISBe ЕВРО4";
Console.WriteLine($"PartsPredictions for {preditionTest}");
Console.WriteLine(PredictPartForSubjectline(preditionTest));

preditionTest = "Распылитель ускорительного насоса САН-Д 21073-1107370";
Console.WriteLine($"PartsPredictions for {preditionTest}");
Console.WriteLine(PredictPartForSubjectline(preditionTest));

preditionTest = "Смартфон Xiaomi Redmi 8A 2/32GB чёрный";
Console.WriteLine($"PartsPredictions for {preditionTest}");
Console.WriteLine(PredictPartForSubjectline(preditionTest));

preditionTest = "Конфеты Chupa Chups Карамель XXL 4D, 29 г х 60 шт";
Console.WriteLine($"PartsPredictions for {preditionTest}");
Console.WriteLine(PredictPartForSubjectline(preditionTest));


string PredictPartForSubjectline(string objectLine)
{
	var model = _mlContext.Model.Load(_modelFilePath, out var modelInputSchema);
	var partSubject = new DataDefinition() { Object = objectLine };
	_predictionEngine = _mlContext.Model.CreatePredictionEngine<DataDefinition, PartsPredictions>(model);

	var result = _predictionEngine.Predict(partSubject);
	return result.IsPart;
}

void SaveModelAsFile()
{
	_mlContext.Model.Save(_model, _trainingDataView.Schema, _modelFilePath);
}

IEstimator<ITransformer> ProcessData()
{
	var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Type", outputColumnName: "Label")
		.Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Object", outputColumnName: "SortType")) 
		.Append(_mlContext.Transforms.Concatenate("Features", "SortType"))
		.AppendCacheCheckpoint(_mlContext);

	return pipeline;
}

IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
{
	var trainingPipeline = pipeline
		.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
		.Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

	// Обучаем модель
	var model = trainingPipeline.Fit(trainingDataView);

	// После обучения, можно получить метрики:
	var metrics = _mlContext.MulticlassClassification.Evaluate(model.Transform(trainingDataView));

	// Выводим метрики
	Console.WriteLine($"Log-loss: {metrics.LogLoss}");
	Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy}");
	Console.WriteLine($"Micro Accuracy: {metrics.MicroAccuracy}");

	// Возвращаем обученную модель
	_model = model;

	return trainingPipeline;
}

public class DataDefinition
{
	[LoadColumn(0)]
	public string Object { get; set; }
	[LoadColumn(1)]
	public string Type { get; set; }
}

public class PartsPredictions
{
	[ColumnName("PredictedLabel")]
	public string? IsPart { get; set; }
}