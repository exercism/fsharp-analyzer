#!/bin/sh
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
/opt/analyzer/Exercism.Analyzers.FSharp $1 $2 $3