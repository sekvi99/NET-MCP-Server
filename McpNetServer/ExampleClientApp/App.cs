using Dumpify;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace ExampleClientApp;

public class App(IChatClient chatClient)
{
    public async Task ExecuteAsync()
    {
        // Create an instance of IMcpClient. This is using StdioClientTransport, so will also run the
        // server locally using the specified command.

        var mcpClient = await CreateMcpClient();

        // Direct call to a tool on the MCP Server. Useful for development, but not how you'd
        // do it in proper code - as it's the role of the LLM to decide what tools to call to
        // fulfil the requirements of a given prompt.

        await ExampleOfDirectCall(mcpClient);

        // Example of an LLM deciding what tools to call (which is the main usecase for MCP Servers)

        // await ExampleUsingAPrompt(mcpClient);

        // Example where the MCP Server can leverage the client's LLM

        // await ExampleUsingAPromptWithSampling(mcpClient);
    }

    private async Task<IMcpClient> CreateMcpClient()
    {
        try 
        {
            var mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new StdioClientTransportOptions
            {
                Name = "PodcastMcpServer",
                Command = "dotnet",
                Arguments = ["run", "--project", @"C:\Users\sekvi\RiderProjects\NET-MCP-Server\McpNetServer\McpNetServer", "--no-build"],
            }), new McpClientOptions
            {
                Capabilities = new ClientCapabilities
                {
                    Sampling = new SamplingCapability
                    {
                        SamplingHandler = chatClient.CreateSamplingHandler(),
                    }
                }
            });

            // Test the connection immediately
            var tools = await mcpClient.ListToolsAsync();
            Console.WriteLine($"Connected to MCP server. Available tools: {tools.Count()}");
            foreach (var tool in tools)
            {
                Console.WriteLine($"Tool Name: {tool.Name}");
                Console.WriteLine($"Tool Description: {tool.Description}");
                Console.WriteLine("---");
            }
        
            return mcpClient;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create MCP client: {ex.Message}");
            throw;
        }
    }

    private static async Task ExampleOfDirectCall(IMcpClient mcpClient)
    {
        try
        {
            Console.WriteLine("Testing different parameter combinations...");

            // Test 1: Empty keywords list
            var arguments1 = new Dictionary<string, object?>
            {
                ["keywords"] = new List<string>()
            };
            Console.WriteLine("Calling with empty keywords list...");
            var result1 = await mcpClient.CallToolAsync("GetDocumentDetails", arguments1);
            result1.Dump("Test 1 - Empty List");

            // Test 2: Null keywords
            var arguments2 = new Dictionary<string, object?>
            {
                ["keywords"] = null
            };
            Console.WriteLine("Calling with null keywords...");
            var result2 = await mcpClient.CallToolAsync("GetDocumentDetails", arguments2);
            result2.Dump("Test 2 - Null");

            // Test 3: Single keyword
            var arguments3 = new Dictionary<string, object?>
            {
                ["keywords"] = new List<string> { "bitcoin" }
            };
            Console.WriteLine("Calling with single keyword...");
            var result3 = await mcpClient.CallToolAsync("GetDocumentDetails", arguments3);
            result3.Dump("Test 3 - Single Keyword");

            // Test 4: No arguments at all
            Console.WriteLine("Calling with no arguments...");
            var result4 = await mcpClient.CallToolAsync("GetDocumentDetails", null);
            result4.Dump("Test 4 - No Arguments");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Tool call failed: {ex.Message}");
            Console.WriteLine($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }

    private async Task ExampleUsingAPrompt(IMcpClient mcpClient)
    {
        const string prompt = "Get a list of documents with keyword 'bitcoin'";

        var tools = await mcpClient.ListToolsAsync();

        var response = await chatClient.GetResponseAsync(prompt, new() { Tools = [.. tools] });

        response.Text.Dump("ExampleUsingAPrompt");
    }

    private async Task ExampleUsingAPromptWithSampling(IMcpClient mcpClient)
    {
        try
        {
            const string prompt = "Get a list of documents with keyword 'bitcoin' that have a comedy slant in the name.";

            Console.WriteLine(prompt);
            var tools = await mcpClient.ListToolsAsync();
            foreach (var tool in tools)
            {
                Console.WriteLine($"Tool Name: {tool.Name}");
            }

            var response = await chatClient.GetResponseAsync(prompt, new() { Tools = [.. tools] });

            response.Text.Dump("ExampleUsingAPromptWithSampling");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create MCP client: {ex.Message}");
            throw ex;
        }
       
    }
}