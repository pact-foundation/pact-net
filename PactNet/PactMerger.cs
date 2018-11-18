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

		public PactMerger(string pactDir, IFileWrapper fileWrapper)
		{
			_pactDir = pactDir;
			_fileWrapper = fileWrapper;
		}

		public void DeleteUnexpectedInteractions(IEnumerable<Interaction> interactions, string consumer, string provider)
		{
			var pactFilePath = GetPactFilePath(_pactDir, consumer, provider);
			if (!_fileWrapper.Exists(pactFilePath)) return;

			if (interactions == null || !interactions.Any())
			{
				_fileWrapper.Delete(pactFilePath);
				return;
			}

			var fileContent = _fileWrapper.ReadAllText(pactFilePath);
			var jsonObject = JObject.Parse(fileContent);

			if (jsonObject.Property(InteractionsPropName).HasValues)
			{
				var existingInteractions = jsonObject.Property(InteractionsPropName).Value.Where(x => x.P)
			}

			if (jsonObject.Property(MessagesPropName).HasValues)
			{
				
			}
		}

		private static string GetPactFilePath(string pactDir, string consumer, string provider)
		{
			var filePath = pactDir + @"\" + consumer.Replace(" ", "_") + "_" + provider.Replace(" ", "_") + ".txt";
			return filePath;
		}
	}
}
