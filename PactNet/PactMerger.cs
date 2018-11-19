using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PactNet.Models;
using PactNet.Wrappers;

namespace PactNet
{
	internal class PactMerger : IPactMerger
	{
		private readonly IFileWrapper _fileWrapper;
		private readonly string _pactDir;
		private const string InteractionsPropName = "interactions";
		private const string MessagesPropName = "messages";
		private const string DescriptionPropName = "description";

		public PactMerger(string pactDir, IFileWrapper fileWrapper)
		{
			_pactDir = pactDir;
			_fileWrapper = fileWrapper;
		}

		public void DeleteUnexpectedInteractions(IEnumerable<Interaction> interactions, string consumer, string provider)
		{
			var pactFilePath = GetPactFilePath(_pactDir, consumer, provider);
			if (!_fileWrapper.Exists(pactFilePath)) return;

			var interactionsArray = interactions?.ToArray();

			if (interactionsArray == null || !interactionsArray.Any())
			{
				_fileWrapper.Delete(pactFilePath);
				return;
			}

			var fileContent = _fileWrapper.ReadAllText(pactFilePath);
			var interactionsDescriptions = interactionsArray.Select(x => x.Description).ToList();

			var jsonObject = JObject.Parse(fileContent);
			var interactionsToRemove = GetUnusedInteractionsFromJson(InteractionsPropName, DescriptionPropName, interactionsDescriptions, jsonObject);
			var messagesToRemove = GetUnusedInteractionsFromJson(MessagesPropName, DescriptionPropName, interactionsDescriptions, jsonObject);

			if ((interactionsToRemove == null || !interactionsToRemove.Any()) && (messagesToRemove == null || !messagesToRemove.Any())) return;

			interactionsToRemove?.ForEach(x => x.Remove());
			messagesToRemove?.ForEach(x => x.Remove());

			_fileWrapper.WriteAllText(pactFilePath, jsonObject.ToString());
		}

		private static string GetPactFilePath(string pactDir, string consumer, string provider)
		{
			var filePath = pactDir + @"\" + consumer.Replace(" ", "_") + "_" + provider.Replace(" ", "_") + ".txt";
			return filePath;
		}

		private List<JToken> GetUnusedInteractionsFromJson(string interactionsPropName, string descriptionPropName,
			ICollection<string> descriptions, JObject jsonObject)
		{
			var itemsToRemove = jsonObject[interactionsPropName]?.Children<JToken>()
				.Where(x => !descriptions.Contains(x[descriptionPropName].Value<string>()))
				.ToList();

			return itemsToRemove;
		}
	}
}
