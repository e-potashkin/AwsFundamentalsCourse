﻿using Amazon.SQS;
using Amazon.SQS.Model;

var queueName = args.Length == 1 ? args[0] : "customers";

var cts = new CancellationTokenSource();
var sqsClient = new AmazonSQSClient();

var queueUriResponse = await sqsClient.GetQueueUrlAsync(queueName);

var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUriResponse.QueueUrl,
    AttributeNames = new List<string> {"All"},
    MessageAttributeNames = new List<string> {"All"}
};

while (!cts.IsCancellationRequested)
{
    var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);

    foreach (var message in response.Messages)
    {
        Console.WriteLine($"Message Id: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");

        await sqsClient.DeleteMessageAsync(queueUriResponse.QueueUrl, message.ReceiptHandle);
    }
 
    await Task.Delay(3000);
}