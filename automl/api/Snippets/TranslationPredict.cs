﻿// Copyright (c) 2019 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.
using CommandLine;
using Google.Cloud.AutoML.V1;
using System;
using System.IO;

namespace GoogleCloudSamples
{
    [Verb("translate", HelpText = "Translate text from the source to the target language")]
    public class AutoMLTranslationPredictOptions : PredictOptions
    {
        [Value(2, HelpText = "Location of file with text to translate")]
        public string FilePath { get; set; }
    }

    public class AutoMLTranslationPredict
    {
        
        public static object TranslationPredict(string projectID,
                                              string modelID,
                                              string filePath)
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            var client = PredictionServiceClient.Create();

            // Get the full path of the model.
            var modelName = ModelName.Format(projectID, "us-central1", modelID);

            string textSnippet = "";
            using (var reader = new StreamReader(filePath))
            {
                textSnippet = reader.ReadToEnd();
            }

            // Construct request
            var predictionRequest = new PredictRequest
            {
                Name = modelName,
                Payload = new ExamplePayload
                {
                    TextSnippet = new TextSnippet
                    {
                        Content = textSnippet
                    }
                },
            };

            var response = client.Predict(predictionRequest);

            foreach(var payload in response.Payload)
            {
                Console.Write($"Translation: {payload.Translation.TranslatedContent.Content}");
            }

            return 0;
        }


        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((AutoMLTranslationPredictOptions opts) =>
                     AutoMLTranslationPredict.TranslationPredict(opts.ProjectID,
                                                                 opts.ModelID,
                                                                 opts.FilePath));
        }
    }
}
