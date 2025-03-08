#!/bin/bash
# Script to publish the Aspire CLI tool as a native AOT executable

# Default parameter values
CONFIGURATION="Release"
RUNTIME=""
OUTPUT_DIR="./publish"

# Parse command-line options
while [[ $# -gt 0 ]]; do
  case $1 in
    --configuration|-c)
      CONFIGURATION="$2"
      shift 2
      ;;
    --runtime|-r)
      RUNTIME="$2"
      shift 2
      ;;
    --output|-o)
      OUTPUT_DIR="$2"
      shift 2
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
done

# Determine runtime identifier if not specified
if [ -z "$RUNTIME" ]; then
  if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    RUNTIME="linux-x64"
  elif [[ "$OSTYPE" == "darwin"* ]]; then
    # Check for Apple Silicon
    if [[ $(uname -m) == "arm64" ]]; then
      RUNTIME="osx-arm64"
    else
      RUNTIME="osx-x64"
    fi
  elif [[ "$OSTYPE" == "msys"* || "$OSTYPE" == "cygwin"* || "$OSTYPE" == "win32"* ]]; then
    RUNTIME="win-x64"
  else
    echo "Error: Could not determine runtime. Please specify with --runtime parameter."
    exit 1
  fi
fi

echo "Publishing native AOT executable for $RUNTIME..."
echo "Configuration: $CONFIGURATION"
echo "Output directory: $OUTPUT_DIR"

# Create the output directory if it doesn't exist
mkdir -p "$OUTPUT_DIR"

# Publish the application as a self-contained native AOT executable
dotnet publish -c "$CONFIGURATION" -r "$RUNTIME" --self-contained \
  -p:PublishAot=true -p:InvariantGlobalization=true -o "$OUTPUT_DIR"

# Check if the publish was successful
if [ $? -eq 0 ]; then
  echo -e "\n✅ Published successfully to $OUTPUT_DIR"
  
  # Determine executable extension
  EXE_EXT=""
  if [[ "$RUNTIME" == "win-"* ]]; then
    EXE_EXT=".exe"
  fi
  
  # Display executable path and show help on how to use it
  EXE_PATH="$OUTPUT_DIR/aspire-cli$EXE_EXT"
  if [ -f "$EXE_PATH" ]; then
    echo -e "\nExecutable path: $EXE_PATH"
    echo -e "\nYou can now run the tool with:"
    echo "  $EXE_PATH --help"
    echo "  $EXE_PATH dev"
    echo "  $EXE_PATH new --template webapp --output MyProject"
    echo "  $EXE_PATH add myservice"
  else
    echo -e "\n⚠️ Executable not found at expected location. Please check the output directory."
  fi
else
  echo -e "\n❌ Publish failed"
fi