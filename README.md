# MCP Server

A Model Context Protocol (MCP) server implementation for document processing and management with built-in AI-powered features.

## Overview

This MCP server provides tools for document discovery, processing, and AI-enhanced content manipulation. It's designed to integrate with AI assistants and language models through the Model Context Protocol, offering both standard document operations and creative AI-powered transformations.

## Architecture

The server follows a modular architecture with the following components:

### Core Components

- **DocumentDetailsExtensions.cs** - Extension methods for document detail operations
- **DocumentsLoader.cs** - Handles loading and processing of documents from the file system
- **DocumentsLoaderExtensions.cs** - Additional extension methods for document loading operations

### Models

- **DocumentDetails.cs** - Core data model representing document metadata and content
- **DocumentInfo.cs** - Supplementary document information model

### Tools

The server exposes functionality through MCP tools that can be called by connected AI assistants:

#### McpDetailsTool
- **Purpose**: Standard document retrieval and search
- **Primary Method**: `GetDocumentDetails`
- **Features**: 
  - Loads documents from the `.\Documents` directory
  - Supports keyword-based filtering
  - Returns structured document details

#### McpDocumentDetailsComedyTool
- **Purpose**: AI-enhanced document processing with creative transformations
- **Primary Method**: `GetComedyDocumentDetailsAsync`
- **Features**:
  - Applies comedic transformations to document titles
  - Uses integrated LLM capabilities for content modification
  - Supports keyword filtering
  - Asynchronous processing with cancellation support

## Key Features

### Document Processing
- Automatic document discovery from specified directories
- Metadata extraction and structured data representation
- Keyword-based search and filtering capabilities

### AI Integration
- Built-in chat client for LLM interactions
- Creative content transformation (comedy enhancement example)
- Asynchronous processing for better performance
- Cancellation token support for long-running operations

### MCP Compliance
- Fully compliant with Model Context Protocol specifications
- Proper tool registration and metadata
- Standardized error handling and response formats

## Usage

### Basic Document Retrieval

The `GetDocumentDetails` tool provides straightforward document access:

```csharp
// Retrieve all documents
var allDocs = McpDetailsTool.GetDocumentDetails(new List<string>());

// Search with keywords
var filteredDocs = McpDetailsTool.GetDocumentDetails(new List<string> {"keyword1", "keyword2"});
```

### AI-Enhanced Processing

The comedy tool demonstrates AI-powered content transformation:

```csharp
// Get documents with AI-transformed titles
var comedyDocs = await McpDocumentDetailsComedyTool.GetComedyDocumentDetailsAsync(
    mcpServer, 
    keywords, 
    cancellationToken);
```

## Configuration

### Document Directory
The server loads documents from the `.\Documents` directory by default. This path may need adjustment depending on your deployment environment.

### Dependencies
- MCP Server framework
- AI/LLM integration capabilities
- File system access for document loading

## Extension Points

The modular design allows for easy extension:

1. **New Tools**: Add additional MCP tools by creating classes with the `[McpServerToolType]` attribute
2. **Document Processors**: Extend `DocumentsLoader` for different file formats or sources
3. **AI Transformations**: Create new tools similar to the comedy tool for different types of content enhancement

## Error Handling

The server implements robust error handling:
- Cancellation token support for graceful shutdown
- Async/await patterns for non-blocking operations
- Proper exception propagation through the MCP protocol

## Future Enhancements

Potential areas for expansion:
- Support for additional document formats
- More sophisticated search capabilities
- Additional AI-powered transformations
- Real-time document monitoring and updates
- Integration with external document sources

## Inspecting
Probably best way to inspect the behaviour is to use npx.
https://github.com/modelcontextprotocol/inspector
```bash
npx @modelcontextprotocol/inspector
```

## Contributing

When extending this server:
1. Follow the established patterns for tool creation
2. Use appropriate attributes for MCP registration
3. Implement proper async patterns where applicable
4. Include comprehensive error handling
5. Document new tools and their capabilities
